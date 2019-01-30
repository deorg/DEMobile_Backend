using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class m_SMS010
    {
        public int SMS010_PK { get; set; }
        public string CON_NO { get; set; }
        public string SMS_NOTE { get; set; }
        public DateTime SMS_TIME { get; set; }
    }
    public class m_SMS020
    {
        public int SMS020_PK { get; set; }
        public string CUST_NO { get; set; }
        public string TITLE { get; set; }
        public string CONTENT { get; set; }
        public string OTP { get; set; }
        public DateTime SEND_DT { get; set; }
        public DateTime EXPIRE_DT { get; set; }
        public string STATUS { get; set; }
    }

    public class m_ConfirmOTP
    {
        public int cust_no { get; set; }
        public string otp { get; set; }
    }

    public class m_Notification
    {
        public int id { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public bool read { get; set; }
        public int cust_no { get; set; }
        public DateTime created_time { get; set; }
    }

    public class m_LineNoti
    {
        public List<string> to { get; set; }
        public List<m_LineMessage> messages { get; set; }
    }
    public class m_LineMessage
    {
        public string type { get; set; }
        public string text { get; set; }
    }
}