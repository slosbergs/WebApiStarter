using CorrelationId;
using CorrelationId.DependencyInjection;
using CorrelationId.HttpClient;
using Elastic.CommonSchema;
using Elastic.CommonSchema.Serilog;
using LoggingWithCorrelationIdDemo.Infrastructure;
using LoggingWithCorrelationIdDemo.Infrastructure.Polly;
using LoggingWithCorrelationIdDemo.Swagger;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;


Serilog.Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("CorrelationId.CorrelationIdMiddleware", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Information)
    .Filter.ByExcluding(logEvent =>
    {
        var requestPath = logEvent.Properties.GetValueOrDefault("RequestPath")?.ToString();
        return requestPath.Contains("/health/", StringComparison.OrdinalIgnoreCase); 
    })

    //.WriteTo.Async(wt => wt.Console(new EcsTextFormatter(new() { IncludeHost = false, IncludeUser = false })))
    //.WriteTo.Async(wt => wt.Console(new RenderedCompactJsonFormatter()))
    .WriteTo.Async(wt => wt.Console(theme: AnsiConsoleTheme.Literate))
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDefaultCorrelationId(options =>
{
    options.CorrelationIdGenerator = () => Guid.NewGuid().ToString();
    options.AddToLoggingScope = true;
    options.LoggingScopeKey = CorrelationIdAttribute.Name;
    options.EnforceHeader = false;
    options.IgnoreRequestHeader = false;
    options.IncludeInResponse = true;
    options.RequestHeader = CorrelationIdAttribute.Name;
    options.UpdateTraceIdentifier = true;
});

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Host.UseSerilog();


// register typed clients with correlation id forwarding and retry policies
builder.Services.AddHttpClient<TodoSystemApiClient>(
    client =>
    {
        client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
        client.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-docs");
    })
    .AddCorrelationIdForwarding()
    .AddPolicyHandler(PollyPolicies.GetDefaultRetryPolicy());

builder.Services.AddHttpClient<FailingHttpClient>()
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

app.Run();
