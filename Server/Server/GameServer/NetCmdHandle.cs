using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Server.ROOM;

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

            NetCmdHandles.Add(NetCmd.C_GS_LOGIN_REQ, new Action<Player, byte[]>(Logic.LRECmd.Login));//登录
            NetCmdHandles.Add(NetCmd.C_GS_REGISTER_REQ, new Action<Player, byte[]>(Logic.LRECmd.Register));//注册
            NetCmdHandles.Add(NetCmd.C_GS_EXIT_REQ, new Action<Player, byte[]>(Logic.LRECmd.Exit));//下线

            NetCmdHandles.Add(NetCmd.C2S_CREATE_ROOM, new Action<Player, byte[]>(RoomCmd.CreateRoom));//创建房间
            NetCmdHandles.Add(NetCmd.C2S_ENTER_ROOM, new Action<Player, byte[]>(RoomCmd.EnterRoom));//进入房间
            NetCmdHandles.Add(NetCmd.C2S_EXIT_ROOM, new Action<Player, byte[]>(RoomCmd.ExitRoom));//退出房间

            //快速匹配
            //请求房间列表
            //请求房间内玩家信息
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
