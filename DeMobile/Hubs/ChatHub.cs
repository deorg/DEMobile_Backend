using System;
using System.Collections.Generic;
using System.Data;
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
        private Database oracle = new Database();
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
        public void receiveMessage(int sms010PK)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    //if (conn.State == System.Data.ConnectionState.Open)
                    //    conn.Close();

                    conn.Open();
                    //conn.ConnectionString = Constants.OracleDb.Development.conString;
                    using (var cmd = new OracleCommand(SqlCmd.Notification.markToRecieve, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.Add(new OracleParameter("sms010pk", sms010PK));
                        cmd.ExecuteNonQueryAsync();
                        cmd.Dispose();
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            //oracle = new Database();
            //try
            //{
            //    List<OracleParameter> parameter = new List<OracleParameter>
            //    {
            //        new OracleParameter("sms010pk", sms010PK)
            //    };
            //    oracle.SqlExecuteWithParams(SqlCmd.Notification.markToRecieve, parameter);
            //    oracle.OracleDisconnect();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
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
                                var context = GlobalHost.ConnectionManager.GetHubContext<MonitorHub>();
                                context.Clients.All.Monitor(new { ConnectionId = Context.ConnectionId, DeviceId = device_id, Message = "Updated", Time = DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToShortTimeString() });
                                context.Clients.All.UserOnline(Context.ConnectionId);
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
            catch(Exception e)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<MonitorHub>();
                context.Clients.All.Monitor(new { ConnectionId = Context.ConnectionId, DeviceId = device_id, Message = e.Message, Time = DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToShortTimeString() });
            }
            //try
            //{
            //    var device = user.checkCurrentDevice(device_id);
            //    if (device != null && device.device_status == "ACT")
            //    {
            //        var connectId = Context.ConnectionId;
            //        oracle = new Database();
            //        List<OracleParameter> parameter = new List<OracleParameter>();
            //        parameter.Add(new OracleParameter("conn_id", connectId));
            //        parameter.Add(new OracleParameter("device_id", device_id));
            //        oracle.SqlExecuteWithParams(SqlCmd.Notification.updateConnectId, parameter);
            //        oracle.OracleDisconnect();
            //        var context = GlobalHost.ConnectionManager.GetHubContext<MonitorHub>();
            //        context.Clients.All.Monitor(new { ConnectionId = Context.ConnectionId, DeviceId = device_id, Message = "Updated", Time = DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToShortTimeString() });
            //        context.Clients.All.UserOnline(Context.ConnectionId);
            //    }
            //    else
            //    {
            //        var context = GlobalHost.ConnectionManager.GetHubContext<MonitorHub>();
            //        context.Clients.All.Monitor(new { ConnectionId = Context.ConnectionId, DeviceId = device_id, Message = "Not found device!" });
            //    }
            //}
            //catch (Exception e)
            //{
            //    var context = GlobalHost.ConnectionManager.GetHubContext<MonitorHub>();
            //    context.Clients.All.Monitor(new { ConnectionId = Context.ConnectionId, DeviceId = device_id, Message = e.Message });
            //}
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
                if (temp != null)
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    context.Clients.Client(temp.ConnectionId).serversend(new { username = "Admin", message = msg });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void sendSmsByCustNo(int cust_no)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            MonitorHub monitor = new MonitorHub();
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.User.getSmsWithConnId, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add(new OracleParameter("cust_no", cust_no));
                        var reader = cmd.ExecuteReader();
                        List<m_sendSms> data = new List<m_sendSms>();
                        while (reader.Read())
                        {
                            data.Add(new m_sendSms
                            {
                                SMS010_PK = Int32.Parse(reader["SMS010_PK"].ToString()),
                                CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                                CONN_ID = reader["CONN_ID"] == DBNull.Value ? string.Empty : reader["CONN_ID"].ToString(),
                                DEVICE_STATUS = reader["DEVICE_STATUS"].ToString(),
                                SMS_NOTE = reader["SMS_NOTE"] == DBNull.Value ? string.Empty : reader["SMS_NOTE"].ToString(),
                                CON_NO = reader["CON_NO"] == DBNull.Value ? string.Empty : reader["CON_NO"].ToString(),
                                SMS_TIME = (DateTime)reader["SMS_TIME"],
                                SENDER = reader["SENDER"] == DBNull.Value ? 0 : Int32.Parse(reader["SENDER"].ToString()),
                                SENDER_TYPE = reader["SENDER_TYPE"].ToString(),
                                SMS010_REF = reader["SMS010_REF"] == DBNull.Value ? 0 : Int32.Parse(reader["SMS010_REF"].ToString()),
                                READ_STATUS = reader["READ_STATUS"].ToString()
                            });
                        }
                        if(data.Count != 0)
                        {
                            List<m_SMS010> sms = new List<m_SMS010>();
                            foreach(var s in data)
                            {
                                var temp = new m_SMS010
                                {
                                    SMS010_PK = s.SMS010_PK,
                                    CUST_NO = s.CUST_NO,
                                    CON_NO = s.CON_NO,
                                    SMS_NOTE = s.SMS_NOTE,
                                    SMS_TIME = DateTime.Now,
                                    SENDER = s.SENDER,
                                    SENDER_TYPE = s.SENDER_TYPE,
                                    SMS010_REF = s.SMS010_REF,
                                    READ_STATUS = s.READ_STATUS
                                };
                                var connectionId = s.CONN_ID;
                                monitor.sendMessage(string.Empty, s.CONN_ID, new { cust_no = cust_no }, new { request_status = "SUCCESS", desc = "Admin ส่งข้อความ", data = temp });
                                context.Clients.Client(connectionId).sms(temp);
                                context.Clients.Client(s.CONN_ID).sms(temp);
                            }
                        }
                        reader.Dispose();
                        cmd.Dispose();
                        //cmd.Parameters.Add(new OracleParameter("cust_no", cust_no));
                        //var reader = cmd.ExecuteReader();
                        //reader.Read();
                        //List<m_SMS010> data = new List<m_SMS010>();
                        //while (reader.Read())
                        //{
                        //    data.Add(new m_SMS010
                        //    {
                        //        SMS010_PK = Int32.Parse(reader["SMS010_PK"].ToString()),
                        //        CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                        //        CON_NO = reader["CON_NO"] == DBNull.Value ? string.Empty : (string)reader["CON_NO"],
                        //        SMS_NOTE = reader["SMS_NOTE"] == DBNull.Value ? string.Empty : (string)reader["SMS_NOTE"],
                        //        SMS_TIME = (DateTime)reader["SMS_TIME"],
                        //        SENDER = reader["SENDER"] == DBNull.Value ? null : (int?)Int32.Parse(reader["SENDER"].ToString()),
                        //        SENDER_TYPE = (string)reader["SENDER_TYPE"],
                        //        SMS010_REF = reader["SMS010_REF"] == DBNull.Value ? null : (int?)Int32.Parse(reader["SMS010_REF"].ToString()),
                        //        READ_STATUS = (string)reader["READ_STATUS"]
                        //    });
                        //}
                        //if(data.Count != 0)
                        //{
                        //    using (var cmd2 = new OracleCommand(SqlCmd.User.getConnIdByCustNo, conn) { CommandType = System.Data.CommandType.Text })
                        //    {
                        //        foreach(var msg in data)
                        //        {
                        //            cmd2.Parameters.Clear();
                        //            cmd2.Parameters.Add(new OracleParameter("cust_no", msg.CUST_NO));
                        //            var reader2 = cmd2.ExecuteReader();
                        //            reader2.Read();
                        //            if (reader2.HasRows)
                        //            {
                        //                if(reader2["CONN_ID"] != DBNull.Value)
                        //                {
                        //                    var connectionId = reader2["CONN_ID"] == DBNull.Value ? string.Empty : (string)reader2["CONN_ID"];
                        //                    context.Clients.Client(connectionId).sms(msg);
                        //                }                                       
                        //            }
                        //        }
                        //    }
                        //}
                        //reader.Dispose();
                        //cmd.Dispose();
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public void sendSmsAll()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            MonitorHub monitor = new MonitorHub();
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd3 = new OracleCommand(SqlCmd.Log.logTest, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd3.Parameters.Add(new OracleParameter("msg", "เริ่มดึงข้อความ"));
                        cmd3.ExecuteNonQueryAsync();
                        cmd3.Dispose();
                    }
                    using (var cmd = new OracleCommand(SqlCmd.User.getAllSmsWithConnId, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        var reader = cmd.ExecuteReader();
                        List<m_sendSms> data = new List<m_sendSms>();
                        while (reader.Read())
                        {
                            data.Add(new m_sendSms
                            {
                                SMS010_PK = Int32.Parse(reader["SMS010_PK"].ToString()),
                                CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                                CONN_ID = reader["CONN_ID"] == DBNull.Value ? string.Empty : reader["CONN_ID"].ToString(),
                                DEVICE_STATUS = reader["DEVICE_STATUS"].ToString(),
                                SMS_NOTE = reader["SMS_NOTE"] == DBNull.Value ? string.Empty : reader["SMS_NOTE"].ToString(),
                                CON_NO = reader["CON_NO"] == DBNull.Value ? string.Empty : reader["CON_NO"].ToString(),
                                SMS_TIME = (DateTime)reader["SMS_TIME"],
                                SENDER = reader["SENDER"] == DBNull.Value ? 0 : Int32.Parse(reader["SENDER"].ToString()),
                                SENDER_TYPE = reader["SENDER_TYPE"].ToString(),
                                SMS010_REF = reader["SMS010_REF"] == DBNull.Value ? 0 : Int32.Parse(reader["SMS010_REF"].ToString()),
                                READ_STATUS = reader["READ_STATUS"].ToString()
                            });
                        }
                        using (var cmd3 = new OracleCommand(SqlCmd.Log.logTest, conn) { CommandType = System.Data.CommandType.Text })
                        {
                            cmd3.Parameters.Add(new OracleParameter("msg", "ดึงข้อความเสร็จ"));
                            cmd3.ExecuteNonQueryAsync();
                            cmd3.Dispose();
                        }
                        using (var cmd3 = new OracleCommand(SqlCmd.Log.logTest, conn) { CommandType = System.Data.CommandType.Text })
                        {
                            cmd3.Parameters.Add(new OracleParameter("msg", "เริ่มส่งข้อความ"));
                            cmd3.ExecuteNonQueryAsync();
                            cmd3.Dispose();
                        }
                        if (data.Count != 0)
                        {
                            List<m_SMS010> sms = new List<m_SMS010>();
                            //using (var cmd3 = new OracleCommand(SqlCmd.Log.logTest, conn) { CommandType = System.Data.CommandType.Text })
                            //{
                            //    cmd3.Parameters.Add(new OracleParameter("msg", OracleDbType.Varchar2));                             
                            //    foreach (var s in data)
                            //    {
                            //        var temp = new m_SMS010
                            //        {
                            //            SMS010_PK = s.SMS010_PK,
                            //            CUST_NO = s.CUST_NO,
                            //            CON_NO = s.CON_NO,
                            //            SMS_NOTE = s.SMS_NOTE,
                            //            SMS_TIME = DateTime.Now,
                            //            SENDER = s.SENDER,
                            //            SENDER_TYPE = s.SENDER_TYPE,
                            //            SMS010_REF = s.SMS010_REF,
                            //            READ_STATUS = s.READ_STATUS
                            //        };
                            //        cmd3.Parameters[0].Value = temp.SMS_NOTE;
                            //        cmd3.ExecuteNonQueryAsync();
                            //        //using (var cmd3 = new OracleCommand(SqlCmd.Log.logTest, conn) { CommandType = System.Data.CommandType.Text })
                            //        //{
                            //        //    cmd3.Parameters.Add(new OracleParameter("msg", temp.SMS_NOTE));
                            //        //    cmd3.ExecuteNonQueryAsync();
                            //        //    cmd3.Dispose();
                            //        //}

                            //        //monitor.sendMessage(string.Empty, s.CONN_ID, new { cust_no = s.CUST_NO }, new { request_status = "SUCCESS", desc = "Admin ส่งข้อความ", data = temp });
                            //        //context.Clients.Client(s.CONN_ID).sms(temp);
                            //    }
                            //    cmd3.Dispose();
                            //}
                            foreach (var s in data)
                            {
                                var temp = new m_SMS010
                                {
                                    SMS010_PK = s.SMS010_PK,
                                    CUST_NO = s.CUST_NO,
                                    CON_NO = s.CON_NO,
                                    SMS_NOTE = s.SMS_NOTE,
                                    SMS_TIME = DateTime.Now,
                                    SENDER = s.SENDER,
                                    SENDER_TYPE = s.SENDER_TYPE,
                                    SMS010_REF = s.SMS010_REF,
                                    READ_STATUS = s.READ_STATUS
                                };
                                //using (var cmd3 = new OracleCommand(SqlCmd.Log.logTest, conn) { CommandType = System.Data.CommandType.Text })
                                //{
                                //    cmd3.Parameters.Add(new OracleParameter("msg", temp.SMS_NOTE));
                                //    cmd3.ExecuteNonQueryAsync();
                                //    cmd3.Dispose();
                                //}

                                monitor.sendMessage(string.Empty, s.CONN_ID, new { cust_no = s.CUST_NO }, new { request_status = "SUCCESS", desc = "Admin ส่งข้อความ", data = temp });
                                context.Clients.Client(s.CONN_ID).sms(temp);
                            }

                            using (var cmd2 = new OracleCommand(SqlCmd.Notification.markToSent, conn) { CommandType = System.Data.CommandType.Text })
                            {
                                cmd2.ExecuteNonQueryAsync();
                                cmd2.Parameters.Clear();               
                            }
                        }
                        //using (var cmd3 = new OracleCommand(SqlCmd.Log.logTest, conn) { CommandType = System.Data.CommandType.Text })
                        //{
                        //    cmd3.Parameters.Add(new OracleParameter("msg", "insert ข้อความลง log_test เสร็จ"));
                        //    cmd3.ExecuteNonQueryAsync();
                        //    cmd3.Dispose();
                        //}
                        using (var cmd3 = new OracleCommand(SqlCmd.Log.logTest, conn) { CommandType = System.Data.CommandType.Text })
                        {
                            cmd3.Parameters.Add(new OracleParameter("msg", "ส่งข้อความเสร็จ"));
                            cmd3.ExecuteNonQueryAsync();
                            cmd3.Dispose();
                        }
                        reader.Dispose();
                        cmd.Dispose();
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public void SendSmsByConnId(m_SMS010 value)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    //if (conn.State == System.Data.ConnectionState.Open)
                    //    conn.Close();

                    conn.Open();
                    //conn.ConnectionString = Constants.OracleDb.Development.conString;
                    using (var cmd = new OracleCommand(SqlCmd.User.getConnIdByCustNo, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.Add(new OracleParameter("cust_no", value.CUST_NO));
                        var reader = cmd.ExecuteReaderAsync();
                        reader.Result.Read();
                        if (reader.Result.HasRows)
                        {                                   
                            var connectionId = reader.Result["CONN_ID"] == DBNull.Value ? string.Empty : (string)reader.Result["CONN_ID"];
                            context.Clients.Client(connectionId).sms(value);
                        }
                        reader.Dispose();
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();                 
                }
            }          
            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            //oracle = new Database();
            //List<OracleParameter> parameter = new List<OracleParameter>
            //{
            //    new OracleParameter("cust_no", value.CUST_NO)
            //};
            //var reader = oracle.SqlQueryWithParams(SqlCmd.User.getConnIdByCustNo, parameter);
            //reader.Read();
            //var connectionId = (string)reader["CONN_ID"];
            //context.Clients.Client(connectionId).sms(value);
            //reader.Dispose();
            //oracle.OracleDisconnect();
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