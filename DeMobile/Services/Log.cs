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
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("note", log.note));
            parameter.Add(new OracleParameter("cust_no", log.cust_no));
            parameter.Add(new OracleParameter("device_id", log.device_id));
            parameter.Add(new OracleParameter("ip_addr", log.ip_addr));
            parameter.Add(new OracleParameter("url", log.url));
            oracle.SqlExecuteWithParams(SqlCmd.Log.logReq, parameter);
            oracle.OracleDisconnect();
        }
        public void logSignin(m_LogReq log)
        {
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>
            {
                new OracleParameter("cust_no", log.cust_no),
                new OracleParameter("device_id", log.device_id),
                new OracleParameter("tel", log.tel),
                new OracleParameter("serial_sim", log.serial_sim),
                new OracleParameter("ip_addr", log.ip_addr),
                new OracleParameter("action", log.action),
                new OracleParameter("status", log.status),
                new OracleParameter("note", log.note)
            };
            oracle.SqlExecuteWithParams(SqlCmd.Log.logSignin, parameter);
        }
        public void logOrder(m_LogOrder log)
        {
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>
            {
                new OracleParameter("cust_no", log.cust_no),
                new OracleParameter("con_no", log.con_no),
                new OracleParameter("order_no", log.order_no),
                new OracleParameter("trans_no", log.trans_no),
                new OracleParameter("channel_id", log.channel_id),
                new OracleParameter("pay_amt", log.pay_amt),
                new OracleParameter("trans_amt", log.trans_amt),
                new OracleParameter("device_id", log.device_id),
                new OracleParameter("tel", log.tel),
                new OracleParameter("note", log.note),
                new OracleParameter("ip_addr", log.ip_addr)
            };
            oracle.SqlExecuteWithParams(SqlCmd.Log.logOrder, parameter);
            oracle.OracleDisconnect();
        }
    }
}