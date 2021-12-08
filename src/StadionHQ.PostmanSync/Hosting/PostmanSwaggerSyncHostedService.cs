using Microsoft.Extensions.Options;
using Stadion.PostmanSync.Client;

namespace Stadion.PostmanSync.Hosting;

public class PostmanSyncHostedService : IHostedService
{
    private readonly IPostmanSyncManager syncManager;
    private readonly IOptions<PostmanSyncOptions> options;
    private readonly IHostApplicationLifetime applicationLifetime;

    public PostmanSyncHostedService(IPostmanSyncManager syncManager, IOptions<PostmanSyncOptions> options, IHostApplicationLifetime applicationLifetime)
    {
        this.syncManager = syncManager;
        this.options = options;
        this.applicationLifetime = applicationLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Once the hosted service starts, we tap into the Application Started event.
        // We need to use this event to ensure the host as started, as PostmanSync
        // needs to call the host in order to access the generated API definition.
        // (for example, the swagger.json generated by SwashBuckle).
        applicationLifetime.ApplicationStarted.Register(HandleApplicationStarted);
        return Task.CompletedTask;
    }

    private void HandleApplicationStarted()
    {
        // This is a synchronous method that starts a task, which then
        // starts post man sync.
        // I've come to using this approach, based on this:
        // https://stackoverflow.com/questions/9343594/how-to-call-asynchronous-method-from-synchronous-method-in-c
        Task.Run(async () =>
        {
            try
            {
                await syncManager.RunAsync();
            }
            catch (UnexpectedPostmanResponseException e)
            {
                Console.WriteLine($"Unexpected response from Postman with status code {e.ResponseStatusCode}");
                Console.WriteLine(e.ResponseContent);
                
                if (options.Value.HostedService.ThrowExceptions)
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An unexpected exception was encountered whilst running Postman Sync");
                if (options.Value.HostedService.ThrowExceptions)
                {
                    throw;
                }
                
                Console.WriteLine(e);
                Console.WriteLine(
                    "Postman Sync manager has thrown an exception. The hosted service has been configured to ignore exceptions.");
            }
        });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}