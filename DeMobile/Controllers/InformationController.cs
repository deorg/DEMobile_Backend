using System;
using System.Web.Http;
using DeMobile.Models.Management;
using DeMobile.Services;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Web.Configuration;

namespace DeMobile.Controllers
{
    public class InformationController : ApiController
    {
        Information info = new Information();

        [Route("api/information/dashboard")]
        public IHttpActionResult GetNumMember()
        {
            try
            {
                var num = info.getDashBoard();
                return Ok(new { status = "SUCCESS", data = num });
            }
            catch(Exception e)
            {
                return Ok(new { status = "FAILURE", data = e.Message });
            }
        }
        [Route("api/monitor/logregistered")]
        public IHttpActionResult GetLogRegis()
        {
            try
            {
                var result = info.getLogRegistered();
                return Ok(new { status = "SUCCESS", data = result });
            }
            catch(Exception e)
            {
                return Ok(new { status = "FAILURE", data = e.Message });
            }
        }
        [Route("api/management/service")]
        public IHttpActionResult GetManageService()
        {
            try
            {
                Configuration config;
                config = WebConfigurationManager.OpenWebConfiguration("~");
                AppSettingsSection appsettings;
                appsettings = (AppSettingsSection)config.GetSection("appSettings");
                var appService = appsettings.Settings["AppService"].Value;
                var smsService = appsettings.Settings["SmsService"].Value;
                var paymentService = appsettings.Settings["PaymentService"].Value;

                

                return Ok(new { status = "SUCCESS", data = new { appService = appService, smsService = smsService, paymentService = paymentService} });
            }
            catch(Exception e)
            {
                return Ok(new { status = "FAILURE", data = e.Message });
            }
        }
        [Route("api/management/service")]
        public IHttpActionResult PostManageService([FromBody]m_Service value)
        {
            try
            {
                Configuration config;
                config = WebConfigurationManager.OpenWebConfiguration("~");
                AppSettingsSection appsettings;
                appsettings = (AppSettingsSection)config.GetSection("appSettings");
                appsettings.Settings["AppService"].Value = value.appService.ToString();
                appsettings.Settings["SmsService"].Value = value.smsService.ToString();
                appsettings.Settings["PaymentService"].Value = value.paymentService.ToString();
                config.Save();
                return Ok(new { status = "SUCCESS", data = "saved" });
            }
            catch(Exception e)
            {
                return Ok(new { status = "FAILURE", data = e.Message });
            }
        }
        //[Route("api/management/setpermit")]
        //public IHttpActionResult GetSetPermit(int cust_no, string permit)
        //{
        //    try
        //    {

        //    }
        //    catch(Exception e)
        //    {

        //    }
        //}


