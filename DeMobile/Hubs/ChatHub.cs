using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DeMobile.Concrete;
using DeMobile.Models;
using DeMobile.Models.AppModel;
using DeMobile.Services;
using Microsoft.AspNet.SignalR;
using Oracle.ManagedDataAccess.Client;

namespace DeMobile.Hubs
{
    public class ChatHub : Hub
    {
        private Database oracle;
        private User user;
        public override Task OnConnected()
        {
            string username = Context.User.Identity.Name;
            string connectId = Context.ConnectionId;
            Clients.Caller.Login($"Your ConnectionId is {connectId}");       
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            string username = Context.User.Identity.Name;
            string connectId = Context.ConnectionId;
            return base.OnDisconnected(stopCalled);
        }
        public void registerContext(string device_id)
        {
            user = new User();
            try
            {
                var device = user.checkCurrentDevice(device_id);
                if (device != null && device.device_status == "ACT")
                {
                    var connectId = Context.ConnectionId;
                    oracle = new Database();
                    List<OracleParameter> parameter = new List<OracleParameter>();
                    parameter.Add(new OracleParameter("conn_id", connectId));
                    parameter.Add(new OracleParameter("device_id", device_id));
                    oracle.SqlExecuteWithParams(SqlCmd.Notification.updateConnectId, parameter);
                    oracle.OracleDisconnect();
                    var context = GlobalHost.ConnectionManager.GetHubContext<MonitorHub>();
                    context.Clients.All.Monitor(new { ConnectionId = Context.ConnectionId, DeviceId = device_id, Message = "Updated", Time = DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToShortTimeString() });
                    context.Clients.All.UserOnline(Context.ConnectionId);
                }
                else
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<MonitorHub>();
                    context.Clients.All.Monitor(new { ConnectionId = Context.ConnectionId, DeviceId = device_id, Message = "Not found device!" });
                }
            }
            catch(Exception e)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<MonitorHub>();
                context.Clients.All.Monitor(new { ConnectionId = Context.ConnectionId, DeviceId = device_id, Message = e.Message });
            }
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
        public void SendSmsByConnId(m_SMS010 value)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>
            {
                new OracleParameter("cust_no", value.CUST_NO)
            };
            var reader = oracle.SqlQueryWithParams(SqlCmd.User.getConnIdByCustNo, parameter);
            reader.Read();
            var connectionId = (string)reader["CONN_ID"];
            context.Clients.Client(connectionId).sms(value);
            reader.Dispose();
            oracle.OracleDisconnect();
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