using DeMobile.Concrete;
using DeMobile.Models.AppModel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Services
{
    public class Log
    {
        private Database oracle;
        public void logRequest(m_LogReq log)
        {
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.Log.logReq, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add(new OracleParameter("note", log.note));
                        cmd.Parameters.Add(new OracleParameter("cust_no", log.cust_no));
                        cmd.Parameters.Add(new OracleParameter("device_id", log.device_id));
                        cmd.Parameters.Add(new OracleParameter("ip_addr", log.ip_addr));
                        cmd.Parameters.Add(new OracleParameter("url", log.url));
                        cmd.ExecuteNonQueryAsync();
                        cmd.Dispose();
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            //oracle = new Database();
            //List<OracleParameter> parameter = new List<OracleParameter>();
            //parameter.Add(new OracleParameter("note", log.note));
            //parameter.Add(new OracleParameter("cust_no", log.cust_no));
            //parameter.Add(new OracleParameter("device_id", log.device_id));
            //parameter.Add(new OracleParameter("ip_addr", log.ip_addr));
            //parameter.Add(new OracleParameter("url", log.url));
            //oracle.SqlExecuteWithParams(SqlCmd.Log.logReq, parameter);
            //oracle.OracleDisconnect();
        }
        public void logSignin(m_LogReq log)
        {
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.Log.logSignin, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add(new OracleParameter("cust_no", log.cust_no));
                        cmd.Parameters.Add(new OracleParameter("device_id", log.device_id));
                        cmd.Parameters.Add(new OracleParameter("tel", log.tel));
                        cmd.Parameters.Add(new OracleParameter("serial_sim", log.serial_sim));
                        cmd.Parameters.Add(new OracleParameter("ip_addr", log.ip_addr));
                        cmd.Parameters.Add(new OracleParameter("action", log.action));
                        cmd.Parameters.Add(new OracleParameter("status", log.status));
                        cmd.Parameters.Add(new OracleParameter("note", log.note));

                        cmd.ExecuteNonQueryAsync();
                        cmd.Dispose();
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            //oracle = new Database();
            //List<OracleParameter> parameter = new List<OracleParameter>
            //{
            //    new OracleParameter("cust_no", log.cust_no),
            //    new OracleParameter("device_id", log.device_id),
            //    new OracleParameter("tel", log.tel),
            //    new OracleParameter("serial_sim", log.serial_sim),
            //    new OracleParameter("ip_addr", log.ip_addr),
            //    new OracleParameter("action", log.action),
            //    new OracleParameter("status", log.status),
            //    new OracleParameter("note", log.note)
            //};
            //oracle.SqlExecuteWithParams(SqlCmd.Log.logSignin, parameter);
        }
        public void logOrder(m_LogOrder log)
        {
            using(var conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using(var cmd = new OracleCommand(SqlCmd.Log.logOrder, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add("cust_no", log.cust_no);
                        cmd.Parameters.Add("con_no", log.con_no);
                        cmd.Parameters.Add("order_no", log.order_no);
                        cmd.Parameters.Add("trans_no", log.trans_no);
                        cmd.Parameters.Add("channel_id", log.channel_id);
                        cmd.Parameters.Add("pay_amt", log.pay_amt);
                        cmd.Parameters.Add("trans_amt", log.trans_amt);
                        cmd.Parameters.Add("device_id", log.device_id);
                        cmd.Parameters.Add("tel", log.tel);
                        cmd.Parameters.Add("note", log.note);
                        cmd.Parameters.Add("ip_addr", log.ip_addr);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            //oracle = new Database();
            //List<OracleParameter> parameter = new List<OracleParameter>
            //{
            //    new OracleParameter("cust_no", log.cust_no),
            //    new OracleParameter("con_no", log.con_no),
            //    new OracleParameter("order_no", log.order_no),
            //    new OracleParameter("trans_no", log.trans_no),
            //    new OracleParameter("channel_id", log.channel_id),
            //    new OracleParameter("pay_amt", log.pay_amt),
            //    new OracleParameter("trans_amt", log.trans_amt),
            //    new OracleParameter("device_id", log.device_id),
            //    new OracleParameter("tel", log.tel),
            //    new OracleParameter("note", log.note),
            //    new OracleParameter("ip_addr", log.ip_addr)
            //};
            //oracle.SqlExecuteWithParams(SqlCmd.Log.logOrder, parameter);
            //oracle.OracleDisconnect();
        }
    }
}