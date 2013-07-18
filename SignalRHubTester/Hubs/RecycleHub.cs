using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;

namespace SignalRHubTester.Hubs
{
    public class RecycleHub : Hub
    {
        private static readonly Dictionary<string, int> OnlineUsers = new Dictionary<string, int>();
        private static int _index;

        public override System.Threading.Tasks.Task OnDisconnected()
        {
            OnlineUsers.Remove(Context.ConnectionId);
            return base.OnDisconnected();
        }

        public void UserOnline()
        {
            var index = _index++;
            OnlineUsers.Add(Context.ConnectionId, index);
            Clients.Client(Context.ConnectionId).Online("You are currently online as user " + index);    
        }

        public void Relay()
        {
            Clients.Client(Context.ConnectionId).Broadcast(DateTime.Now + " - You have successfully pinged the hub.");
            Clients.AllExcept(Context.ConnectionId).Broadcast(DateTime.Now + " - User" + OnlineUsers[Context.ConnectionId] + " has pinged the hub.");
        }
    }
}