using DeMobile.Models.PaymentGateway;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using DeMobile.Concrete;
using DeMobile.Models.AppModel;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace DeMobile.Services
{
    public class Payment
    {
        private HttpClient client;
        private string host = Constants.ChillPay.Uat.Host;
        private string merchantCode = Constants.ChillPay.Uat.MerchantCode;
        private int currecyCode = Constants.ChillPay.Uat.Currency;
        private string langCode = Constants.ChillPay.Uat.LangCode;
        private int routeNo = Constants.ChillPay.Uat.RouteNo;
        private string apiKey = Constants.ChillPay.Uat.ApiKey;
        private string md5SecretKey = Constants.ChillPay.Uat.Md5SecretKey;

        private string paymentUrl = Constants.ChillPay.Uat.Api.CreatePayment;
        private string checkStatusUrl = Constants.ChillPay.Uat.Api.CheckPaymentStatus;

        private string host2 = "https://api.line.me";
        private Database oracle;

        //private string host = Constants.ChillPay.Production.Host;
        //private string merchantCode = Constants.ChillPay.Production.MerchantCode;
        //private string apiKey = Constants.ChillPay.Production.ApiKey;
        //private string md5SecretKey = Constants.ChillPay.Production.Md5SecretKey;

        private static bool AllwaysGoodCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }
        private void ConnectCP()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AllwaysGoodCertificate);
            client = new HttpClient();
            client.BaseAddress = new Uri(host);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
        public OracleCommand testSaveDate()
        {
            string date = "20180712173122";
            var newDate = DateTime.ParseExact(date, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            oracle = new Database();
            string cmd = $@"insert into table1(trans_date) values(:newDate)";
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("newDate", newDate));
            //return oracle.SqlExcute(cmd);
            return oracle.SqlExecuteWithParams(cmd, parameter);
        }
        public List<ChannelCode> getChanneCode()
        {
            oracle = new Database();
            List<ChannelCode> data = new List<ChannelCode>();
            string cmd = $@"SELECT * FROM MPAY010";
            var reader = oracle.SqlQuery(cmd);
            while (reader.Read())
            {
                data.Add(new ChannelCode
                {
                    CHANNEL_ID = (string)reader["CHANNEL_ID"],
                    CHANNEL_NAME = (string)reader["CHANNEL_NAME"]
                });
            }
            if (data.Count == 0)
            {
                reader.Dispose();
                oracle.OracleDisconnect();
                return null;
            }
            reader.Dispose();
            oracle.OracleDisconnect();
            return data;
        }
        public void sendMessageToLine()
        {
            LineNoti msg = new LineNoti();
            List<string> to = new List<string>();
            List<LineMessage> lmsg = new List<LineMessage>();
            to.Add("U7c8a8a90f9727517c12e2bec78288fb3");
            lmsg.Add(new LineMessage { type = "text", text = "test" });
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
        public PaymentRes createPayment(PaymentReq value, string ip)
        {
            try
            {
                //string cmd = $@"INSERT INTO  MPAY100(CUST_NO, CON_NO, DEVICE_ID, CHANNEL_ID, PAY_AMT, TEL, IP_ADDR, DESCRIPTION)
                //                VALUES({value.CustomerId}, '{value.ContractNo}', '{value.DeviceId}', '{value.ChannelCode}', {value.Amount}, '{value.PhoneNumber}', '{ip}', '{value.Description}')";
                var lastOrder = createOrder(value, ip);
                if (lastOrder > 0)
                {
                    string[] sumArr = { merchantCode, lastOrder.ToString(), value.CustomerId.ToString(), value.Amount.ToString(), value.PhoneNumber, value.Description, value.ChannelCode, currecyCode.ToString(), langCode, routeNo.ToString(), ip, apiKey, md5SecretKey };
                    string sumData = string.Concat(sumArr);
                    string checkSum = CreateMD5(sumData);
                    CpPaymentReq req = new CpPaymentReq();
                    req.MerchantCode = merchantCode;
                    req.OrderNo = lastOrder.ToString();
                    req.CustomerId = value.CustomerId;
                    req.Amount = value.Amount;
                    req.PhoneNumber = "";
                    req.Description = value.Description;
                    req.ChannelCode = value.ChannelCode;
                    req.Currency = currecyCode;
                    req.LangCode = langCode;
                    req.RouteNo = routeNo;
                    req.IPAddress = ip;
                    req.ApiKey = apiKey;
                    req.CheckSum = checkSum.ToLower();

                    string postBody = JsonConvert.SerializeObject(req);
                    PaymentRes responseObj;
                    ConnectCP();
                    var action = JsonConvert.SerializeObject(req);
                    var content = new StringContent(action, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(paymentUrl, content);

                    if (response.Result.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.Result.Content.ReadAsStringAsync().Result);
                        responseObj = JsonConvert.DeserializeObject<PaymentRes>(response.Result.Content.ReadAsStringAsync().Result);
                        saveTransaction(responseObj);
                        return responseObj;
                    }
                    else
                    {
                        Console.WriteLine("Error at Create new payment : " + response.Result.Content.ReadAsStringAsync().Result);
                        return null;
                    }
                }
                else
                    return null;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public PaymentStatusRes getPaymentStatus(CpPaymentStatusReq value)
        {
            string[] sumArr = { value.MerchantCode, value.TransactionId, apiKey, md5SecretKey };
            string sumData = string.Concat(sumArr);
            //string sumData = "M000052" + value.OrderNo + value.CustomerId + value.Amount.ToString() + value.PhoneNumber + value.Description + value.ChannelCode + "764" + "TH" + "1" + "183.89.168.20" + "nLZAHCaxlMX9FHpUzSAov0dhTV2TXlAxb47j1GCM5fmRFK6lFBrVq3btTu4yxFWk" + "RyFYDwI3Se9y6_2FiBF4o2_hYgccTvjkt5TBo9mBmDor4IXNB46j5Fj3mIt7BjF_tviacnelruOrioqOpEY5G56qeL1a_xQb6zG1LFq0vq9rLAc2zHDoxpeHPOZE6tbDpYFeQRM_Wqt7vcIefg22S9b3cvIqXMR1Boy9JOlPHuy1n0SmM4AorOMF7T3AabnDRlQAZfKr8SQkyT8yEZR7g1vDKGLaiX6vD9BSPBEbGNb2GBuGdagd3SC1HM2e8Dc";
            string checkSum = CreateMD5(sumData);
            value.CheckSum = checkSum.ToLower();
            try
            {
                string postBody = JsonConvert.SerializeObject(value);
                PaymentStatusRes responseObj;
                ConnectCP();
                var action = JsonConvert.SerializeObject(value);
                var content = new StringContent(action, Encoding.UTF8, "application/json");
                var response = client.PostAsync(checkStatusUrl, content);

                if (response.Result.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.Result.Content.ReadAsStringAsync().Result);
                    responseObj = JsonConvert.DeserializeObject<PaymentStatusRes>(response.Result.Content.ReadAsStringAsync().Result);
                    return responseObj;
                }
                else
                {
                    Console.WriteLine("Error at Create new payment : " + response.Result.Content.ReadAsStringAsync().Result);
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        private string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public int createOrder(PaymentReq value, string ip)
        {
            try
            {
                oracle = new Database();
                string cmd = $@"INSERT INTO  MPAY100(CUST_NO, CON_NO, DEVICE_ID, CHANNEL_ID, PAY_AMT, TEL, IP_ADDR, DESCRIPTION)
                                VALUES(:custId, :contractNo, :deviceId, :channelCode, :amount, :phone, :ip, :description) RETURNING ORDER_NO INTO :order_no";
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("custId", value.CustomerId));
                parameter.Add(new OracleParameter("contractNo", value.ContractNo));
                parameter.Add(new OracleParameter("deviceId", value.DeviceId));
                parameter.Add(new OracleParameter("channelCode", value.ChannelCode));
                parameter.Add(new OracleParameter("amount", value.Amount));
                parameter.Add(new OracleParameter("phone", value.PhoneNumber));
                parameter.Add(new OracleParameter("ip", ip));
                parameter.Add(new OracleParameter("description", value.Description));
                parameter.Add(new OracleParameter
                {
                    ParameterName = "order_no",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                });
                var resInsert = oracle.SqlExecuteWithParams(cmd, parameter);
                var lastOrder = Int32.Parse(resInsert.Parameters["order_no"].Value.ToString());
                oracle.OracleDisconnect();
                return lastOrder;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
            //oracle.OracleDisconnect();
        }
        public int saveTransaction(PaymentRes value)
        {
            try
            {
                oracle = new Database();
                var createDate = DateTime.ParseExact(value.CreatedDate, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                var expireDate = DateTime.ParseExact(value.ExpiredDate, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                string cmd = $@"INSERT INTO MPAY110(TRANS_NO, ORDER_NO, CUST_NO, CHANNEL_ID, REQ_STATUS_ID, TRANS_STATUS_ID, PAY_AMT, RETURN_URL, PAYMENT_URL, IP_ADDR, TOKEN, CREATED_TIME, EXPIRE_TIME)
                                VALUES(:transNo, :orderNo, :custNo, :channelId, :reqStatus, :tranStatus, :amount, :returnUrl, :paymentUrl, :ip, :token, :createTime, :expireTime) RETURNING TRANS_NO INTO :trans_no";
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("transNo", value.TransactionId));
                parameter.Add(new OracleParameter("orderNo", Int32.Parse(value.OrderNo)));
                parameter.Add(new OracleParameter("custNo", Int32.Parse(value.CustomerId)));
                parameter.Add(new OracleParameter("channelId", value.ChannelCode));
                parameter.Add(new OracleParameter("reqStatus", value.Status));
                parameter.Add(new OracleParameter("tranStatus", value.Code));
                parameter.Add(new OracleParameter("amount", value.Amount));
                parameter.Add(new OracleParameter("returnUrl", value.ReturnUrl));
                parameter.Add(new OracleParameter("paymentUrl", value.PaymentUrl));
                parameter.Add(new OracleParameter("ip", value.IpAddress));
                parameter.Add(new OracleParameter("token", value.Token));
                parameter.Add(new OracleParameter("createTime", createDate));
                parameter.Add(new OracleParameter("expireTime", expireDate));
                parameter.Add(new OracleParameter
                {
                    ParameterName = "trans_no",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                });
                var resInsert = oracle.SqlExecuteWithParams(cmd, parameter);
                var lastOrder = Int32.Parse(resInsert.Parameters["trans_no"].Value.ToString());
                oracle.OracleDisconnect();
                return lastOrder;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
    }
}