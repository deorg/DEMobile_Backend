﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Concrete
{
    public static class SqlCmd
    {
        public static class User
        {
            public const string getSms = "SELECT * FROM SMS010 WHERE CUST_NO = :cust_no ORDER BY SMS010_PK";
            public const string readSms = "UPDATE SMS010 SET READ_STATUS = 'READ' WHERE CUST_NO = :cust_no AND SMS010_PK <= :sms010_pk AND SENDER_TYPE != 'CUST'";
            public const string getContract = "SELECT CON_NO, CUST_NO, TOT_AMT, PAY_AMT, PERIOD, BAL_AMT, CON_DATE, DISC_AMT FROM CONTRACT WHERE CUST_NO = :cust_no ORDER BY CON_DATE";
            public const string findContract = "SELECT * FROM CONTRACT WHERE CUST_NO = :cust_no AND CON_NO = :con_no";
            public const string getContractPayment = "SELECT LNC_NO CON_NO, LNL_PAY_DATE PAY_DATE, LNL_PAY_AMT PAY_AMT FROM VW_LOAN_LEDGER_CO WHERE LNC_NO = :lnc_no ORDER BY LNL_SEQ DESC";
            public const string getProfileByPhone = "SELECT * FROM CUSTOMER WHERE TEL = :tel";
            public const string getProfileByCitizen = "SELECT * FROM CUSTOMER WHERE CITIZEN_NO = :citizen_no";
            public const string getProfileById = "SELECT * FROM CUSTOMER WHERE CUST_NO = :cust_no";
            public const string registerNewDevice = "INSERT INTO MPAY020(DEVICE_ID, CUST_NO, DEVICE_STATUS, TEL) VALUES(:device_id, :cust_no, 'ACT', :tel)";
            public const string registerCurrentDevice = "UPDATE MPAY020 SET CUST_NO = :cust_no, CREATED_TIME = SYSDATE, TEL = :tel WHERE DEVICE_ID = :device_id";
            public const string checkCurrentDevice = "SELECT * FROM MPAY020 WHERE DEVICE_ID = :device_id";
            public const string getDeviceByStatus = "SELECT * FROM MPAY020 WHERE DEVICE_STATUS = :status";
            public const string getDeviceByCustNo = "SELECT * FROM MPAY020 WHERE CUST_NO = :cust_no";
            public const string getConnIdByCustNo = "SELECT * FROM MPAY020 WHERE CUST_NO = :cust_no";
        }
        public static class Payment
        {
            public const string getChannelCode = "SELECT * FROM MPAY010";
            public const string createOrder = "INSERT INTO MPAY100(CUST_NO, CON_NO, DEVICE_ID, CHANNEL_ID, PAY_AMT, TEL, IP_ADDR, DESCRIPTION, TRANS_AMT) VALUES(:custId, :contractNo, :deviceId, :channelCode, :payAmt, :phone, :ip, :description, :transAmt) RETURNING ORDER_NO INTO :order_no";
            public const string setStatusOrder = "UPDATE MPAY100 SET ORDER_STATUS = :status WHERE ORDER_NO = :order_no";
            public const string saveTransaction = "INSERT INTO MPAY110(TRANS_NO, ORDER_NO, CUST_NO, CHANNEL_ID, REQ_STATUS_ID, TRANS_STATUS_ID, PAY_AMT, RETURN_URL, PAYMENT_URL, IP_ADDR, TOKEN, CREATED_TIME, EXPIRE_TIME, TRANS_AMT) VALUES(:transNo, :orderNo, :custNo, :channelId, :reqStatus, :tranStatus, :payAmt, :returnUrl, :paymentUrl, :ip, :token, :createTime, :expireTime, :transAmt) RETURNING TRANS_NO INTO :trans_no";
            public const string updateTransaction = "UPDATE MPAY110 SET TRANS_STATUS_ID = :trans_status_id, BANK_REF_CODE = :bank_ref_code, RESULT_STATUS_ID = :result_status_id, PAYMENT_TIME = :payment_time WHERE TRANS_NO = :trans_no";
            public const string getTransactionByOrder_no = "SELECT * FROM MPAY110 WHERE ORDER_NO = :order_no";
            public const string getContractById = "SELECT * FROM CONTRACT WHERE CON_NO = :con_no";
        }
        public static class Notification
        {
            public const string findLastOtp = "SELECT * FROM SMS020 WHERE SMS020_PK = (SELECT MAX(SMS020_PK) FROM SMS020 WHERE CUST_NO = :cust_no)";
            public const string updateStatusOtp = "UPDATE SMS020 SET STATUS = 'EXP' WHERE SMS020_PK = :sms020pk";
            public const string newOtp = "INSERT INTO SMS020(CUST_NO, TITLE, CONTENT, OTP, EXPIRE_DT, STATUS, REF_CODE) VALUES(:cust_no, 'DEMESTIC', 'รหัส OTP คือ ' || :otp || ' รหัสอ้างอิงคือ ' || :refCode || ' โปรดใช้งานภายใน 5 นาที', :otp, :expireTime, 'NEW', :refCode)";
            public const string confirmOtp = "SELECT * FROM SMS020 WHERE CUST_NO = :cust_no AND OTP = :otp";
            public const string setExpireOtp = "UPDATE SMS020 SET STATUS = 'EXP' WHERE SMS020_PK = :sms020pk";        
            public const string updateConnectId = "UPDATE MPAY020 SET CONN_ID = :conn_id WHERE DEVICE_ID = :device_id";
            public const string createNotification = "INSERT INTO SMS030(TYPE, TITLE, CONTENT, CUST_NO) VALUES(:type, :title, :content, :cust_no)";
            public const string getNotification = "SELECT * FROM SMS030 WHERE CUST_NO = :cust_no";
            public const string getConnectionid = "SELECT CONN_ID FROM MPAY020 WHERE CUST_NO = :cust_no";
            public const string newSms = "INSERT INTO SMS010(CUST_NO, CON_NO, SMS_NOTE, SENDER, SENDER_TYPE) VALUES(:cust_no, :con_no, :sms_note, :sender, :sender_type)";
        }
        public static class Log
        {
            public const string logReq = "INSERT INTO MPAY200(NOTE, CUST_NO, DEVICE_ID, IP_ADDR, URL) VALUES(:note, :cust_no, :device_id, :ip_addr, :url)";
            public const string logRegister = "INSERT INTO MPAY201(CUST_NO, DEVICE_ID, TEL, IP_ADDR) VALUES(:cust_no, :device_id, :tel, :ip_addr)";
            public const string logSignin = "INSERT INTO MPAY202(CUST_NO, DEVICE_ID, TEL, IP_ADDR, STATUS, NOTE) VALUES(:cust_no, :device_id, :tel, :ip_addr, :status, :note)";
            public const string logOrder = "INSERT INTO MPAY203(CUST_NO, CON_NO, ORDER_NO, TRANS_NO, CHANNEL_ID, PAY_AMT, TRANS_AMT, DEVICE_ID, TEL, NOTE, IP_ADDR) VALUES(:cust_no, :con_no, :order_no, :trans_no, :channel_id, :pay_amt, :trans_amt, :device_id, :tel, :note, :ip_addr)";
        }
        public static class Information
        {
            public const string getNumMember = "SELECT COUNT(*) SUM_NEW_USER FROM CUSTOMER";
            public const string getRegisteredMember = "select count(distinct cust_no) from mpay020";
            public const string getSignedInMember = "select count(distinct cust_no) from mpay202";
            public const string getLogRegisteredToday = "SELECT * FROM MPAY201 WHERE TRUNC(CREATED_TIME) = TRUNC(SYSDATE)";
            public const string getLogSigninToday = "SELECT * FROM MPAY202 WHERE TRUNC(CREATED_TIME) = TRUNC(SYSDATE)";
        }
    }
}