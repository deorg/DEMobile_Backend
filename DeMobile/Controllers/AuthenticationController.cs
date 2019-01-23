using DeMobile.Models.AppModel;
using DeMobile.Services;
using System;

using System.Web.Http;

namespace DeMobile.Controllers
{
    public class AuthenticationController : ApiController
    {
        [Route("api/authen/register")]
        public IHttpActionResult PostRegister([FromBody]Register data)
        {
            User cust = new User();
            try
            {
                var result = cust.getProfileByCitizenNo(data.citizen_no);
                var result2 = cust.getProfileByPhoneNO(data.phone_no);
                if ((result != null) && (result2 != null))
                {
                    var currentDevice = cust.checkCurrentDevice(data);
                    if (currentDevice)
                        return BadRequest("This device already has been registered!");
                    else
                    {
                        var resInset = cust.registerDevice(data, result.CUST_NO);
                        //Notification otp = new Notification();
                        //otp.sendOTP(result.CUST_NO);
                        return Ok();
                    }
                }
                else
                    return BadRequest("Not found customer!");
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }
        [Route("api/customer/profile")]
        public IHttpActionResult GetProfile(int id)
        {
            User cust = new User();
            try
            {
                var result = cust.getProfileById(id);
                if (result != null)
                    return Json(result);
                else
                    return BadRequest("Not found customer!");
            }
            catch(Exception e)
            {
                return InternalServerError(e);
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
                return InternalServerError(e);
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
                return InternalServerError(e);
            }
        }
    }
}
