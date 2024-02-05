
using Serilog.Events;
using Serilog;
using Elastic.CommonSchema.Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace WebApiStarter.AppConfig
{
    public static class AppConfig
    {
        public static void SetUpSerilog(HostBuilderContext context, LoggerConfiguration configuration)
        {
            configuration
             .Enrich.FromLogContext()
             .Enrich.WithProperty("system.environment", context.HostingEnvironment.EnvironmentName)
             .Enrich.WithProperty("system.service", context.HostingEnvironment.ApplicationName)
             .MinimumLevel.Debug()
             .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
             .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
             .MinimumLevel.Override("CorrelationId.CorrelationIdMiddleware", LogEventLevel.Information)
             .MinimumLevel.Override("System", LogEventLevel.Information)
             .Filter.ByExcluding(logEvent =>
             {
                 var requestPath = logEvent.Properties.GetValueOrDefault("RequestPath")?.ToString();
                 return requestPath == null ? false : requestPath!.Contains("/health/", StringComparison.OrdinalIgnoreCase);
             })

             //.WriteTo.Async(wt => wt.Console(new EcsTextFormatter(new() { IncludeHost = false, IncludeUser = false, IncludeProcess = false })))
             //.WriteTo.Async(wt => wt.Console(new RenderedCompactJsonFormatter()))
             .WriteTo.Async(wt => wt.Console(theme: AnsiConsoleTheme.Literate))
             ;
        }

    }
}
