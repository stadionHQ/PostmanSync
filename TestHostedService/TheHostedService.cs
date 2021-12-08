namespace TestHostedService;

public class TheHostedService: IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var client = new HttpClient();
        var result = await client.GetAsync("https://localhost:7058/swagger/v1/swagger.json", cancellationToken);
        var content = await result.Content.ReadAsStringAsync(cancellationToken);
        Console.WriteLine("IN THE HOSTED SERVICE");
        Console.WriteLine(content);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}