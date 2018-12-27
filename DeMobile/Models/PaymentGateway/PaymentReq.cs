using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeMobile.Models.PaymentGateway
{
    public class PaymentReq
    {
        [Required]
        public string MerchantCode { get; set; }
        [Required]
        public string OrderNo { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public int Amount { get; set; }
        public int PhoneNumber { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ChannelCode { get; set; }
        [Required] 
        public int Currency { get; set; }
        public string LangCode { get; set; }
        [Required]
        public int RouteNo { get; set; } = 1;
        [Required]
        public string IPAddress { get; set; }
        [Required]
        public string ApiKey { get; set; }
        [Required]
        public string CheckSum { get; set; }
    }
}