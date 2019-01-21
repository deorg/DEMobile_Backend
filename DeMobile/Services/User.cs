using DeMobile.Concrete;
using DeMobile.Models.AppModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Services
{
    public class User
    {
        private Database oracle;
        public List<Notification> getNotification(int id)
        {
            oracle = new Database();
            List<Notification> data = new List<Notification>();
            string cmd = $@"SELECT SMS010_PK, CON_NO, SMS_NOTE, SMS_TIME
                            FROM   SMS
                            WHERE  CUST_NO = {id}";
            var reader = oracle.SqlExcute(cmd);
            while (reader.Read())
            {
                data.Add(new Notification
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
            var reader = oracle.SqlExcute(cmd);
            while (reader.Read())
            {
                data.Add(new ConPayment {
                    CON_NO = (string)reader["CON_NO"],
                    PAY_DATE = (DateTime)reader["PAY_DATE"],
                    PAY_AMT = Int32.Parse(reader["PAY_AMT"].ToString())
                });

                //data.Add(new ConPayment
                //{
                //    CON_NO = reader.GetString(0),
                //    PAY_DATE = reader.GetDateTime(1),
                //    PAY_AMT = reader.GetInt32(2)
                //});
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
            var reader = oracle.SqlExcute(cmd);
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
            var reader = oracle.SqlExcute(cmd);
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
            var reader = oracle.SqlExcute(cmd);
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
        public Customer getProfileById(int id)
        {
            oracle = new Database();
            string cmd = $@"SELECT CUST_NO, CUST_NAME, CITIZEN_NO, TEL
                            FROM   CUSTOMER
                            WHERE  CUST_NO = {id}";
            //string cmd = $@"SELECT CUST_NO, CUST_FIRSTNAME||' '||CUST_LASTNAME CUST_NAME, CUST_CITIZEN_NO CITIZEN_NO, TEL_SMS TEL
            //                FROM   CUSTOMER
            //                WHERE  CUST_NO = {id}";
            var reader = oracle.SqlExcute(cmd);
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
            //while (reader.Read())
            //{
            //    data.Add(new MonthlyMeeting
            //    {
            //        brhId = reader["BRH_ID"].ToString(),
            //        saleAmt = reader.GetDouble(1),
            //        payAmt = reader.GetDouble(2),
            //        difTarAmt = reader.GetDouble(3),
            //        accSaleAmt = reader.GetDouble(4),
            //        accPayAmt = reader.GetDouble(5),
            //        accTarAmt = reader.GetDouble(6),
            //        tarAmt = reader.GetDouble(7),
            //        losPdoAmt = reader.GetDouble(8),
            //        pdoAmt = reader.GetDouble(9),
            //        OcustPdoAmt = reader.GetDouble(10),
            //        NcustPdoAmt = reader.GetDouble(11),
            //        fRemainAmt = reader.GetDouble(12),
            //        mgrName = reader["MGRS_Name"] == DBNull.Value ? "" : reader["MGRS_Name"].ToString()
            //    });
            //}
            
        }
    }
}