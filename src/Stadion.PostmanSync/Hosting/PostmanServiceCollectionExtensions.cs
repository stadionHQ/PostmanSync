namespace Stadion.PostmanSync.Hosting;

public static class PostmanServiceCollectionExtensions
{
    public static void AddPostmanSync(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<PostmanSyncOptions>(config.GetSection(PostmanSyncOptions.PostmanSync));
        services.AddSingleton<IPostmanSyncManager, PostmanSyncManager>();
        services.AddHostedService<PostmanSyncHostedService>();
    }
}