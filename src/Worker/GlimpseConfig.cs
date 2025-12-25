using Processor;

public class Symbol
{
    public required string Code { get; set; }
    public required Currency Currency { get; set; }
}


public class GlimpseConfig
{
    public string ApiKey { get; set; } = "demo";
    public Symbol[] Symbols { get; set; } = Array.Empty<Symbol>();
    public int TimeoutSeconds { get; set; } = 5;
    public int MaxConcurrency { get; set; } = 3;
    public int MaxAttempts { get; set; } = 3;
    public int RetryDelaySeconds { get; set; } = 30;
    public int FetchIntervalSeconds { get; set; } = 60;
    public TimeSpan FetchInterval => TimeSpan.FromSeconds(FetchIntervalSeconds);
}
