using DeMobile.Hubs;
using DeMobile.Models;
using DeMobile.Models.AppModel;
using DeMobile.Models.PaymentGateway;
using DeMobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DeMobile.Controllers
{
    public class PaymentController : ApiController
    {
        //[Route("api/payment/newpayment")]
        //public IHttpActionResult PostNewPayment([FromBody]PaymentReq value)
        //{
        //    Payment payment = new Payment();
        //    if (!ModelState.IsValid)
        //        return BadRequest("Invalid parameter!");
        //    PaymentRes res = payment.createPayment(value);
        //    if (res == null)
        //        return Json(new { request_status = "FAILURE", desc = "Internal server error / Invalid parameter!", data = res });
        //    else
        //        return Json(new { request_status = "SUCCESS", desc = "Requested to Payment Gateway", data = res });
        //}
        User user;
        MonitorHub monitor = new MonitorHub();
        Log log = new Log();
        m_LogReq mlog;
        [Route("api/payment/newpayment2")]
        public IHttpActionResult PostNewPayment2([FromBody]PaymentReq value)
        {
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string clientHostname = HttpContext.Current.Request.UserHostName;
            string url = HttpContext.Current.Request.Path;
            try
            {             
                //value.OrderNo = "test001";
                value.Description = "testAPI";
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
                        Payment payment = new Payment();
                        PaymentRes res = payment.createPayment(value, IPAddress);
                        if (res == null)
                        {
                            mlog.cust_no = value.CustomerId;
                            mlog.device_id = value.DeviceId;
                            mlog.ip_addr = IPAddress;
                            mlog.note = "ระบบขัดข้อง ไม่สามารถทำรายการได้";
                            mlog.url = "api/authen/newpayment2";
                            log.logRequest(mlog);
                            monitor.sendMessage(url, clientHostname, value, new { request_status = "FAILURE", desc = "Internal server error / Invalid parameter!", data = res });
                            return Ok(new { request_status = "FAILURE", desc = "Internal server error / Invalid parameter!", data = res });
                        }
                        else
                        {
                            monitor.sendMessage(url, clientHostname, value, new { request_status = "SUCCESS", desc = "Requested to Payment Gateway", data = res });
                            return Ok(new { request_status = "SUCCESS", desc = "Requested to Payment Gateway", data = res });
                        }
                    }
                    else
                    {
                        mlog.cust_no = value.CustomerId;
                        mlog.device_id = value.DeviceId;
                        mlog.ip_addr = IPAddress;
                        mlog.note = "ไม่พบสัญญาของลูกค้า";
                        mlog.url = "api/authen/newpayment2";
                        log.logRequest(mlog);
                        monitor.sendMessage(url, clientHostname, value, new { request_status = "FAILURE", desc = "Not found contract!", data = contract });
                        return Ok(new { request_status = "FAILURE", desc = "Not found contract!", data = contract });
                    }
                }
                mlog.cust_no = value.CustomerId;
                mlog.device_id = value.DeviceId;
                mlog.ip_addr = IPAddress;
                mlog.note = "ไม่พบข้อมูลลูกค้า";
                mlog.url = "api/authen/newpayment2";
                log.logRequest(mlog);
                monitor.sendMessage(url, clientHostname, value, new { request_status = "FAILURE", desc = "Not found customer!", data = cust });
                return Ok(new { request_status = "FAILURE", desc = "Not found customer!", data = cust });
            }
            catch(Exception e)
            {
                mlog.cust_no = value.CustomerId;
                mlog.device_id = value.DeviceId;
                mlog.ip_addr = IPAddress;
                mlog.note = e.Message;
                mlog.url = "api/authen/newpayment2";
                log.logRequest(mlog);
                monitor.sendMessage(url, clientHostname, value, new { request_status = "FAILURE", Message = e.Message });
                return InternalServerError(e.InnerException);
            }
        }
        [Route("api/payment/getstatus")]
        public IHttpActionResult GetPaymentStatus(int trans_no)
        {
            string clientHostname = HttpContext.Current.Request.UserHostName;
            string url = HttpContext.Current.Request.Path;
            Payment payment = new Payment();
            try
            {
                PaymentStatusRes res = payment.getPaymentStatus(trans_no);
                return Ok(res);
            }
            catch(Exception e)
            {
                mlog.note = e.Message;
                mlog.url = "api/authen/newpayment2";
                log.logRequest(mlog);
                monitor.sendMessage(url, clientHostname, new { trans_no = trans_no }, new { request_status = "FAILURE", Message = e.Message });
                return InternalServerError(e.InnerException);
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
                var banks = payment.getChanneCode();
                monitor.sendMessage(url, clientHostname, "none", new { request_status = "SUCCESS", desc = "รหัสธนาคาร", data = banks });
                return Ok(new { request_status = "SUCCESS", desc = "รหัสธนาคาร", data = banks });
            }
            catch(Exception e)
            {
                monitor.sendMessage(url, clientHostname, "none", new { Message = e.Message });
                return InternalServerError(e.InnerException);
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
                    payment.setStatusOrder(Int32.Parse(value.OrderNo), "SUC");
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
            catch(Exception e)
            {
                monitor.sendMessage(url, clientHostname, value, new { Message = e.Message });
            }
            return Ok();
        }
        [Route("api/line/test")]
        public IHttpActionResult GetLine()
        {
            Payment payment = new Payment();
            payment.sendMessageToLine();
            return Json(new { result = "sent" });
        }
    }
}
