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
        public List<ChannelCode> getChannelCode()
        {
            oracle = new Database();
            List<ChannelCode> data = new List<ChannelCode>();
            //string cmd = $@"SELECT * FROM MPAY010";
            var reader = oracle.SqlQuery(SqlCmd.Payment.getChannelCode);
            while (reader.Read())
            {
                data.Add(new ChannelCode
                {
                    CHANNEL_ID = (string)reader["CHANNEL_ID"],
                    CHANNEL_NAME = (string)reader["CHANNEL_NAME"],
                    CHANNEL_IMG = (string)reader["CHANNEL_IMG"]
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
            m_LineNoti msg = new m_LineNoti();
            List<string> to = new List<string>();
            List<m_LineMessage> lmsg = new List<m_LineMessage>();
            to.Add("U7c8a8a90f9727517c12e2bec78288fb3");
            lmsg.Add(new m_LineMessage { type = "text", text = "test" });
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
        public PaymentRes createPayment(PaymentReq value)
        {
            try
            {
                //string cmd = $@"INSERT INTO  MPAY100(CUST_NO, CON_NO, DEVICE_ID, CHANNEL_ID, PAY_AMT, TEL, IP_ADDR, DESCRIPTION)
                //                VALUES({value.CustomerId}, '{value.ContractNo}', '{value.DeviceId}', '{value.ChannelCode}', {value.Amount}, '{value.PhoneNumber}', '{ip}', '{value.Description}')";
                var lastOrder = createOrder(value);
                if (lastOrder > 0)
                {
                    string[] sumArr = { merchantCode, lastOrder.ToString(), value.CustomerId.ToString(), value.Amount.ToString(), value.PhoneNumber == null ? "" : value.PhoneNumber, value.Description, value.ChannelCode, currecyCode.ToString(), langCode, routeNo.ToString(), value.IPAddress, apiKey, md5SecretKey };
                    string sumData = string.Concat(sumArr);
                    string checkSum = CreateMD5(sumData);
                    CpPaymentReq req = new CpPaymentReq();
                    req.MerchantCode = merchantCode;
                    req.OrderNo = lastOrder.ToString();
                    req.CustomerId = value.CustomerId;
                    req.Amount = value.Amount;
                    req.PhoneNumber = value.PhoneNumber == null ? "" : value.PhoneNumber;
                    req.Description = value.Description;
                    req.ChannelCode = value.ChannelCode;
                    req.Currency = currecyCode;
                    req.LangCode = langCode;
                    req.RouteNo = routeNo;
                    req.IPAddress = value.IPAddress;
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
                        var lastTransaction = saveTransaction(value, responseObj);
                        if(responseObj.Status == 0 && responseObj.Code == 200)
                            setStatusOrder(lastOrder, "ACT");
                        if(responseObj.Status != 0)
                        {
                            if (responseObj.Status == 1)
                                setStatusOrder(lastOrder, "FAL");
                            else
                                setStatusOrder(lastOrder, "ERR");
                        }
                        if(responseObj.Code != 200)
                        {
                            if (responseObj.Code < 2007)
                                setStatusOrder(lastOrder, "CAN");
                            else
                                setStatusOrder(lastOrder, "ERR");
                        }
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
        public m_Transaction getTransactionByOrderNo(int order_no)
        {
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("order_no", order_no));
            var reader = oracle.SqlQueryWithParams(SqlCmd.Payment.getTransactionByOrder_no, parameter);
            reader.Read();
            if (reader.HasRows)
            {
                //int trans_no = Int32.Parse(reader["TRANS_NO"].ToString());
                //var bankref_code = reader["BANK_REF_CODE"] == DBNull.Value ? string.Empty : (string)reader["BANK_REF_CODE"];
                //var result_status_id = reader["RESULT_STATUS_ID"] == DBNull.Value ? null : (int?)Int32.Parse(reader["RESULT_STATUS_ID"].ToString());
                //var payment_time = reader["PAYMENT_TIME"];
                var data = new m_Transaction
                {
                    TRANS_NO = Int32.Parse(reader["TRANS_NO"].ToString()),
                    ORDER_NO = Int32.Parse(reader["ORDER_NO"].ToString()),
                    CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                    CHANNEL_ID = (string)reader["CHANNEL_ID"],
                    REQ_STATUS_ID = Int32.Parse(reader["REQ_STATUS_ID"].ToString()),
                    TRANS_STATUS_ID = Int32.Parse(reader["TRANS_STATUS_ID"].ToString()),
                    PAY_AMT = double.Parse(reader["PAY_AMT"].ToString()),
                    RETURN_URL = (string)reader["RETURN_URL"],
                    PAYMENT_URL = (string)reader["PAYMENT_URL"],
                    IP_ADDR = (string)reader["IP_ADDR"],
                    TOKEN = (string)reader["TOKEN"],
                    CREATED_TIME = (DateTime)reader["CREATED_TIME"],
                    EXPIRE_TIME = (DateTime)reader["EXPIRE_TIME"],
                    BANK_REF_CODE = reader["BANK_REF_CODE"] == DBNull.Value ? "" : (string)reader["BANK_REF_CODE"],
                    RESULT_STATUS_ID = reader["RESULT_STATUS_ID"] == DBNull.Value ? null : (int?)Int32.Parse(reader["RESULT_STATUS_ID"].ToString()),
                    PAYMENT_TIME = reader["PAYMENT_TIME"] == DBNull.Value ? null : (DateTime?)reader["PAYMENT_TIME"],
                    TRANS_AMT = double.Parse(reader["TRANS_AMT"].ToString())
                };
                reader.Dispose();
                oracle.OracleDisconnect();
                return data;
            }
            else
            {
                reader.Dispose();
                oracle.OracleDisconnect();
                return null;
            }
        }
        public PaymentStatusRes getPaymentStatus(int transactionId)
        {
            string[] sumArr = { merchantCode, transactionId.ToString(), apiKey, md5SecretKey };
            string sumData = string.Concat(sumArr);
            //string sumData = "M000052" + value.OrderNo + value.CustomerId + value.Amount.ToString() + value.PhoneNumber + value.Description + value.ChannelCode + "764" + "TH" + "1" + "183.89.168.20" + "nLZAHCaxlMX9FHpUzSAov0dhTV2TXlAxb47j1GCM5fmRFK6lFBrVq3btTu4yxFWk" + "RyFYDwI3Se9y6_2FiBF4o2_hYgccTvjkt5TBo9mBmDor4IXNB46j5Fj3mIt7BjF_tviacnelruOrioqOpEY5G56qeL1a_xQb6zG1LFq0vq9rLAc2zHDoxpeHPOZE6tbDpYFeQRM_Wqt7vcIefg22S9b3cvIqXMR1Boy9JOlPHuy1n0SmM4AorOMF7T3AabnDRlQAZfKr8SQkyT8yEZR7g1vDKGLaiX6vD9BSPBEbGNb2GBuGdagd3SC1HM2e8Dc";
            string checkSum = CreateMD5(sumData);
            CpPaymentStatusReq req = new CpPaymentStatusReq();
            req.MerchantCode = merchantCode;
            req.TransactionId = transactionId;
            req.ApiKey = apiKey;
            req.CheckSum = checkSum.ToLower();
            try
            {
                string postBody = JsonConvert.SerializeObject(req);
                PaymentStatusRes responseObj;
                ConnectCP();
                var action = JsonConvert.SerializeObject(req);
                var content = new StringContent(action, Encoding.UTF8, "application/json");
                var response = client.PostAsync(checkStatusUrl, content);

                if (response.Result.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.Result.Content.ReadAsStringAsync().Result);
                    responseObj = JsonConvert.DeserializeObject<PaymentStatusRes>(response.Result.Content.ReadAsStringAsync().Result);
                    if (responseObj.Code == 200 && responseObj.PaymentStatus == 0)
                        setStatusOrder(Int32.Parse(responseObj.OrderNo), "SUC");
                    else if(responseObj.Code == 200 && responseObj.PaymentStatus != 0)
                    {
                        if (responseObj.PaymentStatus == 1)
                            setStatusOrder(Int32.Parse(responseObj.OrderNo), "FAL");
                        else if (responseObj.PaymentStatus == 2 || responseObj.PaymentStatus == 4)
                            setStatusOrder(Int32.Parse(responseObj.OrderNo), "CAN");
                        else if (responseObj.PaymentStatus == 3)
                            setStatusOrder(Int32.Parse(responseObj.OrderNo), "ERR");
                    }
                    else if (responseObj.Code != 200)
                    {
                        if (responseObj.Code < 2007)
                            setStatusOrder(Int32.Parse(responseObj.OrderNo), "CAN");
                        else
                            setStatusOrder(Int32.Parse(responseObj.OrderNo), "ERR");
                    }
                    updateTransaction(responseObj);
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
        public void setStatusOrder(int order_no, string status)
        {
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("order_no", order_no));
            parameter.Add(new OracleParameter("status", status));
            oracle.SqlExecuteWithParams(SqlCmd.Payment.setStatusOrder, parameter);
            oracle.OracleDisconnect();
        }
        public void updateTransaction(PaymentStatusRes value)
        {
            oracle = new Database();
            DateTime paymentTime;
            if(!string.IsNullOrEmpty(value.PaymentDate))
                paymentTime = DateTime.ParseExact(value.PaymentDate, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("trans_no", value.TransactionId));
            parameter.Add(new OracleParameter("trans_status_id", value.Code));
            parameter.Add(new OracleParameter("bank_ref_code", value.BankRefCode));
            parameter.Add(new OracleParameter("result_status_id", value.PaymentStatus));
            if (!string.IsNullOrEmpty(value.PaymentDate))
            {
                paymentTime = DateTime.ParseExact(value.PaymentDate, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                parameter.Add(new OracleParameter("payment_time", paymentTime));
            }
            else
                parameter.Add(new OracleParameter("payment_time", null));
            oracle.SqlExecuteWithParams(SqlCmd.Payment.updateTransaction, parameter);
            oracle.OracleDisconnect();
        }
        public int createOrder(PaymentReq value)
        {
            try
            {
                oracle = new Database();
                //string cmd = $@"INSERT INTO  MPAY100(CUST_NO, CON_NO, DEVICE_ID, CHANNEL_ID, PAY_AMT, TEL, IP_ADDR, DESCRIPTION)
                //                VALUES(:custId, :contractNo, :deviceId, :channelCode, :amount, :phone, :ip, :description) RETURNING ORDER_NO INTO :order_no";
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("custId", value.CustomerId));
                parameter.Add(new OracleParameter("contractNo", value.ContractNo));
                parameter.Add(new OracleParameter("deviceId", value.DeviceId));
                parameter.Add(new OracleParameter("channelCode", value.ChannelCode));
                parameter.Add(new OracleParameter("payAmt", value.PayAmt));
                parameter.Add(new OracleParameter("phone", value.PhoneNumber));
                parameter.Add(new OracleParameter("ip", value.IPAddress));
                parameter.Add(new OracleParameter("description", value.Description));
                parameter.Add(new OracleParameter("transAmt", value.Amount));
                parameter.Add(new OracleParameter
                {
                    ParameterName = "order_no",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                });
                var resInsert = oracle.SqlExecuteWithParams(SqlCmd.Payment.createOrder, parameter);
                //var resInsert = oracle.SqlExecuteWithParams(cmd, parameter);
                var lastOrder = Int32.Parse(resInsert.Parameters["order_no"].Value.ToString());
                resInsert.Dispose();
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
        public int saveTransaction(PaymentReq value, PaymentRes value2)
        {
            try
            {
                oracle = new Database();
                var createDate = DateTime.ParseExact(value2.CreatedDate, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                var expireDate = DateTime.ParseExact(value2.ExpiredDate, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                //string cmd = $@"INSERT INTO MPAY110(TRANS_NO, ORDER_NO, CUST_NO, CHANNEL_ID, REQ_STATUS_ID, TRANS_STATUS_ID, PAY_AMT, RETURN_URL, PAYMENT_URL, IP_ADDR, TOKEN, CREATED_TIME, EXPIRE_TIME)
                //                VALUES(:transNo, :orderNo, :custNo, :channelId, :reqStatus, :tranStatus, :amount, :returnUrl, :paymentUrl, :ip, :token, :createTime, :expireTime) RETURNING TRANS_NO INTO :trans_no";
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("transNo", value2.TransactionId));
                parameter.Add(new OracleParameter("orderNo", Int32.Parse(value2.OrderNo)));
                parameter.Add(new OracleParameter("custNo", Int32.Parse(value2.CustomerId)));
                parameter.Add(new OracleParameter("channelId", value2.ChannelCode));
                parameter.Add(new OracleParameter("reqStatus", value2.Status));
                parameter.Add(new OracleParameter("tranStatus", value2.Code));
                parameter.Add(new OracleParameter("payAmt", value.PayAmt));
                parameter.Add(new OracleParameter("returnUrl", value2.ReturnUrl));
                parameter.Add(new OracleParameter("paymentUrl", value2.PaymentUrl));
                parameter.Add(new OracleParameter("ip", value2.IpAddress));
                parameter.Add(new OracleParameter("token", value2.Token));
                parameter.Add(new OracleParameter("createTime", createDate));
                parameter.Add(new OracleParameter("expireTime", expireDate));
                parameter.Add(new OracleParameter("transAmt", value2.Amount));
                parameter.Add(new OracleParameter
                {
                    ParameterName = "trans_no",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                });
                var resInsert = oracle.SqlExecuteWithParams(SqlCmd.Payment.saveTransaction, parameter);
                //var resInsert = oracle.SqlExecuteWithParams(cmd, parameter);
                var lastTransaction = Int32.Parse(resInsert.Parameters["trans_no"].Value.ToString());
                //parameter.Clear();
                //parameter.Add(new OracleParameter("order_no", value.OrderNo));
                //oracle.SqlExecuteWithParams(SqlCmd.Payment.setActiveOrder, parameter);

                resInsert.Dispose();
                oracle.OracleDisconnect();
                return lastTransaction;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
    }
}