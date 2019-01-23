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
        [Route("api/payment/newpayment2")]
        public IHttpActionResult PostNewPayment2([FromBody]PaymentReq value)
        {
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            //value.OrderNo = "test001";
            value.Description = "testAPI";
            Payment payment = new Payment();
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameter!");
            PaymentRes res = payment.createPayment(value, IPAddress);
            if (res == null)
                return Json(new { request_status = "FAILURE", desc = "Internal server error / Invalid parameter!", data = res });
            else
                return Json(new { request_status = "SUCCESS", desc = "Requested to Payment Gateway", data = res });
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
                return Json(new { request_status = "SUCCESS", desc = "รหัสธนาคาร", data = banks });
            }
            catch(Exception e)
            {
                return InternalServerError(e.InnerException);
            }
        }
        [Route("api/payment/notify")]
        public IHttpActionResult PostSendMessage([FromBody]NotifyPayment value)
        {
            TransactionHub hub = new TransactionHub();
            hub.SendMessage(value.connectionId, value.success);
            return Json(new { result = "sent" });
        }
        [Route("api/payment/notify/chillpay")]
        public IHttpActionResult PostNotifyChillpay([FromBody]PaymentNotify value)
        {
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
