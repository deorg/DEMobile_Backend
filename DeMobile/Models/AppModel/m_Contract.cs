using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class m_Contract
    {
        public string CON_NO { get; set; }
        public int CUST_NO { get; set; }
        public int TOT_AMT { get; set; }
        public int PAY_AMT { get; set; }
        public int PERIOD { get; set; }
        public int BAL_AMT { get; set; }
        public DateTime CON_DATE { get; set; }
        public int DISC_AMT { get; set; }
    }
    public class m_ConPayment
    {
        public string CON_NO { get; set; }
        public DateTime PAY_DATE { get; set; }
        public int PAY_AMT { get; set; }
    }
}