using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Server.Logic
{
    class HeartBeats
    {
        static public void OnReiceve(Player player, byte[] data)
        {
            LogHelper.DEBUGLOG("onReiceve Heart Beats From [{0}]!", player.GetSocket.RemoteEndPoint.ToString());
            player.GetSocket.Send(MessageHelper.PackData(NetCmd.RAWSTRING, MessageHelper.SerializeToBinary(new ST_RAWSTRING("HeartBeat Ack!"))));
        }
    }
}
