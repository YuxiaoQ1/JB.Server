using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Server.ROOM
{
    //目前暂时只有休闲模式下的房间
    class RoomCmd
    {
        public static void CreateRoom(Player player, byte[] data)
        {
            Room room = new Room(IDmaker.GetNewID(), player);

            if(player.GetGameServer.AddRoomToRelax(room))
            {
                player.roomId = room.RoomId;
                //将房间信息发送到客户端
                ST_PLAYER_BASE_INFO[] sT_PLAYER_INFOs = new ST_PLAYER_BASE_INFO[1];//房间内所有玩家信息，目前只有房主
                sT_PLAYER_INFOs[0] = new ST_PLAYER_BASE_INFO(player.Username, player.CoinCounts, player.DiamondCounts, player.Level,
                    player.Exp, player.ClothId);
                ST_ROOM_INFO _ROOM_INFO = new ST_ROOM_INFO(room.RoomId, room.GetCapacity, room.CurrentPlayerNum, room.HouseOwnerNumber, sT_PLAYER_INFOs);
                player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_CREATE_ROOM_SUCCESS, MessageHelper.SerializeToBinary(_ROOM_INFO)));
                LogHelper.DEBUGLOG("Player [{0}] create room [{1}] success!", player.Username, room.RoomId);
                return;
            }
            player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_CREATE_ROOM_FAILED, new byte[0]));
            LogHelper.DEBUGLOG("Player [{0}] create room failed!", player.Username);
        }

        public static void EnterRoom(Player player, byte[] data)
        {
            //从展示的房间列表选择一个进入:需要客户端传递roomID, 从ST_ROOM_LIST_INFO中获取
            //需要给玩家展示此时房间内其他玩家的信息
            ST_ROOM_LIST_INFO roomInfo = (ST_ROOM_LIST_INFO)MessageHelper.DeserializeWithBinary(data);
            Room room = player.GetGameServer.GetRoomByRoomID(roomInfo.roomId);
            if(room == null)
            {
                player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_ENTER_ROOM_FAILED, new byte[0]));
                LogHelper.DEBUGLOG("Room [{0}] not exist!", roomInfo.roomId);
                return;
            }
            lock(room)
            {
                if(room.CurrentPlayerNum == room.GetCapacity)
                {
                    player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_ENTER_ROOM_FAILED, new byte[0]));
                    LogHelper.DEBUGLOG("Room [{0}] is full!", roomInfo.roomId);
                    return;
                }
                else
                {
                    room.AddPlayer(player);
                    player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_ENTER_ROOM_SUCCESS, new byte[0]));
                    //将房间信息同步给当前房间内所有玩家
                    room.SyncRoomInfoToAllPlayer();
                }
            }
        }

        public static void ExitRoom(Player player, byte[] data)
        {
            ST_ROOM_LIST_INFO roomInfo = (ST_ROOM_LIST_INFO)MessageHelper.DeserializeWithBinary(data);
            Room room = player.GetGameServer.GetRoomByRoomID(roomInfo.roomId);
            if(room != null)
            {
                lock(room)
                {
                    room.RemovePlayer(player);
                    if (room.players[room.HouseOwnerNumber] == player)
                    {
                        if(room.CurrentPlayerNum == 0)
                        {
                            room.Destory();
                            player.GetGameServer.RemoveRoomByRoomID(room.RoomId);
                            return;
                        }
                        //寻找下一个房主
                        room.ChangeHouseOwner();
                    }
                    room.SyncRoomInfoToAllPlayer();
                }
            }
        }
    }
}
