using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class m_SMS010
    {
        public int SMS010_PK { get; set; }
        public int CUST_NO { get; set; }
        public string CON_NO { get; set; }
        public string SMS_NOTE { get; set; }
        public DateTime SMS_TIME { get; set; }
        public int? SENDER { get; set; }
        public string SENDER_TYPE { get; set; }
        public int? SMS010_REF { get; set; }
        public string READ_STATUS { get; set; }
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
    public class m_sendSms
    {
        public int SMS010_PK { get; set; }
        public int CUST_NO { get; set; }
        public string CONN_ID { get; set; }
        public string DEVICE_STATUS { get; set; }
        public string SMS_NOTE { get; set; }
        public string CON_NO { get; set; }
        public DateTime SMS_TIME { get; set; }
        public int SENDER { get; set; }
        public string SENDER_TYPE { get; set; }
        public int SMS010_REF { get; set; }
        public string READ_STATUS { get; set; }
    }
    public class m_sendSmsLine
    {
        public int SMS010_PK { get; set; }
        public int CUST_NO { get; set; }
        public string LINE_USER_ID { get; set; }
        public string SMS_NOTE { get; set; }
        public string CON_NO { get; set; }
        public DateTime SMS_TIME { get; set; }
        public int SENDER { get; set; }
        public string SENDER_TYPE { get; set; }
        public int SMS010_REF { get; set; }
        public string READ_STATUS { get; set; }
        public string RECEIVED { get; set; }
    }


    public class m_ConfirmOTP
    {
        public int cust_no { get; set; }
        public string otp { get; set; }
    }

    public class m_Notification
    {
        public int cust_no { get; set; }
        public string con_no { get; set; }
        public string note { get; set; }
        public string conn_id { get; set; }
        public DateTime time { get; set; }
    }

    public class m_CustMessage
    {
        public int sms010_pk { get; set; }
        public int cust_no { get; set; }
        public string message { get; set; }
    }

    public class m_CustReadMsg
    {
        public int cust_no { get; set; }
        public int sms010_pk { get; set; }
    }

    public class m_LineNoti
    {
        public List<string> to { get; set; }
        public List<m_LineMessage> messages { get; set; }
    }
    public class m_LineReply
    {
        public string replyToken { get; set; }
        public List<m_LineMessage> messages { get; set; }
    }
    public class m_LineMessage
    {
        public string type { get; set; }
        public string text { get; set; }
    }

}