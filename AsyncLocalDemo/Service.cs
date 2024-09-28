using ILogger = Serilog.ILogger;

namespace AsyncLocalDemo;

public class Service
{
    private readonly ILogger _logger;
    private readonly IHttpClientFactory _clientFactory;
    private static ThreadLocal<int> _threadLocal = new();

    public Service(ILogger logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public void DemoThreadLocal()
    {
        // _threadLocal = new ThreadLocal<int>(() => Environment.CurrentManagedThreadId);
        var threads = new Thread[3];

        _threadLocal.Value = new Random().Next(1, 100);
        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", Environment.CurrentManagedThreadId, _threadLocal.Value);

        for (var i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() =>
            {
                _threadLocal.Value = new Random().Next(1, 100);
                var threadLocalValue = _threadLocal.Value;
                _logger.Information("Spawned thread. Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", Environment.CurrentManagedThreadId, threadLocalValue);
            });

            threads[i].Start();
        }

        foreach (var t in threads)
        {
            t.Join();
        }
        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", Environment.CurrentManagedThreadId, _threadLocal.Value);

        
        _clientFactory.CreateClient(" :-) ").GetAsync("https://example.com").GetAwaiter().GetResult();
        
        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", Environment.CurrentManagedThreadId, _threadLocal.Value);
    }
    
    public async Task DemoThreadLocalAsync()
    {
        // _threadLocal = new ThreadLocal<int>(() => Environment.CurrentManagedThreadId);
        var threads = new Thread[3];

        _threadLocal.Value = new Random().Next(1, 100);
        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", Environment.CurrentManagedThreadId, _threadLocal.Value);

        for (var i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() =>
            {
                _threadLocal.Value = new Random().Next(1, 100);
                var threadLocalValue = _threadLocal.Value;
                _logger.Information("Spawned thread. Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", Environment.CurrentManagedThreadId, threadLocalValue);
            });

            threads[i].Start();
        }

        foreach (var t in threads)
        {
            t.Join();
        }
        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", Environment.CurrentManagedThreadId, _threadLocal.Value);

        await _clientFactory.CreateClient(" :-) ").GetAsync("https://example.com");
        
        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", Environment.CurrentManagedThreadId, _threadLocal.Value);
    }
}