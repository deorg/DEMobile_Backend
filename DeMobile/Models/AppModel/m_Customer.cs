using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class m_Customer
    {
        public int CUST_NO { get; set; }
        public string CUST_NAME { get; set; }
        public string CITIZEN_NO { get; set; } 
        public string TEL { get; set; }
        public string PERMIT { get; set; }
    }
    public class m_device
    {
        public string device_id { get; set; }
        public int cust_no { get; set; }
        public string conn_id { get; set; }
        public string device_status { get; set; }
        public DateTime created_time { get; set; }
    }
}