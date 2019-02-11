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
        public int CustomerId { get; set; }
        [Required]
        public string ContractNo { get; set; }
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public int Amount { get; set; }
        public double PayAmt { get; set; }
        public string IPAddress { get; set; }
        public string PhoneNumber { get; set; } = "";
        public string Description { get; set; }
        [Required]
        public string ChannelCode { get; set; }
    }
}