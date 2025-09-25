namespace Husa.Uploader.Api.ServiceBus.Subscribers
{
    using System;
    using Husa.Uploader.Crosscutting.Options;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.Options;

    public class ResidentialRequestSubscriber : IResidentialRequestSubscriber
    {
        public ResidentialRequestSubscriber(IOptions<ServiceBusSettings> options)
        {
            var busOptions = options is null ? throw new ArgumentNullException(nameof(options)) : options.Value;
            this.Client = new SubscriptionClient(busOptions.ConnectionString, topicPath: busOptions.UploaderService.TopicName, busOptions.UploaderService.SubscriptionName);
        }

        public ISubscriptionClient Client { get; set; }
    }
}
