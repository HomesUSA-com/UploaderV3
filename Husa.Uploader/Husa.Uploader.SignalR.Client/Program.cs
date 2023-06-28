// See https://aka.ms/new-console-template for more information
using Microsoft.AspNet.SignalR.Client;

await Do();

static async Task Do()
{
#pragma warning disable S1075 // URIs should not be hardcoded
    var connection = new HubConnection(url: "https://husauploadersignalrwebapp.azurewebsites.net//");
#pragma warning restore S1075 // URIs should not be hardcoded
    var echo = connection.CreateHubProxy("uploaderHub");
    echo.On<Dictionary<string, string>>(
        eventName: "broadcastSelectedList",
        onData: response =>
        {
        });
    await connection.Start();
    await echo.Invoke<string>(method: "Send", "Bot", "Hello, I'm a bot. Mi name is grio!");
    Console.WriteLine("Press Enter to exit");
    Console.ReadLine();
}
