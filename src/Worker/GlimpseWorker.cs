namespace Worker;

public class GlimpseWorker : BackgroundService
{
    private readonly ILogger<GlimpseWorker> _logger;
    // Placeholder config. need to load them from somewhere
    private readonly int _fetchInterval = 60000;
    private readonly int _maxAttempts = 3;
    private readonly int _maxConcurrency = 3;
    private readonly int _timeoutSeconds = 5;
    private readonly string _apiKey = "demo";
    private readonly string[] _symbols = { "AAPL.US", "TSLA.US", "VTI.US", "foo", "AMZN.US", "BTC-USD.CC" };
    private readonly TimeSpan _retryDelay = TimeSpan.FromSeconds(1);

    public GlimpseWorker(ILogger<GlimpseWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var http = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(10),
            MaxConnectionsPerServer = _maxConcurrency,
        })
        {
            Timeout = TimeSpan.FromSeconds(_timeoutSeconds)
        };

        var client = new EodhdClient(http, _apiKey);
        var semaphore = new SemaphoreSlim(_maxConcurrency);

        while (!stoppingToken.IsCancellationRequested)
        {
            var tasks = _symbols.Select(async symbol =>
            {
                await semaphore.WaitAsync(stoppingToken);

                try
                {
                    string json = await client.GetRealtimeWithRetryAsync(symbol, _retryDelay, _maxAttempts, stoppingToken);
                    ProcessJson(json);
                }
                catch (Exception) when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogError("Failed to get real-time data for symbol {symbol}", symbol);
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToArray();

            await Task.WhenAll(tasks);
            await Task.Delay(_fetchInterval, stoppingToken);
        }
    }


    /// <summary>
    /// Dummy method. To be replaced with actual implementation
    /// </summary>
    private void ProcessJson(string json)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Result: {resJson}", json);
        }
    }
}
