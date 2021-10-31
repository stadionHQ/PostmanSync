using Microsoft.OpenApi.Models;
using Stadion.PostmanSync.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    if (hostingContext.HostingEnvironment.IsDevelopment())
    {
        // Add local configuration file so that we don't share secrets
        // in the public repo
        config.AddJsonFile("appsettings.Local.json", true, true);
    }
});

// Add services to the container.

#if DEBUG
builder.Services.AddPostmanSync(builder.Configuration);
#endif

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {Title = "Postman Sync Demo", Version = "v1"});
    
    c.AddServer(new OpenApiServer
    {
        Url = "https://localhost:7038",
        Description = "Development"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StadionHQ.PostmanSync.DevApi v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();