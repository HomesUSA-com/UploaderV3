using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace UploaderSignalRClient
{
    class Program
    {
        const string url = "https://husauploadersignalrwebapp.azurewebsites.net//";
                            
        static void Main(string[] args)
        {
            Do().Wait();
        }

        static async Task Do()
        {    
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
    }
}
