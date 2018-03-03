using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Server.Logic
{
    class RawString
    {
        static public void OnReiceve(Player player, byte[] data)
        {
            ST_RAWSTRING rawString = (ST_RAWSTRING)MessageHelper.DeserializeWithBinary(data);
            LogHelper.DEBUGLOG("From [{0}]: {1}", player.GetSocket.RemoteEndPoint.ToString(), rawString.data);
        }
    }
}
