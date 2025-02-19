using Microsoft.AspNetCore.Mvc;

namespace OcelotApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(ILogger<WeatherController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        // Log the transformed header
        var transformedHeader = Request.Headers["X-Custom-Header"].ToString();
        _logger.LogInformation($"Received transformed header: {transformedHeader}");

        var weatherData = new
        {
            Temperature = 25,
            Description = "Sunny",
            TransformedHeader = transformedHeader
        };

        return Ok(weatherData);
    }
}
