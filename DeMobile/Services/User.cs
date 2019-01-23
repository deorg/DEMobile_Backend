using DeMobile.Concrete;
using DeMobile.Models.AppModel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Services
{
    public class User
    {
        private Database oracle;
        public List<SMS010> getNotification(int id)
        {
            oracle = new Database();
            List<SMS010> data = new List<SMS010>();
            //string cmd = $@"SELECT SMS010_PK, CON_NO, SMS_NOTE, SMS_TIME
            //                FROM   SMS010
            //                WHERE  CUST_NO = {id}";
            //var reader = oracle.SqlQuery(cmd);
            string cmd = $@"SELECT SMS010_PK, CON_NO, SMS_NOTE, SMS_TIME
                            FROM   SMS010
                            WHERE  CUST_NO = :id";
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("id", id));
            //var reader = oracle.SqlQuery(cmd);
            var reader = oracle.SqlQueryWithParams(cmd, parameter);
            while (reader.Read())
            {
                data.Add(new SMS010
                {
                    SMS010_PK = Int32.Parse(reader["SMS010_PK"].ToString()),
                    CON_NO = (string)reader["CON_NO"],
                    SMS_NOTE = (string)reader["SMS_NOTE"],
                    SMS_TIME = (DateTime)reader["SMS_TIME"]
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
        public List<ConPayment> getPayment(string no)
        {
            oracle = new Database();
            List<ConPayment> data = new List<ConPayment>();
            string cmd = $@"SELECT LNC_NO CON_NO, LNL_PAY_DATE PAY_DATE, LNL_PAY_AMT PAY_AMT
                            FROM   VW_LOAN_LEDGER_CO
                            WHERE  LNC_NO = '{no}'
                            ORDER BY LNL_SEQ DESC";
            var reader = oracle.SqlQuery(cmd);
            while (reader.Read())
            {
                data.Add(new ConPayment {
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
        public List<Contract> getContract(int id)
        {
            oracle = new Database();
            List<Contract> data = new List<Contract>();
            string cmd = $@"SELECT CON_NO, CUST_NO, TOT_AMT, PAY_AMT, PERIOD, BAL_AMT, CON_DATE, DISC_AMT
                            FROM   CONTRACT
                            WHERE  CUST_NO = {id}
                            ORDER BY CON_DATE
                            ";
            //string cmd = $@"SELECT L.CON_NO, CON_CUST_NO CUST_NO, TOT_AMT, PAY_AMT, PERIOD, BAL_AMT, CON_DATE, DISC_AMT
            //                FROM   LOAN_CARDV L, VW_CON_CUSTOMER_CO C
            //                WHERE  L.CON_NO = C.CON_NO AND LNC_STS = 'A' AND CON_CUST_NO = {id}
            //                ORDER BY CON_DATE
            //                ";
            var reader = oracle.SqlQuery(cmd);
            while (reader.Read())
            {
                data.Add(new Contract
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
            if(data.Count == 0)
            {
                reader.Dispose();
                oracle.OracleDisconnect();
                return null;
            }
            reader.Dispose();
            oracle.OracleDisconnect();
            return data;
        }
        public Customer getProfileByPhoneNO(string phone)
        {
            oracle = new Database();
            string cmd = $@"SELECT CUST_NO, CUST_NAME, CITIZEN_NO, TEL
                            FROM CUSTOMER 
                            WHERE TEL = '{phone}'";
            var reader = oracle.SqlQuery(cmd);
            reader.Read();
            if (reader.HasRows)
            {
                var data = new Customer
                {
                    CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                    CUST_NAME = (string)reader["CUST_NAME"],
                    CITIZEN_NO = reader["CITIZEN_NO"] == DBNull.Value ? string.Empty : (string)reader["CITIZEN_NO"],
                    TEL = reader["TEL"] == DBNull.Value ? string.Empty : (string)reader["TEL"]
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
        public Customer getProfileByCitizenNo(string citizen)
        {
            oracle = new Database();
            string cmd = $@"SELECT CUST_NO, CUST_NAME, CITIZEN_NO, TEL
                            FROM CUSTOMER 
                            WHERE CITIZEN_NO = '{citizen}'";
            var reader = oracle.SqlQuery(cmd);
            reader.Read();
            if (reader.HasRows)
            {
                var data = new Customer
                {
                    CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                    CUST_NAME = (string)reader["CUST_NAME"],
                    CITIZEN_NO = reader["CITIZEN_NO"] == DBNull.Value ? string.Empty : (string)reader["CITIZEN_NO"],
                    TEL = reader["TEL"] == DBNull.Value ? string.Empty : (string)reader["TEL"]
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
        public bool checkCurrentDevice(Register regis)
        {
            oracle = new Database();
            string cmd = $@"SELECT * FROM MPAY020 WHERE DEVICE_ID = '{regis.device_id}'";
            var reader = oracle.SqlQuery(cmd);
            reader.Read();
            return reader.HasRows;
        }
        public int registerDevice(Register regis, int cust_no)
        {
            oracle = new Database();
            string cmd = $@"INSERT INTO MPAY020(DEVICE_ID, CUST_NO, DEVICE_STATUS) VALUES('{regis.device_id}', {cust_no}, 'ACT')";
            var result = oracle.SqlExcute(cmd);
            oracle.OracleDisconnect();
            return result;
        }
        public Customer getProfileById(int id)
        {
            oracle = new Database();
            string cmd = $@"SELECT CUST_NO, CUST_NAME, CITIZEN_NO, TEL
                            FROM   CUSTOMER
                            WHERE  CUST_NO = {id}";
            //string cmd = $@"SELECT CUST_NO, CUST_FIRSTNAME||' '||CUST_LASTNAME CUST_NAME, CUST_CITIZEN_NO CITIZEN_NO, TEL_SMS TEL
            //                FROM   CUSTOMER
            //                WHERE  CUST_NO = {id}";
            var reader = oracle.SqlQuery(cmd);
            reader.Read();
            if (reader.HasRows)
            {
                var data = new Customer
                {
                    CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                    CUST_NAME = (string)reader["CUST_NAME"],
                    CITIZEN_NO = reader["CITIZEN_NO"] == DBNull.Value ? string.Empty : (string)reader["CITIZEN_NO"],
                    TEL = reader["TEL"] == DBNull.Value ? string.Empty : (string)reader["TEL"]
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