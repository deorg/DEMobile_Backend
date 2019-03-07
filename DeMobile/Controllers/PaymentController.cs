using DeMobile.Hubs;
using DeMobile.Models;
using DeMobile.Models.AppModel;
using DeMobile.Models.PaymentGateway;
using DeMobile.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace DeMobile.Controllers
{
    public class PaymentController : ApiController
    {
        User user = new User();
        MonitorHub monitor = new MonitorHub();
        Log log = new Log();
        ChatHub chat = new ChatHub();
        m_LogReq mlog;
        m_LogOrder mlogOrder;

        [Route("api/payment/newpayment2")]
        public IHttpActionResult PostNewPayment2([FromBody]PaymentReq value)
        {
            var setting = (AppSettingsSection)WebConfigurationManager.OpenWebConfiguration("~").GetSection("appSettings");
            var appService = setting.Settings["AppService"].Value;
            var paymentService = setting.Settings["PaymentService"].Value;
            if (appService == "False" || paymentService == "False")
                return Unauthorized();
            value.IPAddress = HttpContext.Current.Request.UserHostAddress;
            string clientHostname = HttpContext.Current.Request.UserHostName;
            string url = HttpContext.Current.Request.Path;
            try
            {
                //value.OrderNo = "test001";
                value.Description = "testAPI";
                string strAmt = value.Amount.ToString();
                strAmt = strAmt.Insert(strAmt.Length - 2, ".");
                value.PayAmt = double.Parse(strAmt);
                mlog = new m_LogReq();
                if (!ModelState.IsValid)
                    return BadRequest("Invalid parameter!");

                user = new User();
                var cust = user.getProfileById(value.CustomerId);
                if (cust != null)
                {
                    var contract = user.findContract(value.CustomerId, value.ContractNo);
                    if (contract != null)
                    {
                        if (value.PayAmt <= (contract.BAL_AMT - contract.DISC_AMT))
                        {
                            Payment payment = new Payment();
                            PaymentRes res = payment.createPayment(value);
                            if (res == null)
                            {
                                //mlogOrder = new m_LogOrder();
                                //mlogOrder.cust_no = value.CustomerId;
                                //mlogOrder.con_no = value.ContractNo;
                                //mlogOrder.channel_id = value.ChannelCode;
                                //mlogOrder.pay_amt = value.PayAmt;
                                //mlogOrder.trans_amt = value.Amount;
                                //mlogOrder.device_id = value.DeviceId;
                                //mlogOrder.tel = value.PhoneNumber;
                                //mlogOrder.note = "ระบบขัดข้อง ไม่สามารถทำรายการได้";
                                //mlogOrder.ip_addr = value.IPAddress;
                                //log.logOrder(mlogOrder);



                                monitor.sendMessage(url, clientHostname, value, new { request_status = "FAILURE", desc = "Internal server error / Invalid parameter!", data = res });
                                return Ok(new { code = 500, message = "ระบบขัดข้อง ไม่สามารถทำรายการได้", data = res });
                            }
                            else
                            {
                                monitor.sendMessage(url, clientHostname, value, new { request_status = "SUCCESS", desc = "Requested to Payment Gateway", data = res });
                                return Ok(new { code = 200, message = "สร้างรายการชำระเงินสำเร็จ", data = res });
                            }
                        }
                        else
                        {
                            mlogOrder = new m_LogOrder();
                            mlogOrder.cust_no = value.CustomerId;
                            mlogOrder.con_no = value.ContractNo;
                            mlogOrder.channel_id = value.ChannelCode;
                            mlogOrder.pay_amt = value.PayAmt;
                            mlogOrder.trans_amt = value.Amount;
                            mlogOrder.device_id = value.DeviceId;
                            mlogOrder.tel = value.PhoneNumber;
                            mlogOrder.note = "จำนวนเงินที่ต้องการชำระมากกว่ายอดคงเหลือหลังจากที่หักส่วนลดแล้ว";
                            mlogOrder.ip_addr = value.IPAddress;
                            log.logOrder(mlogOrder);
                            //mlog.cust_no = value.CustomerId;
                            //mlog.device_id = value.DeviceId;
                            //mlog.ip_addr = value.IPAddress;
                            //mlog.note = "จำนวนเงินที่ต้องการชำระมากกว่ายอดคงเหลือหลังจากที่หักส่วนลดแล้ว";
                            //mlog.url = "api/authen/newpayment2";
                            //log.logRequest(mlog);
                            monitor.sendMessage(url, clientHostname, value, new { request_status = "FAILURE", desc = "จำนวนเงินที่ต้องการชำระมากกว่ายอดคงเหลือหลังจากที่หักส่วนลดแล้ว", data = string.Empty });
                            return Ok(new { code = 400, message = "จำนวนเงินที่ต้องการชำระมากกว่ายอดคงเหลือหลังจากที่หักส่วนลดแล้ว", data = contract });
                        }
                    }
                    else
                    {
                        mlogOrder = new m_LogOrder();
                        mlogOrder.cust_no = value.CustomerId;
                        mlogOrder.con_no = value.ContractNo;
                        mlogOrder.channel_id = value.ChannelCode;
                        mlogOrder.pay_amt = value.PayAmt;
                        mlogOrder.trans_amt = value.Amount;
                        mlogOrder.device_id = value.DeviceId;
                        mlogOrder.tel = value.PhoneNumber;
                        mlogOrder.note = "ไม่พบสัญญาของลูกค้า";
                        mlogOrder.ip_addr = value.IPAddress;
                        log.logOrder(mlogOrder);
                        //mlog.cust_no = value.CustomerId;
                        //mlog.device_id = value.DeviceId;
                        //mlog.ip_addr = value.IPAddress;
                        //mlog.note = "ไม่พบสัญญาของลูกค้า";
                        //mlog.url = "api/authen/newpayment2";
                        //log.logRequest(mlog);
                        monitor.sendMessage(url, clientHostname, value, new { request_status = "FAILURE", desc = "Not found contract!", data = contract });
                        return Ok(new { code = 400, message = "ไม่พบข้อมูลสัญญาในระบบ", data = contract });
                    }
                }
                else
                {
                    mlogOrder = new m_LogOrder();
                    mlogOrder.cust_no = value.CustomerId;
                    mlogOrder.con_no = value.ContractNo;
                    mlogOrder.channel_id = value.ChannelCode;
                    mlogOrder.pay_amt = value.PayAmt;
                    mlogOrder.trans_amt = value.Amount;
                    mlogOrder.device_id = value.DeviceId;
                    mlogOrder.tel = value.PhoneNumber;
                    mlogOrder.note = "ไม่พบข้อมูลลูกค้าในระบบ";
                    mlogOrder.ip_addr = value.IPAddress;
                    log.logOrder(mlogOrder);
                    //mlog.cust_no = value.CustomerId;
                    //mlog.device_id = value.DeviceId;
                    //mlog.ip_addr = value.IPAddress;
                    //mlog.note = "ไม่พบข้อมูลลูกค้า";
                    //mlog.url = "api/authen/newpayment2";
                    //log.logRequest(mlog);
                    monitor.sendMessage(url, clientHostname, value, new { request_status = "FAILURE", desc = "Not found customer!", data = cust });
                    return Ok(new { code = 400, message = "ไม่พบข้อมูลลูกค้าในระบบ", data = cust });
                }
            }
            catch (Exception e)
            {
                mlog = new m_LogReq();
                mlog.cust_no = value.CustomerId;
                mlog.device_id = value.DeviceId;
                mlog.ip_addr = value.IPAddress;
                mlog.note = e.Message;
                mlog.url = "api/authen/newpayment2";
                log.logRequest(mlog);
                monitor.sendMessage(url, clientHostname, value, new { request_status = "FAILURE", Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        [Route("api/payment/getstatus")]
        public IHttpActionResult GetPaymentStatus(int order_no)
        {
            var setting = (AppSettingsSection)WebConfigurationManager.OpenWebConfiguration("~").GetSection("appSettings");
            var appService = setting.Settings["AppService"].Value;
            var paymentService = setting.Settings["PaymentService"].Value;
            if (appService == "False" || paymentService == "False")
                return Unauthorized();
            string clientHostname = HttpContext.Current.Request.UserHostName;
            string url = HttpContext.Current.Request.Path;
            Payment payment = new Payment();
            try
            {
                var transaction = payment.getTransactionByOrderNo(order_no);
                PaymentStatusRes res = payment.getPaymentStatus(transaction.TRANS_NO);
                return Ok(new { code = 200, message = "ตรวจสอบรายการชำระสำเร็จ", data = res });
            }
            catch (Exception e)
            {
                mlog = new m_LogReq();
                mlog.note = e.Message;
                mlog.url = "api/authen/newpayment2";
                log.logRequest(mlog);
                monitor.sendMessage(url, clientHostname, new { trans_no = order_no }, new { request_status = "FAILURE", Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        [Route("api/test/getdate")]
        public IHttpActionResult GetDate()
        {
            //string date = "20180712173122";
            //var newDate = DateTime.ParseExact(date, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            string clientHostname = HttpContext.Current.Request.UserHostName;
            string url = HttpContext.Current.Request.Path;
            return Ok(DateTime.Now.ToShortTimeString());
            //Payment payment = new Payment();
            //return Ok(payment.testSaveDate());
        }
        [Route("api/payment/getchannel")]
        public IHttpActionResult GetBankCode()
        {
            string clientHostname = HttpContext.Current.Request.UserHostName;
            string url = HttpContext.Current.Request.Path;
            try
            {
                Payment payment = new Payment();
                var banks = payment.getChannelCode();
                monitor.sendMessage(url, clientHostname, "none", new { request_status = "SUCCESS", desc = "รหัสธนาคาร", data = banks });
                return Ok(new { code = 200, message = "ดึงรหัสธนาคารสำเร็จ", data = banks });
            }
            catch (Exception e)
            {
                monitor.sendMessage(url, clientHostname, "none", new { Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        //[Route("api/payment/notify")]
        //public IHttpActionResult PostSendMessage([FromBody]NotifyPayment value)
        //{
        //    TransactionHub hub = new TransactionHub();
        //    hub.SendMessage(value.connectionId, value.success);
        //    return Json(new { result = "sent" });
        //}
        [Route("api/payment/notify/chillpay")]
        public IHttpActionResult PostNotifyChillpay([FromBody]PaymentStatusRes value)
        {
            string clientHostname = HttpContext.Current.Request.UserHostName;
            string url = HttpContext.Current.Request.Path;
            Payment payment = new Payment();
            try
            {
                if (value.Code == 200 && value.PaymentStatus == 0)
                {
                    payment.setStatusOrder(Int32.Parse(value.OrderNo), "SUC");
                }
                else if (value.Code == 200 && value.PaymentStatus != 0)
                {
                    if (value.PaymentStatus == 1)
                        payment.setStatusOrder(Int32.Parse(value.OrderNo), "FAL");
                    else if (value.PaymentStatus == 2 || value.PaymentStatus == 4)
                        payment.setStatusOrder(Int32.Parse(value.OrderNo), "CAN");
                    else if (value.PaymentStatus == 3)
                        payment.setStatusOrder(Int32.Parse(value.OrderNo), "ERR");
                }
                else if (value.Code != 200)
                {
                    if (value.Code < 2007)
                        payment.setStatusOrder(Int32.Parse(value.OrderNo), "CAN");
                    else
                        payment.setStatusOrder(Int32.Parse(value.OrderNo), "ERR");
                }
                payment.updateTransaction(value);

                TransactionHub hub = new TransactionHub();
                hub.NotifyPayment(value);
                monitor.sendMessage(url, clientHostname, value, new { request_status = "SUCCESS" });
            }
            catch (Exception e)
            {
                monitor.sendMessage(url, clientHostname, value, new { Message = e.Message });
            }
            return Ok();
        }
        [Route("api/line/test")]
        public IHttpActionResult GetLine()
        {
            Payment payment = new Payment();
            payment.sendMessageToLine("test");
            return Json(new { result = "sent" });
        }
        //[Route("api/line/webhook")]
        //public IHttpActionResult PostWebhook([FromBody]lineData value)
        //{
        //    if (value.events.First().message.type == "text")
        //    {
        //        var text2 = value.events.First().message.text.Split(new char[0]);
        //        var cust_no = Int32.Parse(text2[0]);
        //        var message = text2[1];
        //        message = String.Join("", text2).Replace(text2[0], "");
        //        m_SMS010 sms = new m_SMS010();
        //        sms.CUST_NO = cust_no;
        //        sms.CON_NO = string.Empty;
        //        sms.SMS_NOTE = message;
        //        sms.SENDER_TYPE = "SYSTEM";
        //        sms.READ_STATUS = "UNREAD";
        //        Notification noti = new Notification();
        //        sms.SMS010_PK = noti.createSms(sms);
        //        sms.SMS_TIME = DateTime.Now;


        //        //chat.SendSmsByConnId(sms);

        //        //monitor.sendMessage(url, clientHostname, value, new { request_status = "SUCCESS", desc = "Admin ส่งข้อความ", data = sms });
        //    }
        //    return Ok();
        //}
        [Route("api/line/webhook")]
        public IHttpActionResult PostWebhook([FromBody]lineData value)
        {
            Line line = new Line();
            foreach (var e in value.events)
            {
                if (e.type == "message")
                {
                    if (e.message.type == "text")
                    {
                        if (e.message.text == "ลงทะเบียน")
                        {
                            var process = line.getProcessByUserId(e.source.userId);
                            if (process == null)
                            {
                                line.setRegister(e.source.userId, "PENDING", "First register", "SUCCESS");
                                line.sendMessageUserId(e.source.userId, "กรุณากรอกเลขประจำตัวประชาชน");
                            }
                            else
                            {
                                if (process.PROCESS == "REGISTER")
                                {
                                    if (process.PROCESS_STATUS == "SUCCESS")
                                    {
                                        line.sendMessageUserId(e.source.userId, "ข้อมูลของคุณได้รับการลงทะเบียนแล้ว");
                                    }
                                    else
                                    {
                                        if (process.ACTION == "First register")
                                        {
                                            line.sendMessageUserId(e.source.userId, "กรุณากรอกเลขประจำตัวประชาชน");
                                        }
                                        else if (process.ACTION == "Input citizen_no")
                                        {
                                            if (process.ACTION_STATUS == "FAILED")
                                            {
                                                line.sendMessageUserId(e.source.userId, "กรุณากรอกเลขประจำตัวประชาชน");
                                            }
                                            else if (process.ACTION_STATUS == "SUCCESS")
                                            {
                                                line.sendMessageUserId(e.source.userId, "กรุณากรอกหมายเลขโทรศัพท์");
                                            }
                                        }
                                        else if (process.ACTION == "Input phone_no")
                                        {
                                            if (process.ACTION_STATUS == "FAILED")
                                            {
                                                line.sendMessageUserId(e.source.userId, "กรุณากรอกหมายเลขโทรศัพท์");
                                            }
                                            else if (process.ACTION_STATUS == "SUCCESS")
                                            {
                                                line.sendMessageUserId(e.source.userId, "ข้อมูลของคุณได้รับการลงทะเบียนแล้ว");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (line.IsNumeric(e.message.text))
                            {
                                if (e.message.text.Length == 13)
                                {
                                    var process = line.getProcessByUserId(e.source.userId);
                                    if (process != null)
                                    {
                                        if (process.PROCESS == "REGISTER")
                                        {
                                            if (process.PROCESS_STATUS == "PENDING")
                                            {
                                                if (process.ACTION == "First register")
                                                {
                                                    var profile = user.getProfileByCitizenNo(e.message.text);
                                                    if (profile == null)
                                                    {
                                                        line.setRegister(e.source.userId, "PENDING", "Input citizen_no", "FAILED");
                                                        line.sendMessageUserId(e.source.userId, "ไม่พบเลขประจำตัวประชาชนของคุณในระบบ");
                                                    }
                                                    else
                                                    {
                                                        line.setRegister(e.source.userId, "PENDING", "Input citizen_no", "SUCCESS");
                                                        line.sendMessageUserId(e.source.userId, "กรุณากรอกหมายเลขโทรศัพท์");
                                                    }
                                                }
                                                else if (process.ACTION == "Input citizen_no" && process.ACTION_STATUS == "FAILED")
                                                {
                                                    var profile = user.getProfileByCitizenNo(e.message.text);
                                                    if (profile == null)
                                                    {
                                                        line.setRegister(e.source.userId, "PENDING", "Input citizen_no", "FAILED");
                                                        line.sendMessageUserId(e.source.userId, "ไม่พบเลขประจำตัวประชาชนของคุณในระบบ");
                                                    }
                                                    else
                                                    {
                                                        line.setRegister(e.source.userId, "PENDING", "Input citizen_no", "SUCCESS");
                                                        line.sendMessageUserId(e.source.userId, "กรุณากรอกหมายเลขโทรศัพท์");
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                                else if (e.message.text.Length == 10)
                                {
                                    var process = line.getProcessByUserId(e.source.userId);
                                    if (process != null)
                                    {
                                        if (process.PROCESS == "REGISTER")
                                        {
                                            if (process.PROCESS_STATUS == "PENDING")
                                            {
                                                if (process.ACTION == "Input citizen_no" && process.ACTION_STATUS == "SUCCESS")
                                                {
                                                    var profile = user.getProfileByPhoneNO(e.message.text);
                                                    if (profile == null)
                                                    {
                                                        line.setRegister(e.source.userId, "PENDING", "Input phone_no", "FAILED");
                                                        line.sendMessageUserId(e.source.userId, "ไม่หมายเลขโทรศัพท์ของคุณในระบบ");
                                                    }
                                                    else
                                                    {
                                                        line.setRegister(e.source.userId, "SUCCESS", "Input phone_no", "SUCCESS");
                                                        line.sendMessageUserId(e.source.userId, $"ยินดีต้อนรับคุณ{profile.CUST_NAME}");
                                                    }

                                                }
                                                else if (process.ACTION == "Input phone_no" && process.ACTION_STATUS == "FAILED")
                                                {
                                                    var profile = user.getProfileByPhoneNO(e.message.text);
                                                    if (profile == null)
                                                    {
                                                        line.setRegister(e.source.userId, "PENDING", "Input phone_no", "FAILED");
                                                        line.sendMessageUserId(e.source.userId, "ไม่พบเลขประจำตัวประชาชนของคุณในระบบ");
                                                    }
                                                    else
                                                    {
                                                        line.setRegister(e.source.userId, "SUCCESS", "Input phone_no", "SUCCESS");
                                                        line.sendMessageUserId(e.source.userId, $"ยินดีต้อนรับคุณ{profile.CUST_NAME}");
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Ok();
        }
        public class lineData
        {
            public lineEvent[] events { get; set; }
        }
        public class lineEvent
        {
            public string replyToken { get; set; }
            public string type { get; set; }
            public DateTime timestamp { get; set; }
            public lineSource source { get; set; }
            public lineMessage message { get; set; }
        }
        public class lineSource
        {
            public string type { get; set; }
            public string userId { get; set; }
        }
        public class lineMessage
        {
            public string id { get; set; }
            public string type { get; set; }
            public string text { get; set; }
        }
    }
}
