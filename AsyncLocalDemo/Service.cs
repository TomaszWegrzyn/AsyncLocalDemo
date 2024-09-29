using ILogger = Serilog.ILogger;

namespace AsyncLocalDemo;

public class Service
{
    private readonly ILogger _logger;
    private readonly IHttpClientFactory _clientFactory;
    private static ThreadLocal<int> _threadLocal = new();
    private static AsyncLocal<int> _asyncLocal = new();

    public Service(ILogger logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public void DemoThreadLocal()
    {
        var threads = new Thread[3];

        _threadLocal.Value = new Random().Next(1, 100);
        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", 
            Environment.CurrentManagedThreadId, _threadLocal.Value);

        for (var i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() =>
            {
                _logger.Information("Spawned thread. Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}",
                    Environment.CurrentManagedThreadId, _threadLocal.Value);
                _threadLocal.Value = new Random().Next(1, 100);
                _logger.Information("Spawned thread. Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}",
                    Environment.CurrentManagedThreadId, _threadLocal.Value);
            });

            threads[i].Start();
        }

        foreach (var t in threads)
        {
            t.Join();
        }
        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", 
            Environment.CurrentManagedThreadId, _threadLocal.Value);

        
        _clientFactory.CreateClient(" :-) ").GetAsync("https://example.com").GetAwaiter().GetResult();
        
        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", 
            Environment.CurrentManagedThreadId, _threadLocal.Value);
    }
    
    public async Task DemoThreadLocalAsync()
    {
        var threads = new Thread[3];

        _threadLocal.Value = new Random().Next(1, 100);
        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", 
            Environment.CurrentManagedThreadId, _threadLocal.Value);

        for (var i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() =>
            {
                _logger.Information("Spawned thread. Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", 
                    Environment.CurrentManagedThreadId, _threadLocal.Value);

                _threadLocal.Value = new Random().Next(1, 100);
                _logger.Information("Spawned thread. Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", 
                    Environment.CurrentManagedThreadId, _threadLocal.Value);
            });

            threads[i].Start();
        }

        foreach (var t in threads)
        {
            t.Join();
        }
        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", 
            Environment.CurrentManagedThreadId, _threadLocal.Value);

        await _clientFactory.CreateClient(" :-) ").GetAsync("https://example.com");
        
        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", 
            Environment.CurrentManagedThreadId, _threadLocal.Value);
    }
    
    public async Task DemoAsyncLocalAsync()
    {
        _asyncLocal.Value = new Random().Next(1, 100);
        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {AsyncLocalValue}", Environment.CurrentManagedThreadId, _asyncLocal.Value);

        var tasks = new Task[3];

        for (var i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                _logger.Information("Spawned Task. Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", Environment.CurrentManagedThreadId, _asyncLocal.Value);

                _asyncLocal.Value = new Random().Next(1, 100);
                _logger.Information("Spawned Task. Thread ID: {CurrentThreadManagedThreadId}, Value: {ThreadLocalValue}", Environment.CurrentManagedThreadId, _asyncLocal.Value);
            });
        }

        await Task.WhenAll(tasks);

        _logger.Information("Thread ID: {CurrentThreadManagedThreadId}, Value: {AsyncLocalValue}", Environment.CurrentManagedThreadId, _asyncLocal.Value);
    }
}