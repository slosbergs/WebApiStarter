using CloudNative.CloudEvents;
using KafkaFlow;
using KafkaFlow.Producers;
using WebApiStarter.Domain.Events;
using WebApiStarter.Domain.Model;
using static Confluent.Kafka.ConfigPropertyNames;

namespace WebApiStarter.Services
{
    public class EventProducer
    {
        private readonly IMessageProducer _producer;
        private readonly ILogger<EventProducer> _logger;

        public EventProducer(ILogger<EventProducer> logger, IProducerAccessor producerAccessor)
        {
            this._producer = producerAccessor.GetProducer("say-hello");
            this._logger = logger;
        }

        public async Task ProduceAsync(TodoItemCreatedNotification eventMessage)
        {
            CloudEvent cloudEvent = new CloudEventBuilder()
                .WithSource("demo.todo.noifications")
                .WithData(eventMessage)
                .Build();

            var deliveryReport = await _producer.ProduceAsync(null, cloudEvent);
            _logger.LogInformation("produced event {type} to {topic}:{partition}:{offset}", cloudEvent.Type, deliveryReport.Topic, deliveryReport.Partition, deliveryReport.Offset);
        }
    }
}
