using Polly;

namespace TestHostedService;

public class TheBackgroundService: BackgroundService
{
    private readonly IHostApplicationLifetime _applicationLifetime;

    public TheBackgroundService(IHostApplicationLifetime applicationLifetime)
    {
        _applicationLifetime = applicationLifetime;
    }

    // https://medium.com/@daniel.sagita/backgroundservice-for-a-long-running-work-3debe8f8d25b
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // var policy = Policy.Handle<Exception>().RetryAsync(3);

        // var client = new HttpClient();
        // // var result = client.GetAsync("https://localhost:7058/swagger/v1/swagger.json", stoppingToken);
        // var result = client.GetAsync("https://www.google.com", stoppingToken);
        // // var content = await result.Content.ReadAsStringAsync(stoppingToken);
        // Console.WriteLine("IN THE BACKGROUND SERVICE");
        // await Task.WhenAny(result);
        // if(result.IsCompletedSuccessfully)
        // {
        //    Console.WriteLine("RESULT " + result.Result.StatusCode);
        // }
        // else
        // {
        //     Console.WriteLine("Call didn't complete successfully " + result.Status.ToString());
        // }

        _applicationLifetime.ApplicationStarted.Register(Test);

        // Console.WriteLine(content);

        // await policy.ExecuteAsync(async () =>
        // {
        //     Console.WriteLine("IN POLLY THING");
        //     var client = new HttpClient();
        //     var result = await client.GetAsync("https://localhost:7058/swagger/v1/swagger.json", stoppingToken);
        //     var content = await result.Content.ReadAsStringAsync(stoppingToken);
        //     Console.WriteLine("IN THE BACKGROUND SERVICE");
        //     Console.WriteLine(content);
        // });
    }

    public void Test()
    {
        
        Console.WriteLine("IN TEST");
        var client = new HttpClient();
        var res = Task.Run(async () =>
        {
            var response = await client.GetAsync("https://localhost:7058/swagger/v1/swagger.json");
            return await response.Content.ReadAsStringAsync();
        });
        Console.WriteLine(res.Result);
    }
}