using Elastic.CommonSchema;
using LoggingWithCorrelationIdDemo.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace LoggingWithCorrelationIdDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class FailingController : ControllerBase
{
    private readonly ILogger<FailingController> _logger;
    private readonly FailingHttpClient _failingHttpClient;

    public FailingController(ILogger<FailingController> logger, FailingHttpClient failingHttpClient)
    {
        _logger = logger;
        _failingHttpClient = failingHttpClient;
    }


    [HttpGet("testRetry")]
    public async Task<ActionResult> TestRetry()
    {
        try
        {
            await _failingHttpClient.TestRetry();
            return Ok();
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, message: e.Message);
            return Problem(e.Message, nameof(TestRetry));
        }
    }
}