        [Route("api/info/getbranch")]
        public IHttpActionResult GetBranch(string brh)
        {
            string conString = "User Id=DE;Password=DE;Data Source=192.168.1.10:1521/HPDB;";
            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = $"select sale_brh_id brh_id, sale_brh_name brh_name from sale_branch where sale_brh_use = 'Y' and sale_brh_id like '{brh}' order by brh_id";
                        //cmd.CommandText = "select first_name from employees where department_id = :id";

                        //OracleParameter id = new OracleParameter("id", 50);
                        //cmd.Parameters.Add(id);
                        string result = "";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Console.WriteLine("**************************************Branch: " + reader.GetString(0));
                            result += reader.GetValue(0);
                        }
                        reader.Dispose();
                        return Ok(result);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return BadRequest(ex.Message);
                    }
                }
            }
        }
        [Route("api/info/monthlymeeting")]
        public IHttpActionResult GetMonthlyMeeting(string startDate, string endDate)
        {
            Report report = new Report();
            var result = report.getMonthlyMeeting(startDate, endDate);
            if (result != null)
                return Json(new { status = "SUCCESS", data = result });
            else
                return Json(new { status = "FAILURE", data = result });
        }
        //[Route("api/info/saleinfo")]
        //public IHttpActionResult GetSaleInfo(string startDate, string endDate, string filter, string sort = "สาขา")
        //{
        //    string conString = "User Id=DE;Password=DE;Data Source=192.168.1.10:1521/HPDB;";
        //    using (OracleConnection con = new OracleConnection(conString))
        //    {
        //        using (OracleCommand cmd = con.CreateCommand())
        //        {
        //            try
        //            {
        //                List<MonthlyMeeting> data = new List<MonthlyMeeting>();
        //                con.Open();
        //                cmd.BindByName = true;
        //                cmd.CommandText = $@"SELECT
        //                                      BRH_ID,
        //                                      SUM(SALE_AMT) SALE_AMT,
        //                                      SUM(PAY_AMT) PAY_AMT,
      
        //                                      SUM(SALE_AMT) - PSA.FBRH_SUM(BRH_ID, 'TAR', MIN(MDATE), MAX(DDATE)) DIF_TAR_AMT,
         
        //                                      PSA.FBRH_SUM(BRH_ID, 'SALE', TRUNC(MDATE, 'YY'), MAX(DDATE)) ACC_SALE_AMT,
        //                                      PSA.FBRH_SUM(BRH_ID, 'PAY', TRUNC(MDATE, 'YY'), MAX(DDATE)) ACC_PAY_AMT,
        //                                      PSA.FBRH_SUM(BRH_ID, 'TAR', TRUNC(MDATE, 'YY'), MAX(DDATE)) ACC_TAR_AMT,
        //                                      Nvl(PSA.FBRH_SUM(BRH_ID, 'TAR', MIN(MDATE), MAX(DDATE)), 0) TAR_AMT,  

        //                                      SUM(PDO_LOSS_GAIN) LOS_PDO_AMT,
        //                                      SUM(PDO_AMT) PDO_AMT,
        //                                      PNODE.FCUST_PDO_AMT(BRH_ID, MDATE, 'O') OCUST_PDO_AMT,
        //                                      PNODE.FCUST_PDO_AMT(BRH_ID, MDATE, 'N') NCUST_PDO_AMT,
        //                                      PNODE.FREMAIN_AMT(BRH_ID, MDATE) FREMAIN_AMT,

        //                                      PNODE.FMGRS_NAME(BRH_ID, MDATE) MGRS_Name

        //                                      FROM SA010V
        //                                      WHERE BRH_ID LIKE '%'
        //                                      AND MDATE BETWEEN TRUNC(TO_DATE('{startDate}','DD/MM/RRRR'),'MM') AND TRUNC(TO_DATE('{endDate}','DD/MM/RRRR'),'MM')
        //                                      AND BRH_ID< 66


        //                                      GROUP BY BRH_ID, MDATE
        //                                      ORDER BY BRH_ID";
        //                //cmd.CommandText = "select first_name from employees where department_id = :id";

        //                //OracleParameter id = new OracleParameter("id", 50);
        //                //cmd.Parameters.Add(id);

        //                OracleDataReader reader = cmd.ExecuteReader();
        //                reader.Read();
        //                while (reader.Read())
        //                {
        //                    data.Add(new MonthlyMeeting
        //                    {
        //                        brhId = reader["BRH_ID"].ToString(),
        //                        saleAmt = reader.GetDouble(1),
        //                        payAmt = reader.GetDouble(2),
        //                        difTarAmt = reader.GetDouble(3),
        //                        accSaleAmt = reader.GetDouble(4),
        //                        accPayAmt = reader.GetDouble(5),
        //                        accTarAmt = reader.GetDouble(6),
        //                        tarAmt = reader.GetDouble(7),
        //                        losPdoAmt = reader.GetDouble(8),
        //                        pdoAmt = reader.GetDouble(9),
        //                        OcustPdoAmt = reader.GetDouble(10),
        //                        NcustPdoAmt = reader.GetDouble(11),
        //                        fRemainAmt = reader.GetDouble(12),
        //                        mgrName = reader["MGRS_Name"] == DBNull.Value ? "" : reader["MGRS_Name"].ToString()
        //                    });
        //                    //Console.WriteLine("**************************************Branch: " + reader.GetString(0));
        //                    //result += reader.GetValue(0);
        //                }
        //                reader.Dispose();
        //                return Ok(data);
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(ex.Message);
        //                return BadRequest(ex.Message);
        //            }
        //        }
        //    }
        //}
    }
}
