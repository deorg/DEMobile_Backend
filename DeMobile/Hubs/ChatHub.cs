using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DeMobile.Models;
using Microsoft.AspNet.SignalR;

namespace DeMobile.Hubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnected()
        {
            string username = Context.User.Identity.Name;
            string connectId = Context.ConnectionId;
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            string username = Context.User.Identity.Name;
            string connectId = Context.ConnectionId;
            return base.OnDisconnected(stopCalled);
        }
        public void setID(string user)
        {
            if (DataRepository.signalrClients == null)
            {
                DataRepository.signalrClients = new List<SignalrClient>();
            }
            SignalrClient obj = new SignalrClient();
            obj.Username = user;
            obj.ConnectionId = Context.ConnectionId;

            SignalrClient temp = DataRepository.signalrClients.Where(w => w.Username.Equals(obj.Username) || w.ConnectionId.Equals(obj.ConnectionId)).FirstOrDefault();
            if (temp == null)
                DataRepository.signalrClients.Add(obj);
            else
            {
                DataRepository.signalrClients.Remove(temp);
                DataRepository.signalrClients.Add(obj);
            }
            var other = DataRepository.signalrClients.Where(w => !w.ConnectionId.Equals(obj.ConnectionId)).ToList();
            List<string> connectIds = new List<string>();
            other.ForEach(e => connectIds.Add(e.ConnectionId));
            Clients.Clients(connectIds).Login(new { username = "Admin", message = $"{obj.Username} is online!" });
        }
        public void clientSend(string msg)
        {
            SignalrClient temp = DataRepository.signalrClients.Where(w => w.ConnectionId.Equals(Context.ConnectionId)).FirstOrDefault();
            var other = DataRepository.signalrClients.Where(w => !w.ConnectionId.Equals(Context.ConnectionId)).ToList();
            List<string> connectIds = new List<string>();
            other.ForEach(e => connectIds.Add(e.ConnectionId));
            Clients.Clients(connectIds).Login(new { username = temp.Username, message = msg });
        }
        public void SendMessage(string msg, string user)
        {
            try
            {
                SignalrClient temp = DataRepository.signalrClients.Where(w => w.Username.Equals(user)).FirstOrDefault();
                if(temp != null)
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    context.Clients.Client(temp.ConnectionId).serversend(new { username = "Admin", message = msg });
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void SendMessageAll(string msg)
        {
            //try
            //{
                var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                context.Clients.All.broadcast(new { username = "Admin", message = msg });
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
        }
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}