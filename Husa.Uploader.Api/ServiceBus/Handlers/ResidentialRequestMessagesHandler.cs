namespace Husa.Uploader.Api.ServiceBus.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Husa.Extensions.Common.Enums;
    using Husa.Extensions.ServiceBus.Extensions;
    using Husa.Extensions.ServiceBus.Services;
    using Husa.Uploader.Api.ServiceBus.Helpers;
    using Husa.Uploader.Api.ServiceBus.Subscribers;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.ServiceBus.Contracts;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class ResidentialRequestMessagesHandler : MessagesHandler<ResidentialRequestMessagesHandler>, IResidentialRequestMessagesHandler
    {
        private readonly UploaderUserSettings uplaoderUserOptions;

        public ResidentialRequestMessagesHandler(
            IResidentialRequestSubscriber residentialRequestSubscriber,
            IServiceScopeFactory scopeFactory,
            IMapper mapper,
            IOptions<UploaderUserSettings> options,
            ILogger<ResidentialRequestMessagesHandler> logger)
            : base(residentialRequestSubscriber, scopeFactory, logger)
        {
            this.uplaoderUserOptions = options is null ? throw new ArgumentNullException(nameof(options)) : options.Value;
        }

        public override async Task ProcessMessageAsync(Message message, IServiceScope scope, CancellationToken cancellationToken)
        {
            HandlerHelper.ConfigureUserAgent(scope);
            HandlerHelper.ConfigureContext(message, this.uplaoderUserOptions, scope);
            this.Logger.LogDebug("Deserializing message {MessageId}.", message.MessageId);
            var receivedMessage = message.DeserializeMessage();
            switch (receivedMessage)
            {
                case ResidentialRequestMessage residentialRequestMessage:
                    await ProcessResidentialMessage(residentialRequestMessage);
                    break;
                default:
                    this.Logger.LogWarning("Message type not recognized for message {MessageId}.", message.MessageId);
                    break;
            }

            Task ProcessResidentialMessage(ResidentialRequestMessage residentialMessage)
            {
                this.Logger.LogInformation("Processing message for listing with requestId {RequestId}", residentialMessage.RequestId);
                if (residentialMessage.MarketCode != MarketCode.DFW)
                {
                    this.Logger.LogInformation("Market: {MarketCode} is not supported", residentialMessage.MarketCode);
                    return Task.CompletedTask;
                }

                var uploaderServiceFactory = scope.ServiceProvider.GetRequiredService<IUploaderServiceFactory>();
                var currentService = uploaderServiceFactory.GetService(residentialMessage.MarketCode);
                return currentService.FullUploadListing(residentialMessage.MarketCode, residentialMessage.RequestId, cancellationToken);
            }
        }
    }
}
