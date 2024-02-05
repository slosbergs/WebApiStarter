using KafkaFlow;
using Microsoft.Extensions.Logging;

namespace WebApiStarter.Domain.Events
{
    public class UnhandledEvent
    {
        internal static void Handle(IMessageContext context)
        {
            ILogger<UnhandledEvent> logger = context.DependencyResolver.Resolve<ILogger<UnhandledEvent>>();
            logger.LogWarning("unhandled message {message}", context.Message.Value);
        }
    }
}
