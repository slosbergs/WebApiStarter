using Elastic.CommonSchema;
using WebApiStarter.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace WebApiStarter.Controllers;

[ApiController]
[Route("[controller]")]
public class FailingController : ControllerBase
{
    private readonly ILogger<FailingController> _logger;
    private readonly IFailingHttpClient _failingHttpClient;

    public FailingController(ILogger<FailingController> logger, IFailingHttpClient failingHttpClient)
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

    [HttpGet("testException")]
    public async Task<ActionResult> TestRetryWithUnhandledException()
    {
    
            throw new Exception("bar");
            return Ok("foo");
       
    }
}
