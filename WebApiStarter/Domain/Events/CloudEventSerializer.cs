using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using KafkaFlow;
using System.Net.Mime;
using System.Runtime.Serialization;

namespace WebApiStarter.Domain.Events
{
    public class CloudEventSerializer : ISerializer
    {
        private readonly JsonEventFormatter formatter = new();
        public Task SerializeAsync(object message, Stream output, ISerializerContext context)
        {
            ContentType contentType = null;
            var bytes = formatter.EncodeStructuredModeMessage((CloudEvent)message, out contentType);
            if (contentType == null)
            {
                throw new Exception("failed to encode cloud event");
            }
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(bytes.ToArray());
            writer.Flush();

            return Task.CompletedTask;
        }
    }

    public class CloudEventDeserializer : IDeserializer
    {
        private readonly JsonEventFormatter formatter = new();
        private readonly string cloudEventMediaType = "application/cloudevents+json; charset=utf-8";

        public async Task<object> DeserializeAsync(Stream input, Type type, ISerializerContext context)
        {
            return await formatter.DecodeStructuredModeMessageAsync(input, new ContentType(cloudEventMediaType), Array.Empty<CloudEventAttribute>());
        }
    }
}
