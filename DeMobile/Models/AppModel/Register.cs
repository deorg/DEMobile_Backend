using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class m_Register
    {
        public string citizen_no { get; set; }
        public string phone_no { get; set; }
        public string device_id { get; set; }
        public string serial_sim { get; set; }
        public string phone_no_sim { get; set; }
        public string operator_name { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public int api_version { get; set; }
        public string pin { get; set; }
        public string ip_addr { get; set; }
        public string platform { get; set; }
        public double app_version { get; set; }
    }
}