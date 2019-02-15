using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DeMobile.Concrete;
using DeMobile.Models.AppModel;
using DeMobile.Models.PaymentGateway;
using DeMobile.Services;
using Microsoft.AspNet.SignalR;
using Oracle.ManagedDataAccess.Client;

namespace DeMobile.Hubs
{
    public class TransactionHub : Hub
    {
        private Database oracle;
        private User user;
        public override Task OnConnected()
        {
            string username = Context.User.Identity.Name;
            string connectId = Context.ConnectionId;
            Clients.Caller.Connect($"Your ConnectionId is {connectId}");
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            string username = Context.User.Identity.Name;
            string connectId = Context.ConnectionId;
            return base.OnDisconnected(stopCalled);
        }
        //public void GetConnectionID()
        //{
        //    string connectId = Context.ConnectionId;
        //    Clients.Client(connectId).Login(new { username = temp.Username, message = msg });
        //}
        public void registerContext(string device_id)
        {
            user = new User();
            var hasDevice = user.checkCurrentDevice(device_id);
            if (hasDevice != null)
            {
                var connectId = Context.ConnectionId;
                oracle = new Database();
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("conn_id", connectId));
                parameter.Add(new OracleParameter("device_id", device_id));
                oracle.SqlExecuteWithParams(SqlCmd.Notification.updateConnectId, parameter);
                oracle.OracleDisconnect();
            }
        }
        public void sendMessage(string type, List<string> connectId, string title, string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<TransactionHub>();
            context.Clients.Clients(connectId).Notify(new { title = title, message = message});
        }
        public void sendSMS(List<m_Notification> value)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<TransactionHub>();
            foreach(var msg in value)
            {
                context.Clients.Client(msg.conn_id).Sms(msg);
            }
        }
        //public void SendMessage(string connectionId, bool status)
        //{
        //    var context = GlobalHost.ConnectionManager.GetHubContext<TransactionHub>();
        //    context.Clients.Client(connectionId).Notify(new { status = status });
        //}
        public void NotifyPayment(PaymentStatusRes value)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<TransactionHub>();
            context.Clients.All.Notify(value);
        }
    }
}