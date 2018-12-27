using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class Contract
    {
        public string CON_NO { get; set; }
        public int CUST_NO { get; set; }
        public int TOT_AMT { get; set; }
        public int PAY_AMT { get; set; }
        public int PERIOD { get; set; }
        public int BAL_AMT { get; set; }
    }
}