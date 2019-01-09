using DeMobile.Models.PaymentGateway;
using DeMobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DeMobile.Controllers
{
    public class PaymentController : ApiController
    {
        [Route("api/payment/newpayment")]
        public IHttpActionResult PostNewPayment([FromBody]PaymentReq value)
        {
            Payment payment = new Payment();
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameter!");
            PaymentRes res = payment.createPayment(value);
            if (res == null)
                return Json(new { request_status = "FAILURE", desc = "Internal server error / Invalid parameter!", data = res });
            else
                return Json(new { request_status = "SUCCESS", desc = "Requested to Payment Gateway", data = res });
        }
        [Route("api/payment/newpayment2")]
        public IHttpActionResult PostNewPayment2([FromBody]PaymentReq value)
        {
            value.OrderNo = "test001";
            value.Description = "testAPI";
            Payment payment = new Payment();
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameter!");
            PaymentRes res = payment.createPayment(value);
            if (res == null)
                return Json(new { return_status = "FAILURE", desc = "Internal server error / Invalid parameter!", data = res });
            else
                return Json(new { request_status = "SUCCESS", desc = "Requested to Payment Gateway", data = res });
        }
    }
}
