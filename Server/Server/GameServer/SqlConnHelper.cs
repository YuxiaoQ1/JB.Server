using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Server;

namespace Server
{
    class SqlConnHelper
    {
        public const string CONNECTIONSTRING = "datasource=127.0.0.1;port=3306;database=jbgame;user=jbserver;pwd=jbserver;";

        private static MySqlConnection conn;

        public static MySqlConnection Connect()
        {
            if(conn == null)
            {
                conn = new MySqlConnection(CONNECTIONSTRING);
            }
            try
            {
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                LogHelper.ERRORLOG(e);
                return null;
            }

        }
        public static void Close()
        {
            if (conn != null)
                conn.Close();
            else
            {
                LogHelper.ERRORLOG("conn is null!");
            }
        }
    }
}
