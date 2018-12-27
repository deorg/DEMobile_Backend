using DeMobile.Concrete;
using DeMobile.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Services
{
    public class Report
    {
        private Database oracle;
        public List<MonthlyMeeting> getMonthlyMeeting(string startDate, string endDate)
        {
            oracle = new Database();
            List<MonthlyMeeting> data = new List<MonthlyMeeting>();
            string cmd = $@"SELECT                                             
                            BRH_ID,
                            SUM(SALE_AMT) SALE_AMT,
                            SUM(PAY_AMT) PAY_AMT,
                            SUM(SALE_AMT) - PSA.FBRH_SUM(BRH_ID, 'TAR', MIN(MDATE), MAX(DDATE)) DIF_TAR_AMT,
                            PSA.FBRH_SUM(BRH_ID, 'SALE', TRUNC(MDATE, 'YY'), MAX(DDATE)) ACC_SALE_AMT,
                            PSA.FBRH_SUM(BRH_ID, 'PAY', TRUNC(MDATE, 'YY'), MAX(DDATE)) ACC_PAY_AMT,
                            PSA.FBRH_SUM(BRH_ID, 'TAR', TRUNC(MDATE, 'YY'), MAX(DDATE)) ACC_TAR_AMT,
                            Nvl(PSA.FBRH_SUM(BRH_ID, 'TAR', MIN(MDATE), MAX(DDATE)), 0) TAR_AMT,  
                            SUM(PDO_LOSS_GAIN) LOS_PDO_AMT,
                            SUM(PDO_AMT) PDO_AMT,
                            PNODE.FCUST_PDO_AMT(BRH_ID, MDATE, 'O') OCUST_PDO_AMT,
                            PNODE.FCUST_PDO_AMT(BRH_ID, MDATE, 'N') NCUST_PDO_AMT,
                            PNODE.FREMAIN_AMT(BRH_ID, MDATE) FREMAIN_AMT,
                            PNODE.FMGRS_NAME(BRH_ID, MDATE) MGRS_Name
                            FROM SA010V
                            WHERE BRH_ID LIKE '%'
                            AND MDATE BETWEEN TRUNC(TO_DATE('{startDate}','DD/MM/RRRR'),'MM') AND TRUNC(TO_DATE('{endDate}','DD/MM/RRRR'),'MM')
                            AND BRH_ID< 66
                            GROUP BY BRH_ID, MDATE
                            ORDER BY BRH_ID";
            var reader = oracle.SqlExcute(cmd);
            reader.Read();
            while (reader.Read())
            {
                data.Add(new MonthlyMeeting
                {
                    brhId = reader["BRH_ID"].ToString(),
                    saleAmt = reader.GetDouble(1),
                    payAmt = reader.GetDouble(2),
                    difTarAmt = reader.GetDouble(3),
                    accSaleAmt = reader.GetDouble(4),
                    accPayAmt = reader.GetDouble(5),
                    accTarAmt = reader.GetDouble(6),
                    tarAmt = reader.GetDouble(7),
                    losPdoAmt = reader.GetDouble(8),
                    pdoAmt = reader.GetDouble(9),
                    OcustPdoAmt = reader.GetDouble(10),
                    NcustPdoAmt = reader.GetDouble(11),
                    fRemainAmt = reader.GetDouble(12),
                    mgrName = reader["MGRS_Name"] == DBNull.Value ? "" : reader["MGRS_Name"].ToString()
                });
            }
            reader.Dispose();
            oracle.OracleDisconnect();
            return data;
        }
    }
}