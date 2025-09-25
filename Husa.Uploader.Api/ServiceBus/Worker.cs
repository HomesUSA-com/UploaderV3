namespace Husa.Uploader.Api.ServiceBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Husa.Extensions.ServiceBus.Extensions;
    using Husa.Uploader.Api.ServiceBus.Handlers;
    using Husa.Uploader.Api.ServiceBus.Subscribers;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class Worker : IHostedService
    {
        private readonly IResidentialRequestSubscriber subscribeToResidentialRequestService;
        private readonly IResidentialRequestMessagesHandler residentialRequestMessagesHandler;
        private readonly ILogger<Worker> logger;

        public Worker(
            IResidentialRequestSubscriber subscribeToResidentialRequestService,
            IResidentialRequestMessagesHandler residentialRequestMessagesHandler,
            ILogger<Worker> logger)
        {
            this.subscribeToResidentialRequestService = subscribeToResidentialRequestService ?? throw new ArgumentNullException(nameof(subscribeToResidentialRequestService));
            this.residentialRequestMessagesHandler = residentialRequestMessagesHandler ?? throw new ArgumentNullException(nameof(residentialRequestMessagesHandler));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.logger.LogInformation("Registering client handlers: {Time}", DateTimeOffset.Now);
                this.subscribeToResidentialRequestService.ConfigureClient(this.residentialRequestMessagesHandler);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to register the client handlers: {Time}", DateTimeOffset.Now);
                throw;
            }

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Closing subcription clients connection: {Time}", DateTimeOffset.Now);
            await this.subscribeToResidentialRequestService.CloseClient();
        }
    }
}
