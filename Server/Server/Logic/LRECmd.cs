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

            LogHelper.DEBUGLOG("Login Info U:[{0}]  P:[{1}]", info.username, info.password);
            MySqlConnection conn = SqlConnHelper.Connect();
            if(conn == null)
            {
                LogHelper.ERRORLOG("Connect to mysql failed!");
                player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_LOGIN_FAILED, new byte[0]));
                player.Close();
                return;
            }
            string sql = string.Format("select * from t_user where username = '{0}' and password = '{1}'", info.username, info.password);
            MySqlDataReader userInfo = SqlConnHelper.ExecuteQuery(sql);
            if(userInfo == null)
            {
                LogHelper.ERRORLOG("User not exit!");
                player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_LOGIN_FAILED, new byte[0]));
                player.Close();
                userInfo.Close();
                return;
            }
            _InitPlayerInfo(player, userInfo);
            player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_LOGIN_SUCCESS, 
                MessageHelper.SerializeToBinary(_PackPlayerInfo(player))));
            player.OnLine = true;
            userInfo.Close();
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
            
            //插入到数据库
            string sql = string.Format("insert into t_user(username, password) values('{0}', '{1}')", info.username, info.password);
            if (SqlConnHelper.Execute(sql) > 0)
            {
                sql = string.Format("select * from t_user where username = '{0}'", info.username);
                MySqlDataReader userInfo = SqlConnHelper.ExecuteQuery(sql);

                _InitPlayerInfo(player, userInfo);
                userInfo.Close();
                player.OnLine = true;
                player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_REGISTER_SUCCESS,
                    MessageHelper.SerializeToBinary(_PackPlayerInfo(player))));
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
                reader.Read();
                player.Username = reader["username"].ToString();
                player.CoinCounts = int.Parse(reader["CoinCounts"].ToString());
                player.DiamondCounts = int.Parse(reader["DiamondCounts"].ToString());
                player.Level = int.Parse(reader["Level"].ToString());
                player.Exp = int.Parse(reader["Exp"].ToString());
                player.ClothId = int.Parse(reader["ClothId"].ToString()); 
            } while (false);
        }

        private static ST_PLAYER_BASE_INFO _PackPlayerInfo(Player player)
        {
            return new ST_PLAYER_BASE_INFO(
                player.Username, 
                player.CoinCounts, 
                player.DiamondCounts, 
                player.Level, 
                player.Exp, 
                player.ClothId);
        }
    }
}
