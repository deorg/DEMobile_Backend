using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class ConPayment
    {
        public string CON_NO { get; set; }
        public DateTime PAY_DATE { get; set; }
        public int PAY_AMT { get; set; }
    }
}