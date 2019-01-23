using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DeMobile.Concrete
{
    public class Database
    {
        private static string host = Constants.OracleDb.Development.Host;
        private static string port = Constants.OracleDb.Development.Port;
        private static string username = Constants.OracleDb.Development.Username;
        private static string password = Constants.OracleDb.Development.Password;
        private static string database = Constants.OracleDb.Development.Source;

        //private static string host = Constants.OracleDb.Production.Host;
        //private static string port = Constants.OracleDb.Production.Port;
        //private static string username = Constants.OracleDb.Production.Username;
        //private static string password = Constants.OracleDb.Production.Password;
        //private static string database = Constants.OracleDb.Production.Source;

        private static string conString = $"User Id={username};Password={password};Data Source={host}:{port}/{database};";
        private OracleConnection con;
        private OracleCommand cmd;

        public Database()
        {
            OracleConnect();
        }
        public void OracleConnect()
        {
            cmd = new OracleCommand();
            con = new OracleConnection(conString);
            con.Open();
            cmd = con.CreateCommand();
            cmd.BindByName = true;
        }
        public void OracleDisconnect()
        {
            con.Dispose();
            cmd.Dispose();
        }
        public OracleDataReader SqlQuery(string command)
        {
            try
            {
                cmd.BindByName = true;
                cmd.CommandText = command;
                return cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public OracleDataReader SqlQueryWithParams(string command, List<OracleParameter> parameter)
        {
            try
            {
                cmd.BindByName = true;
                cmd.CommandText = command;
                foreach(var p in parameter)
                {
                    cmd.Parameters.Add(p);
                }
                return cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public int SqlExcute(string command)
        {
            try
            {
                cmd.BindByName = true;
                cmd.CommandText = command;
                var result = cmd.ExecuteNonQuery();
                return result;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public OracleCommand SqlExecuteWithParams(string command, List<OracleParameter> parameter)
        {
            try
            {
                cmd.BindByName = true;
                cmd.CommandText = command;
                foreach (var p in parameter)
                {
                    cmd.Parameters.Add(p);
                }
                var result = cmd.ExecuteNonQuery();
                return cmd;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}