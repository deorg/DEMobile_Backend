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
            value.OrderNo = "test001";
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
        [Route("api/payment/getbankcode")]
        public IHttpActionResult GetBankCode()
        {
            BankCode[] banks = new BankCode[]
            {
                new BankCode { Code = "internetbank_bay",  Remark = "ธนาคารกรุงศรีอยุธยา" },
                new BankCode { Code = "internetbank_bbl", Remark = "ธนาคารกรุงเทพ" },
                new BankCode { Code = "internetbank_scb", Remark = "ธนาคารไทยพาณิช" },
                new BankCode { Code = "internetbank_ktb", Remark = "ธนาคารกรุงไทย" },
                new BankCode { Code = "payplus_kbank", Remark = "ธนาคารกสิกรไทย" }
            };
            return Json(new { request_status = "SUCCESS", desc = "รหัสธนาคาร", data = banks });
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
