﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeMobile.Models.PaymentGateway
{
    public class PaymentReq
    {
        public string OrderNo { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public int Amount { get; set; }
        public string PhoneNumber { get; set; } = "";
        public string Description { get; set; }
        [Required]
        public string ChannelCode { get; set; }
    }
}