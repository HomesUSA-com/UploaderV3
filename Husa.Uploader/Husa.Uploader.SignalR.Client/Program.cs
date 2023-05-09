// See https://aka.ms/new-console-template for more information
using Microsoft.AspNet.SignalR.Client;

await Do();

static async Task Do()
{
    const string url = "https://husauploadersignalrwebapp.azurewebsites.net//";
    var connection = new HubConnection(url);
    var echo = connection.CreateHubProxy("uploaderHub");
    echo.On<Dictionary<string, string>>("broadcastSelectedList", response =>
    {

    });
    await connection.Start();
    await echo.Invoke<string>("Send", "Bot", "Hello, I'm a bot. Mi name is grio!");
    Console.WriteLine("Press Enter to exit");
    Console.ReadLine();
}
