using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeMobile.Concrete
{
    public class Database
    {
        //private static string host = Properties.Settings.Default.Host;
        //private static string port = Properties.Settings.Default.Port;
        //private static string username = Properties.Settings.Default.Username;
        //private static string password = Properties.Settings.Default.Password;
        //private static string database = Properties.Settings.Default.Database;

        private static string host = Constants.OracleDb.Production.Host;
        private static string port = Constants.OracleDb.Production.Port;
        private static string username = Constants.OracleDb.Production.Username;
        private static string password = Constants.OracleDb.Production.Password;
        private static string database = Constants.OracleDb.Production.Source;

        private static string conString = $"User Id={username};Password={password};Data Source={host}:{port}/{database};";
        private OracleConnection con;
        private OracleCommand cmd;

        public Database()
        {
            OracleConnect();
        }
        public void OracleConnect()
        {
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
        public OracleDataReader SqlExcute(string command)
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
    }
}