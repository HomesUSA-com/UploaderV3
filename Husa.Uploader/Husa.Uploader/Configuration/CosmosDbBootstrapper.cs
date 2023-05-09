namespace Husa.Uploader.Configuration
{
    using Husa.Uploader.Crosscutting.Options;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Fluent;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public static class CosmosDbBootstrapper
    {
        public static IServiceCollection ConfigureCosmosDb(this IServiceCollection services)
        {
            services
                .AddOptions<CosmosDbOptions>()
                .Configure<IConfiguration>((settings, config) => config.GetSection(CosmosDbOptions.Section).Bind(settings))
                .ValidateOnStart();

            services.AddSingleton<CosmosClient>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<CosmosDbOptions>>().Value;

                var cosmoClient = new CosmosClientBuilder(options.Endpoint, options.AuthToken)
                .WithCustomSerializer(BuildCosmosJsonDotNetSerializer())
                .Build();

                return cosmoClient;
            });

            return services;
        }

        public static CosmosJsonDotNetSerializer BuildCosmosJsonDotNetSerializer()
        {
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            };

            var jsonConverter = new StringEnumConverter(
                namingStrategy: new CamelCaseNamingStrategy(),
                allowIntegerValues: true);
            jsonSerializerSettings.Converters.Add(jsonConverter);
            return new(jsonSerializerSettings);
        }
    }
}
