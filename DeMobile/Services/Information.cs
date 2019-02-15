using DeMobile.Concrete;
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