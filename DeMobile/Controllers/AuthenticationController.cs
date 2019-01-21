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
            User cust = new User();
            try
            {
                var result = cust.getProfile(id);
                return Json(result);
            }
            catch(Exception e)
            {
                return Json(new { error = e.Message });
            }
        }
        [Route("api/customer/sms")]
        public IHttpActionResult GetSms(int id)
        {
            User sms = new User();
            try
            {
                var result = sms.getNotification(id);
                return Json(result);
            }
            catch(Exception e)
            {
                return Json(new { error = e.Message });
            }
        }
        [Route("api/customer/contract")]
        public IHttpActionResult GetContract(int id)
        {
            User contract = new User();
            try
            {
                var result = contract.getContract(id);
                return Json(result);
            }
            catch(Exception e)
            {
                return Json(new { error = e.Message });
            }
        }
        [Route("api/customer/payment")]
        public IHttpActionResult GetPayment(string no)
        {
            User payment = new User();
            try
            {
                var result = payment.getPayment(no);
                return Json(result);
            }
            catch(Exception e)
            {
                return Json(new { error = e.Message });
            }
        }
    }
}
