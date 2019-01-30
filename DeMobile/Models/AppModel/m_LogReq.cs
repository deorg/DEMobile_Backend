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
}