using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeMobile.Models.PaymentGateway
{
    public class CpPaymentReq
    {
        [Required]
        public string MerchantCode { get; set; }
        [Required]
        public string OrderNo { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public int Amount { get; set; }
        public string PhoneNumber { get; set; } = "";
        [Required]
        public string Description { get; set; }
        [Required]
        public string ChannelCode { get; set; }
        [Required]
        public int Currency { get; set; }
        public string LangCode { get; set; }
        [Required]
        public int RouteNo { get; set; }
        [Required]
        public string IPAddress { get; set; }
        [Required]
        public string ApiKey { get; set; }
        [Required]
        public string CheckSum { get; set; }
    }
}