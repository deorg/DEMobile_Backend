using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class SMS010
    {
        public int SMS010_PK { get; set; }
        public string CON_NO { get; set; }
        public string SMS_NOTE { get; set; }
        public DateTime SMS_TIME { get; set; }
    }
    public class SMS020
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

    public class LineNoti
    {
        public List<string> to { get; set; }
        public List<LineMessage> messages { get; set; }
    }
    public class LineMessage
    {
        public string type { get; set; }
        public string text { get; set; }
    }
}