using DeMobile.Concrete;
using DeMobile.Models.AppModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Services
{
    public class Authen
    {
        private Database oracle;
        public List<Notification> getNotification(int id)
        {
            oracle = new Database();
            List<Notification> data = new List<Notification>();
            string cmd = $@"SELECT SMS010_PK, CON_NO, NOTE SMS_NOTE, CREATED_TIME SMS_TIME
                            FROM   SMS010
                            WHERE  CUST_NO = {id} AND CREATED_TIME > TRUNC(SYSDATE,'YY')";
            var reader = oracle.SqlExcute(cmd);
            while (reader.Read())
            {
                data.Add(new Notification
                {
                    SMS010_PK = reader.GetInt32(0),
                    CON_NO = reader.GetString(1),
                    SMS_NOTE = reader.GetString(2),
                    SMS_TIME = reader.GetDateTime(3)
                });
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
                data.Add(new ConPayment
                {
                    CON_NO = reader.GetString(0),
                    PAY_DATE = reader.GetDateTime(1),
                    PAY_AMT = reader.GetInt32(2),

                });
            }
            reader.Dispose();
            oracle.OracleDisconnect();
            return data;
        }
        public List<Contract> getContract(int id)
        {
            oracle = new Database();
            List<Contract> data = new List<Contract>();
            string cmd = $@"SELECT L.CON_NO, CON_CUST_NO CUST_NO, TOT_AMT, PAY_AMT, PERIOD, BAL_AMT, CON_DATE, DISC_AMT
                            FROM   LOAN_CARDV L, VW_CON_CUSTOMER_CO C
                            WHERE  L.CON_NO = C.CON_NO AND LNC_STS = 'A' AND CON_CUST_NO = {id}
                            ORDER BY CON_DATE
                            ";
            var reader = oracle.SqlExcute(cmd);
            while (reader.Read())
            {
                data.Add(new Contract
                {
                    CON_NO = reader.GetString(0),
                    CUST_NO = reader.GetInt32(1),
                    TOT_AMT = reader.GetInt32(2),
                    PAY_AMT = reader.GetInt32(3),
                    PERIOD = reader.GetInt32(4),
                    BAL_AMT = reader.GetInt32(5),
                    CON_DATE = reader.GetDateTime(6),
                    DISC_AMT = reader.GetInt32(7)
                });
            }
            reader.Dispose();
            oracle.OracleDisconnect();
            return data;
        }
        public Customer getProfile(int id)
        {
            oracle = new Database();
            string cmd = $@"SELECT CUST_NO, CUST_FIRSTNAME||' '||CUST_LASTNAME CUST_NAME, CUST_CITIZEN_NO CITIZEN_NO, TEL_SMS TEL
                            FROM   CUSTOMER
                            WHERE  CUST_NO = {id}";
            var reader = oracle.SqlExcute(cmd);
            reader.Read();
            var data = new Customer { CUST_NO = reader.GetInt32(0), CUST_NAME = reader.GetString(1), CITIZEN_NO = reader.GetString(2), TEL = reader.GetString(3) };
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
            reader.Dispose();
            oracle.OracleDisconnect();
            return data;
        }
    }
}