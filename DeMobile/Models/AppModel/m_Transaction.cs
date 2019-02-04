using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.AppModel
{
    public class m_Transaction
    {
        public int TRANS_NO { get; set; }
        public int ORDER_NO { get; set; }
        public int CUST_NO { get; set; }
        public string CHANNEL_ID { get; set; }
        public int REQ_STATUS_ID { get; set; }
        public int TRANS_STATUS_ID { get; set; }
        public int PAY_AMT { get; set; }
        public string RETURN_URL { get; set; }
        public string PAYMENT_URL { get; set; }
        public string IP_ADDR { get; set; }
        public string TOKEN { get; set; }
        public DateTime CREATED_TIME { get; set; }
        public DateTime EXPIRE_TIME { get; set; }
        public string BANK_REF_CODE { get; set; }
        public int? RESULT_STATUS_ID { get; set; }
        public DateTime? PAYMENT_TIME { get; set; }
    }
}