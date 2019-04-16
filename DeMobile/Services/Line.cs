using DeMobile.Concrete;
using DeMobile.Hubs;
using DeMobile.Models.AppModel;
using DeMobile.Models.Line;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace DeMobile.Services
{
    public class Line
    {
        private HttpClient client;
        private string host2 = "https://api.line.me";
        //private Database oracle;
        private MonitorHub monitor = new MonitorHub();

        private static bool AllwaysGoodCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }
        private void ConnectLine()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AllwaysGoodCertificate);
            client = new HttpClient();
            client.BaseAddress = new Uri(host2);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer i0NdEjILrlbWZUfsjjGWMRfh8qXtt0USpM87WuIHs5135Qu/fU/kkr0HgX80Q0RJduLr/pU9Q05/ZFMtbX6YhNRZSj75rEbv8nzmzycV+84WzGBJ+L1sTKeq8/lH+i2sBMW4rR1Q4C54U4eOjk6W5AdB04t89/1O/w1cDnyilFU=");
        }
        public m_process getProcessByUserId(string user_id)
        {
            using(OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using(var cmd = new OracleCommand(SqlCmd.Line.getProcessByUserId, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add("line_user_id", user_id);
                        var reader = cmd.ExecuteReader();
                        List<m_process> data = new List<m_process>();
                        while(reader.Read())
                        {
                            data.Add(new m_process
                            {
                                MPAY300_SEQ = Int32.Parse(reader["MPAY300_SEQ"].ToString()),
                                LINE_USER_ID = reader["LINE_USER_ID"] == DBNull.Value ? string.Empty : reader["LINE_USER_ID"].ToString(),
                                PROCESS = reader["PROCESS"].ToString(),
                                PROCESS_STATUS = reader["PROCESS_STATUS"].ToString(),
                                ACTION = reader["ACTION"].ToString(),
                                ACTION_STATUS = reader["ACTION_STATUS"].ToString(),
                                CREATED_TIME = (DateTime)reader["CREATED_TIME"]
                            });
                        }
                        if(data.Count == 0)
                        {
                            reader.Dispose();
                            return null;
                        }
                        cmd.Dispose();
                        return data.LastOrDefault();
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public void sendMessageByToken(string token, string message)
        {
            m_LineReply msg = new m_LineReply();
            msg.replyToken = token;        
            List<m_LineMessage> lmsg = new List<m_LineMessage>();
            lmsg.Add(new m_LineMessage { type = "text", text = message });
            msg.messages = lmsg;

            try
            {
                string postBody = JsonConvert.SerializeObject(msg);
                string result;
                ConnectLine();
                var action = JsonConvert.SerializeObject(msg);
                var content = new StringContent(action, Encoding.UTF8, "application/json");
                var response = client.PostAsync("v2/bot/message/reply", content);

                if (response.Result.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.Result.Content.ReadAsStringAsync().Result);
                    result = response.Result.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine("Error at Create new payment : " + response.Result.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void setRegister(string user_id, string process_status, string action, string action_status, string note)
        {
            using(OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.Line.setRegister, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add("line_user_id", user_id);
                        cmd.Parameters.Add("process", "REGISTER");
                        cmd.Parameters.Add("process_status", process_status);
                        cmd.Parameters.Add("action", action);
                        cmd.Parameters.Add("action_status", action_status);
                        cmd.Parameters.Add("note", note);
                        cmd.ExecuteNonQuery();
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
        public void registerUserId(string user_id, int cust_no)
        {
            using(OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using(var cmd = new OracleCommand(SqlCmd.Line.registerUserId, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add("line_user_id", user_id);
                        cmd.Parameters.Add("cust_no", cust_no);
                        cmd.ExecuteNonQuery();
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
        public m_Customer getProfileByUserId(string user_id)
        {
            using(OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using(var cmd = new OracleCommand(SqlCmd.Line.getProfileByUserId, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add("line_user_id", user_id);
                        var reader = cmd.ExecuteReader();
                        reader.Read();
                        if (reader.HasRows)
                        {
                            var data = new m_Customer
                            {
                                CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                                CUST_NAME = (string)reader["CUST_NAME"],
                                CITIZEN_NO = reader["CITIZEN_NO"] == DBNull.Value ? string.Empty : (string)reader["CITIZEN_NO"],
                                TEL = reader["TEL"] == DBNull.Value ? string.Empty : (string)reader["TEL"],
                                PERMIT = (string)reader["PERMIT"],
                                LINE_USER_ID = reader["LINE_USER_ID"] == DBNull.Value ? string.Empty : reader["LINE_USER_ID"].ToString()
                            };
                            reader.Dispose();
                            cmd.Dispose();
                            return data;
                        }
                        reader.Dispose();
                        cmd.Dispose();
                        return null;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        } 
        public void sendSmsById(long id)
        {
            int total = 0, sent = 0, success = 0, fail = 0;
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
                    using (var cmd = new OracleCommand(SqlCmd.Line.getSmsByIdWithUserId, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add("sms010_pk", id);
                        var reader = cmd.ExecuteReader();
                        List<m_sendSmsLine> data = new List<m_sendSmsLine>();
                        while (reader.Read())
                        {
                            data.Add(new m_sendSmsLine
                            {
                                SMS010_PK = Int32.Parse(reader["SMS010_PK"].ToString()),
                                CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                                LINE_USER_ID = reader["LINE_USER_ID"] == DBNull.Value ? string.Empty : reader["LINE_USER_ID"].ToString(),
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
                            total = data.Count;
                            bool res = false;
                            var msg = data.LastOrDefault();
                            res = sendMessageUserId(msg.LINE_USER_ID, msg.SMS_NOTE);
                            sent++;
                            if (res == true)
                                success++;
                            else
                                fail++;

                            using (var cmd2 = new OracleCommand(SqlCmd.Notification.markToSent, conn) { CommandType = System.Data.CommandType.Text })
                            {
                                cmd2.ExecuteNonQueryAsync();
                                cmd2.Parameters.Clear();
                            }
                        }
                        using (var cmd3 = new OracleCommand(SqlCmd.Log.logTest, conn) { CommandType = System.Data.CommandType.Text })
                        {
                            cmd3.Parameters.Add(new OracleParameter("msg", "ส่งข้อความเสร็จ"));
                            cmd3.ExecuteNonQueryAsync();
                            cmd3.Dispose();
                            monitor.sendMessage("/api/admin/sendmessageall/line", "localhost", "", new { totalSms = total, sentSms = sent, success = success, fail = fail });
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
        public void sendSmsAll()
        {
            int total = 0, sent = 0, success = 0, fail = 0;
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
                    using (var cmd = new OracleCommand(SqlCmd.Line.getSmsWithUserId, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        var reader = cmd.ExecuteReader();
                        List<m_sendSmsLine> data = new List<m_sendSmsLine>();
                        while (reader.Read())
                        {
                            data.Add(new m_sendSmsLine
                            {
                                SMS010_PK = Int32.Parse(reader["SMS010_PK"].ToString()),
                                CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                                LINE_USER_ID = reader["LINE_USER_ID"] == DBNull.Value ? string.Empty : reader["LINE_USER_ID"].ToString(),
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
                            total = data.Count;
                            List<m_SMS010> sms = new List<m_SMS010>();
                            bool res = false;
                            foreach (var s in data)
                            {
                                //var temp = new m_SMS010
                                //{
                                //    SMS010_PK = s.SMS010_PK,
                                //    CUST_NO = s.CUST_NO,
                                //    CON_NO = s.CON_NO,
                                //    SMS_NOTE = s.SMS_NOTE,
                                //    SMS_TIME = DateTime.Now,
                                //    SENDER = s.SENDER,
                                //    SENDER_TYPE = s.SENDER_TYPE,
                                //    SMS010_REF = s.SMS010_REF,
                                //    READ_STATUS = s.READ_STATUS
                                //};
                                //monitor.sendMessage(string.Empty, s.CONN_ID, new { cust_no = s.CUST_NO }, new { request_status = "SUCCESS", desc = "Admin ส่งข้อความ", data = temp });
                                //context.Clients.Client(s.CONN_ID).sms(temp);
                                res = sendMessageUserId(s.LINE_USER_ID, s.SMS_NOTE);
                                sent++;
                                if (res == true)
                                    success++;
                                else
                                    fail++;
                            }

                            using (var cmd2 = new OracleCommand(SqlCmd.Notification.markToSent, conn) { CommandType = System.Data.CommandType.Text })
                            {
                                cmd2.ExecuteNonQueryAsync();
                                cmd2.Parameters.Clear();
                            }
                        }
                        using (var cmd3 = new OracleCommand(SqlCmd.Log.logTest, conn) { CommandType = System.Data.CommandType.Text })
                        {
                            cmd3.Parameters.Add(new OracleParameter("msg", "ส่งข้อความเสร็จ"));
                            cmd3.ExecuteNonQueryAsync();
                            cmd3.Dispose();
                            monitor.sendMessage("/api/admin/sendmessageall/line", "localhost", "", new { totalSms = total, sentSms = sent, success = success, fail = fail });
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
        public bool sendMessageUserId(string user_id, string message)
        {
            m_LineNoti msg = new m_LineNoti();
            List<string> to = new List<string>();
            List<m_LineMessage> lmsg = new List<m_LineMessage>();
            to.Add(user_id);
            lmsg.Add(new m_LineMessage { type = "text", text = message });
            msg.to = to;
            msg.messages = lmsg;

            try
            {
                string postBody = JsonConvert.SerializeObject(msg);
                string result;
                if (client == null)
                    ConnectLine();
                var action = JsonConvert.SerializeObject(msg);
                var content = new StringContent(action, Encoding.UTF8, "application/json");
                var response = client.PostAsync("v2/bot/message/multicast", content);

                if (response.Result.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.Result.Content.ReadAsStringAsync().Result);
                    result = response.Result.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                    return true;
                }
                else
                {
                    Console.WriteLine("Error at Create new payment : " + response.Result.Content.ReadAsStringAsync().Result);
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool IsNumeric(string Expression)
        {
            long retNum;

            bool isNum = long.TryParse(Expression, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
    }
}