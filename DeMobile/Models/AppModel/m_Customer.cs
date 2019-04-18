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
        public string LINE_USER_ID { get; set; }
    }
    public class m_identify
    {
        public int CUST_NO { get; set; }
        public string CUST_NAME { get; set; }
        public string CITIZEN_NO { get; set; }
        public string TEL { get; set; }
        public string PERMIT { get; set; }
        public double APP_VERSION { get; set; }
        public string CHAT { get; set; }
    }
    public class m_device
    {
        public string device_id { get; set; }
        public int cust_no { get; set; }
        //public string conn_id { get; set; }
        public string device_status { get; set; }
        public string tel { get; set; }
        //public string tel_sim { get; set; }
        public string serial_sim { get; set; }
        //public string operator_name { get; set; }
        //public string brand { get; set; }
        //public string model { get; set; }
        //public int api_version { get; set; }
        public string pin { get; set; }
        public DateTime created_time { get; set; }
        public double app_version { get; set; }
    }
}