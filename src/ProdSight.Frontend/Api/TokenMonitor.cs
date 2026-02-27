namespace ProdSight.Frontend.Api;

public class TokenMonitor : IAsyncDisposable
{
    private readonly ILogger _logger;
    private readonly Timer? _timer;
    private DateTimeOffset? _expires=null;
    public TimeSpan? Left => _expires.HasValue ? _expires.Value - DateTimeOffset.UtcNow : null;
    public event Action? OnTick;

    public TokenMonitor(ILogger<TokenMonitor> logger)
    {
        _logger = logger;
       _timer = new  Timer(_ => _ = DoWorkAsync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
    }

    public void SetExpires(DateTimeOffset? expires)
    {
        _expires = expires;
    }

    private async Task DoWorkAsync()
    {
        _logger.LogDebug("Time left: {Left}", Left);
        OnTick?.Invoke();
    }

    public ValueTask DisposeAsync()
    {
        _timer?.Change(Timeout.Infinite, 0);
        _timer?.Dispose();
        return ValueTask.CompletedTask;
    }
}