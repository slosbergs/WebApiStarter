using Polly;
using Polly.Extensions.Http;

namespace WebApiStarter.Infrastructure.Polly
{
    public static class PollyPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetDefaultRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromMilliseconds(200));
        }


    }
}
