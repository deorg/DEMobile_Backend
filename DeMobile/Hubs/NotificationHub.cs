using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DeMobile.Concrete;
using DeMobile.Services;
using Microsoft.AspNet.SignalR;
using Oracle.ManagedDataAccess.Client;

namespace DeMobile.Hubs
{
    public class NotificationHub : Hub
    {
        private User user;
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
        public void registerContext(string device_id)
        {
            user = new User();
            var connectId = Context.ConnectionId;
            try
            {
                var device = user.checkCurrentDevice(device_id);
                if (device != null && device.device_status == "ACT")
                {
                    using (OracleConnection conn = new OracleConnection(Database.conString))
                    {
                        try
                        {
                            conn.Open();
                            using (var cmd = new OracleCommand(SqlCmd.Notification.updateConnectId, conn) { CommandType = System.Data.CommandType.Text })
                            {
                                cmd.Parameters.Add(new OracleParameter("conn_id", connectId));
                                cmd.Parameters.Add(new OracleParameter("device_id", device_id));
                                cmd.ExecuteNonQueryAsync();
                                cmd.Dispose();
                                //var context = GlobalHost.ConnectionManager.GetHubContext<MonitorHub>();
                                //context.Clients.All.Monitor(new { ConnectionId = Context.ConnectionId, DeviceId = device_id, Message = "Updated", Time = DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToShortTimeString() });
                                //context.Clients.All.UserOnline(Context.ConnectionId);
                            }
                        }
                        finally
                        {
                            conn.Close();
                            conn.Dispose();
                        }
                    }
                }
                else
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<MonitorHub>();
                    context.Clients.All.Monitor(new { ConnectionId = Context.ConnectionId, DeviceId = device_id, Message = "Not found device!", Time = DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToShortTimeString() });
                }
            }
            catch (Exception e)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<MonitorHub>();
                context.Clients.All.Monitor(new { ConnectionId = Context.ConnectionId, DeviceId = device_id, Message = e.Message, Time = DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToShortTimeString() });
            }
        }
    }
}