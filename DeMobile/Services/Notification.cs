using DeMobile.Concrete;
using DeMobile.Models.AppModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Services
{
    public class Notification
    {
        private User _user;
        private Database oracle;

        public void sendOTP(int cust_no)
        {
            _user = new User();
            var user = _user.getProfileById(cust_no);
            if (user != null)
            {
                oracle = new Database();
                string cmd = $@"SELECT * FROM SMS020 
                                WHERE SMS020_PK = (SELECT MAX(SMS020_PK) FROM SMS020 WHERE CUST_NO = {cust_no})";
                var reader = oracle.SqlQuery(cmd);
                reader.Read();
                if (reader.HasRows)
                {
                    int smsPk = reader.GetInt32(0);
                    string status = (string)reader["STATUS"];
                    if(status == "NEW")
                    {
                        cmd = $@"UPDATE SMS020 SET STATUS = 'EXP' WHERE SMS020_PK = {smsPk}";
                        var res = oracle.SqlExcute(cmd);
                    }
                    Security code = new Security();
                    var otp = code.generateOTP();
                    var refCode = code.generateRefCode();
                    //var data = new SMS020
                    //{
                    //    SMS020_PK = Int32.Parse(reader["SMS020_PK"].ToString()),
                    //    CUST_NO = (string)reader["CUST_NO"],
                    //    TITLE = reader["TITLE"] == DBNull.Value ? string.Empty : (string)reader["TITLE"],
                    //    CONTENT = reader["CONTENT"] == DBNull.Value ? string.Empty : (string)reader["CONTENT"],
                    //    OTP = reader["OTP"] == DBNull.Value ? string.Empty : (string)reader["OTP"],
                    //    SEND_DT = reader["SEND_DT"] == DBNull.Value ? DateTime.Now : (DateTime)reader["SEND_DT"],
                    //    EXPIRE_DT = reader["EXPIRE_DT"] == DBNull.Value ? DateTime.Now : (DateTime)reader["EXPIRE_DT"],
                    //    STATUS = reader["STATUS"] == DBNull.Value ? string.Empty : (string)reader["STATUS"]
                    //};
                    reader.Dispose();
                    oracle.OracleDisconnect();
                }
            }
        }
    }
}