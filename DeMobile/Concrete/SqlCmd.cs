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
            public const string getSms = "SELECT * FROM SMS010_M WHERE CUST_NO IN(SELECT CUST_NO FROM MPAY021 WHERE DEVICE_ID = fcust2device_id(:cust_no)) ORDER BY SMS010_PK";
            public const string getLastSms = "SELECT * FROM SMS010_M WHERE SMS010_PK = (SELECT MAX(SMS010_PK) FROM SMS010_M WHERE CUST_NO = :cust_no) ORDER BY SMS010_PK DESC";
            public const string getSmsWithConnId = @"select sms010_pk, s010.cust_no, conn_id, device_status, sms_note,  con_no, sms_time, sender, sender_type, sms010_ref, read_status
                                                        from sms010_m s010 join MPAY020 m020
                                                        on s010.cust_no = m020.cust_no
                                                        where s010.cust_no = :cust_no";
            public const string getAllSmsWithConnId = @"select sms010_pk, s010.cust_no, conn_id, device_status, sms_note,  s010.con_no, sms_time, sender, sender_type, sms010_ref, read_status, received
                                                        from sms010_m s010 join MPAY020 m020
                                                        on s010.cust_no = m020.cust_no where received = 'N'";
            public const string readSms = "UPDATE SMS010_M SET READ_STATUS = 'READ', RECEIVED = 'Y' WHERE CUST_NO = :cust_no AND SMS010_PK <= :sms010_pk AND SENDER_TYPE != 'CUST'";
            public const string getContract = "SELECT * FROM CONTRACT_M WHERE CUST_NO = :cust_no ORDER BY CON_DATE";
            public const string findContract = "SELECT * FROM CONTRACT_M WHERE CUST_NO = :cust_no AND CON_NO = :con_no";
            public const string getContractPayment = "SELECT LNC_NO CON_NO, LNL_PAY_DATE PAY_DATE, LNL_PAY_AMT PAY_AMT FROM VW_LOAN_LEDGER_CO WHERE LNC_NO = :lnc_no ORDER BY LNL_SEQ DESC";
            public const string getProfileByPhone = "SELECT * FROM CUSTOMER_M WHERE TEL = :tel";
            public const string getProfileBySerialSim = "SELECT * FROM CUSTOMER_M WHERE CUST_NO = (SELECT CUST_NO FROM MPAY020 WHERE SERIAL_SIM = :serial_sim AND ROWNUM = 1)";
            public const string getProfileByDeviceId = "SELECT * FROM CUSTOMER_M WHERE CUST_NO = (SELECT CUST_NO FROM MPAY020 WHERE DEVICE_ID = :device_id)";
            public const string getProfileByCitizen = "SELECT * FROM CUSTOMER_M WHERE CITIZEN_NO = :citizen_no";
            public const string getProfileById = "SELECT * FROM CUSTOMER_M WHERE CUST_NO = :cust_no";
            public const string getCustNoByPhone = "SELECT  FGETCUST_NO(:phone) FROM DUAL";
            //public const string registerNewDevice = @"INSERT INTO MPAY020(DEVICE_ID, CUST_NO, DEVICE_STATUS, TEL, TEL_SIM, SERIAL_SIM, OPERATOR, BRAND, MODEL, API_VERSION, PIN, APP_VERSION, PLATFORM) " +
            //"VALUES(:device_id, :cust_no, 'ACT', :tel, :telSim, :serial_sim, :operator, :brand, :model, :api_version, :pin, :app_version, :platform)";
            public const string registerCurrentDevice = "UPDATE MPAY020 SET CUST_NO = :cust_no, DEVICE_STATUS = 'ACT', TEL = :tel, SERIAL_SIM = :serial_sim, API_VERSION = :api_version, APP_VERSION = :app_version, CREATED_TIME = SYSDATE, LAST_IDENT = SYSDATE WHERE DEVICE_ID = :device_id";
            public const string registerDevice = @"INSERT INTO MPAY020(DEVICE_ID, CUST_NO, DEVICE_STATUS, TEL, SERIAL_SIM, BRAND, MODEL, API_VERSION,PIN, APP_VERSION, PLATFORM) VALUES(:device_id, :cust_no, 'ACT', :tel, :serial_sim, :brand, :model, :api_version, :pin, :app_version, :platform)";
            public const string registerNewDevice = "UPDATE MPAY020 SET DEVICE_ID = :device_id, DEVICE_STATUS = 'ACT', TEL = :tel, SERIAL_SIM = :serial_sim, BRAND = :brand, MODEL = :model, API_VERSION = :api_version, APP_VERSION = :app_version, PLATFORM = :platform, CREATED_TIME = SYSDATE, LAST_IDENT = SYSDATE WHERE CUST_NO = :cust_no";
            public const string checkCurrentDevice = "SELECT * FROM MPAY020 WHERE DEVICE_ID = :device_id";
            public const string getDeviceByStatus = "SELECT * FROM MPAY020 WHERE DEVICE_STATUS = :status";
            public const string getDeviceByCustNo = "SELECT * FROM MPAY020 WHERE CUST_NO = :cust_no";
            public const string getConnIdByCustNo = "SELECT * FROM MPAY020 WHERE CUST_NO = :cust_no AND DEVICE_STATUS = 'ACT'";
            public const string updateAppVersion = "UPDATE MPAY020 SET APP_VERSION = :app_version WHERE DEVICE_ID = :device_id";
            public const string updateMPAY021 = "UPDATE MPAY021 SET DEVICE_ID = :device_id where CUST_NO = :cust_no";
            public const string logout = "UPDATE MPAY020 SET DEVICE_STATUS = 'SUS' WHERE CUST_NO = :cust_no";
            public const string updateIdentify = "UPDATE MPAY020 SET LAST_IDENT = SYSDATE WHERE DEVICE_ID = :device_id";
            public const string updateReadSMS = "UPDATE MPAY020 SET LAST_RETRIEVE = SYSDATE WHERE CUST_NO = :cust_no";
            public const string getAppVersion = "SELECT APP_VERSION FROM MPAY020 WHERE CUST_NO = :cust_no";
        }
        public static class Payment
        {
            public const string getChannelCode = "SELECT * FROM MPAY010";
            public const string createOrder = "INSERT INTO MPAY100(CUST_NO, CON_NO, CHANNEL_ID, PAY_AMT, TEL, IP_ADDR, DESCRIPTION, TRANS_AMT) VALUES(:custId, :contractNo, :channelCode, :payAmt, :phone, :ip, :description, :transAmt) RETURNING ORDER_NO INTO :order_no";
            public const string setStatusOrder = "UPDATE MPAY100 SET ORDER_STATUS = :order_status WHERE ORDER_NO = :order_no";
            public const string saveTransaction = "INSERT INTO MPAY110(TRANS_NO, ORDER_NO, CUST_NO, CHANNEL_ID, REQ_STATUS_ID, TRANS_STATUS_ID, PAY_AMT, RETURN_URL, PAYMENT_URL, IP_ADDR, TOKEN, CREATED_TIME, EXPIRE_TIME, TRANS_AMT) VALUES(:transNo, :orderNo, :custNo, :channelId, :reqStatus, :tranStatus, :payAmt, :returnUrl, :paymentUrl, :ip, :token, :createTime, :expireTime, :transAmt) RETURNING TRANS_NO INTO :trans_no";
            public const string updateTransaction = "UPDATE MPAY110 SET TRANS_STATUS_ID = :trans_status_id, BANK_REF_CODE = :bank_ref_code, RESULT_STATUS_ID = :result_status_id, PAYMENT_TIME = :payment_time WHERE TRANS_NO = :trans_no";
            public const string getTransactionByOrder_no = "SELECT * FROM MPAY110 WHERE ORDER_NO = :order_no";
            public const string getContractById = "SELECT * FROM CONTRACT_M WHERE CON_NO = :con_no";           
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
            public const string getCustNo = "SELECT * FROM MPAY020 WHERE CONN_ID = :conn_id";
            public const string getConnectionid = "SELECT CONN_ID FROM MPAY020 WHERE CUST_NO = :cust_no";
            public const string newSms = "INSERT INTO SMS010_M(CUST_NO, CON_NO, SMS_NOTE, SENDER, SENDER_TYPE) VALUES(:cust_no, :con_no, :sms_note, :sender, :sender_type) RETURNING SMS010_PK INTO :sms010_pk";
            public const string markToRecieve = "UPDATE SMS010_M SET RECEIVED = 'Y' WHERE SMS010_PK = :sms010pk";
            public const string markToSent = "UPDATE SMS010_M SET RECEIVED = 'S' WHERE RECEIVED = 'N'";
            public const string markToSentById = "UPDATE SMS010_M SET RECEIVED = 'S' WHERE SMS010_PK = :sms010_pk";
        }
        public static class Log
        {
            public const string logReq = "INSERT INTO MPAY200(NOTE, CUST_NO, DEVICE_ID, IP_ADDR, URL) VALUES(:note, :cust_no, :device_id, :ip_addr, :url)";
            public const string logRegister = "INSERT INTO MPAY202(CUST_NO, DEVICE_ID, TEL, IP_ADDR, STATUS, NOTE, ACTION, SERIAL_SIM, BRAND, MODEL, API_VERSION) VALUES(:cust_no, :device_id, :tel, :ip_addr, :status, :note, :action, :serial_sim, :brand, :model, :api_version)";
            public const string logSignin = "INSERT INTO MPAY202(CUST_NO, DEVICE_ID, TEL, SERIAL_SIM, IP_ADDR,ACTION, STATUS, NOTE, BRAND, MODEL, APP_VERSION, API_VERSION) VALUES(:cust_no, :device_id, :tel, :serial_sim, :ip_addr,:action, :status, :note, :brand, :model, :app_version, :api_version)";
            public const string logActivity = "INSERT INTO MPAY202(CUST_NO, DEVICE_ID, TEL, SERIAL_SIM, IP_ADDR,ACTION, STATUS, NOTE, BRAND, MODEL, APP_VERSION, API_VERSION) VALUES(:cust_no, :device_id, :tel, :serial_sim, :ip_addr,:action, :status, :note, :brand, :model, :app_version, :api_version)";
            public const string logOrder = "INSERT INTO MPAY203(CUST_NO, CON_NO, ORDER_NO, TRANS_NO, CHANNEL_ID, PAY_AMT, TRANS_AMT, DEVICE_ID, TEL, NOTE, IP_ADDR) VALUES(:cust_no, :con_no, :order_no, :trans_no, :channel_id, :pay_amt, :trans_amt, :device_id, :tel, :note, :ip_addr)";
            public const string logTest = "INSERT INTO LOG_TEST(TXT) VALUES(:msg)";
            public const string logReadSms = "INSERT INTO MPAY202(CUST_NO, DEVICE_ID, TEL, IP_ADDR, STATUS, NOTE, ACTION, SERIAL_SIM) VALUES(:cust_no, :device_id, :tel, :ip_addr, :status, :note, :action, :serial_sim)";
        }
        public static class Information
        {
            //public const string getAppVersion = "SELECT APP_VERSION FROM MPAY999";
            public const string getAndroidVersion = "SELECT ANDROID_VERSION FROM MPAY999";
            public const string getIosVersion = "SELECT IOS_VERSION FROM MPAY999";
            public const string getStatusCode = "SELECT * FROM MPAY060";
            public const string getNumMember = "SELECT COUNT(*) SUM_NEW_USER FROM CUSTOMER_M";
            public const string getRegisteredMember = "select count(distinct cust_no) from MPAY020";
            public const string getSignedInMember = "select count(distinct cust_no) from mpay202";
            public const string getLogRegistered = "SELECT * FROM MPAY202 ORDER BY WHERE ACTION IN('REGISTER', 'REGISTER NEW DEVICE', 'REGISTER CURRENT DEVICE') ORDER BY MPAY202_SEQ";
            //public const string getLogRegisteredToday = "SELECT * FROM MPAY201 WHERE TRUNC(CREATED_TIME) = TRUNC(SYSDATE)";
            public const string getLogSigninToday = "SELECT * FROM MPAY202 WHERE TRUNC(CREATED_TIME) = TRUNC(SYSDATE)";
            public const string getRegisterType = "SELECT REGISTER_TYPE FROM MPAY999";
            public const string getChatOn = "SELECT CHAT FROM MPAY999";
            public const string getBroadcast = "SELECT NOTE, START_TIME, END_TIME, CREATED_TIME FROM MPAY070 WHERE SYSDATE BETWEEN START_TIME AND END_TIME";
        }
        public static class Line
        {

            public const string getProcessByUserId = "SELECT * FROM MPAY300 WHERE LINE_USER_ID = :line_user_id ORDER BY MPAY300_SEQ";
            public const string getProfileByUserId = "SELECT * FROM CUSTOMER_M WHERE LINE_USER_ID = :line_user_id";
            public const string registerUserId = "UPDATE CUSTOMER_M SET LINE_USER_ID = :line_user_id where CUST_NO = :cust_no";
            public const string setRegister = @"INSERT INTO MPAY300(LINE_USER_ID, PROCESS, PROCESS_STATUS, ACTION, ACTION_STATUS, NOTE)
                                                    VALUES(:line_user_id, :process, :process_status, :action, :action_status, :note)";
            public const string getSmsWithUserId = @"select sms010_pk, s010.cust_no, line_user_id, sms_note,  s010.con_no, sms_time, sender, sender_type, sms010_ref, read_status, received
                                                        from sms010_m s010 join customer_m cust
                                                        on s010.cust_no = cust.cust_no where cust.line_user_id is not null and sender_type = 'SYSTEM' and received = 'N'";
            public const string getSmsByIdWithUserId = @"select sms010_pk, s010.cust_no, line_user_id, sms_note,  s010.con_no, sms_time, sender, sender_type, sms010_ref, read_status, received
                                                        from sms010_m s010 join customer_m cust
                                                        on s010.cust_no = cust.cust_no where cust.line_user_id is not null and sender_type = 'SYSTEM' and received = 'N'
                                                        and sms010_pk = :sms010_pk";
        }

    }
}