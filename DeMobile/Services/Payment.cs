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
        public PaymentRes createPayment(PaymentReq value)
        {
            string[] sumArr = { merchantCode, value.OrderNo, value.CustomerId, value.Amount.ToString(), value.PhoneNumber, value.Description, value.ChannelCode, currecyCode.ToString(), langCode, routeNo.ToString(), "183.89.168.20", apiKey, md5SecretKey };
            string sumData = string.Concat(sumArr);
            //string sumData = "M000052" + value.OrderNo + value.CustomerId + value.Amount.ToString() + value.PhoneNumber + value.Description + value.ChannelCode + "764" + "TH" + "1" + "183.89.168.20" + "nLZAHCaxlMX9FHpUzSAov0dhTV2TXlAxb47j1GCM5fmRFK6lFBrVq3btTu4yxFWk" + "RyFYDwI3Se9y6_2FiBF4o2_hYgccTvjkt5TBo9mBmDor4IXNB46j5Fj3mIt7BjF_tviacnelruOrioqOpEY5G56qeL1a_xQb6zG1LFq0vq9rLAc2zHDoxpeHPOZE6tbDpYFeQRM_Wqt7vcIefg22S9b3cvIqXMR1Boy9JOlPHuy1n0SmM4AorOMF7T3AabnDRlQAZfKr8SQkyT8yEZR7g1vDKGLaiX6vD9BSPBEbGNb2GBuGdagd3SC1HM2e8Dc";
            string checkSum = CreateMD5(sumData);
            CpPaymentReq req = new CpPaymentReq();
            req.MerchantCode = merchantCode;
            req.OrderNo = value.OrderNo;
            req.CustomerId = value.CustomerId;
            req.Amount = value.Amount;
            req.PhoneNumber = "";
            req.Description = value.Description;
            req.ChannelCode = value.ChannelCode;
            req.Currency = currecyCode;
            req.LangCode = langCode;
            req.RouteNo = routeNo;
            req.IPAddress = "183.89.168.20";
            req.ApiKey = apiKey;
            req.CheckSum = checkSum.ToLower();
            try
            {
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
                    return responseObj;
                }
                else
                {
                    Console.WriteLine("Error at Create new payment : " + response.Result.Content.ReadAsStringAsync().Result);
                    return null;
                }
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
    }
}