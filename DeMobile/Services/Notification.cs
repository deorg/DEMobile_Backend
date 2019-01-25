using DeMobile.Concrete;
using DeMobile.Models.AppModel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DeMobile.Services
{
    public class Notification
    {
        private User _user;
        private Database oracle;

        public string sendOTP(int cust_no)
        {
            _user = new User();
            var user = _user.getProfileById(cust_no);
            if (user != null)
            {
                oracle = new Database();
                //string cmd = $@"SELECT * FROM SMS020 
                //                //WHERE SMS020_PK = (SELECT MAX(SMS020_PK) FROM SMS020 WHERE CUST_NO = {cust_no})";
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("cust_no", cust_no));
                var reader = oracle.SqlQueryWithParams(SqlCmd.Notification.findLastOTP, parameter);
                reader.Read();
                if (reader.HasRows)
                {
                    int smsPk = reader.GetInt32(0);
                    string status = (string)reader["STATUS"];
                    if (status == "NEW")
                    {
                        //cmd = $@"UPDATE SMS020 SET STATUS = 'EXP' WHERE SMS020_PK = {smsPk}";
                        parameter.Clear();
                        parameter.Add(new OracleParameter("sms020pk", smsPk));
                        var res = oracle.SqlExecuteWithParams(SqlCmd.Notification.updateStatusOTP, parameter);
                    }
                    Security code = new Security();
                    var otp = code.generateOTP();
                    var refCode = code.generateRefCode();
                    var expireTime = DateTime.Now.AddMinutes(5);
                    parameter.Clear();
                    parameter.Add(new OracleParameter("cust_no", cust_no));
                    parameter.Add(new OracleParameter("otp", otp));
                    parameter.Add(new OracleParameter("refCode", refCode));
                    parameter.Add(new OracleParameter("expireTime", expireTime));
                    oracle.SqlExecuteWithParams(SqlCmd.Notification.newOTP, parameter);
                    reader.Dispose();
                    oracle.OracleDisconnect();
                    return refCode;
                }
                else
                {
                    Security code = new Security();
                    var otp = code.generateOTP();
                    var refCode = code.generateRefCode();
                    var expireTime = DateTime.Now.AddMinutes(5);
                    parameter.Clear();
                    parameter.Add(new OracleParameter("cust_no", cust_no));
                    parameter.Add(new OracleParameter("otp", otp));
                    parameter.Add(new OracleParameter("refCode", refCode));
                    parameter.Add(new OracleParameter("expireTime", expireTime));
                    oracle.SqlExecuteWithParams(SqlCmd.Notification.newOTP, parameter);
                    reader.Dispose();
                    oracle.OracleDisconnect();
                    return refCode;
                }
            }
            return null;
        }
        public string confirmOtp(int cust_no, string otp)
        {
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("cust_no", cust_no));
            parameter.Add(new OracleParameter("otp", otp));
            var reader = oracle.SqlQueryWithParams(SqlCmd.Notification.confirmOTP, parameter);
            reader.Read();
            if (reader.HasRows)
            {
                int smsPK = Int32.Parse(reader["SMS020_PK"].ToString());
                int _custNo = Int32.Parse(reader["CUST_NO"].ToString());
                string _otp = (string)reader["OTP"];
                DateTime expireTime = (DateTime)reader["EXPIRE_DT"];
                string status = (string)reader["STATUS"];
                parameter.Clear();
                parameter.Add(new OracleParameter("sms020pk", smsPK));
                oracle.SqlExecuteWithParams(SqlCmd.Notification.setExpireOTP, parameter);
                reader.Dispose();
                oracle.OracleDisconnect();
                if (cust_no != _custNo)
                    return "Invalid customer_no!";
                if (otp != _otp)
                    return "Invalid otp!";
                if (DateTime.Now > expireTime)
                    return "OTP has been expired!";
                if (status == "EXP")
                    return "OTP has been epired!";

                return "SUCCESS";           
            }
            else
            {
                reader.Dispose();
                oracle.OracleDisconnect();
                return "Invalid otp!";
            }
        }
    }
}