﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Models
{
    public class SendMessage
    {
        public string username { get; set; }
        public string message { get; set; }
    }
    public class NotifyPayment
    {
        public string connectionId { get; set; }
        public bool success { get; set; }
    }
}