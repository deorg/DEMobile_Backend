using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Concrete
{
    public static class SqlCmd
    {
        public static class User
        {
            public const string getSms = "SELECT SMS010_PK, CON_NO, SMS_NOTE, SMS_TIME FROM SMS010 WHERE CUST_NO = :cust_no";
            public const string getContract = "SELECT CON_NO, CUST_NO, TOT_AMT, PAY_AMT, PERIOD, BAL_AMT, CON_DATE, DISC_AMT FROM CONTRACT WHERE CUST_NO = :cust_no ORDER BY CON_DATE";
            public const string getContractPayment = "SELECT LNC_NO CON_NO, LNL_PAY_DATE PAY_DATE, LNL_PAY_AMT PAY_AMT FROM VW_LOAN_LEDGER_CO WHERE LNC_NO = :lnc_no ORDER BY LNL_SEQ DESC";
            public const string getProfileByPhone = "SELECT CUST_NO, CUST_NAME, CITIZEN_NO, TEL FROM CUSTOMER WHERE TEL = :tel";
            public const string getProfileByCitizen = "SELECT CUST_NO, CUST_NAME, CITIZEN_NO, TEL FROM CUSTOMER WHERE CITIZEN_NO = :citizen_no";
            public const string getProfileById = "SELECT CUST_NO, CUST_NAME, CITIZEN_NO, TEL FROM CUSTOMER WHERE CUST_NO = :cust_no";
            public const string registerNewDevice = "INSERT INTO MPAY020(DEVICE_ID, CUST_NO, DEVICE_STATUS) VALUES(:device_id, :cust_no, 'ACT')";
            public const string checkCurrentDevice = "SELECT * FROM MPAY020 WHERE DEVICE_ID = :device_id";
        }
        public static class Payment
        {
            public const string getChannelCode = "SELECT * FROM MPAY010";
            public const string createOrder = "INSERT INTO MPAY100(CUST_NO, CON_NO, DEVICE_ID, CHANNEL_ID, PAY_AMT, TEL, IP_ADDR, DESCRIPTION) VALUES(:custId, :contractNo, :deviceId, :channelCode, :amount, :phone, :ip, :description) RETURNING ORDER_NO INTO :order_no";
            public const string setStatusOrder = "UPDATE MPAY100 SET ORDER_STATUS = :status WHERE ORDER_NO = :order_no";
            public const string saveTransaction = "INSERT INTO MPAY110(TRANS_NO, ORDER_NO, CUST_NO, CHANNEL_ID, REQ_STATUS_ID, TRANS_STATUS_ID, PAY_AMT, RETURN_URL, PAYMENT_URL, IP_ADDR, TOKEN, CREATED_TIME, EXPIRE_TIME) VALUES(:transNo, :orderNo, :custNo, :channelId, :reqStatus, :tranStatus, :amount, :returnUrl, :paymentUrl, :ip, :token, :createTime, :expireTime) RETURNING TRANS_NO INTO :trans_no";
        }
        public static class Notification
        {
            public const string findLastOTP = "SELECT * FROM SMS020 WHERE SMS020_PK = (SELECT MAX(SMS020_PK) FROM SMS020 WHERE CUST_NO = :cust_no)";
            public const string updateStatusOTP = "UPDATE SMS020 SET STATUS = 'EXP' WHERE SMS020_PK = :sms020pk";
            public const string newOTP = "INSERT INTO SMS020(CUST_NO, TITLE, CONTENT, OTP, EXPIRE_DT, STATUS, REF_CODE) VALUES(:cust_no, 'DEMESTIC', 'รหัส OTP คือ ' || :otp || ' รหัสอ้างอิงคือ ' || :refCode || ' โปรดใช้งานภายใน 5 นาที', :otp, :expireTime, 'NEW', :refCode)";
            public const string confirmOTP = "SELECT * FROM SMS020 WHERE CUST_NO = :cust_no AND OTP = :otp";
            public const string setExpireOTP = "UPDATE SMS020 SET STATUS = 'EXP' WHERE SMS020_PK = :sms020pk";
        }
    }
}