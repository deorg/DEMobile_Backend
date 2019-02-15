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
    public class User
    {
        private Database oracle;
        public List<m_SMS010> getNotification(int id)
        {
            oracle = new Database();
            List<m_SMS010> data = new List<m_SMS010>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("cust_no", id));
            //var reader = oracle.SqlQuery(cmd);
            var reader = oracle.SqlQueryWithParams(SqlCmd.User.getSms, parameter);
            while (reader.Read())
            {
                var test = reader["CON_NO"];
                data.Add(new m_SMS010
                {
                    SMS010_PK = Int32.Parse(reader["SMS010_PK"].ToString()),
                    CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                    CON_NO = reader["CON_NO"] == DBNull.Value ? string.Empty : (string)reader["CON_NO"],
                    SMS_NOTE = (string)reader["SMS_NOTE"],
                    SMS_TIME = (DateTime)reader["SMS_TIME"],
                    SENDER = reader["SENDER"] == DBNull.Value ? null : (int?)Int32.Parse(reader["SENDER"].ToString()),
                    SENDER_TYPE = (string)reader["SENDER_TYPE"],
                    SMS010_REF = reader["SMS010_REF"] == DBNull.Value ? null : (int?)Int32.Parse(reader["SMS010_REF"].ToString()),
                    READ_STATUS = (string)reader["READ_STATUS"]
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
        public void readSms(m_CustReadMsg value)
        {
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>
            {
                new OracleParameter("cust_no", value.cust_no),
                new OracleParameter("sms010_pk", value.sms010_pk)
            };
            oracle.SqlExecuteWithParams(SqlCmd.User.readSms, parameter);
            oracle.OracleDisconnect();
        }
        public List<m_ConPayment> getPayment(string no)
        {
            oracle = new Database();
            List<m_ConPayment> data = new List<m_ConPayment>();
            string cmd = $@"SELECT LNC_NO CON_NO, LNL_PAY_DATE PAY_DATE, LNL_PAY_AMT PAY_AMT
                            FROM   VW_LOAN_LEDGER_CO
                            WHERE  LNC_NO = '{no}'
                            ORDER BY LNL_SEQ DESC";
            var reader = oracle.SqlQuery(cmd);
            while (reader.Read())
            {
                data.Add(new m_ConPayment
                {
                    CON_NO = (string)reader["CON_NO"],
                    PAY_DATE = (DateTime)reader["PAY_DATE"],
                    PAY_AMT = Int32.Parse(reader["PAY_AMT"].ToString())
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
        public m_Contract findContract(int cust_no, string con_no)
        {
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("cust_no", cust_no));
            parameter.Add(new OracleParameter("con_no", con_no));
            var reader = oracle.SqlQueryWithParams(SqlCmd.User.findContract, parameter);
            reader.Read();
            if (reader.HasRows)
            {
                var data = new m_Contract
                {
                    CON_NO = (string)reader["CON_NO"],
                    CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                    TOT_AMT = double.Parse(reader["TOT_AMT"].ToString()),
                    PAY_AMT = double.Parse(reader["PAY_AMT"].ToString()),
                    PERIOD = Int32.Parse(reader["PERIOD"].ToString()),
                    BAL_AMT = double.Parse(reader["BAL_AMT"].ToString()),
                    CON_DATE = (DateTime)reader["CON_DATE"],
                    DISC_AMT = double.Parse(reader["DISC_AMT"].ToString())
                };
                reader.Dispose();
                oracle.OracleDisconnect();
                return data;
            }
            else
            {
                reader.Dispose();
                oracle.OracleDisconnect();
                return null;
            }
        }
        public List<m_Contract> getContract(int id)
        {
            oracle = new Database();
            List<m_Contract> data = new List<m_Contract>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("cust_no", id));
            var reader = oracle.SqlQueryWithParams(SqlCmd.User.getContract, parameter);
            //var reader = oracle.SqlQuery(cmd);
            while (reader.Read())
            {
                data.Add(new m_Contract
                {
                    CON_NO = (string)reader["CON_NO"],
                    CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                    TOT_AMT = Int32.Parse(reader["TOT_AMT"].ToString()),
                    PAY_AMT = Int32.Parse(reader["PAY_AMT"].ToString()),
                    PERIOD = Int32.Parse(reader["PERIOD"].ToString()),
                    BAL_AMT = Int32.Parse(reader["BAL_AMT"].ToString()),
                    CON_DATE = (DateTime)reader["CON_DATE"],
                    DISC_AMT = Int32.Parse(reader["DISC_AMT"].ToString())
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
        public m_Customer getProfileByPhoneNO(string phone)
        {
            oracle = new Database();
            //string cmd = $@"SELECT CUST_NO, CUST_NAME, CITIZEN_NO, TEL
            //                FROM CUSTOMER 
            //                WHERE TEL = '{phone}'";
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("tel", phone));
            var reader = oracle.SqlQueryWithParams(SqlCmd.User.getProfileByPhone, parameter);
            //var reader = oracle.SqlQuery(cmd);
            reader.Read();
            if (reader.HasRows)
            {
                var data = new m_Customer
                {
                    CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                    CUST_NAME = (string)reader["CUST_NAME"],
                    CITIZEN_NO = reader["CITIZEN_NO"] == DBNull.Value ? string.Empty : (string)reader["CITIZEN_NO"],
                    TEL = reader["TEL"] == DBNull.Value ? string.Empty : (string)reader["TEL"],
                    PERMIT = (string)reader["PERMIT"]
                };
                reader.Dispose();
                oracle.OracleDisconnect();
                return data;
            }
            else
            {
                reader.Dispose();
                oracle.OracleDisconnect();
                return null;
            }
        }
        public m_Customer getProfileByCitizenNo(string citizen)
        {
            oracle = new Database();
            //string cmd = $@"SELECT CUST_NO, CUST_NAME, CITIZEN_NO, TEL
            //                FROM CUSTOMER 
            //                WHERE CITIZEN_NO = '{citizen}'";
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("citizen_no", citizen));
            var reader = oracle.SqlQueryWithParams(SqlCmd.User.getProfileByCitizen, parameter);
            //var reader = oracle.SqlQuery(cmd);
            reader.Read();
            if (reader.HasRows)
            {
                var data = new m_Customer
                {
                    CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                    CUST_NAME = (string)reader["CUST_NAME"],
                    CITIZEN_NO = reader["CITIZEN_NO"] == DBNull.Value ? string.Empty : (string)reader["CITIZEN_NO"],
                    TEL = reader["TEL"] == DBNull.Value ? string.Empty : (string)reader["TEL"],
                    PERMIT = (string)reader["PERMIT"]
                };
                reader.Dispose();
                oracle.OracleDisconnect();
                return data;
            }
            else
            {
                reader.Dispose();
                oracle.OracleDisconnect();
                return null;
            }
        }
        public m_device checkCurrentDevice(string id)
        {
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("device_id", id));
            var reader = oracle.SqlQueryWithParams(SqlCmd.User.checkCurrentDevice, parameter);
            //var reader = oracle.SqlQuery(cmd);
            reader.Read();
            if (reader.HasRows)
            {
                var data = new m_device
                {
                    device_id = (string)reader["DEVICE_ID"],
                    cust_no = Int32.Parse(reader["CUST_NO"].ToString()),
                    conn_id = reader["CONN_ID"] == DBNull.Value ? string.Empty : (string)reader["CONN_ID"],
                    device_status = (string)reader["DEVICE_STATUS"],
                    created_time = (DateTime)reader["CREATED_TIME"]
                };
                reader.Dispose();
                oracle.OracleDisconnect();
                return data;
            }
            else
            {
                reader.Dispose();
                oracle.OracleDisconnect();
                return null;
            }
        }
        public m_device getDeviceByCustNo(int cust_no)
        {
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("cust_no", cust_no));
            var reader = oracle.SqlQueryWithParams(SqlCmd.User.getDeviceByCustNo, parameter);
            reader.Read();
            if (reader.HasRows)
            {
                var data = new m_device
                {
                    device_id = (string)reader["DEVICE_ID"],
                    cust_no = Int32.Parse(reader["CUST_NO"].ToString()),
                    conn_id = reader["CONN_ID"] == DBNull.Value ? string.Empty : (string)reader["CONN_ID"],
                    device_status = (string)reader["DEVICE_STATUS"],
                    created_time = (DateTime)reader["CREATED_TIME"]
                };
                reader.Dispose();
                oracle.OracleDisconnect();
                return data;
            }
            else
            {
                reader.Dispose();
                oracle.OracleDisconnect();
                return null;
            }
        }
        public List<m_device> getDeviceByStatus(string status)
        {
            oracle = new Database();
            List<m_device> data = new List<m_device>();
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("status", status));
            var reader = oracle.SqlQueryWithParams(SqlCmd.User.getDeviceByStatus, parameter);
            while (reader.Read())
            {
                data.Add(new m_device
                {
                    device_id = (string)reader["DEVICE_ID"],
                    cust_no = Int32.Parse(reader["CUST_NO"].ToString()),
                    conn_id = reader["CONN_ID"] == DBNull.Value ? string.Empty : (string)reader["CONN_ID"],
                    device_status = (string)reader["DEVICE_STATUS"],
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
        public void registerDevice(m_Register regis, int cust_no)
        {
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>
            {
                new OracleParameter("device_id", regis.device_id),
                new OracleParameter("cust_no", cust_no),
                new OracleParameter("tel", regis.phone_no)
            };
            var result = oracle.SqlExecuteWithParams(SqlCmd.User.registerNewDevice, parameter);
            //var resInsert = oracle.SqlExecuteWithParams(cmd, parameter);
            //string cmd = $@"INSERT INTO MPAY020(DEVICE_ID, CUST_NO, DEVICE_STATUS) VALUES('{regis.device_id}', {cust_no}, 'ACT')";
            //var result = oracle.SqlExcute(cmd);
            parameter = new List<OracleParameter>
            {
                new OracleParameter("cust_no", cust_no),
                new OracleParameter("device_id", regis.device_id),
                new OracleParameter("ip_addr", regis.ip_addr)
            };
            oracle.SqlExecuteWithParams(SqlCmd.Log.logRegister, parameter);
            oracle.OracleDisconnect();
        }
        public void registerCurrentDevice(m_Register regis, int cust_no)
        {
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter> {
                new OracleParameter("cust_no", cust_no),
                new OracleParameter("tel", regis.phone_no),
                new OracleParameter("device_id", regis.device_id)
            };
            //string cmd = $@"UPDATE MPAY020 SET CUST_NO = {cust_no}, CREATED_TIME = SYSDATE WHERE DEVICE_ID = '{regis.device_id}'";
            var result = oracle.SqlExecuteWithParams(SqlCmd.User.registerCurrentDevice, parameter);
            //var resInsert = oracle.SqlExecuteWithParams(cmd, parameter);
            parameter = new List<OracleParameter>
            {
                new OracleParameter("cust_no", cust_no),
                new OracleParameter("device_id", regis.device_id),
                new OracleParameter("ip_addr", regis.ip_addr)
            };
            oracle.SqlExecuteWithParams(SqlCmd.Log.logRegister, parameter);
            oracle.OracleDisconnect();
        }
        public m_Customer getProfileById(int id)
        {
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("cust_no", id));
            var reader = oracle.SqlQueryWithParams(SqlCmd.User.getProfileById, parameter);
            //var reader = oracle.SqlQuery(cmd);
            reader.Read();
            if (reader.HasRows)
            {
                var data = new m_Customer
                {
                    CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                    CUST_NAME = (string)reader["CUST_NAME"],
                    CITIZEN_NO = reader["CITIZEN_NO"] == DBNull.Value ? string.Empty : (string)reader["CITIZEN_NO"],
                    TEL = reader["TEL"] == DBNull.Value ? string.Empty : (string)reader["TEL"],
                    PERMIT = (string)reader["PERMIT"]
                };
                reader.Dispose();
                oracle.OracleDisconnect();
                return data;
            }
            else
            {
                reader.Dispose();
                oracle.OracleDisconnect();
                return null;
            }
        }
    }
}