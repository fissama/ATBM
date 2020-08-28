using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace ATBM_1
{
    class ConnectionString
    {
        public static OracleConnection GetConnection(string host, int port, String sid, String user, String password)
        {
            Console.WriteLine("Getting Connection ...");

            // 'Connection String' kết nối trực tiếp tới Oracle.
            string connString = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = "
                     + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = "
                     + sid + ")));Password=" + password + ";User ID=" + user;

            OracleConnection conn = new OracleConnection();

            conn.ConnectionString = connString;

            return conn;
        }
    }
    class DBConnection
    {
        public static OracleConnection GetConnection(string username, string password)
        {
            string host = "localhost";
            int port = 1521;
            string sid = "XE";
            string user = username;
            string pass = password;

            return ConnectionString.GetConnection(host, port, sid, user, pass);
        }
        public static bool checkConnection(String user, String password)
        {
            OracleConnection con = DBConnection.GetConnection("SYSTEM", "161299");

            return true;
        }
    }

    
}
