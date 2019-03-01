//using DeMobile.Models.AppModel;
using DeMobile.Hubs;
using DeMobile.Models.AppModel;
using DeMobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace DeMobile.Controllers
{
    public class NotificationController : ApiController
    {
        User user = new User();
        MonitorHub monitor = new MonitorHub();
        ChatHub chat = new ChatHub();
        Payment payment = new Payment();
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
            //user = new User();
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
        [Route("api/notification/send/all")]
        public IHttpActionResult GetTestNoti(string message)
        {
            ChatHub hub = new ChatHub();
            hub.SendMessageAll(message);
            return Ok();
        }
        [Route("api/notification/send/cust_no")]
        public IHttpActionResult GetSendNotiCust(int cust_no, string type,  string title, string message)
        {
            //user = new User();
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
        [Route("api/notification/send/sms")]
        public IHttpActionResult PostSendSMS([FromBody]m_SMS010[] msg)
        {
            try
            {
                Notification noti = new Notification();
                TransactionHub sender = new TransactionHub();
                var notiData = noti.genNotification(msg);
                sender.sendSMS(notiData);
                return Ok("sent");
            }
            catch(Exception e)
            {
                return InternalServerError(e.InnerException);
            }
        }
        [Route("api/customer/sendmessage")]
        public IHttpActionResult PostCustSendMsg([FromBody]m_CustMessage value)
        {
            string clientHostname = HttpContext.Current.Request.UserHostName;
            string url = HttpContext.Current.Request.Path;
            try
            {
                m_SMS010 sms = new m_SMS010();
                sms.CUST_NO = value.cust_no;
                sms.CON_NO = string.Empty;
                sms.SMS_NOTE = value.message;
                sms.SENDER = value.cust_no;
                sms.SENDER_TYPE = "CUST";
                Notification noti = new Notification();
                var lastSms = noti.createSms(sms);
                sms.SMS010_PK = lastSms;
                sms.READ_STATUS = "UNREAD";
                sms.SMS_TIME = DateTime.Now;


                //var banks = payment.getChannelCode();
                //chat.SendSmsByConnId(sms);
                var cust = user.getProfileById(value.cust_no);
                payment.sendMessageToLine($"[{cust.CUST_NO.ToString()}] คุณ{cust.CUST_NAME} => {value.message}");
                monitor.sendMessage(url, clientHostname, value, new { request_status = "SUCCESS", desc = "ลูกค้าส่งข้อความ", data = sms });
                return Ok(new { code = 200, message = "ส่งข้อความสำเร็จ", data = sms });
            }
            catch (Exception e)
            {
                monitor.sendMessage(url, clientHostname, value, new { Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        [Route("api/admin/sendmessage")]
        public IHttpActionResult PostAdminSendMsg([FromBody]m_CustMessage value)
        {
            string clientHostname = HttpContext.Current.Request.UserHostName;
            string url = HttpContext.Current.Request.Path;
            try
            {
                m_SMS010 sms = new m_SMS010();
                sms.CUST_NO = value.cust_no;
                sms.CON_NO = string.Empty;
                sms.SMS_NOTE = value.message;
                sms.SENDER = value.cust_no;
                sms.SENDER_TYPE = "SYSTEM";
                sms.READ_STATUS = "UNREAD";
                Notification noti = new Notification();
                sms.SMS_TIME = DateTime.Now;
                sms.SMS010_PK = value.sms010_pk;
                chat.SendSmsByConnId(sms);
                monitor.sendMessage(url, clientHostname, value, new { request_status = "SUCCESS", desc = "Admin ส่งข้อความ", data = sms });
                return Ok(new { code = 200, message = "ส่งข้อความสำเร็จ", data = sms });
            }
            catch(Exception e)
            {
                monitor.sendMessage(url, clientHostname, value, new { Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        [Route("api/test/sendmessageall")]
        public IHttpActionResult PostTestSendMessage([FromBody]m_CustMessage value)
        {
            string clientHostname = HttpContext.Current.Request.UserHostName;
            string url = HttpContext.Current.Request.Path;
            try
            {
                chat.sendSmsAll();
                return Ok();
            }
            catch (Exception e)
            {
                monitor.sendMessage(url, clientHostname, new { parameter = "no data" }, new { Message = e.Message });
                return Ok(e.Message);
            }
        }
        [Route("api/test/sendmessage")]
        public IHttpActionResult GetTestSendSms(int cust_no)
        {
            string clientHostname = HttpContext.Current.Request.UserHostName;
            string url = HttpContext.Current.Request.Path;
            try
            {
                chat.sendSmsByCustNo(cust_no);
                return Ok();
            }
            catch(Exception e)
            {
                monitor.sendMessage(url, clientHostname, new { cust_no = cust_no }, new { Message = e.Message });
                return Ok(e.Message);
            }
        }
        //[Route("api/notification/get")]
        //public IHttpActionResult GetNotification(int cust_no)
        //{
        //    Notification noti = new Notification();
        //    MonitorHub monitor = new MonitorHub();
        //    string clientHostname = HttpContext.Current.Request.UserHostName;
        //    string url = HttpContext.Current.Request.Path;
        //    try
        //    {      
        //        var notis = noti.getNotification(cust_no);
        //        monitor.sendMessage(url, clientHostname, new { cust_no = cust_no }, notis);
        //        return Ok(notis);
        //    }
        //    catch (Exception e)
        //    {
        //        monitor.sendMessage(url, clientHostname, new { cust_no = cust_no }, new { Message = e.Message });
        //        return InternalServerError(e.InnerException);
        //    }
        //}
    }
}
