using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Net.Sockets;

namespace TestClient
{
    class NetCmdHandle
    {
        private static Dictionary<NetCmd, Action<Socket, byte[]>> NetCmdHandles = new Dictionary<NetCmd, Action<Socket, byte[]>>();

        static public void Init()   //在这里注册所有的逻辑处理函数
        {
            NetCmdHandles.Add(NetCmd.RAWSTRING, new Action<Socket, byte[]>(Logic.RawString.OnReiceve));
            NetCmdHandles.Add(NetCmd.BROADCAST, new Action<Socket, byte[]>(Logic.BroadCast.OnReiceve));
            NetCmdHandles.Add(NetCmd.S2C_LOGIN_SUCCESS, new Action<Socket, byte[]>(Logic.LoginSuccess.OnReceive));
        }

        static public void Dispatch(MessageHelper messageHelper, Socket server)
        {
            while (messageHelper.HasMessage())
            {
                messageHelper.ReadMessage(out NetCmd cmd, out byte[] data);
                if (NetCmdHandles.TryGetValue(cmd, out Action<Socket, byte[]> action))
                {
                    action?.Invoke(server, data);
                }
                Console.WriteLine("[{0}]Recieve msg: {1} {2}", server.RemoteEndPoint.ToString(), cmd, data.ToString());
            }
        }
    }
}
