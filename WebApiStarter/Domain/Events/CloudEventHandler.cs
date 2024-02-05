using CloudNative.CloudEvents;
using KafkaFlow;

namespace WebApiStarter.Domain.Events
{
    public class CloudEventHandler(ILogger<CloudEventHandler> logger) : IMessageHandler<CloudEvent>
    {
        public async Task Handle(IMessageContext context, CloudEvent message)
        {
            logger.LogInformation("received message {partition}:{offset} -- {eventMessage}",
                context.ConsumerContext.Partition, context.ConsumerContext.Offset, message.ToString());
        }
    }
}
