using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Common;

namespace TestClient.Logic
{
    class RawString
    {
        static public void OnReiceve(Socket server, byte[] data)
        {
            ST_RAWSTRING rawString = (ST_RAWSTRING)MessageHelper.DeserializeWithBinary(data);
            Console.WriteLine("From [{0}]: {1}", server.RemoteEndPoint.ToString(), rawString.data);
        }
    }

    class BroadCast
    {
        static public void OnReiceve(Socket server, byte[] data)
        {
            ST_BROADCAST rawString = (ST_BROADCAST)MessageHelper.DeserializeWithBinary(data);
            Console.WriteLine("来自[{0}]的玩家发送了\n\t {1}", rawString.ID, rawString.data);
        }
    }
}
