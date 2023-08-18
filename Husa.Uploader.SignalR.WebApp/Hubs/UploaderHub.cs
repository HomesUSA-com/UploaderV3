namespace Husa.Uploader.SignalR.WebApp.Hubs
{
    using Husa.Uploader.Crosscutting.Models;
    using Microsoft.AspNetCore.SignalR;

    public class UploaderHub : Hub
    {
        private Dictionary<string, Dictionary<string, Item>> itemsDictionary = new();
        private Dictionary<string, Item> selectedItems = new();

        public Dictionary<string, Dictionary<string, Item>> ItemsDictionary => this.itemsDictionary;

        public Dictionary<string, Item> SelectedItems => this.selectedItems;

        public async Task SendMessage(string user, string message)
        {
            await this.Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public void SendSelectedItem(string userName, Item item)
        {
            bool hasChanged = false;
            // This code helps to clean the Dictionaries
            if (!this.ItemsDictionary.ContainsKey(DateTime.Now.ToShortDateString()))
            {
                this.selectedItems = new Dictionary<string, Item>();
                this.itemsDictionary = new Dictionary<string, Dictionary<string, Item>>();
                this.ItemsDictionary.Add(DateTime.Now.ToShortDateString(), this.SelectedItems);
            }

            if (!this.SelectedItems.ContainsKey(userName))
            {
                this.SelectedItems.Add(userName, item);
                hasChanged = true;
            }
            else
            {
                Item itemStored = this.SelectedItems[userName];
                if (itemStored.SelectedItemID != item.SelectedItemID || itemStored.Status != item.Status)
                {
                    this.SelectedItems.Remove(userName);
                    this.SelectedItems.Add(userName, item);
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                this.Clients.All.SendAsync("broadcastMessage", userName, item);
            }
        }

        public void GetWorkerItems()
        {
            this.Clients.All.SendAsync("updateWorkerList", this.SelectedItems);
        }
    }
}
