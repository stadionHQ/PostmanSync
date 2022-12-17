namespace Stadion.PostmanSync.Hosting;

public static class PostmanServiceCollectionExtensions
{
    public static void AddPostmanSync(this IServiceCollection services, IConfiguration config)
    {
        var configSection = config.GetSection(PostmanSyncOptions.PostmanSync);
        services.Configure<PostmanSyncOptions>(configSection);
        services.AddSingleton<IPostmanSyncManager, PostmanSyncManager>();

        var value = configSection.Get<PostmanSyncOptions>();

        if (value == null)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();                
            });

            var logger = loggerFactory.CreateLogger(typeof(PostmanServiceCollectionExtensions));
            logger.LogInformation($"Cannot find Postman Sync options for config section. Check you have included the {PostmanSyncOptions.PostmanSync} in your app settings json. Postman Sync will not be started.");
            return;
        }

        if (value.HostedService?.Enabled ?? false)
        {
            services.AddHostedService<PostmanSyncHostedService>();
        }
    }
}