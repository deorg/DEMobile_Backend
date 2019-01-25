using DeMobile.Models.AppModel;
using DeMobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DeMobile.Controllers
{
    public class NotificationController : ApiController
    {
        [Route("api/sms/sendOtp")]
        public IHttpActionResult GetSendOtp(int cust_no)
        {
            Notification noti = new Notification();
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameter!");
            try
            {
                var refCode = noti.sendOTP(cust_no);
                return Ok(refCode);
            }
            catch (Exception e)
            {
                return InternalServerError(e.InnerException);
            }
        }
        [Route("api/sms/confirmOtp")]
        public IHttpActionResult PostConfirmOTP([FromBody]ConfirmOTP value)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid parameter!");
            Notification noti = new Notification();
            try
            {
                var result = noti.confirmOtp(value.cust_no, value.otp);
                if (result == "SUCCESS")
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception e)
            {
                return InternalServerError(e.InnerException);
            }
        }
        //[Route("api/notification/all")]
        //public IHttpActionResult GetSendNotiAll(string msg)
        //{

        //}
    }
}
