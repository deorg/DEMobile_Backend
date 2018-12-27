using DeMobile.Models.PaymentGateway;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Threading.Tasks;

namespace DeMobile.Services
{
    public class Payment
    {
        private HttpClient client;
        private void MakeHeader()
        {
            client = new HttpClient();
            //clear header
            client.DefaultRequestHeaders.Clear();
            //custom header
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/json");
        }
        public async Task<PaymentRes> createPayment(PaymentReq value)
        {
            try
            {
                string postBody = JsonConvert.SerializeObject(value);
                PaymentRes responseObj;
                MakeHeader();
                HttpResponseMessage response = await client.PostAsync("https://uatappsrv2.modern-pay.com/api/v1/Payment/", new StringContent(postBody, System.Text.Encoding.UTF8, "application/json"));
                var result = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(result);
                    responseObj = JsonConvert.DeserializeObject<PaymentRes>(result);
                    return responseObj;
                }
                else
                {
                    Console.WriteLine("Error at Create new payment : " + result);
                    return null;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}