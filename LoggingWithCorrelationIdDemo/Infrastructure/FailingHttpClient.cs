using LoggingWithCorrelationIdDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;

namespace LoggingWithCorrelationIdDemo.Infrastructure;

/// <summary>
/// https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
/// </summary>
public sealed class FailingHttpClient(
                                        HttpClient httpClient,
                                        ILogger<FailingHttpClient> logger) : IDisposable

{
    [HttpGet]
    public async Task TestRetry()
    {
        logger.LogDebug("testing polly retry...");
        var foo = await httpClient.GetAsync("https://httpstat.us/500");
        foo.EnsureSuccessStatusCode();
        logger.LogDebug("finished polly retry...");
    }

    public void Dispose() => httpClient?.Dispose();
}