using DeMobile.Models.PaymentGateway;
using DeMobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DeMobile.Controllers
{
    public class PaymentController : ApiController
    {
        private Payment _payment;
        public PaymentController(Payment payment)
        {
            _payment = payment;
        }
        [Route("api/payment/newpayment")]
        public async System.Threading.Tasks.Task<IHttpActionResult> PostNewPaymentAsync([FromBody]PaymentReq value)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameter!");
            PaymentRes res = await _payment.createPayment(value);
            if (res == null)
                return Json(new { request_status = "FAILURE", desc = "Internal server error / Invalid parameter!", data = res });
            else
                return Json(new { request_status = "SUCCESS", desc = "Requested to Payment Gateway", data = res });
        }
    }
}
