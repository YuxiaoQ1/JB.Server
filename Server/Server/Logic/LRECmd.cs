using Common;
using MySql.Data.MySqlClient;
namespace Server.Logic
{
    //客户端 登录、注册、退出
    class LRECmd
    {
        public static void Login(Player player, byte[] data)
        {
            ST_LOGIN_REGISTER info = (ST_LOGIN_REGISTER)MessageHelper.DeserializeWithBinary(data);
            MySqlConnection conn = SqlConnHelper.Connect();
            if(conn == null)
            {
                LogHelper.ERRORLOG("Connect to mysql failed!");
                player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_LOGIN_FAILED, new byte[0]));
                player.Close();
                return;
            }
            string sql = string.Format("select * from t_user where username = '%s' and password = ''", info.username, info.password);
            MySqlDataReader userInfo = SqlConnHelper.ExecuteQuery(sql);
            if(userInfo == null)
            {
                LogHelper.ERRORLOG("User not exit!");
                player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_LOGIN_FAILED, new byte[0]));
                player.Close();
                return;
            }
            _InitPlayerInfo(player, userInfo);
            player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_LOGIN_SUCCESS, new byte[0]));
            player.OnLine = true;
        }

        public static void Register(Player player, byte[] data)
        {
            ST_LOGIN_REGISTER info = (ST_LOGIN_REGISTER)MessageHelper.DeserializeWithBinary(data);
            MySqlConnection conn = SqlConnHelper.Connect();
            if(conn == null)
            {
                LogHelper.ERRORLOG("Connect to mysql failed!");
                player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_REGISTER_FAILED, new byte[0]));
                return;
            }
            string sql = string.Format("select * from t_user where username = '%s'", info.username);
            MySqlDataReader userInfo = SqlConnHelper.ExecuteQuery(sql);
            if (userInfo == null)
            {
                LogHelper.ERRORLOG("User not exit!");
                player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_REGISTER_FAILED, new byte[0]));
                return;
            }
            //插入到数据库
            sql = string.Format("insert into t_user(username, password) values('%s', '%s')", info.username, info.password);
            if (SqlConnHelper.Execute(sql) > 0)
            {
                _InitPlayerInfo(player, userInfo);
                player.OnLine = true;
                player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_REGISTER_SUCCESS, new byte[0]));
            }
            else
            {
                LogHelper.ERRORLOG("Insert into mysql failed!");
                player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_REGISTER_FAILED, new byte[0]));
            }
        }

        public static void Exit(Player player, byte[] data)
        {
            //TODO:退出操作，如保存某些信息
            player.OnLine = false;
            player.Close();
        }

        private static void _InitPlayerInfo(Player player, MySqlDataReader reader)
        {
            do
            {
                player.Uid = int.Parse(reader["uid"].ToString());
                player.Username = reader["username"].ToString();
                //TODO:初始化更多玩家信息
            } while (false);
        }
    }
}
