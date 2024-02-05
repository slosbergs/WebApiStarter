using CorrelationId;
using CorrelationId.DependencyInjection;
using CorrelationId.HttpClient;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using WebApiStarter.AppConfig;
using WebApiStarter.Domain.Events;
using WebApiStarter.Infrastructure;
using WebApiStarter.Infrastructure.Polly;
using WebApiStarter.Services;
using WebApiStarter.Swagger;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDefaultCorrelationId(options =>
{
    options.CorrelationIdGenerator = () => Guid.NewGuid().ToString();
    options.AddToLoggingScope = true;
    options.LoggingScopeKey = CorrelationIdAttribute.Name;
    options.EnforceHeader = false;
    options.IncludeInResponse = true;
    options.RequestHeader = CorrelationIdAttribute.Name;
});


const string topicName = "sample-topic";
const string producerName = "say-hello";
builder.Services.AddKafka(
    kafka => kafka
        .UseConsoleLog()
        .AddCluster(
            cluster => cluster
                .WithBrokers(new[] { "localhost:19092" })
                
                .AddProducer(
                    producerName,
                    producer => producer
                        .WithProducerConfig(new Confluent.Kafka.ProducerConfig()
                        {
                            MessageTimeoutMs = 5000,
                            RetryBackoffMs = 100 // default
                        })
                        .DefaultTopic(topicName)
                        .AddMiddlewares(m =>
                            m.AddSerializer<CloudEventSerializer>()
                            )
                )
                .AddConsumer(consumer => consumer
                    .Topic(topicName)
                    .WithGroupId("sample-group")
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .AddMiddlewares(middlewares => middlewares
                        .AddDeserializer<CloudEventDeserializer>()
                        .AddTypedHandlers(h =>
                        {
                            h.AddHandler<CloudEventHandler>();
                            h.WhenNoHandlerFound(UnhandledEvent.Handle);
                            
                        })
                    )
                )
        )
);


builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Host.UseSerilog(AppConfig.SetUpSerilog);

builder.Services.AddScoped<EventProducer>();

// register typed clients with correlation id forwarding and retry policies
builder.Services.AddHttpClient<ITodoSystemApiClient, TodoSystemApiClient>(
    client =>
    {
        client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
        client.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-docs");
    })
    .AddCorrelationIdForwarding()
    .AddPolicyHandler(PollyPolicies.GetDefaultRetryPolicy());

builder.Services.AddHttpClient<IFailingHttpClient, FailingHttpClient>()
    .AddCorrelationIdForwarding()
    .AddPolicyHandler(PollyPolicies.GetDefaultRetryPolicy());


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DemoService", Version = "v1", });
    c.OperationFilter<CorrelationIdAttribute>();

});

var app = builder.Build();
app.UseCorrelationId();

// Write streamlined request completion events, instead of the more verbose ones from the framework.
// To use the default framework request logging instead, remove this line and set the "Microsoft"
// level in appsettings.json to "Information".
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    // ref https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0#separate-readiness-and-liveness-probes
    Predicate = healthCheck => healthCheck.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});


var bus = app.Services.CreateKafkaBus();
await bus.StartAsync();

app.Run();
