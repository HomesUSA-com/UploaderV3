using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace UploaderSignalR
{
    [HubName("uploaderHub")]
    public partial class UploaderHub : Hub
    {
        public static Dictionary<string, Dictionary<string, Item>> DictionarySupport = new Dictionary<string, Dictionary<string, Item>>();
        
        public static Dictionary<string, Item> SelectedItems = new Dictionary<string, Item>();

        public void Send(string userName, string selectionID)
        {
            Clients.All.broadcastMessage(userName, selectionID);
        }

        public void SendSelectedItem(string userName, Item item)
        {
            bool hasChanged = false;
            // This code helps to clean the Dictionaries
            if (!DictionarySupport.ContainsKey(DateTime.Now.ToShortDateString()))
            {
                SelectedItems = new Dictionary<string, Item>();
                DictionarySupport = new Dictionary<string, Dictionary<string, Item>>();
                DictionarySupport.Add(DateTime.Now.ToShortDateString(), SelectedItems);
            }
            
            if (!SelectedItems.ContainsKey(userName))
            {
                SelectedItems.Add(userName, item);
                hasChanged = true;
            }
            else
            {
                Item itemStored = SelectedItems[userName];
                if (itemStored.SelectedItemID != item.SelectedItemID || itemStored.Status != item.Status)
                {
                    SelectedItems.Remove(userName);
                    SelectedItems.Add(userName, item);
                    hasChanged = true;
                }
            }

            if(hasChanged)
            {
                // 1. Send to Others all Selected List
                //Clients.All.broadcastSelectedList(SelectedItems);
                Clients.All.broadcastSelectedList(userName, item);

                // 2. Send to test page "Chat" the current mesage
                Clients.All.broadcastMessage(userName, item.SelectedItemID + ". Status: " + item.Status);

                // 3. Send to caller user the current selected to refresh local dictionary
                //Clients.Caller.updateCurrentSelectedList(item.SelectedItemID, item.Status);
            }
        }

        public void GetWorkerItems()
        {
            Clients.All.updateWorkerList(SelectedItems);
        }

    }
}