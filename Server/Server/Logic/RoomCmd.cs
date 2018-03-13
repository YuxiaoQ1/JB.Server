using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Server.Logic
{
    class RoomCmd
    {
        public static void CreateRoom(Player player, byte[] data)
        {
            Room room = new Room
            {
                RoomId = RoomIDmaker.GetNewID()
                //TODO:初始化更多房间信息
            };
            if(player.GetGameServer.AddRoom(room))
            {
                //TODO:将房间信息发送到客户端
                return;
            }
            player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_CREATE_ROOM_FAILED, new byte[0]));
        }

        public static void EnterRoom(Player player, byte[] data)
        {

        }

        public static void ExitRoom(Player player, byte[] data)
        {

        }

        public static void DestoryRoom(Player player, byte[] data)
        {

        }

    }
}
