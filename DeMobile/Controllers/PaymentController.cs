using DeMobile.Hubs;
using DeMobile.Models;
using DeMobile.Models.PaymentGateway;
using DeMobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DeMobile.Controllers
{
    public class PaymentController : ApiController
    {
        //[Route("api/payment/newpayment")]
        //public IHttpActionResult PostNewPayment([FromBody]PaymentReq value)
        //{
        //    Payment payment = new Payment();
        //    if (!ModelState.IsValid)
        //        return BadRequest("Invalid parameter!");
        //    PaymentRes res = payment.createPayment(value);
        //    if (res == null)
        //        return Json(new { request_status = "FAILURE", desc = "Internal server error / Invalid parameter!", data = res });
        //    else
        //        return Json(new { request_status = "SUCCESS", desc = "Requested to Payment Gateway", data = res });
        //}
        User user;
        [Route("api/payment/newpayment2")]
        public IHttpActionResult PostNewPayment2([FromBody]PaymentReq value)
        {
            try
            {
                string IPAddress = HttpContext.Current.Request.UserHostAddress;
                //value.OrderNo = "test001";
                value.Description = "testAPI";
                if (!ModelState.IsValid)
                    return BadRequest("Invalid parameter!");
                user = new User();
                var cust = user.getProfileById(value.CustomerId);
                if (cust != null)
                {
                    var contract = user.findContract(value.CustomerId, value.ContractNo);
                    if (contract != null)
                    {
                        Payment payment = new Payment();
                        PaymentRes res = payment.createPayment(value, IPAddress);
                        if (res == null)
                            return Ok(new { request_status = "FAILURE", desc = "Internal server error / Invalid parameter!", data = res });
                        else
                            return Ok(new { request_status = "SUCCESS", desc = "Requested to Payment Gateway", data = res });
                    }
                    else
                        return Ok(new { request_status = "FAILURE", desc = "Not found contract!", data = contract });
                }
                return Ok(new { request_status = "FAILURE", desc = "Not found customer?", data = cust });
            }
            catch(Exception e)
            {
                return InternalServerError(e.InnerException);
            }
        }
        [Route("api/payment/getstatus")]
        public IHttpActionResult GetPaymentStatus(int trans_no)
        {
            Payment payment = new Payment();
            try
            {
                PaymentStatusRes res = payment.getPaymentStatus(trans_no);
                return Ok(res);
            }
            catch(Exception e)
            {
                return InternalServerError(e.InnerException);
            }
        }
        [Route("api/test/getdate")]
        public IHttpActionResult GetDate()
        {
            //string date = "20180712173122";
            //var newDate = DateTime.ParseExact(date, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            Payment payment = new Payment();
            return Ok(payment.testSaveDate());
        }
        [Route("api/payment/getchannel")]
        public IHttpActionResult GetBankCode()
        {
            try
            {
                Payment payment = new Payment();
                var banks = payment.getChanneCode();
                return Ok(new { request_status = "SUCCESS", desc = "รหัสธนาคาร", data = banks });
            }
            catch(Exception e)
            {
                return InternalServerError(e.InnerException);
            }
        }
        //[Route("api/payment/notify")]
        //public IHttpActionResult PostSendMessage([FromBody]NotifyPayment value)
        //{
        //    TransactionHub hub = new TransactionHub();
        //    hub.SendMessage(value.connectionId, value.success);
        //    return Json(new { result = "sent" });
        //}
        [Route("api/payment/notify/chillpay")]
        public IHttpActionResult PostNotifyChillpay([FromBody]PaymentStatusRes value)
        {
            Payment payment = new Payment();
            if (value.Code == 200 && value.PaymentStatus == 0)
                payment.setStatusOrder(Int32.Parse(value.OrderNo), "SUC");
            else if (value.Code == 200 && value.PaymentStatus != 0)
            {
                if (value.PaymentStatus == 1)
                    payment.setStatusOrder(Int32.Parse(value.OrderNo), "FAL");
                else if (value.PaymentStatus == 2 || value.PaymentStatus == 4)
                    payment.setStatusOrder(Int32.Parse(value.OrderNo), "CAN");
                else if (value.PaymentStatus == 3)
                    payment.setStatusOrder(Int32.Parse(value.OrderNo), "ERR");
            }
            else if (value.Code != 200)
            {
                if (value.Code < 2007)
                    payment.setStatusOrder(Int32.Parse(value.OrderNo), "CAN");
                else
                    payment.setStatusOrder(Int32.Parse(value.OrderNo), "ERR");
            }
            payment.updateTransaction(value);

            TransactionHub hub = new TransactionHub();
            hub.NotifyPayment(value);
            return Ok();
        }
        [Route("api/line/test")]
        public IHttpActionResult GetLine()
        {
            Payment payment = new Payment();
            payment.sendMessageToLine();
            return Json(new { result = "sent" });
        }
    }
}
