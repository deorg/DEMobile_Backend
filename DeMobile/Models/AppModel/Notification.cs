using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class Notification
    {
        public int noti_id { get; set; }
        public string con_id { get; set; }
        public string content { get; set; }
        public DateTime send_dt { get; set; }
    }
}