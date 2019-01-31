//using DeMobile.Models.AppModel;
using DeMobile.Hubs;
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
        User user;
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
        public IHttpActionResult PostConfirmOTP([FromBody]m_ConfirmOTP value)
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
        [Route("api/notification/send/all")]
        public IHttpActionResult GetSendNotiAll(string title, string message)
        {
            user = new User();
            try
            {
                var device = user.getDeviceByStatus("ACT");
                if(device.Count > 0)
                {
                    List<string> conn_id = new List<string>();
                    foreach (var d in device)
                        conn_id.Add(d.conn_id);
                    TransactionHub noti = new TransactionHub();
                    noti.sendMessage("SYSTEM", conn_id, title,  message);
                }
                return Ok("sent");
            }
            catch(Exception e)
            {
                return InternalServerError(e.InnerException);
            }
        }
        [Route("api/notification/send/cust_no")]
        public IHttpActionResult GetSendNotiCust(int cust_no, string type,  string title, string message)
        {
            user = new User();
            try
            {
                var device = user.getDeviceByCustNo(cust_no);
                if(device != null)
                {
                    Notification noti = new Notification();
                    noti.createNoti(type, title, message, cust_no);
                    TransactionHub sender = new TransactionHub();
                    sender.sendMessage("SYSTEM", new List<string> { device.conn_id }, title, message);
                }
                return Ok("sent");
            }
            catch (Exception e)
            {
                return InternalServerError(e.InnerException);
            }
        }
        [Route("api/notification/get")]
        public IHttpActionResult GetNotification(int cust_no)
        {
            Notification noti = new Notification();
            MonitorHub monitor = new MonitorHub();
            try
            {      
                var notis = noti.getNotification(cust_no);
                monitor.sendMessage(new { cust_no = cust_no }, notis);
                return Ok(notis);
            }
            catch (Exception e)
            {
                monitor.sendMessage(new { cust_no = cust_no }, new { Message = e.Message });
                return InternalServerError(e.InnerException);
            }
        }
    }
}
