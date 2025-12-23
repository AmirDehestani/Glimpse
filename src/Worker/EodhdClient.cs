sealed class EodhdClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public EodhdClient(HttpClient http, string apiKey)
    {
        _http = http;
        _apiKey = apiKey;
    }

    /// <summary>
    /// Adds retry logic on top of GetRealtimeAsycn
    /// </summary>
    public async Task<String> GetRealtimeWithRetryAsync(string symbol, TimeSpan initialDelay, int maxRetries, CancellationToken ct)
    {
        TimeSpan delay = initialDelay;

        for (int attempt = 0; attempt <= maxRetries + 1; attempt++)
        {
            try
            {
                return await GetRealtimeAsync(symbol, ct);
            }
            catch (Exception) when (!ct.IsCancellationRequested)
            {
                if (attempt == maxRetries)
                    throw;

                Console.WriteLine($"Retry#{attempt}: waiting for {delay.TotalSeconds} seconds");
                await Task.Delay(delay, ct);

                delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2);
            }
        }

        throw new InvalidOperationException("Unexpected retry loop exit");
    }

    /// <summary>
    /// Fetches raw JSON of real-time data for the given symbol.
    /// </summary>
    private async Task<string> GetRealtimeAsync(string symbol, CancellationToken ct)
    {
        var url = $"https://eodhd.com/api/real-time/{symbol}?api_token={_apiKey}&fmt=json";
        return await _http.GetStringAsync(url, ct);
    }
}
