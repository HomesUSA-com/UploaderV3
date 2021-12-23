using System;
using Elasticsearch.Net.JsonNet;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace Husa.Cargador.Support
{
    internal static class LoggingSupport
    {
        internal const string ApplicationId = "UploaderDesktopClient";

        internal static ILogger GetLogger(string trackingId, Guid correlationId)
        {
            var config = UploaderConfiguration.GetConfiguration();

            return new LoggerConfiguration()
                   .WriteTo.Trace()
                   .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(config.ElasticSearchServerUrl))
                   {
                       IndexFormat = "husalogs-{0:yyyy.MM.dd}",
                       MinimumLogEventLevel = LogEventLevel.Debug,
                       Serializer = new ElasticsearchJsonNetSerializer(),
                       Period = TimeSpan.FromMilliseconds(200)
                   })
                   .CreateLogger()
                   .ForContext("SystemComponent", ApplicationId)
                   .ForContext("SystemComponentVersion", VersionManager.ApplicationBuildDate)
                   .ForContext("TrackingId", trackingId)
                   .ForContext("CorrelationId", correlationId.ToString());
        }
    }
}