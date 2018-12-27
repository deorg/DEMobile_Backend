using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models.PaymentGateway
{
    public class PaymentRes
    {
        public int Status { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public int TransactionId { get; set; }
        public int Amount { get; set; }
        public string OrderNo { get; set; }
        public string CustomerId { get; set; }
        public string ChannelCode { get; set; }
        public string ReturnUrl { get; set; }
        public string PaymentUrl { get; set; }
        public string IpAddress { get; set; }
        public string Token { get; set; }
        public string CreatedDate { get; set; }
        public string ExpiredDate { get; set; }
    }
}