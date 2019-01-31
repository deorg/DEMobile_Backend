using DeMobile.Hubs;
using DeMobile.Models.AppModel;
using DeMobile.Services;
using System;
using System.Web;
using System.Web.Http;

namespace DeMobile.Controllers
{
    public class AuthenticationController : ApiController
    {
        MonitorHub monitor = new MonitorHub();
        [Route("api/authen/register")]
        public IHttpActionResult PostRegister([FromBody]m_Register data)
        {
            User cust = new User();
            Log log = new Log();       
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            try
            {
                var result = cust.getProfileByCitizenNo(data.citizen_no);
                var result2 = cust.getProfileByPhoneNO(data.phone_no);
                if ((result != null) && (result2 != null))
                {
                    var currentDevice = cust.checkCurrentDevice(data.device_id);
                    if (currentDevice != null)
                    {
                        m_LogReq mlog = new m_LogReq();
                        mlog.cust_no = result.CUST_NO;
                        mlog.device_id = currentDevice.device_id;
                        mlog.ip_addr = IPAddress;
                        mlog.note = "ผู้ใช้งานลงทะเบียนซ้ำ / มีคนพยายามแอบอ้างลงทะเบียนเครื่องด้วยข้อมูลของลูกค้า";
                        mlog.url = "api/authen/register";
                        log.logRequest(mlog);
                        monitor.sendMessage(data, new { code = 400, message = "เครื่องนี้ได้รับการลงทะเบียนแล้ว!", data = result });
                        return Ok(new { code = 400, message = "เครื่องนี้ได้รับการลงทะเบียนแล้ว!", data = result });
                    }
                    else
                    {
                        var resInset = cust.registerDevice(data, result.CUST_NO);
                        //Notification otp = new Notification();
                        //otp.sendOTP(result.CUST_NO);
                        monitor.sendMessage(data, new { code = 200, message = "ลงทะเบียนสำเร็จ", data = result });
                        return Ok(new { code = 200, message = "ลงทะเบียนสำเร็จ", data = result });
                    }
                }
                else
                {
                    m_LogReq mlog = new m_LogReq();
                    mlog.cust_no = result.CUST_NO;
                    mlog.device_id = data.device_id;
                    mlog.ip_addr = IPAddress;
                    mlog.note = "ไม่พบข้อมูลลูกค้า / มีคนพยายามแอบอ้างลงทะเบียนเครื่อง";
                    mlog.url = "api/authen/register";
                    log.logRequest(mlog);
                    monitor.sendMessage(data, new { code = 400, message = "ไม่พบข้อมูลค้า!", data = result });
                    return Ok(new { code = 400, message = "ไม่พบข้อมูลค้า!", data = result });
                }
            }
            catch (Exception e)
            {
                return Ok(new { code = 500, message = e.Message });
            }
        }
        [Route("api/authen/identify")]
        public IHttpActionResult GetCheckPhone(string phone, string deviceId)
        {
            User cust = new User();
            Log log = new Log();
            m_LogReq mlog;
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            try
            {
                var result = cust.getProfileByPhoneNO(phone);
                if (result != null && result.CUST_NO != 0)
                {
                    var device = cust.checkCurrentDevice(deviceId);
                    if (device != null)
                    {
                        if(device.device_status == "ACT")
                            return Ok(new { code = 200, message = "ข้อมูลถูกต้อง", data = result });
                        else
                        {
                            mlog = new m_LogReq();
                            mlog.device_id = deviceId;
                            mlog.cust_no = result.CUST_NO;
                            mlog.ip_addr = IPAddress;
                            mlog.note = "เครื่องลูกค้าถูกระงับการใช้งาน";
                            mlog.url = "api/authen/identify";
                            log.logRequest(mlog);
                            monitor.sendMessage(new { phone = phone, deviceId = deviceId }, new { code = 400, message = "เครื่องลูกค้าถูกระงับการใช้งาน!", data = result });
                            return Ok(new { code = 400, message = "เครื่องลูกค้าถูกระงับการใช้งาน!", data = result });
                        }
                    }
                    else
                    {
                        mlog = new m_LogReq();
                        mlog.device_id = deviceId;
                        mlog.cust_no = result.CUST_NO;
                        mlog.ip_addr = IPAddress;
                        mlog.note = "ไม่พบเครื่องผู้ใช้งานในระบบ / ผู้ใช้งานเปลี่ยนเครื่อง";
                        mlog.url = "api/authen/identify";
                        log.logRequest(mlog);
                        monitor.sendMessage(new { phone = phone, deviceId = deviceId }, new { code = 400, message = "ไม่พบเครื่องผู้ใช้งานในระบบ!", data = result });
                        return Ok(new { code = 400, message = "ไม่พบเครื่องผู้ใช้งานในระบบ!", data = result });
                    }
                }
                else
                {
                    mlog = new m_LogReq();
                    mlog.device_id = deviceId;
                    mlog.ip_addr = IPAddress;
                    mlog.note = "ไม่พบเบอร์โทรศัพท์ผู้ใช้งานในระบบ / ผู้ใช้งานเปลี่ยนเบอร์โทรศัพท์";
                    mlog.url = "api/authen/identify";
                    log.logRequest(mlog);
                    monitor.sendMessage(new { phone = phone, deviceId = deviceId }, new { code = 400, message = "ไม่พบเบอร์โทรศัพท์ผู้ใช้งานในระบบ!", data = result });
                    return Ok(new { code = 400, message = "ไม่พบเบอร์โทรศัพท์ผู้ใช้งานในระบบ!", data = result });
                }
                //var result = cust.getProfileByCitizenNo(data.citizen_no);
                //var result2 = cust.getProfileByPhoneNO(data.phone_no);
                //if ((result != null) && (result2 != null))
                //{
                //    var currentDevice = cust.checkCurrentDevice(data.device_id);
                //    if (currentDevice)
                //        return Ok(new { code = 400, message = "เครื่องนี้ได้รับการลงทะเบียนแล้ว!", data = result });
                //    else
                //    {
                //        var resInset = cust.registerDevice(data, result.CUST_NO);
                //        return Ok(new { code = 200, message = "ลงทะเบียนสำเร็จ", data = result });
                //    }
                //}
                //else
                //    return Ok(new { code = 400, message = "ไม่พบข้อมูลค้า!", data = result });
            }
            catch (Exception e)
            {
                mlog = new m_LogReq();
                mlog.device_id = deviceId;
                mlog.ip_addr = IPAddress;
                mlog.note = e.Message;
                mlog.url = "api/authen/identify";
                log.logRequest(mlog);
                monitor.sendMessage(new { phone = phone, deviceId = deviceId }, new { Message = e.Message });
                return Ok(new { code = 500, message = e.Message });
            }
        }
        [Route("api/customer/profile")]
        public IHttpActionResult GetProfile(int id)
        {
            User cust = new User();
            Log log = new Log();
            m_LogReq mlog;
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            try
            {
                var result = cust.getProfileById(id);
                if (result != null && result.CUST_NO != 0)
                    return Json(result);
                else
                {
                    mlog = new m_LogReq();
                    mlog.ip_addr = IPAddress;
                    mlog.note = "มีคนพยายามแอบอ้างเข้าถึงข้อมูล Profile ของลูกค้าโดยไม่ได้รับอนุญาต";
                    mlog.url = "api/customer/profile";
                    log.logRequest(mlog);
                    monitor.sendMessage(new { id = id }, new { Message = "Not found customer!"});
                    return BadRequest("Not found customer!");
                }
            }
            catch(Exception e)
            {
                mlog = new m_LogReq();
                mlog.ip_addr = IPAddress;
                mlog.note = e.Message;
                mlog.url = "api/customer/profile";
                log.logRequest(mlog);
                monitor.sendMessage(new { id = id }, new { Message = e.Message });
                return InternalServerError(e.InnerException);
            }
        }
        [Route("api/customer/sms")]
        public IHttpActionResult GetSms(int id)
        {
            User cust = new User();
            Log log;
            m_LogReq mlog;
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            try
            {
                var result = cust.getProfileById(id);
                if (result != null && result.CUST_NO != 0)
                {
                    var sms = cust.getNotification(id);
                    return Json(sms);
                }
                else
                {
                    log = new Log();
                    mlog = new m_LogReq();
                    mlog.ip_addr = IPAddress;
                    mlog.note = "มีคนพยายามแอบอ้างเข้าถึงข้อมูล SMS ของลูกค้าโดยไม่ได้รับอนุญาต";
                    mlog.url = "api/customer/sms";
                    log.logRequest(mlog);
                    monitor.sendMessage(new { id = id }, new { Message = "Not found customer!" });
                    return BadRequest("Not found customer!");
                }
            }
            catch(Exception e)
            {
                log = new Log();
                mlog = new m_LogReq();
                mlog.ip_addr = IPAddress;
                mlog.note = e.Message;
                mlog.url = "api/customer/sms";
                log.logRequest(mlog);
                monitor.sendMessage(new { id = id }, new { Message = e.Message });
                return InternalServerError(e.InnerException);
            }
        }
        [Route("api/customer/contract")]
        public IHttpActionResult GetContract(int id)
        {
            User cust = new User();
            Log log;
            m_LogReq mlog;
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            try
            {
                var result = cust.getProfileById(id);
                if (result != null && result.CUST_NO != 0)
                {
                    var contract = cust.getContract(id);
                    monitor.sendMessage(new { id = id }, contract);
                    return Json(contract);
                }
                else
                {
                    log = new Log();
                    mlog = new m_LogReq();
                    mlog.ip_addr = IPAddress;
                    mlog.note = "มีคนพยายามแอบอ้างเข้าถึงข้อมูลสัญญาของลูกค้าโดยไม่ได้รับอนุญาต";
                    mlog.url = "api/customer/contract";
                    log.logRequest(mlog);
                    monitor.sendMessage(new { id = id }, new { Message = "Not found customer!" });
                    return BadRequest("Not found customer!");
                }
            }
            catch(Exception e)
            {
                log = new Log();
                mlog = new m_LogReq();
                mlog.ip_addr = IPAddress;
                mlog.note = e.Message;
                mlog.url = "api/customer/contract";
                log.logRequest(mlog);
                monitor.sendMessage(new { id = id }, new { Message = e.Message });
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
                monitor.sendMessage(new { no = no }, result);
                return Json(result);
            }
            catch(Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}
