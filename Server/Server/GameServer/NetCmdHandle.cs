using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Server
{
    static class NetCmdHandle
    {
        private static Dictionary<NetCmd, Action<Player, byte[]>> NetCmdHandles = new Dictionary<NetCmd, Action<Player, byte[]>>();

        static public void Init()   //在这里注册所有的逻辑处理函数
        {
            NetCmdHandles.Add(NetCmd.RAWSTRING, new Action<Player, byte[]>(Logic.RawString.OnReiceve));
            NetCmdHandles.Add(NetCmd.HEARTBEAT, new Action<Player, byte[]>(Logic.HeartBeats.OnReiceve));
            NetCmdHandles.Add(NetCmd.BROADCAST, new Action<Player, byte[]>(Logic.Broadcast.OnReiceve));
            NetCmdHandles.Add(NetCmd.C_GS_LOGIN_REQ, new Action<Player, byte[]>(Logic.LRECmd.Login));
            NetCmdHandles.Add(NetCmd.C_GS_REGISTER_REQ, new Action<Player, byte[]>(Logic.LRECmd.Register));
            NetCmdHandles.Add(NetCmd.C_GS_EXIT_REQ, new Action<Player, byte[]>(Logic.LRECmd.Exit));
        }

        static public void Dispatch(MessageHelper messageHelper, Player player)
        {
            while(messageHelper.HasMessage())
            {
                messageHelper.ReadMessage(out NetCmd cmd, out byte[] data);
                if(NetCmdHandles.TryGetValue(cmd, out Action<Player, byte[]> action))
                {
                    action?.Invoke(player, data);
                }
                LogHelper.DEBUGLOG("[{0}]Recieve msg: {1} {2}",player.GetSocket.RemoteEndPoint.ToString(), cmd, data.ToString());
            }
        }
    }
}
