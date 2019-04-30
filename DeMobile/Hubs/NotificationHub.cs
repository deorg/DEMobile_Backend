using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace DeMobile.Hubs
{
    public class NotificationHub : Hub
    {
        public override Task OnConnected()
        {
            string connectId = Context.ConnectionId;
            Clients.Caller.Connect($"Your ConnectionId is {connectId}");
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
        public void sendMessageAll(string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.All.notify(message);
        }
    }
}