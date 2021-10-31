namespace Stadion.PostmanSync.Hosting;

public class PostmanSyncHostedService : IHostedService
{
    private readonly IPostmanSyncManager syncManager;

    public PostmanSyncHostedService(IPostmanSyncManager syncManager)
    {
        this.syncManager = syncManager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Runs Postman Sync on startup
        await syncManager.RunAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}