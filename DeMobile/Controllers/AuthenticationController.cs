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
        private User _user = new User();
        Log log = new Log();

        [Route("api/authen/register")]
        public IHttpActionResult PostRegister([FromBody]m_Register data)
        {
            //User cust = new User();     
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            data.ip_addr = IPAddress;
            //string IPAddress = HttpContext.Current.Request.UserHostName;
            string url = HttpContext.Current.Request.Path;
            try
            {
                var result = _user.getProfileByCitizenNo(data.citizen_no);
                var result2 = _user.getProfileByPhoneNO(data.phone_no);
                if ((result != null) && (result2 != null))
                {
                    var currentDevice = _user.checkCurrentDevice(data.device_id);
                    if (currentDevice != null)
                    {
                        _user.registerCurrentDevice(data, result.CUST_NO);
                        //Notification otp = new Notification();
                        //otp.sendOTP(result.CUST_NO);
                        monitor.sendMessage(url, IPAddress, data, new { code = 200, message = "ลงทะเบียนสำเร็จ", data = result });
                        return Ok(new { code = 200, message = "ลงทะเบียนสำเร็จ", data = result });
                    }
                    else
                    {
                        _user.registerDevice(data, result.CUST_NO);
                        //Notification otp = new Notification();
                        //otp.sendOTP(result.CUST_NO);
                        monitor.sendMessage(url, IPAddress, data, new { code = 200, message = "ลงทะเบียนสำเร็จ", data = result });
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
                    monitor.sendMessage(url, IPAddress, data, new { code = 400, message = "ไม่พบข้อมูลค้า!", data = result });
                    return Ok(new { code = 400, message = "ไม่พบข้อมูลค้า!", data = result });
                }
            }
            catch (Exception e)
            {
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        [Route("api/authen/identify")]
        public IHttpActionResult GetCheckPhone(string phone, string deviceId)
        {
            //User cust = new User();
            m_LogReq mlog;
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string url = HttpContext.Current.Request.Path;
            try
            {
                var result = _user.getProfileByPhoneNO(phone);
                if (result != null && result.CUST_NO != 0)
                {
                    var device = _user.checkCurrentDevice(deviceId);
                    if (device != null)
                    {
                        if (device.device_status == "ACT")
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
                            monitor.sendMessage(url, IPAddress, new { phone = phone, deviceId = deviceId }, new { code = 400, message = "เครื่องลูกค้าถูกระงับการใช้งาน!", data = result });
                            return Ok(new { code = 400, message = "เครื่องลูกค้าถูกระงับการใช้งาน!", data = result });
                        }
                    }
                    else
                    {
                        mlog = new m_LogReq();
                        mlog.device_id = deviceId;
                        mlog.cust_no = result.CUST_NO;
                        mlog.ip_addr = IPAddress;
                        mlog.note = "ไม่พบเครื่องลูกค้าในระบบ / ลูกค้าเปลี่ยนเครื่อง";
                        mlog.url = "api/authen/identify";
                        log.logRequest(mlog);
                        monitor.sendMessage(url, IPAddress, new { phone = phone, deviceId = deviceId }, new { code = 400, message = "ไม่พบเครื่องลูกค้าในระบบ!", data = result });
                        return Ok(new { code = 400, message = "ไม่พบเครื่องลูกค้าในระบบ!", data = result });
                    }
                }
                else
                {
                    mlog = new m_LogReq();
                    mlog.device_id = deviceId;
                    mlog.ip_addr = IPAddress;
                    mlog.note = "ไม่พบเบอร์โทรศัพท์ลูกค้าในระบบ / ลูกค้าเปลี่ยนเบอร์โทรศัพท์";
                    mlog.url = "api/authen/identify";
                    log.logRequest(mlog);
                    monitor.sendMessage(url, IPAddress, new { phone = phone, deviceId = deviceId }, new { code = 400, message = "ไม่พบเบอร์โทรศัพท์ลูกค้าในระบบ!", data = result });
                    return Ok(new { code = 400, message = "ไม่พบเบอร์โทรศัพท์ลูกค้าในระบบ!", data = result });
                }
            }
            catch (Exception e)
            {
                mlog = new m_LogReq();
                mlog.device_id = deviceId;
                mlog.ip_addr = IPAddress;
                mlog.note = e.Message;
                mlog.url = "api/authen/identify";
                log.logRequest(mlog);
                monitor.sendMessage(url, IPAddress, new { phone = phone, deviceId = deviceId }, new { Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        [Route("api/customer/profile")]
        public IHttpActionResult GetProfile(int id)
        {
            //User cust = new User();
            m_LogReq mlog;
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string url = HttpContext.Current.Request.Path;
            try
            {
                var result = _user.getProfileById(id);
                if (result != null && result.CUST_NO != 0)
                    return Ok(new { code = 200, message = "ค้นหาข้อมูลสำเร็จ", data = result });
                else
                {
                    mlog = new m_LogReq();
                    mlog.ip_addr = IPAddress;
                    mlog.note = "มีคนพยายามแอบอ้างเข้าถึงข้อมูล Profile ของลูกค้าโดยไม่ได้รับอนุญาต";
                    mlog.url = "api/customer/profile";
                    log.logRequest(mlog);
                    monitor.sendMessage(url, IPAddress, new { id = id }, new { Message = "Not found customer!" });
                    return Ok(new { code = 400, message = "ไม่พบข้อมูลลูกค้าในระบบ", data = result });
                }
            }
            catch (Exception e)
            {
                mlog = new m_LogReq();
                mlog.ip_addr = IPAddress;
                mlog.note = e.Message;
                mlog.url = "api/customer/profile";
                log.logRequest(mlog);
                monitor.sendMessage(url, IPAddress, new { id = id }, new { Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        [Route("api/customer/sms")]
        public IHttpActionResult GetSms(int id)
        {
            //User cust = new User();
            m_LogReq mlog;
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string url = HttpContext.Current.Request.Path;
            try
            {
                var result = _user.getProfileById(id);
                if (result != null && result.CUST_NO != 0)
                {
                    var sms = _user.getNotification(id);
                    monitor.sendMessage(url, IPAddress, new { id = id }, new { data = sms });
                    return Ok(new { code = 200, message = "ดึงข้อมูล Sms สำเร็จ", data = sms });
                }
                else
                {
                    mlog = new m_LogReq();
                    mlog.ip_addr = IPAddress;
                    mlog.note = "มีคนพยายามแอบอ้างเข้าถึงข้อมูล SMS ของลูกค้าโดยไม่ได้รับอนุญาต";
                    mlog.url = "api/customer/sms";
                    log.logRequest(mlog);
                    monitor.sendMessage(url, IPAddress, new { id = id }, new { Message = "Not found customer!" });
                    return Ok(new { code = 400, message = "ไม่พบข้อมูลลูกค้าในระบบ", data = result });
                }
            }
            catch (Exception e)
            {
                mlog = new m_LogReq();
                mlog.ip_addr = IPAddress;
                mlog.note = e.Message;
                mlog.url = "api/customer/sms";
                log.logRequest(mlog);
                monitor.sendMessage(url, IPAddress, new { id = id }, new { Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        [Route("api/customer/contract")]
        public IHttpActionResult GetContract(int id)
        {
            //User cust = new User();
            m_LogReq mlog;
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string url = HttpContext.Current.Request.Path;
            try
            {
                var result = _user.getProfileById(id);
                if (result != null && result.CUST_NO != 0)
                {
                    var contract = _user.getContract(id);
                    monitor.sendMessage(url, IPAddress, new { id = id }, contract);
                    return Ok(new { code = 200, message = "ดึงข้อมูลสัญญาสำเร็จ", data = contract });
                }
                else
                {
                    mlog = new m_LogReq();
                    mlog.ip_addr = IPAddress;
                    mlog.note = "มีคนพยายามแอบอ้างเข้าถึงข้อมูลสัญญาของลูกค้าโดยไม่ได้รับอนุญาต";
                    mlog.url = "api/customer/contract";
                    log.logRequest(mlog);
                    monitor.sendMessage(url, IPAddress, new { id = id }, new { Message = "Not found customer!" });
                    return Ok(new { code = 400, message = "ไม่พบข้อมูลลูกค้าในระบบ", data = result });
                }
            }
            catch (Exception e)
            {
                mlog = new m_LogReq();
                mlog.ip_addr = IPAddress;
                mlog.note = e.Message;
                mlog.url = "api/customer/contract";
                log.logRequest(mlog);
                monitor.sendMessage(url, IPAddress, new { id = id }, new { Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        [Route("api/customer/payment")]
        public IHttpActionResult GetPayment(string no)
        {
            //User payment = new User();
            string IPAddress = HttpContext.Current.Request.UserHostName;
            string url = HttpContext.Current.Request.Path;
            try
            {
                var result = _user.getPayment(no);
                monitor.sendMessage(url, IPAddress, new { no = no }, result);
                return Ok(new { code = 200, message = "ดึงข้อมูลการชำระเงินสำเร็จ", data = result });
            }
            catch (Exception e)
            {
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
    }
}
