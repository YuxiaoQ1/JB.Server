using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Server.Logic
{
    class Broadcast
    {
        static public void OnReiceve(Player player, byte[] data)
        {
            ST_BROADCAST rawString = (ST_BROADCAST)MessageHelper.DeserializeWithBinary(data);
            rawString.ID = player.GetSocket.RemoteEndPoint.ToString();
            LogHelper.DEBUGLOG("来自[{0}]的玩家发送了\n\t {1}", rawString.ID, rawString.data);
            foreach(var i in player.GetGameServer.GetonlinePlayers)
            {
                if(i != player)
                {
                    i.GetSocket.Send(MessageHelper.PackData(NetCmd.BROADCAST, MessageHelper.SerializeToBinary(rawString)));
                }
            }
        }
    }
}
