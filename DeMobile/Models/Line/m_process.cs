using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.Line
{
    public class m_process
    {
        public int MPAY300_SEQ { get; set; }
        public string LINE_USER_ID { get; set; }
        public string PROCESS { get; set; }
        public string PROCESS_STATUS { get; set; }
        public DateTime CREATED_TIME { get; set; }
        public string ACTION { get; set; }
        public string ACTION_STATUS { get; set; }
    }
}