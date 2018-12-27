using DeMobile.Services;
using System;

using System.Web.Http;

namespace DeMobile.Controllers
{
    public class AuthenticationController : ApiController
    {
        [Route("api/customer/profile")]
        public IHttpActionResult GetProfile(int id)
        {
            Authen cust = new Authen();
            var result = cust.getProfile(id);
            return Json(result);
        }
        [Route("api/customer/sms")]
        public IHttpActionResult GetSms(int id)
        {
            Authen sms = new Authen();
            var result = sms.getNotification(id);
            return Json(result);
        }
        [Route("api/customer/contract")]
        public IHttpActionResult GetContract(int id)
        {
            Authen contract = new Authen();
            var result = contract.getContract(id);
            return Json(result);
        }
        [Route("api/customer/payment")]
        public IHttpActionResult GetPayment(string no)
        {
            Authen payment = new Authen();
            var result = payment.getPayment(no);
            return Json(result);
        }
    }
}
