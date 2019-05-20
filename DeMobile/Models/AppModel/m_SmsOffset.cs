using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class m_SmsOffset
    {
        public int cust_no { get; set; }
        public int skip { get; set; }
        public int take { get; set; }
        public string device_id { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public double app_version { get; set; }
        public string api_version { get; set; }
    }
}