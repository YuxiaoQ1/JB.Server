using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Server;

namespace Server
{
    class SqlConnHelper
    {
        public const string CONNECTIONSTRING = "datasource=127.0.0.1;port=3306;database=jbgame;user=root;pwd=123456;";

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
        //增删改:返回值大于0表示执行成功
        //执行查询:返回-1
        public static int Execute(string sql)
        {
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            int ret = -2;
            try
            {
                ret = cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                LogHelper.ERRORLOG(e.Message);
                return -2;
            }
            return ret;
        }
        public static MySqlDataReader ExecuteQuery(string sql)
        {
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            return cmd.ExecuteReader();;
            /*
             * 如果查询多行结果，可以如下方式遍历
            while(MySqlDataReader.Read())
            {

            }
            */
        }
        /*

        public static void PrepareCmd(MySqlCommand cmd, 
            MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, 
            string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (MySqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }
        //查询
        public static MySqlDataReader ExecuteReader(string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            if (conn.State != ConnectionState.Open)
                conn.Open();
            try
            {
                PrepareCmd(cmd, conn, null, CommandType.Text, cmdText, commandParameters);
                MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return reader;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        //增、删、改
        public static int ExecuteNonQuery(string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCmd(cmd, conn, null, CommandType.Text, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        */
    }
}
