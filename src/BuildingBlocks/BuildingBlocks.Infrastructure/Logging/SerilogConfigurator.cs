using Microsoft.Extensions.Configuration;
using Serilog;

public static class SerilogConfigurator
{
    public static void Configure(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo
                                                   .File("Logs/log.txt", rollingInterval: RollingInterval.Day)
                                                   .CreateLogger();
        //.ReadFrom.Configuration(configuration)
        //.Enrich.FromLogContext()
        //.Enrich.WithProperty("Application", "ECommerce")
        //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        //.WriteTo.Console(new RenderedCompactJsonFormatter())
        //.WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch
        //    .ElasticsearchSinkOptions(
        //        new Uri(configuration["Elastic:Uri"]!))
        //{
        //    AutoRegisterTemplate = true,
        //    IndexFormat = "ecommerce-logs-{0:yyyy.MM}"
        //})
        //.CreateLogger();
    }
}