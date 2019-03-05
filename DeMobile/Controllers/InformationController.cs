using System;
using System.Web.Http;
using DeMobile.Models.Management;
using DeMobile.Services;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Web.Configuration;

namespace DeMobile.Controllers
{
    public class InformationController : ApiController
    {
        Information info = new Information();

        /// <summary>
        /// ส่วนจัดการระบบและ monitor
        /// </summary>
        /// <returns></returns>

        #region ดูคำอธิบาย status code จาก server
        [Route("api/information/getstatuscode")]
        public IHttpActionResult GetStatusCode()
        {
            try
            {
                var code = info.getStatusCode();
                return Ok(new { status = "SUCCESS", data = code });
            }
            catch(Exception e)
            {
                return Ok(new { status = "FAILURE", data = e.Message });
            }
        }
        #endregion

        #region เรียกดูข้อมูลสำหรับหน้า Dashboard
        [Route("api/information/dashboard")]
        public IHttpActionResult GetNumMember()
        {
            try
            {
                var num = info.getDashBoard();
                return Ok(new { status = "SUCCESS", data = num });
            }
            catch(Exception e)
            {
                return Ok(new { status = "FAILURE", data = e.Message });
            }
        }
        #endregion

        #region เรียกดูประวัติการลงทะเบียน Application Dmobile
        [Route("api/monitor/logregistered")]
        public IHttpActionResult GetLogRegis()
        {
            try
            {
                var result = info.getLogRegistered();
                return Ok(new { status = "SUCCESS", data = result });
            }
            catch(Exception e)
            {
                return Ok(new { status = "FAILURE", data = e.Message });
            }
        }
        #endregion

        #region เรียกดูสถานะของบริการต่างๆ เปิด/ปิด
        [Route("api/management/service")]
        public IHttpActionResult GetManageService()
        {
            try
            {
                ///////////////////// เรียกดูข้อมูลการตั้งค่าบริการต่างๆจากไฟล์ Web.config //////////////////////////////
                Configuration config;
                config = WebConfigurationManager.OpenWebConfiguration("~");
                AppSettingsSection appsettings;
                appsettings = (AppSettingsSection)config.GetSection("appSettings");
                var appService = appsettings.Settings["AppService"].Value;
                var smsService = appsettings.Settings["SmsService"].Value;
                var paymentService = appsettings.Settings["PaymentService"].Value;             

                return Ok(new { status = "SUCCESS", data = new { appService = appService, smsService = smsService, paymentService = paymentService} });
            }
            catch(Exception e)
            {
                return Ok(new { status = "FAILURE", data = e.Message });
            }
        }
        #endregion

        #region จัดการเปิด/ปิด บริการต่างๆ
        [Route("api/management/service")]
        public IHttpActionResult PostManageService([FromBody]m_Service value)
        {
            try
            {
                ///////////////////// แก้ไขข้อมูลตัวแปรบริการต่างๆบนไฟล์ web.config ///////////////////////
                Configuration config;
                config = WebConfigurationManager.OpenWebConfiguration("~");
                AppSettingsSection appsettings;
                appsettings = (AppSettingsSection)config.GetSection("appSettings");
                appsettings.Settings["AppService"].Value = value.appService.ToString();
                appsettings.Settings["SmsService"].Value = value.smsService.ToString();
                appsettings.Settings["PaymentService"].Value = value.paymentService.ToString();
                config.Save();
                return Ok(new { status = "SUCCESS", data = "saved" });
            }
            catch(Exception e)
            {
                return Ok(new { status = "FAILURE", data = e.Message });
            }
        }
        #endregion

    }
}
