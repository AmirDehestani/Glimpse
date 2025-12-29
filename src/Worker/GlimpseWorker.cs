namespace Worker;

using Processor;

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
                    RawQuote quote = await client.GetRealtimeWithRetryAsync(symbol.Code, _config.RetryDelaySeconds, _config.MaxAttempts, stoppingToken);
                    var processedQuote = QuoteProcessor.processQuote(quote);
                    _logger.LogInformation("{processedQuote}", processedQuote);
                }
                catch (Exception e) when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogError("Failed to get real-time data for symbol {symbol}", symbol.Code);
                    _logger.LogError(e.Message);
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
}
