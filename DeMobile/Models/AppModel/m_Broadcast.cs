using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class m_Broadcast
    {
        public string note { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public DateTime created_time { get; set; }
    }
}