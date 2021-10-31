using Microsoft.AspNetCore.Mvc;

namespace Stadion.PostmanSync.DevApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
            .ToArray();
    }

     [HttpPost]
     public IActionResult Post()
     {
         return BadRequest("Only accepting rain.");
     }
     
     [HttpDelete]
     public IActionResult Delete()
     {
         return Ok("Try moving to Western Australia.");
     }
     
     [HttpPut]
     public IActionResult Update()
     {
         return Ok("Maybe buy an umbrella instead.");
     }
}