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
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.User.getSms, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        //cmd.CommandTimeout = 30;
                        cmd.Parameters.Add(new OracleParameter("cust_no", id));
                        var reader = cmd.ExecuteReader();
                        List<m_SMS010> data = new List<m_SMS010>();
                        while (reader.Read())
                        {
                            data.Add(new m_SMS010
                            {
                                SMS010_PK = Int32.Parse(reader["SMS010_PK"].ToString()),
                                CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                                CON_NO = reader["CON_NO"] == DBNull.Value ? string.Empty : (string)reader["CON_NO"],
                                SMS_NOTE = reader["SMS_NOTE"] == DBNull.Value ? string.Empty : (string)reader["SMS_NOTE"],
                                SMS_TIME = (DateTime)reader["SMS_TIME"],
                                SENDER = reader["SENDER"] == DBNull.Value ? null : (int?)Int32.Parse(reader["SENDER"].ToString()),
                                SENDER_TYPE = (string)reader["SENDER_TYPE"],
                                SMS010_REF = reader["SMS010_REF"] == DBNull.Value ? null : (int?)Int32.Parse(reader["SMS010_REF"].ToString()),
                                READ_STATUS = (string)reader["READ_STATUS"]
                            });
                        }
                        if (data.Count == 0)
                        {
                            cmd.Dispose();
                            reader.Dispose();
                            return data;
                        }
                        cmd.Dispose();
                        reader.Dispose();
                        return data;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            //oracle = new Database();
            //List<m_SMS010> data = new List<m_SMS010>();
            //List<OracleParameter> parameter = new List<OracleParameter>();
            //parameter.Add(new OracleParameter("cust_no", id));
            //var reader = oracle.SqlQueryWithParams(SqlCmd.User.getSms, parameter);
            //while (reader.Read())
            //{
            //    var test = reader["CON_NO"];
            //    data.Add(new m_SMS010
            //    {
            //        SMS010_PK = Int32.Parse(reader["SMS010_PK"].ToString()),
            //        CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
            //        CON_NO = reader["CON_NO"] == DBNull.Value ? string.Empty : (string)reader["CON_NO"],
            //        SMS_NOTE = reader["SMS_NOTE"] == DBNull.Value ? string.Empty : (string)reader["SMS_NOTE"],
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
            //    return data;
            //}
            //reader.Dispose();
            //oracle.OracleDisconnect();
            //return data;
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
            using(var conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.User.findContract, conn) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.Add(new OracleParameter("cust_no", cust_no));
                        cmd.Parameters.Add(new OracleParameter("con_no", con_no));
                        var reader = cmd.ExecuteReader();
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
                            cmd.Dispose();
                            reader.Dispose();
                            return data;
                        }
                        else
                        {
                            cmd.Dispose();
                            reader.Dispose();
                            return null;
                        }
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
            //parameter.Add(new OracleParameter("cust_no", cust_no));
            //parameter.Add(new OracleParameter("con_no", con_no));
            //var reader = oracle.SqlQueryWithParams(SqlCmd.User.findContract, parameter);
            //reader.Read();
            //if (reader.HasRows)
            //{
            //    var data = new m_Contract
            //    {
            //        CON_NO = (string)reader["CON_NO"],
            //        CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
            //        TOT_AMT = double.Parse(reader["TOT_AMT"].ToString()),
            //        PAY_AMT = double.Parse(reader["PAY_AMT"].ToString()),
            //        PERIOD = Int32.Parse(reader["PERIOD"].ToString()),
            //        BAL_AMT = double.Parse(reader["BAL_AMT"].ToString()),
            //        CON_DATE = (DateTime)reader["CON_DATE"],
            //        DISC_AMT = double.Parse(reader["DISC_AMT"].ToString())
            //    };
            //    reader.Dispose();
            //    oracle.OracleDisconnect();
            //    return data;
            //}
            //else
            //{
            //    reader.Dispose();
            //    oracle.OracleDisconnect();
            //    return null;
            //}
        }
        public List<m_Contract> getContract(int id)
        {
            using(var conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using(var cmd = new OracleCommand(SqlCmd.User.getContract, conn) { CommandType = CommandType.Text })
                    {
                        var data = new List<m_Contract>();
                        cmd.Parameters.Add("cust_no", id);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            data.Add(new m_Contract
                            {
                                CON_NO = reader["CON_NO"].ToString(),
                                CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                                CON_DATE = (DateTime)reader["CON_DATE"],
                                TOT_AMT = Double.Parse(reader["TOT_AMT"].ToString()),
                                PAY_AMT = double.Parse(reader["PAY_AMT"].ToString()),
                                PERIOD = int.Parse(reader["PERIOD"].ToString()),
                                BAL_AMT = double.Parse(reader["BAL_AMT"].ToString()),
                                DISC_AMT = double.Parse(reader["DISC_AMT"].ToString())
                            });
                        }
                        reader.Dispose();
                        cmd.Dispose();
                        return data;
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            //oracle = new Database();
            //List<m_Contract> data = new List<m_Contract>();
            //List<OracleParameter> parameter = new List<OracleParameter>();
            //parameter.Add(new OracleParameter("cust_no", id));
            //var reader = oracle.SqlQueryWithParams(SqlCmd.User.getContract, parameter);
            ////var reader = oracle.SqlQuery(cmd);
            //while (reader.Read())
            //{
            //    data.Add(new m_Contract
            //    {
            //        CON_NO = (string)reader["CON_NO"],
            //        CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
            //        TOT_AMT = Int32.Parse(reader["TOT_AMT"].ToString()),
            //        PAY_AMT = Int32.Parse(reader["PAY_AMT"].ToString()),
            //        PERIOD = Int32.Parse(reader["PERIOD"].ToString()),
            //        BAL_AMT = Int32.Parse(reader["BAL_AMT"].ToString()),
            //        CON_DATE = (DateTime)reader["CON_DATE"],
            //        DISC_AMT = Int32.Parse(reader["DISC_AMT"].ToString())
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
        public m_Customer getProfileByPhoneNO(string phone)
        {
            oracle = new Database();
            List<OracleParameter> parameter = new List<OracleParameter>();
            parameter.Add(new OracleParameter("tel", phone));
            var reader = oracle.SqlQueryWithParams(SqlCmd.User.getProfileByPhone, parameter);
            reader.Read();
            if (reader.HasRows)
            {
                var data = new m_Customer
                {
                    CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                    CUST_NAME = (string)reader["CUST_NAME"],
                    CITIZEN_NO = reader["CITIZEN_NO"] == DBNull.Value ? string.Empty : (string)reader["CITIZEN_NO"],
                    TEL = reader["TEL"] == DBNull.Value ? string.Empty : (string)reader["TEL"],
                    PERMIT = (string)reader["PERMIT"],
                    LINE_USER_ID = reader["LINE_USER_ID"] == DBNull.Value ? string.Empty : reader["LINE_USER_ID"].ToString()
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
        public int getCustNoByPhoneNo(string phone)
        {
            using(OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using(var cmd = new OracleCommand(SqlCmd.User.getCustNoByPhone, conn) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.Add("phone", phone);
                        var reader = cmd.ExecuteReader();
                        reader.Read();
                        if (reader.HasRows)
                        {
                            if (reader[0] != DBNull.Value)
                            {
                                var cust_no = Int32.Parse(reader[0].ToString());
                                reader.Dispose();
                                cmd.Dispose();
                                return cust_no;
                            }
                            else
                            {
                                reader.Dispose();
                                cmd.Dispose();
                                return 0;
                            }
                        }
                        else
                        {
                            reader.Dispose();
                            cmd.Dispose();
                            return 0;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public string getChatOn()
        {
            using(OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using(var cmd = new OracleCommand(SqlCmd.Information.getChatOn, conn) { CommandType = CommandType.Text })
                    {
                        var reader = cmd.ExecuteReader();
                        reader.Read();
                        if (reader.HasRows)
                        {
                            var chat = reader["CHAT"].ToString();
                            cmd.Dispose();
                            reader.Dispose();
                            return chat;
                        }
                        else
                        {
                            cmd.Dispose();
                            reader.Dispose();
                            return string.Empty;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public double getAppVersion(string serial_sim)
        {
            using(OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(serial_sim == "1111111111" ? SqlCmd.Information.getIosVersion : SqlCmd.Information.getAndroidVersion, conn) { CommandType = CommandType.Text })
                    {
                        var reader = cmd.ExecuteReader();
                        reader.Read();
                        if (reader.HasRows)
                        {
                            var version = serial_sim == "1111111111" ? double.Parse(reader["IOS_VERSION"].ToString()) : double.Parse(reader["ANDROID_VERSION"].ToString());
                            cmd.Dispose();
                            reader.Dispose();
                            return version;
                        }
                        else
                        {
                            cmd.Dispose();
                            reader.Dispose();
                            return 0;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public List<m_Customer> getProfilesByPhone(string phone)
        {
            using(OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using(var cmd = new OracleCommand(SqlCmd.User.getProfileByPhone, conn) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.Add("tel", phone);
                        var reader = cmd.ExecuteReader();
                        List<m_Customer> data = new List<m_Customer>();
                        while (reader.Read())
                        {
                            data.Add(new m_Customer
                            {
                                CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
                                CUST_NAME = reader["CUST_NAME"].ToString(),
                                CITIZEN_NO = reader["CITIZEN_NO"] == DBNull.Value ? string.Empty : (string)reader["CITIZEN_NO"],
                                TEL = reader["TEL"] == DBNull.Value ? string.Empty : (string)reader["TEL"],
                                PERMIT = (string)reader["PERMIT"]
                            });
                        }
                        if(data.Count > 0)
                        {
                            cmd.Dispose();
                            reader.Dispose();
                            return data;
                        }
                        else
                        {
                            cmd.Dispose();
                            reader.Dispose();
                            return null;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public m_Customer getProfileByDeviceId(string device_id)
        {
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.User.getProfileByDeviceId, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add(new OracleParameter("device_id", device_id));
                        var reader = cmd.ExecuteReader();
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
                            cmd.Dispose();
                            reader.Dispose();
                            return data;
                        }
                        else
                        {
                            cmd.Dispose();
                            reader.Dispose();
                            return null;
                        }
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public m_Customer getProfileBySerialSim(string serial_sim)
        {
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.User.getProfileBySerialSim, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add(new OracleParameter("serial_sim", serial_sim));
                        var reader = cmd.ExecuteReader();
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
                            cmd.Dispose();
                            reader.Dispose();
                            return data;
                        }
                        else
                        {
                            cmd.Dispose();
                            reader.Dispose();
                            return null;
                        }
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
            //    new OracleParameter("serial_sim", serial_sim)
            //};
            //var reader = oracle.SqlQueryWithParams(SqlCmd.User.getProfileBySerialSim, parameter);
            //reader.Read();
            //if (reader.HasRows)
            //{
            //    var data = new m_Customer
            //    {
            //        CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
            //        CUST_NAME = (string)reader["CUST_NAME"],
            //        CITIZEN_NO = reader["CITIZEN_NO"] == DBNull.Value ? string.Empty : (string)reader["CITIZEN_NO"],
            //        TEL = reader["TEL"] == DBNull.Value ? string.Empty : (string)reader["TEL"],
            //        PERMIT = (string)reader["PERMIT"]
            //    };
            //    reader.Dispose();
            //    oracle.OracleDisconnect();
            //    return data;
            //}
            //else
            //{
            //    reader.Dispose();
            //    oracle.OracleDisconnect();
            //    return null;
            //}
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
                    PERMIT = (string)reader["PERMIT"],
                    LINE_USER_ID = reader["LINE_USER_ID"] == DBNull.Value ? string.Empty : reader["LINE_USER_ID"].ToString()
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
        public void updateAppVersion(double version, string device_id)
        {
            using(OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using(var cmd = new OracleCommand(SqlCmd.User.updateAppVersion, conn) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.Add(new OracleParameter("app_version", version));
                        cmd.Parameters.Add(new OracleParameter("device_id", device_id));
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
        }
        public string getRegisterType()
        {
            using(OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using(var cmd = new OracleCommand(SqlCmd.Information.getRegisterType, conn) { CommandType = CommandType.Text })
                    {
                        var reader = cmd.ExecuteReader();
                        reader.Read();
                        var type = reader["REGISTER_TYPE"].ToString();
                        reader.Dispose();
                        conn.Close();
                        conn.Dispose();
                        return type;
                    }
                }
                catch
                {
                    conn.Close();
                    conn.Dispose();
                    return null;
                }
            }
        }
        public m_device checkCurrentDevice(string id)
        {
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.User.checkCurrentDevice, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add(new OracleParameter("device_id", id));
                        var reader = cmd.ExecuteReader();
                        reader.Read();
                        if (reader.HasRows)
                        {
                            var data = new m_device
                            {
                                device_id = (string)reader["DEVICE_ID"],
                                cust_no = Int32.Parse(reader["CUST_NO"].ToString()),
                               // conn_id = reader["CONN_ID"] == DBNull.Value ? string.Empty : (string)reader["CONN_ID"],
                                device_status = (string)reader["DEVICE_STATUS"],
                                tel = reader["TEL"] == DBNull.Value ? string.Empty : (string)reader["TEL"],
                               // tel_sim = reader["TEL_SIM"] == DBNull.Value ? string.Empty : (string)reader["TEL_SIM"],
                                serial_sim = reader["SERIAL_SIM"] == DBNull.Value ? string.Empty : (string)reader["SERIAL_SIM"],
                                //operator_name = reader["OPERATOR"] == DBNull.Value ? string.Empty : (string)reader["OPERATOR"],
                                //brand = reader["BRAND"] == DBNull.Value ? string.Empty : (string)reader["BRAND"],
                                //model = reader["MODEL"] == DBNull.Value ? string.Empty : (string)reader["MODEL"],
                                //api_version = Int32.Parse(reader["API_VERSION"].ToString()),
                                pin = reader["PIN"] == DBNull.Value ? string.Empty : (string)reader["PIN"],
                                created_time = (DateTime)reader["CREATED_TIME"],
                                app_version = double.Parse(reader["APP_VERSION"].ToString())
                            };
                            cmd.Dispose();
                            reader.Dispose();
                            return data;
                        }
                        else
                        {
                            cmd.Dispose();
                            reader.Dispose();
                            return null;
                        }
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
                //parameter.Add(new OracleParameter("device_id", id));
                //var reader = oracle.SqlQueryWithParams(SqlCmd.User.checkCurrentDevice, parameter);
                //reader.Read();
                //if (reader.HasRows)
                //{
                //    var data = new m_device
                //    {
                //        device_id = (string)reader["DEVICE_ID"],
                //        cust_no = Int32.Parse(reader["CUST_NO"].ToString()),
                //        conn_id = reader["CONN_ID"] == DBNull.Value ? string.Empty : (string)reader["CONN_ID"],
                //        device_status = (string)reader["DEVICE_STATUS"],
                //        tel = reader["TEL"] == DBNull.Value ? string.Empty : (string)reader["TEL"],
                //        tel_sim = reader["TEL_SIM"] == DBNull.Value ? string.Empty : (string)reader["TEL_SIM"],
                //        serial_sim = reader["SERIAL_SIM"] == DBNull.Value ? string.Empty : (string)reader["SERIAL_SIM"],
                //        operator_name = reader["OPERATOR"] == DBNull.Value ? string.Empty : (string)reader["OPERATOR"],
                //        brand = reader["BRAND"] == DBNull.Value ? string.Empty : (string)reader["BRAND"],
                //        model = reader["MODEL"] == DBNull.Value ? string.Empty : (string)reader["MODEL"],
                //        api_version = Int32.Parse(reader["API_VERSION"].ToString()),
                //        pin = reader["PIN"] == DBNull.Value ? string.Empty : (string)reader["PIN"],
                //        created_time = (DateTime)reader["CREATED_TIME"]
                //    };
                //    reader.Dispose();
                //    oracle.OracleDisconnect();
                //    return data;
                //}
                //else
                //{
                //    reader.Dispose();
                //    oracle.OracleDisconnect();
                //    return null;
                //}
        }
        public void checkCurrentBySerialAndDevice(string serial, string device)
        {

        }
        public List<m_device> getDeviceByCustNo(int cust_no)
        {
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.User.getDeviceByCustNo, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add(new OracleParameter("cust_no", cust_no));
                        var reader = cmd.ExecuteReader();
                        List<m_device> data = new List<m_device>();
                        while (reader.Read())
                        {
                            data.Add(new m_device
                            {
                                device_id = (string)reader["DEVICE_ID"],
                                cust_no = Int32.Parse(reader["CUST_NO"].ToString()),
                                //conn_id = reader["CONN_ID"] == DBNull.Value ? string.Empty : (string)reader["CONN_ID"],
                                device_status = (string)reader["DEVICE_STATUS"],
                                created_time = (DateTime)reader["CREATED_TIME"]
                            });
                        }
                        if(data.Count > 0)
                        {
                            reader.Dispose();
                            cmd.Dispose();
                            return data;
                        }
                        else
                        {
                            cmd.Dispose();
                            reader.Dispose();
                            return null;
                        }
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
            //parameter.Add(new OracleParameter("cust_no", cust_no));
            //var reader = oracle.SqlQueryWithParams(SqlCmd.User.getDeviceByCustNo, parameter);
            //reader.Read();
            //if (reader.HasRows)
            //{
            //    var data = new m_device
            //    {
            //        device_id = (string)reader["DEVICE_ID"],
            //        cust_no = Int32.Parse(reader["CUST_NO"].ToString()),
            //        //conn_id = reader["CONN_ID"] == DBNull.Value ? string.Empty : (string)reader["CONN_ID"],
            //        device_status = (string)reader["DEVICE_STATUS"],
            //        created_time = (DateTime)reader["CREATED_TIME"]
            //    };
            //    reader.Dispose();
            //    oracle.OracleDisconnect();
            //    return data;
            //}
            //else
            //{
            //    reader.Dispose();
            //    oracle.OracleDisconnect();
            //    return null;
            //}
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
                    //conn_id = reader["CONN_ID"] == DBNull.Value ? string.Empty : (string)reader["CONN_ID"],
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
        public void logout(int cust_no)
        {
            using(OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using(var cmd = new OracleCommand(SqlCmd.User.logout, conn) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.Add("cust_no", cust_no);
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
        }
        public void registerNewDevice(m_Register regis, int cust_no)
        {
            using(OracleConnection conn = new OracleConnection(Database.conString)) {
                try
                {
                    conn.Open();
                    using(var cmd = new OracleCommand(SqlCmd.User.registerNewDevice, conn) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.Add("device_id", regis.device_id);
                        cmd.Parameters.Add("tel", regis.phone_no);
                        cmd.Parameters.Add("serial_sim", regis.serial_sim);
                        cmd.Parameters.Add("brand", regis.brand);
                        cmd.Parameters.Add("model", regis.model);
                        cmd.Parameters.Add("api_version", regis.api_version);
                        cmd.Parameters.Add("app_version", regis.app_version);
                        cmd.Parameters.Add("platform", regis.platform);
                        cmd.Parameters.Add("cust_no", cust_no);
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
        }
        public void registerDevice(m_Register regis, int cust_no)
        {
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.User.registerDevice, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add(new OracleParameter("device_id", regis.device_id));
                        cmd.Parameters.Add(new OracleParameter("cust_no", cust_no));
                        cmd.Parameters.Add(new OracleParameter("tel", regis.phone_no));
                        //cmd.Parameters.Add(new OracleParameter("telSim", regis.phone_no_sim));
                        cmd.Parameters.Add(new OracleParameter("serial_sim", regis.serial_sim));
                        //cmd.Parameters.Add(new OracleParameter("operator", regis.operator_name));
                        cmd.Parameters.Add(new OracleParameter("brand", regis.brand));
                        cmd.Parameters.Add(new OracleParameter("model", regis.model));
                        cmd.Parameters.Add(new OracleParameter("api_version", regis.api_version));
                        cmd.Parameters.Add(new OracleParameter("pin", regis.pin));
                        cmd.Parameters.Add(new OracleParameter("app_version", regis.app_version));
                        cmd.Parameters.Add(new OracleParameter("platform", regis.platform));
                        
                        cmd.ExecuteNonQueryAsync();
                        cmd.Dispose();
                    }
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
                //oracle = new Database();
                //List<OracleParameter> parameter = new List<OracleParameter>
                //{
                //    new OracleParameter("device_id", regis.device_id),
                //    new OracleParameter("cust_no", cust_no),
                //    new OracleParameter("tel", regis.phone_no),
                //    new OracleParameter("telSim", regis.phone_no_sim),
                //    new OracleParameter("serial_sim", regis.serial_sim),
                //    new OracleParameter("operator", regis.operator_name),
                //    new OracleParameter("brand", regis.brand),
                //    new OracleParameter("model", regis.model),
                //    new OracleParameter("api_version", regis.api_version),
                //    new OracleParameter("pin", regis.pin)
                //};
                //var result = oracle.SqlExecuteWithParams(SqlCmd.User.registerNewDevice, parameter);
                //oracle.OracleDisconnect();
            }
        }
        public void updateIdentify(string device_id)
        {
            using(OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.User.updateIdentify, conn) { CommandType = CommandType.Text })
                    {
                        cmd.Parameters.Add("device_id", device_id);
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
        }
        public void registerCurrentDevice(m_Register regis, int cust_no)
        {
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.User.registerCurrentDevice, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.Parameters.Add("cust_no", cust_no);
                        cmd.Parameters.Add("tel", regis.phone_no);
                        cmd.Parameters.Add("serial_sim", regis.serial_sim);
                        cmd.Parameters.Add("api_version", regis.api_version);
                        cmd.Parameters.Add("app_version", regis.app_version);
                        cmd.Parameters.Add("device_id", regis.device_id);
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
            //List<OracleParameter> parameter = new List<OracleParameter> {                
            //        new OracleParameter("cust_no", cust_no),
            //        new OracleParameter("tel", regis.phone_no),
            //      //  new OracleParameter("telSim", regis.phone_no_sim),
            //        new OracleParameter("serial_sim", regis.serial_sim),
            //       // new OracleParameter("operator", regis.operator_name),
            //        //new OracleParameter("brand", regis.brand),
            //        //new OracleParameter("model", regis.model),
            //        new OracleParameter("api_version", regis.api_version),
            //        //new OracleParameter("pin", regis.pin),
            //        new OracleParameter("app_version", regis.app_version),
            //        //new OracleParameter("platform", regis.platform),
            //        new OracleParameter("device_id", regis.device_id),
            //    };
            //var result = oracle.SqlExecuteWithParams(SqlCmd.User.registerCurrentDevice, parameter);
            //oracle.OracleDisconnect();
        }
        public m_Customer getProfileById(int id)
        {
            using (OracleConnection conn = new OracleConnection(Database.conString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new OracleCommand(SqlCmd.User.getProfileById, conn) { CommandType = System.Data.CommandType.Text })
                    {
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.Add(new OracleParameter("cust_no", id));
                        var reader = cmd.ExecuteReader();
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
                            cmd.Dispose();
                            return data;
                        }
                        else
                        {
                            reader.Dispose();
                            return null;
                        }
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
            //parameter.Add(new OracleParameter("cust_no", id));
            //var reader = oracle.SqlQueryWithParams(SqlCmd.User.getProfileById, parameter);
            //reader.Read();
            //if (reader.HasRows)
            //{
            //    var data = new m_Customer
            //    {
            //        CUST_NO = Int32.Parse(reader["CUST_NO"].ToString()),
            //        CUST_NAME = (string)reader["CUST_NAME"],
            //        CITIZEN_NO = reader["CITIZEN_NO"] == DBNull.Value ? string.Empty : (string)reader["CITIZEN_NO"],
            //        TEL = reader["TEL"] == DBNull.Value ? string.Empty : (string)reader["TEL"],
            //        PERMIT = (string)reader["PERMIT"]
            //    };
            //    reader.Dispose();
            //    oracle.OracleDisconnect();
            //    return data;
            //}
            //else
            //{
            //    reader.Dispose();
            //    oracle.OracleDisconnect();
            //    return null;
            //}
        }
    }
}