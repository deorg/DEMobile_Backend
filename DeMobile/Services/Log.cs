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
    }
}