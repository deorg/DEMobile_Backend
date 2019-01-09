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

namespace DeMobile.Services
{
    public class Payment
    {
        private HttpClient client;
        private string host = "https://uatappsrv2.modern-pay.com";
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
        public PaymentRes createPayment(PaymentReq value)
        {
            string sumData = "M000052" + value.OrderNo + value.CustomerId + value.Amount.ToString() + value.PhoneNumber + value.Description + value.ChannelCode + "764" + "TH" + "1" + "183.89.168.20" + "nLZAHCaxlMX9FHpUzSAov0dhTV2TXlAxb47j1GCM5fmRFK6lFBrVq3btTu4yxFWk" + "RyFYDwI3Se9y6_2FiBF4o2_hYgccTvjkt5TBo9mBmDor4IXNB46j5Fj3mIt7BjF_tviacnelruOrioqOpEY5G56qeL1a_xQb6zG1LFq0vq9rLAc2zHDoxpeHPOZE6tbDpYFeQRM_Wqt7vcIefg22S9b3cvIqXMR1Boy9JOlPHuy1n0SmM4AorOMF7T3AabnDRlQAZfKr8SQkyT8yEZR7g1vDKGLaiX6vD9BSPBEbGNb2GBuGdagd3SC1HM2e8Dc";
            string checkSum = CreateMD5(sumData);
            CpPaymentReq req = new CpPaymentReq();
            req.MerchantCode = "M000052";
            req.OrderNo = value.OrderNo;
            req.CustomerId = value.CustomerId;
            req.Amount = value.Amount;
            req.PhoneNumber = "";
            req.Description = value.Description;
            req.ChannelCode = value.ChannelCode;
            req.Currency = 764;
            req.LangCode = "TH";
            req.RouteNo = 1;
            req.IPAddress = "183.89.168.20";
            req.ApiKey = "nLZAHCaxlMX9FHpUzSAov0dhTV2TXlAxb47j1GCM5fmRFK6lFBrVq3btTu4yxFWk";
            req.CheckSum = checkSum.ToLower();
            try
            {
                string postBody = JsonConvert.SerializeObject(req);
                PaymentRes responseObj;
                ConnectCP();
                var action = JsonConvert.SerializeObject(req);
                var content = new StringContent(action, Encoding.UTF8, "application/json");
                var response = client.PostAsync("api/v1/Payment/", content);

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