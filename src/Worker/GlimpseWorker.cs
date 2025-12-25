namespace Worker;

public class GlimpseWorker : BackgroundService
{
    private readonly ILogger<GlimpseWorker> _logger;
    private readonly GlimpseConfig _config;

    public GlimpseWorker(ILogger<GlimpseWorker> logger, GlimpseConfig config)
    {
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var http = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(10),
            MaxConnectionsPerServer = _config.MaxConcurrency,
        })
        {
            Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds)
        };

        var client = new EodhdClient(http, _config.ApiKey);
        var semaphore = new SemaphoreSlim(_config.MaxConcurrency);

        while (!stoppingToken.IsCancellationRequested)
        {
            var tasks = _config.Symbols.Select(async symbol =>
            {
                await semaphore.WaitAsync(stoppingToken);

                try
                {
                    Quote quote = await client.GetRealtimeWithRetryAsync(symbol, _config.RetryDelaySeconds, _config.MaxAttempts, stoppingToken);
                    ProcessQuote(quote);
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
            await Task.Delay(_config.FetchInterval, stoppingToken);
        }
    }


    /// <summary>
    /// Dummy method. To be replaced with actual implementation
    /// </summary>
    private void ProcessQuote(Quote quote)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Result: {quote}", quote);
        }
    }
}
