using DeMobile.Concrete;
using DeMobile.Models.AppModel;
using DeMobile.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Services
{
    public class Information
    {
        private Database oracle;


        public List<m_StatusCode> getStatusCode()
        {
            oracle = new Database();
            List<m_StatusCode> data = new List<m_StatusCode>();
            var reader = oracle.SqlQuery(SqlCmd.Information.getStatusCode);
            while (reader.Read())
            {
                data.Add(new m_StatusCode
                {
                    status_code = Int32.Parse(reader["SERVER_STATUS_NO"].ToString()),
                    status_name = (string)reader["STATUS_NAME"],
                    created_time = (DateTime)reader["CREATED_TIME"]
                });
            }
            if (data.Count == 0)
            {
                reader.Dispose();
                oracle.OracleDisconnect();
                return null;
            }
            reader.Dispose();
            oracle.OracleDisconnect();
            return data;
        }
        public List<m_LogReg> getLogRegistered()
        {
            oracle = new Database();
            List<m_LogReg> data = new List<m_LogReg>();
            var reader = oracle.SqlQuery(SqlCmd.Information.getLogRegistered);
            while (reader.Read())
            {
                data.Add(new m_LogReg
                {
                    log_reg_no = Int32.Parse(reader["LOG_REG_NO"].ToString()),
                    cust_no = Int32.Parse(reader["CUST_NO"].ToString()),
                    device_id = (string)reader["DEVICE_ID"],
                    tel = (string)reader["TEL"],
                    ip_addr = (string)reader["IP_ADDR"],
                    created_time = (DateTime)reader["CREATED_TIME"]
                });
            }
            if (data.Count == 0)
            {
                reader.Dispose();
                oracle.OracleDisconnect();
                return null;
            }
            reader.Dispose();
            oracle.OracleDisconnect();
            return data;
        }
        public m_Member getDashBoard()
        {
            oracle = new Database();
            m_Member data = new m_Member();
            var reader = oracle.SqlQuery(SqlCmd.Information.getNumMember);
            reader.Read();
            if (reader.HasRows)
                data.member = reader.GetInt32(0);
            else
                data.member = 0;
            reader.Dispose();
            reader = oracle.SqlQuery(SqlCmd.Information.getRegisteredMember);
            reader.Read();
            if (reader.HasRows)
                data.registeredMember = reader.GetInt32(0);
            else
                data.registeredMember = 0;
            reader.Dispose();
            reader = oracle.SqlQuery(SqlCmd.Information.getSignedInMember);
            reader.Read();
            if (reader.HasRows)
                data.signedInMember = reader.GetInt32(0);
            else
                data.signedInMember = 0;

            reader.Dispose();
            oracle.OracleDisconnect();
            return data;
            //oracle = new Database();
            //List<m_SMS010> data = new List<m_SMS010>();
            //List<OracleParameter> parameter = new List<OracleParameter>();
            //parameter.Add(new OracleParameter("cust_no", id));
            ////var reader = oracle.SqlQuery(cmd);
            //var reader = oracle.SqlQueryWithParams(SqlCmd.User.getSms, parameter);
            //while (reader.Read())
            //{
            //    var test = reader["CON_NO"];
            //    data.Add(new m_SMS010
            //    {
            //        SMS010_PK = Int32.Parse(reader["SMS010_PK"].ToString()),
            //        CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
            //        CON_NO = reader["CON_NO"] == DBNull.Value ? string.Empty : (string)reader["CON_NO"],
            //        SMS_NOTE = (string)reader["SMS_NOTE"],
            //        SMS_TIME = (DateTime)reader["SMS_TIME"],
            //        SENDER = reader["SENDER"] == DBNull.Value ? null : (int?)Int32.Parse(reader["SENDER"].ToString()),
            //        SENDER_TYPE = (string)reader["SENDER_TYPE"],
            //        SMS010_REF = reader["SMS010_REF"] == DBNull.Value ? null : (int?)Int32.Parse(reader["SMS010_REF"].ToString()),
            //        READ_STATUS = (string)reader["READ_STATUS"]
            //    });
            //}
            //if (data.Count == 0)
            //{
            //    reader.Dispose();
            //    oracle.OracleDisconnect();
            //    return null;
            //}
            //reader.Dispose();
            //oracle.OracleDisconnect();
            //return data;
        }
    }
}