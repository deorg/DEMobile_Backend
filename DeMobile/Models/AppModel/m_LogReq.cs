using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class m_LogReq
    {
        public string note { get; set; }
        public int cust_no { get; set; }
        public string device_id { get; set; }
        public string ip_addr { get; set; }
        public string url { get; set; }
    }
    public class m_LogOrder
    {
        public int cust_no { get; set; }
        public string con_no { get; set; }
        public int order_no { get; set; }
        public int trans_no { get; set; }
        public string channel_id { get; set; }
        public double pay_amt { get; set; }
        public int trans_amt { get; set; }
        public string device_id { get; set; }
        public string tel { get; set; }
        public string note { get; set; }
        public string ip_addr { get; set; }
    }
}