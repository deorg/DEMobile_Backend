﻿using DeMobile.Hubs;
using DeMobile.Models.AppModel;
using DeMobile.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Linq;

namespace DeMobile.Controllers
{
    public class AuthenticationController : ApiController
    {
        MonitorHub monitor = new MonitorHub();
        private User _user = new User();
        Log log = new Log();
        Payment payment = new Payment();

        [Route("api/authen/logout")]
        public IHttpActionResult GetLogout(int cust_no)
        {
            //m_LogReg mlog = new m_LogReg();
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string url = HttpContext.Current.Request.Path;
            try
            {
                _user.logout(cust_no);
                monitor.sendMessage(url, IPAddress, cust_no, new { code = 200, message = "ออกจากระบบสำเร็จ" });
                return Ok(new { code = 200, message = "ออกจากระบบสำเร็จ" });
            }
            catch
            {
                monitor.sendMessage(url, IPAddress, cust_no, new { code = 411, message = "ออกจากระบบไม่สำเร็จ" });
                return Ok(new { code = 411, message = "ออกจากระบบไม่สำเร็จ" });
            }
        }
        [Route("api/authen/checkphone")]
        public IHttpActionResult GetCheckPhone(string phone)
        {
            //m_LogReq mlog = new m_LogReq();
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string url = HttpContext.Current.Request.Path;
            try
            {
                var customers = _user.getProfilesByPhone(phone);
                if(customers != null)
                {
                    monitor.sendMessage(url, IPAddress, phone, new { code = 200, message = "พบเบอร์โทรศัพท์", data = customers });
                    return Ok(new { code = 200, message = "พบเบอร์โทรศัพท์", data = customers });
                }
                else
                {
                    monitor.sendMessage(url, IPAddress, phone, new { code = 405, message = "พบเบอร์โทรศัพท์", data = customers });
                    return Ok(new { code = 405, message = "ไม่พบหมายเลขโทรศัพท์ของท่านในระบบ", data = customers });
                }
            }
            catch(Exception e)
            {
                monitor.sendMessage(url, IPAddress, phone, new { code = 500, message = e.Message, data = new List<m_Customer>() });
                return Ok(new { code = 500, message = e.Message, data = new List<m_Customer>() });
            }
        }
        #region ลงทะเบียนติดตั้ง Application DMobile
        [Route("api/authen/register")]
        public IHttpActionResult PostRegister([FromBody]m_Register data)
        {
            m_LogReq mlog = new m_LogReq();
            //m_LogReg mlog = new m_LogReg();
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string url = HttpContext.Current.Request.Path;
            try
            {
                //var setting = (AppSettingsSection)WebConfigurationManager.OpenWebConfiguration("~").GetSection("appSettings");
                //var appService = setting.Settings["AppService"].Value;
                //if (appService == "False")
                //    return Unauthorized();             
                data.ip_addr = IPAddress;

                int cust_no;

                if (data.cust_no != 0)
                    cust_no = data.cust_no;
                else
                    cust_no = _user.getCustNoByPhoneNo(data.phone_no);

                if(cust_no == 0)
                {
                    mlog.cust_no = 0;
                    mlog.device_id = data.device_id;
                    mlog.tel = data.phone_no;
                    mlog.serial_sim = data.serial_sim;
                    mlog.ip_addr = IPAddress;
                    mlog.action = "REGISTER";
                    mlog.status = "FAIL";
                    mlog.note = "ไม่พบหมายเลขโทรศัพท์ลูกค้าในระบบ";
                    mlog.brand = data.brand;
                    mlog.model = data.model;
                    mlog.app_version = data.app_version;
                    mlog.api_version = data.api_version;
                    //mlog.brand = data.brand;
                    //mlog.model = data.model;
                    //mlog.api_version = data.api_version;
                    //log.logSignup(mlog);
                    log.logSignin(mlog);
                    monitor.sendMessage(url, IPAddress, data, new { code = 405, message = "ไม่พบหมายเลขโทรศัพท์ลูกค้าในระบบ!", data = new m_Customer() });
                    return Ok(new { code = 405, message = "ไม่พบหมายเลขโทรศัพท์ลูกค้าในระบบ!", data = new m_Customer() });
                }
                var result2 = _user.getProfileById(cust_no);
                //if (result == null)
                //{
                //    mlog.cust_no = 0;
                //    mlog.device_id = data.device_id;
                //    mlog.tel = data.phone_no;
                //    mlog.serial_sim = data.serial_sim;
                //    mlog.ip_addr = IPAddress;
                //    mlog.action = "REGISTER";
                //    mlog.status = "FAIL";
                //    mlog.note = "ไม่พบเลขประจำตัวประชาชนลูกค้าในระบบ";
                //    log.logSignin(mlog);
                //    monitor.sendMessage(url, IPAddress, data, new { code = 406, message = "ไม่พบเลขประจำตัวประชาชนของลูกค้าในระบบ!", data = result });
                //    return Ok(new { code = 406, message = "ไม่พบเลขประจำตัวประชาชนของลูกค้าในระบบ!", data = result });
                //}
                if (result2 == null)
                {
                    mlog.cust_no = 0;
                    mlog.device_id = data.device_id;
                    mlog.tel = data.phone_no;
                    mlog.serial_sim = data.serial_sim;
                    mlog.ip_addr = IPAddress;
                    mlog.action = "REGISTER";
                    mlog.status = "FAIL";
                    mlog.note = "ไม่พบหมายเลขโทรศัพท์ลูกค้าในระบบ";
                    mlog.brand = data.brand;
                    mlog.model = data.model;
                    mlog.api_version = data.api_version;
                    mlog.app_version = data.app_version;
                    //mlog.brand = data.brand;
                    //mlog.model = data.model;
                    //mlog.api_version = data.api_version;
                    //log.logSignup(mlog);
                    log.logSignin(mlog);
                    monitor.sendMessage(url, IPAddress, data, new { code = 405, message = "ไม่พบหมายเลขโทรศัพท์ลูกค้าในระบบ!", data = result2 });
                    return Ok(new { code = 405, message = "ไม่พบหมายเลขโทรศัพท์ลูกค้าในระบบ!", data = result2 });
                }
                else
                {
                    var broadcast = _user.getBroadcast();
                    var version = _user.getAppVersion(data.serial_sim);
                    var chat = _user.getChatOn();

                    var currentDevice = _user.checkCurrentDevice(data.device_id);
                    if (currentDevice != null)
                    {                   
                        _user.registerCurrentDevice(data, result2.CUST_NO);
                        //Notification otp = new Notification();
                        //otp.sendOTP(result.CUST_NO);

                        mlog.cust_no = result2.CUST_NO;
                        mlog.device_id = data.device_id;
                        mlog.tel = result2.TEL;
                        mlog.serial_sim = data.serial_sim;
                        mlog.ip_addr = IPAddress;
                        mlog.action = "REGISTER CURRENT DEVICE";
                        mlog.status = "SUCCESS";
                        mlog.note = "ลงทะเบียนสำเร็จ";
                        mlog.brand = data.brand;
                        mlog.model = data.model;
                        mlog.app_version = data.app_version;
                        mlog.api_version = data.api_version;
                        //mlog.brand = data.brand;
                        //mlog.model = data.model;
                        //mlog.api_version = data.api_version;
                        //log.logSignup(mlog);
                        log.logSignin(mlog);
                        monitor.sendMessage(url, IPAddress, data, new { code = 200, message = "ลงทะเบียนสำเร็จ", data = result2 });
                        return Ok(new { code = 200, message = "ข้อมูลถูกต้อง", data = new m_identify { CUST_NO = result2.CUST_NO, CUST_NAME = result2.CUST_NAME, CITIZEN_NO = result2.CITIZEN_NO, TEL = result2.TEL, PERMIT = result2.PERMIT, CHAT = chat, APP_VERSION = version, BROADCAST = broadcast } });
                        //return Ok(new { code = 200, message = "ลงทะเบียนสำเร็จ", data = result2 });
                    }
                    else
                    {
                        var devices = _user.getDeviceByCustNo(result2.CUST_NO);
                        if(devices != null)
                        {
                            _user.registerNewDevice(data, result2.CUST_NO);
                            mlog.cust_no = result2.CUST_NO;
                            mlog.device_id = data.device_id;
                            mlog.tel = result2.TEL;
                            mlog.serial_sim = data.serial_sim;
                            mlog.ip_addr = IPAddress;
                            mlog.action = "REGISTER NEW DEVICE";
                            mlog.status = "SUCCESS";
                            mlog.note = "ลงทะเบียนสำเร็จ";
                            mlog.brand = data.brand;
                            mlog.model = data.model;
                            mlog.app_version = data.app_version;
                            mlog.api_version = data.api_version;
                            //log.logSignup(mlog);
                            log.logSignin(mlog);
                            payment.sendMessageToLine($"[{result2.CUST_NO.ToString()}] คุณ{result2.CUST_NAME} => ลงทะเบียนสำเร็จ");
                            monitor.sendMessage(url, IPAddress, data, new { code = 200, message = "ลงทะเบียนสำเร็จ", data = result2 });
                            return Ok(new { code = 200, message = "ข้อมูลถูกต้อง", data = new m_identify { CUST_NO = result2.CUST_NO, CUST_NAME = result2.CUST_NAME, CITIZEN_NO = result2.CITIZEN_NO, TEL = result2.TEL, PERMIT = result2.PERMIT, CHAT = chat, APP_VERSION = version, BROADCAST = broadcast } });
                            //return Ok(new { code = 200, message = "ลงทะเบียนสำเร็จ", data = result2 });
                        }
                        else
                        {
                            _user.registerDevice(data, result2.CUST_NO);
                            //Notification otp = new Notification();
                            //otp.sendOTP(result.CUST_NO);
                            mlog.cust_no = result2.CUST_NO;
                            mlog.device_id = data.device_id;
                            mlog.tel = result2.TEL;
                            mlog.serial_sim = data.serial_sim;
                            mlog.ip_addr = IPAddress;
                            mlog.action = "REGISTER NEW DEVICE";
                            mlog.status = "SUCCESS";
                            mlog.note = "ลงทะเบียนสำเร็จ";
                            mlog.brand = data.brand;
                            mlog.model = data.model;
                            mlog.app_version = data.app_version;
                            mlog.api_version = data.api_version;
                            //log.logSignup(mlog);
                            log.logSignin(mlog);
                            payment.sendMessageToLine($"[{result2.CUST_NO.ToString()}] คุณ{result2.CUST_NAME} => ลงทะเบียนสำเร็จ");
                            monitor.sendMessage(url, IPAddress, data, new { code = 200, message = "ลงทะเบียนสำเร็จ", data = result2 });
                            return Ok(new { code = 200, message = "ลงทะเบียนสำเร็จ", data = result2 });
                        }
                    }
                }
                //else
                //{
                //    mlog.cust_no = 0;
                //    mlog.device_id = data.device_id;
                //    mlog.tel = data.phone_no;
                //    mlog.serial_sim = data.serial_sim;
                //    mlog.ip_addr = IPAddress;
                //    mlog.action = "REGISTER";
                //    mlog.status = "FAIL";
                //    mlog.note = "ไม่พบข้อมูลลูกค้า";
                //    log.logSignin(mlog);
                //    monitor.sendMessage(url, IPAddress, data, new { code = 400, message = "ไม่พบข้อมูลค้า!", data = result });
                //    return Ok(new { code = 400, message = "ไม่พบข้อมูลค้า!", data = result });
                //}
            }
            catch (Exception e)
            {
                mlog.cust_no = 0;
                mlog.device_id = data.device_id;
                mlog.tel = data.phone_no;
                mlog.serial_sim = data.serial_sim;
                mlog.ip_addr = IPAddress;
                mlog.action = "REGISTER";
                mlog.status = "FAIL";
                mlog.note = e.Message;
                mlog.brand = data.brand;
                mlog.model = data.model;
                mlog.app_version = data.app_version;
                mlog.api_version = data.api_version;
                //log.logSignup(mlog);
                log.logSignin(mlog);
                monitor.sendMessage(url, IPAddress, data, new { code = 500, message = e.Message, data = data });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        #endregion

        #region ระบุตัวตนก่อนเข้าใช้งาน Application
        [Route("api/authen/identify")]
        public IHttpActionResult GetCheckPhone(string serial_sim, string deviceId, double app_version)
        {
            //var setting = (AppSettingsSection)WebConfigurationManager.OpenWebConfiguration("~").GetSection("appSettings");
            //var appService = setting.Settings["AppService"].Value;
            //if (appService == "False")
            //    return Unauthorized();
            m_LogReq mlog = new m_LogReq();
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string url = HttpContext.Current.Request.Path;
            try
            {            
                m_Customer result = new m_Customer();
                if (serial_sim == "1111111111" || serial_sim == "2222222222")
                    result = _user.getProfileByDeviceId(deviceId);
                //else
                    //result = _user.getProfileBySerialSim(serial_sim);

                if (result != null && result.CUST_NO != 0)
                {
                    if (result.PERMIT == "SMS" || result.PERMIT == "BOTH")
                    {
                        var device = _user.checkCurrentDevice(deviceId);
                        if (device != null)
                        {                    
                            if (app_version != device.app_version)
                                _user.updateAppVersion(app_version, deviceId);

                            _user.updateIdentify(deviceId);
                            var chat = _user.getChatOn();
                            if (device.device_status == "ACT")
                            {
                                var version = _user.getAppVersion(serial_sim);
                                var broadcast = _user.getBroadcast();

                                mlog.cust_no = result.CUST_NO;
                                mlog.device_id = deviceId;
                                mlog.tel = result.TEL;
                                mlog.serial_sim = serial_sim;
                                mlog.ip_addr = IPAddress;
                                mlog.action = "IDENTIFY";
                                mlog.status = "SUCCESS";
                                mlog.note = "ระบุตัวตนสำเร็จ";
                                mlog.app_version = app_version;
                                log.logSignin(mlog);
                                monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version = app_version*/ }, new { code = 200, message = "ระบุตัวตนสำเร็จ", data = result });
                                return Ok(new { code = 200, message = "ข้อมูลถูกต้อง", data = new m_identify { CUST_NO = result.CUST_NO, CUST_NAME = result.CUST_NAME, CITIZEN_NO = result.CITIZEN_NO, TEL = result.TEL, PERMIT = result.PERMIT, CHAT = chat, APP_VERSION = version, BROADCAST = broadcast } });
                            }
                            else if (device.device_status == "CHANGE_TEL")
                            {
                                mlog.cust_no = result.CUST_NO;
                                mlog.device_id = deviceId;
                                mlog.tel = result.TEL;
                                mlog.serial_sim = serial_sim;
                                mlog.ip_addr = IPAddress;
                                mlog.action = "IDENTIFY";
                                mlog.status = "FAIL";
                                mlog.note = "ลูกค้าเปลี่ยนหมายเลขโทรศัพท์";
                                mlog.app_version = app_version;
                                log.logSignin(mlog);
                                monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version = app_version*/ }, new { code = 402, message = "ข้อมูลลูกค้าอยู่ในขั้นตอนการเปลี่ยนหมายเลขโทรศัพท์", data = result });
                                return Ok(new { code = 402, message = "กรุณาใช้หมายเลขโทรศัพท์ใหม่", data = result });
                            }
                            else
                            {
                                mlog.cust_no = result.CUST_NO;
                                mlog.device_id = deviceId;
                                mlog.tel = result.TEL;
                                mlog.serial_sim = serial_sim;
                                mlog.ip_addr = IPAddress;
                                mlog.action = "IDENTIFY";
                                mlog.status = "FAIL";
                                mlog.note = "เครื่องลูกค้าถูกระงับการใช้งาน";
                                mlog.app_version = app_version;
                                //log.logSignin(mlog);
                                monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version= app_version */}, new { code = 403, message = "เครื่องลูกค้าถูกระงับการใช้งาน!", data = result });
                                return Ok(new { code = 403, message = "เครื่องลูกค้าถูกระงับการใช้งาน!", data = result });
                            }                           
                        }
                        else
                        {
                            mlog.cust_no = result.CUST_NO;
                            mlog.device_id = deviceId;
                            mlog.tel = result.TEL;
                            mlog.serial_sim = serial_sim;
                            mlog.ip_addr = IPAddress;
                            mlog.action = "IDENTIFY";
                            mlog.status = "FAIL";
                            mlog.note = "ไม่พบเครื่องลูกค้าในระบบ";
                            mlog.app_version = app_version;
                            log.logSignin(mlog);
                            monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version = app_version*/}, new { code = 404, message = "ไม่พบเครื่องลูกค้าในระบบ!", data = result });
                            return Ok(new { code = 404, message = "ไม่พบเครื่องลูกค้าในระบบ!", data = result });
                        }
                    }
                    else
                    {
                        mlog.cust_no = result.CUST_NO;
                        mlog.device_id = deviceId;
                        mlog.tel = result.TEL;
                        mlog.serial_sim = serial_sim;
                        mlog.ip_addr = IPAddress;
                        mlog.action = "IDENTIFY";
                        mlog.status = "FAIL";
                        mlog.note = "ลูกค้าถูกระงับบริการ SMS";
                        mlog.app_version = app_version;
                        log.logSignin(mlog);
                        monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version = app_version*/ }, new { code = 401, message = "ลูกค้าถูกระงับบริการ SMS!", data = result });
                        return Ok(new { code = 401, message = "ลูกค้าถูกระงับบริการ SMS!", data = result });
                    }
                }
                else
                {
                    mlog.cust_no = 0;
                    mlog.device_id = deviceId;
                    mlog.tel = string.Empty;
                    mlog.serial_sim = serial_sim;
                    mlog.ip_addr = IPAddress;
                    mlog.action = "IDENTIFY";
                    mlog.status = "FAIL";
                    mlog.note = "ไม่พบเครื่องของลูกค้าในระบบ";

                    //log.logSignin(mlog);
                    monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version = app_version*/ }, new { code = 407, message = "ไม่พบเลขซิมการ์ดของลูกค้าในระบบ!", data = result });
                    return Ok(new { code = 409, message = "ไม่พบเครื่องของลูกค้าในระบบ!", data = result });
                }
            }
            catch (Exception e)
            {
                mlog.cust_no = 0;
                mlog.device_id = deviceId;
                mlog.tel = string.Empty;
                mlog.serial_sim = serial_sim;
                mlog.ip_addr = IPAddress;
                mlog.action = "IDENTIFY";
                mlog.status = "FAIL";
                mlog.note = e.Message;
                mlog.app_version = app_version;
                log.logSignin(mlog);
                monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version = app_version */}, new { Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        #endregion

        #region ระบุตัวตนก่อนเข้าใช้งาน Application2
        [Route("api/authen/identify2")]
        public IHttpActionResult GetCheckPhone2(string serial_sim, string deviceId, string brand, string model, double app_version, string api_version)
        {
            //var setting = (AppSettingsSection)WebConfigurationManager.OpenWebConfiguration("~").GetSection("appSettings");
            //var appService = setting.Settings["AppService"].Value;
            //if (appService == "False")
            //    return Unauthorized();
            m_LogReq mlog = new m_LogReq();
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string url = HttpContext.Current.Request.Path;
            try
            {
                m_Customer result = new m_Customer();
                if (serial_sim == "1111111111" || serial_sim == "2222222222")
                    result = _user.getProfileByDeviceId(deviceId);
                //else
                //result = _user.getProfileBySerialSim(serial_sim);

                if (result != null && result.CUST_NO != 0)
                {
                    if (result.PERMIT == "SMS" || result.PERMIT == "BOTH")
                    {
                        var device = _user.checkCurrentDevice(deviceId);
                        if (device != null)
                        {
                            if (app_version != device.app_version)
                                _user.updateAppVersion(app_version, deviceId);

                            _user.updateIdentify(deviceId);
                            var chat = _user.getChatOn();
                            if (device.device_status == "ACT")
                            {
                                var version = _user.getAppVersion(serial_sim);
                                var broadcast = _user.getBroadcast();

                                mlog.cust_no = result.CUST_NO;
                                mlog.device_id = deviceId;
                                mlog.tel = result.TEL;
                                mlog.serial_sim = serial_sim;
                                mlog.ip_addr = IPAddress;
                                mlog.action = "IDENTIFY";
                                mlog.status = "SUCCESS";
                                mlog.note = "ระบุตัวตนสำเร็จ";
                                mlog.brand = brand;
                                mlog.model = model;
                                mlog.app_version = app_version;
                                mlog.api_version = api_version;
                                log.logSignin(mlog);
                                monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version = app_version*/ }, new { code = 200, message = "ระบุตัวตนสำเร็จ", data = result });
                                return Ok(new { code = 200, message = "ข้อมูลถูกต้อง", data = new m_identify { CUST_NO = result.CUST_NO, CUST_NAME = result.CUST_NAME, CITIZEN_NO = result.CITIZEN_NO, TEL = result.TEL, PERMIT = result.PERMIT, CHAT = chat, APP_VERSION = version, BROADCAST = broadcast } });
                            }
                            else if (device.device_status == "CHANGE_TEL")
                            {
                                mlog.cust_no = result.CUST_NO;
                                mlog.device_id = deviceId;
                                mlog.tel = result.TEL;
                                mlog.serial_sim = serial_sim;
                                mlog.ip_addr = IPAddress;
                                mlog.action = "IDENTIFY";
                                mlog.status = "FAIL";
                                mlog.note = "ลูกค้าเปลี่ยนหมายเลขโทรศัพท์";
                                mlog.brand = brand;
                                mlog.model = model;
                                mlog.app_version = app_version;
                                mlog.api_version = api_version;
                                log.logSignin(mlog);
                                monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version = app_version*/ }, new { code = 402, message = "ข้อมูลลูกค้าอยู่ในขั้นตอนการเปลี่ยนหมายเลขโทรศัพท์", data = result });
                                return Ok(new { code = 402, message = "กรุณาใช้หมายเลขโทรศัพท์ใหม่", data = result });
                            }
                            else
                            {
                                mlog.cust_no = result.CUST_NO;
                                mlog.device_id = deviceId;
                                mlog.tel = result.TEL;
                                mlog.serial_sim = serial_sim;
                                mlog.ip_addr = IPAddress;
                                mlog.action = "IDENTIFY";
                                mlog.status = "FAIL";
                                mlog.note = "เครื่องลูกค้าถูกระงับการใช้งาน";
                                mlog.brand = brand;
                                mlog.model = model;
                                mlog.app_version = app_version;
                                mlog.api_version = api_version;
                                //log.logSignin(mlog);
                                monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version= app_version */}, new { code = 403, message = "เครื่องลูกค้าถูกระงับการใช้งาน!", data = result });
                                return Ok(new { code = 403, message = "เครื่องลูกค้าถูกระงับการใช้งาน!", data = result });
                            }
                        }
                        else
                        {
                            mlog.cust_no = result.CUST_NO;
                            mlog.device_id = deviceId;
                            mlog.tel = result.TEL;
                            mlog.serial_sim = serial_sim;
                            mlog.ip_addr = IPAddress;
                            mlog.action = "IDENTIFY";
                            mlog.status = "FAIL";
                            mlog.note = "ไม่พบเครื่องลูกค้าในระบบ";
                            mlog.brand = brand;
                            mlog.model = model;
                            mlog.app_version = app_version;
                            mlog.api_version = api_version;
                            log.logSignin(mlog);
                            monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version = app_version*/}, new { code = 404, message = "ไม่พบเครื่องลูกค้าในระบบ!", data = result });
                            return Ok(new { code = 404, message = "ไม่พบเครื่องลูกค้าในระบบ!", data = result });
                        }
                    }
                    else
                    {
                        mlog.cust_no = result.CUST_NO;
                        mlog.device_id = deviceId;
                        mlog.tel = result.TEL;
                        mlog.serial_sim = serial_sim;
                        mlog.ip_addr = IPAddress;
                        mlog.action = "IDENTIFY";
                        mlog.status = "FAIL";
                        mlog.note = "ลูกค้าถูกระงับบริการ SMS";
                        mlog.brand = brand;
                        mlog.model = model;
                        mlog.app_version = app_version;
                        mlog.api_version = api_version;
                        log.logSignin(mlog);
                        monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version = app_version*/ }, new { code = 401, message = "ลูกค้าถูกระงับบริการ SMS!", data = result });
                        return Ok(new { code = 401, message = "ลูกค้าถูกระงับบริการ SMS!", data = result });
                    }
                }
                else
                {
                    mlog.cust_no = 0;
                    mlog.device_id = deviceId;
                    mlog.tel = string.Empty;
                    mlog.serial_sim = serial_sim;
                    mlog.ip_addr = IPAddress;
                    mlog.action = "IDENTIFY";
                    mlog.status = "FAIL";
                    mlog.note = "ไม่พบเครื่องของลูกค้าในระบบ";

                    //log.logSignin(mlog);
                    monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version = app_version*/ }, new { code = 407, message = "ไม่พบเลขซิมการ์ดของลูกค้าในระบบ!", data = result });
                    return Ok(new { code = 409, message = "ไม่พบเครื่องของลูกค้าในระบบ!", data = result });
                }
            }
            catch (Exception e)
            {
                mlog.cust_no = 0;
                mlog.device_id = deviceId;
                mlog.tel = string.Empty;
                mlog.serial_sim = serial_sim;
                mlog.ip_addr = IPAddress;
                mlog.action = "IDENTIFY";
                mlog.status = "FAIL";
                mlog.note = e.Message;
                mlog.brand = brand;
                mlog.model = model;
                mlog.app_version = app_version;
                mlog.api_version = api_version;
                log.logSignin(mlog);
                monitor.sendMessage(url, IPAddress, new { serial_sim = serial_sim, deviceId = deviceId/*, app_version = app_version */}, new { Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        #endregion

        #region ดูข้อมูลส่วนตัวของลูกค้า
        [Route("api/customer/profile")]
        public IHttpActionResult GetProfile(int id)
        {
            //User cust = new User();
            //var setting = (AppSettingsSection)WebConfigurationManager.OpenWebConfiguration("~").GetSection("appSettings");
            //var appService = setting.Settings["AppService"].Value;
            //if (appService == "False")
            //    return Unauthorized();
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
                    //mlog = new m_LogReq();
                    //mlog.ip_addr = IPAddress;
                    //mlog.note = "มีคนพยายามแอบอ้างเข้าถึงข้อมูล Profile ของลูกค้าโดยไม่ได้รับอนุญาต";
                    //mlog.url = "api/customer/profile";
                    //log.logRequest(mlog);
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
        #endregion

        #region ดูข้อมูล SMS ของลูกค้าแบบ Offset
        [Route("api/customer/sms/offset")]
        public IHttpActionResult GetSmsOffset(int id, int skip, int take)
        {
            m_LogReq mlog;
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string url = HttpContext.Current.Request.Path;
            try
            {
                var result = _user.getProfileById(id);
                var app_version = _user.getAppVersionByCust_no(id);
                //app_version = 3.2;
                if (result != null && result.CUST_NO != 0)
                {
                    mlog = new m_LogReq();
                    mlog.cust_no = id;
                    //mlog.device_id = ;
                    mlog.tel = result.TEL;
                   // mlog.serial_sim = serial_sim;
                    mlog.ip_addr = IPAddress;
                    mlog.action = "RETRIEVE SMS";
                    mlog.status = "SUCCESS";
                    mlog.note = "ดึงข้อมูล SMS สำเร็จ";
                    var sms = _user.getNotification(id);
                    _user.updateReadSms(id);
                    if (sms.Count > 0)
                    {
                        if (app_version < 3.2)
                        {
                            sms.RemoveAll(s => s.MSG_TYPE == "IMAGE");
                        }

                        if (skip != 0)
                            skip = skip - 5;
                        sms = sms.OrderByDescending(p => p.SMS010_PK).Skip(skip).Take(5).ToList();
                        if(skip == 0)
                            sms = sms.OrderBy(p => p.SMS010_PK).ToList();           
                    }
                    log.logSignin(mlog);
                    monitor.sendMessage(url, IPAddress, new { id = id, skip = skip, take = take }, new { data = sms });
                    return Ok(new { code = 200, message = "ดึงข้อมูล Sms สำเร็จ", data = sms });
                }
                else
                {
                    //mlog = new m_LogReq();
                    //mlog.cust_no = id;
                    //mlog.tel = result.TEL;
                    //mlog.ip_addr = IPAddress;
                    //mlog.action = "RETRIEVE SMS";
                    //mlog.status = "FAIL";
                    //mlog.note = "ไม่พบข้อมูลลูกค้าในระบบ";
                    monitor.sendMessage(url, IPAddress, new { id = id, skip = skip, take = take }, new { Message = "Not found customer!" });
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
                monitor.sendMessage(url, IPAddress, new { id = id }, new { Type = "Error", Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        #endregion

        #region ดูข้อมูล SMS ของลูกค้าแบบ Offset2
        [Route("api/customer/sms/offset2")]
        public IHttpActionResult PostSmsOffset2(m_SmsOffset value)
        {
            m_LogActivity mlog;
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string url = HttpContext.Current.Request.Path;
            try
            {
                var result = _user.getProfileById(value.cust_no);
                var app_version = _user.getAppVersionByCust_no(value.cust_no);
                //app_version = 3.2;
                if (result != null && result.CUST_NO != 0)
                {
                    mlog = new m_LogActivity();
                    mlog.cust_no = value.cust_no;
                    mlog.device_id = value.device_id;
                    mlog.tel = result.TEL;
                    // mlog.serial_sim = serial_sim;
                    mlog.ip_addr = IPAddress;
                    mlog.action = "RETRIEVE SMS";
                    mlog.status = "SUCCESS";
                    mlog.note = "ดึงข้อมูล SMS สำเร็จ";
                    mlog.brand = value.brand;
                    mlog.model = value.model;
                    mlog.app_version = value.app_version;
                    mlog.api_version = value.api_version;
                    var sms = _user.getNotification(value.cust_no);
                    _user.updateReadSms(value.cust_no);
                    if (sms.Count > 0)
                    {
                        if (app_version < 3.2)
                        {
                            sms.RemoveAll(s => s.MSG_TYPE == "IMAGE");
                        }

                        if (value.skip != 0)
                            value.skip = value.skip - 5;
                        sms = sms.OrderByDescending(p => p.SMS010_PK).Skip(value.skip).Take(5).ToList();
                        if (value.skip == 0)
                            sms = sms.OrderBy(p => p.SMS010_PK).ToList();
                    }
                    log.logActivity(mlog);
                    monitor.sendMessage(url, IPAddress, new { id = value.cust_no, skip = value.skip, take = value.take }, new { data = sms });
                    return Ok(new { code = 200, message = "ดึงข้อมูล Sms สำเร็จ", data = sms });
                }
                else
                {
                    //mlog = new m_LogReq();
                    //mlog.cust_no = id;
                    //mlog.tel = result.TEL;
                    //mlog.ip_addr = IPAddress;
                    //mlog.action = "RETRIEVE SMS";
                    //mlog.status = "FAIL";
                    //mlog.note = "ไม่พบข้อมูลลูกค้าในระบบ";
                    monitor.sendMessage(url, IPAddress, new { id = value.cust_no, skip = value.skip, take = value.take }, new { Message = "Not found customer!" });
                    return Ok(new { code = 400, message = "ไม่พบข้อมูลลูกค้าในระบบ", data = result });
                }
            }
            catch (Exception e)
            {
                mlog = new m_LogActivity();
                mlog.ip_addr = IPAddress;
                mlog.note = e.Message;
                mlog.url = "api/customer/sms";
                log.logRequest2(mlog);
                monitor.sendMessage(url, IPAddress, new { id = value.cust_no }, new { Type = "Error", Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        #endregion

        #region ดูข้อมูล SMS ของลูกค้า
        [Route("api/customer/sms")]
        public IHttpActionResult GetSms(int id)
        {
            //var setting = (AppSettingsSection)WebConfigurationManager.OpenWebConfiguration("~").GetSection("appSettings");
            //var appService = setting.Settings["AppService"].Value;
            //var smsService = setting.Settings["SmsService"].Value;
            //if (appService == "False" || smsService == "False")
            //    return Unauthorized();
            //User cust = new User();
            m_LogReq mlog;
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string url = HttpContext.Current.Request.Path;
            try
            {
                var app_version = _user.getAppVersionByCust_no(id);
                var result = _user.getProfileById(id);
                if (result != null && result.CUST_NO != 0)
                {
                    var sms = _user.getNotification(id);
                    _user.updateReadSms(id);
                    if (app_version < 3.2)
                    {
                        sms.RemoveAll(s => s.MSG_TYPE == "IMAGE");
                    }
                    mlog = new m_LogReq();
                    mlog.cust_no = id;
                    //mlog.device_id = ;
                    mlog.tel = result.TEL;
                    // mlog.serial_sim = serial_sim;
                    mlog.ip_addr = IPAddress;
                    mlog.action = "RETRIEVE SMS";
                    mlog.status = "SUCCESS";
                    mlog.note = "ดึงข้อมูล SMS สำเร็จ";
                    log.logSignin(mlog);
                    monitor.sendMessage(url, IPAddress, new { id = id }, new { data = sms });
                    return Ok(new { code = 200, message = "ดึงข้อมูล Sms สำเร็จ", data = sms });
                }
                else
                {
                    //mlog = new m_LogReq();
                    //mlog.ip_addr = IPAddress;
                    //mlog.note = "มีคนพยายามแอบอ้างเข้าถึงข้อมูล SMS ของลูกค้าโดยไม่ได้รับอนุญาต";
                    //mlog.url = "api/customer/sms";
                    //log.logRequest(mlog);
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
                monitor.sendMessage(url, IPAddress, new { id = id }, new { Type = "Error", Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        #endregion

        #region กำหนดสถานะให้ข้อความว่าลูกค้าอ่านแล้ว
        [Route("api/customer/sms/marktoread")]
        public IHttpActionResult PostMarktoRead([FromBody]m_CustReadMsg value)
        {
            m_LogReq mlog;
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            string url = HttpContext.Current.Request.Path;
            try
            {
                var result = _user.getProfileById(value.cust_no);
                if (result != null && result.CUST_NO != 0)
                {
                    _user.readSms(value);
                    monitor.sendMessage(url, IPAddress, value, value);
                    return Ok(new { code = 200, message = "อัพเดท sms สำเร็จ", data = value });
                }
                else
                {
                    //mlog = new m_LogReq();
                    //mlog.ip_addr = IPAddress;
                    //mlog.note = "มีคนพยายามแอบอ้างเข้าถึงข้อมูล SMS ของลูกค้าโดยไม่ได้รับอนุญาต";
                    //mlog.url = "api/customer/sms/marktoread";
                    //log.logRequest(mlog);
                    monitor.sendMessage(url, IPAddress, value, new { Message = "Not found customer!" });
                    return Ok(new { code = 400, message = "ไม่พบข้อมูลลูกค้าในระบบ", data = result });
                }
            }
            catch (Exception e)
            {
                mlog = new m_LogReq();
                mlog.ip_addr = IPAddress;
                mlog.note = e.Message;
                mlog.url = "api/customer/sms/marktoread";
                log.logRequest(mlog);
                monitor.sendMessage(url, IPAddress, value, new { Message = e.Message });
                return Ok(new { code = 500, message = e.Message, data = string.Empty });
            }
        }
        #endregion

        #region ดูข้อมูลสัญญาของลูกค้า
        [Route("api/customer/contract")]
        public IHttpActionResult GetContract(int id)
        {
            //User cust = new User();
            //var setting = (AppSettingsSection)WebConfigurationManager.OpenWebConfiguration("~").GetSection("appSettings");
            //var appService = setting.Settings["AppService"].Value;
            //var paymentService = setting.Settings["PaymentService"].Value;
            //if (appService == "False" || paymentService == "False")
            //    return Unauthorized();
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
                    //mlog = new m_LogReq();
                    //mlog.ip_addr = IPAddress;
                    //mlog.note = "มีคนพยายามแอบอ้างเข้าถึงข้อมูลสัญญาของลูกค้าโดยไม่ได้รับอนุญาต";
                    //mlog.url = "api/customer/contract";
                    //log.logRequest(mlog);
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
        #endregion

        #region ดูข้อมูลการชำระค่างวดของสัญญานั้นๆ
        [Route("api/customer/payment")]
        public IHttpActionResult GetPayment(string no)
        {
            //User payment = new User();
            //var setting = (AppSettingsSection)WebConfigurationManager.OpenWebConfiguration("~").GetSection("appSettings");
            //var appService = setting.Settings["AppService"].Value;
            //var paymentService = setting.Settings["PaymentService"].Value;
            //if (appService == "False" || paymentService == "False")
            //    return Unauthorized();
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
        #endregion
    }
}
