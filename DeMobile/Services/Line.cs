using DeMobile.Concrete;
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
        private Database oracle;

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
        public void setRegister(string user_id, string process_status, string action, string action_status)
        {
            using(OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.Line.setRegister, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add(":line_user_id", user_id);
                        cmd.Parameters.Add(":process", "REGISTER");
                        cmd.Parameters.Add(":process_status", process_status);
                        cmd.Parameters.Add(":action", action);
                        cmd.Parameters.Add(":action_status", action_status);
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
        public void sendMessageUserId(string user_id, string message)
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
                ConnectLine();
                var action = JsonConvert.SerializeObject(msg);
                var content = new StringContent(action, Encoding.UTF8, "application/json");
                var response = client.PostAsync("v2/bot/message/multicast", content);

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
        public bool IsNumeric(string Expression)
        {
            long retNum;

            bool isNum = long.TryParse(Expression, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
    }
}