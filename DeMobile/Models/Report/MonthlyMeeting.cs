using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.Report
{
    public class MonthlyMeeting
    {
        public string brhId { get; set; }
        public double saleAmt { get; set; }
        public double payAmt { get; set; }
        public double difTarAmt { get; set; }
        public double accSaleAmt { get; set; }
        public double accPayAmt { get; set; }
        public double accTarAmt { get; set; }
        public double tarAmt { get; set; }
        public double losPdoAmt { get; set; }
        public double pdoAmt { get; set; }
        public double OcustPdoAmt { get; set; }
        public double NcustPdoAmt { get; set; }
        public double fRemainAmt { get; set; }
        public string mgrName { get; set; }
    }
}