using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class Notification
    {
        public int SMS010_PK { get; set; }
        public string CON_NO { get; set; }
        public string SMS_NOTE { get; set; }
        public DateTime SMS_TIME { get; set; }
    }
}