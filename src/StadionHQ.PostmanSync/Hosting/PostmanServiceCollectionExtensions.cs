using Microsoft.Extensions.Options;

namespace Stadion.PostmanSync.Hosting;

public static class PostmanServiceCollectionExtensions
{
    public static void AddPostmanSync(this IServiceCollection services, IConfiguration config)
    {
        var configSection = config.GetSection(PostmanSyncOptions.PostmanSync);
        services.Configure<PostmanSyncOptions>(configSection);
        services.AddSingleton<IPostmanSyncManager, PostmanSyncManager>();

        var value = configSection.Get<PostmanSyncOptions>();

        if (value.HostedService?.Enabled ?? false)
        {
            services.AddHostedService<PostmanSyncHostedService>();
        }
    }
}