namespace Husa.Uploader.Core.Interfaces
{
    using Microsoft.AspNetCore.SignalR.Client;

    public interface ISignalRConnectionService
    {
        HubConnection GetConnectionAsync();
    }
}
