namespace Husa.Uploader.Core.Services
{
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Options;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.Options;

    public class SignalRConnectionService : ISignalRConnectionService
    {
        private readonly string url;
        public SignalRConnectionService(IOptions<ApplicationOptions> options)
        {
            this.url = options.Value.SignalRURLServer;
        }

        public HubConnection GetConnectionAsync()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl($"{this.url}/uploaderHub")
                .Build();

            return connection;
        }
    }
}
