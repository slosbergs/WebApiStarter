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
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(retryAttempt * 500));
        }


    }
}
