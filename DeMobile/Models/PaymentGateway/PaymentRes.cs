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
    public class CpPaymentStatusReq
    {
        public string MerchantCode { get; set; }
        public string TransactionId { get; set; }
        public string ApiKey { get; set; }
        public string CheckSum { get; set; }
    }
    public class PaymentStatusRes
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public int Amount { get; set; }
        public string OrderNo { get; set; }
        public string CustomerId { get; set; }
        public string BankCode { get; set; }
        public string PaymentDate { get; set; }
        public int PaymentStatus { get; set; }
        public string BankRefCode { get; set; }
        public int MerchantResponseStatus { get; set; }
        public string MerchantResponseMessage { get; set; }
        public string CurrentDate { get; set; }
        public string CurrentTime { get; set; }
        public string PaymentDescription { get; set; }
    }
    public class PaymentNotify
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public int TransactionId { get; set; }
        public int Amount { get; set; }
        public string OrderNo { get; set; }
        public string CustomerId { get; set; }
        public string BankCode { get; set; }
        public string PaymentDate { get; set; }
        public int PaymentStatus { get; set; }
        public int BankRefCode { get; set; }
        public int MerchantResponseStatus { get; set; }
        public string MerchantResponseMessage { get; set; }
        public string CurrentDate { get; set; }
        public string CurrentTime { get; set; }
        public string PaymentDescription { get; set; }
    }
}