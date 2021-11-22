using Microsoft.Extensions.Options;

namespace Stadion.PostmanSync.Hosting;

public class PostmanSyncHostedService : IHostedService
{
    private readonly IPostmanSyncManager syncManager;
    private readonly IOptions<PostmanSyncOptions> options;

    public PostmanSyncHostedService(IPostmanSyncManager syncManager, IOptions<PostmanSyncOptions> options)
    {
        this.syncManager = syncManager;
        this.options = options;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Runs Postman Sync on startup
        try
        {
            await syncManager.RunAsync();
        }
        catch (Exception e)
        {
            if (options.Value.HostedService.ThrowExceptions)
            {
                throw;
            }
            
            Console.WriteLine(e);
            Console.WriteLine("Postman Sync manager has thrown an exception. The hosted service has been configured to ignore exceptions.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}