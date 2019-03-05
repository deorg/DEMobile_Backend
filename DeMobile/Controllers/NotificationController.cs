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

        #region ส่งรหัส OTP ไปให้ลูกค้า
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
        #endregion

        #region ตรวจสอบระหัส OTP จากลูกค้า
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
        #endregion

        #region ทดสอบส่งข้อความผ่าน Websocket ไปหา client ทุกคน
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
        #endregion

        #region ทดสอบส่งข้อความผ่าน Websocket ไปหา client ทุกคน2
        [Route("api/notification/send/all")]
        public IHttpActionResult GetTestNoti(string message)
        {
            ChatHub hub = new ChatHub();
            hub.SendMessageAll(message);
            return Ok();
        }
        #endregion

        #region ลูกค้าส่งข้อความมาหาระบบ
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
                ////////// insert ข้อความของลูกค้าลง sms010 ////////////
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
        #endregion

        #region ส่งขอความหาลูกค้าด้วย cust_no
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
        #endregion

        #region ดึงข้อความ sms ทั้งหมดของลูกค้าทุกคนจาก sms010 และส่งไปหาลูกค้าทาง Websocket
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
        #endregion

        #region ดึงข้อความ sms ทั้งหมดของลูกค้านั้นๆ จาก sms010 และส่งไปหาลูกค้าทาง Websocket
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
        #endregion
    }
}
