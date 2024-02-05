using CloudNative.CloudEvents;
namespace WebApiStarter.Domain.Events;


public class CloudEventBuilder
{
    private CloudEvent cloudEvent;

    public CloudEventBuilder()
    {
        cloudEvent = new CloudEvent()
        {
            Id = Guid.NewGuid().ToString(),
            DataContentType = "application/json",
            Time = DateTime.Now
        };
    }

    // Setters for optional fields
    public CloudEventBuilder WithSource(string source)
    {
        cloudEvent.Source = new Uri(source, UriKind.Relative);
        return this;
    }

    public CloudEventBuilder WithData(object data)
    {
        cloudEvent.Data = data;
        cloudEvent.Type = data.GetType().FullName;
        return this;
    }

    public CloudEvent Build() { return cloudEvent.Validate(); }
}
