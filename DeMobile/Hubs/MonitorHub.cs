using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace DeMobile.Hubs
{
    public class MonitorHub : Hub
    {
        public override Task OnConnected()
        {
            string username = Context.User.Identity.Name;
            string connectId = Context.ConnectionId;
            Clients.Caller.Login($"Your ConnectionId is {connectId}, username is {username}");
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            string username = Context.User.Identity.Name;
            string connectId = Context.ConnectionId;
            return base.OnDisconnected(stopCalled);
        }
        public void sendMessage(string url, string host, object request, object result)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<MonitorHub>();
            context.Clients.All.Monitor(new { Url = url, ClientHostName = host, Parameter = request, Result = result, Time = DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToShortTimeString() });
        }
    }
}