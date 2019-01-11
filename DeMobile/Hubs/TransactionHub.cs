using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DeMobile.Models.PaymentGateway;
using Microsoft.AspNet.SignalR;

namespace DeMobile.Hubs
{
    public class TransactionHub : Hub
    {
        public override Task OnConnected()
        {
            string username = Context.User.Identity.Name;
            string connectId = Context.ConnectionId;
            Clients.All.Login(new { message = $"{connectId} is online!" });
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            string username = Context.User.Identity.Name;
            string connectId = Context.ConnectionId;
            return base.OnDisconnected(stopCalled);
        }
        public void SendMessage(string connectionId, bool status)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<TransactionHub>();
            context.Clients.Client(connectionId).Notify(new { status = status });
        }
        public void NotifyPayment(PaymentNotify value)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<TransactionHub>();
            context.Clients.All.Notify(value);
        }
    }
}