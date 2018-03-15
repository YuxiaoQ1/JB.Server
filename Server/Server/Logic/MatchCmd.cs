using Common;
using Server.ROOM;
using System.Collections.Generic;

namespace Server.Logic
{
    //匹配命令
    class MatchCmd
    {
        //匹配房间
        public static void Match(Player player, byte[] data)
        {
            //玩家战力公式：3*Exp + 3*Level + 4*FightCount
            //获取玩家Exp、Level、FightCount 计算战力 FP1
            //遍历房间中所有房主的Exp、Level、FightCount 
            //
        }

        /**
         * 用于休闲模式下：直接匹配战斗,找得到未满的即可进入战斗，否则开启新战斗
         */
        public static void QuickMatch(Player player, byte[] data)
        {
            List<Fight> fights = player.GetGameServer.GetFightList;
            Fight fight = null;
            bool startNew = true;
            lock(fights)
            {
                foreach (Fight f in fights)
                {
                    if (f.GetPlayers.Count < Fight.Capacity)
                    {
                        fight = f;
                        startNew = false;
                        break;
                    }
                }
                if (fight == null)
                    fight = new Fight(IDmaker.GetNewID());
                fight.AddPlayer(player);
                
                if (startNew)
                {
                    player.GetGameServer.GetFightList.Add(fight);
                    //通知客户端开启新战斗：只发送fightID
                    //客户端收到通知后再通知服务器启动战斗：在FightCmd中
                    player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_START_NEW_FIGHT,
                        MessageHelper.SerializeToBinary(new ST_FIGHT_ID(fight.fightID))));
                    LogHelper.DEBUGLOG("Player [{0}] start new fight [{1}].", player.Username, fight.fightID);
                }
                else
                {
                    //通知客户端进入战斗：发送fight内所有信息给客户端创建物件
                    byte[] fightData = fight.PackFightData();
                    player.GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_ENTER_FIGHT, fightData));
                    LogHelper.DEBUGLOG("Player [{0}] enter fight [{1}].", player.Username, fight.fightID);
                }
                //这里每天同步操作是因为每0.05s
                //同步一次，几乎在玩家进入战斗的
                //同时进行同步操作
            }
        }

        private Room _GetSuitableRoom(Player player)
        {
            List<Room> rooms = player.GetGameServer.GetRoomList;
            if(rooms.Count == 0 || _NoSuitableRoom(rooms))
            {
                Room room = new Room(IDmaker.GetNewID(), player);
            }
            return null;
        }

        private bool _NoSuitableRoom(List<Room> rooms)
        {
            foreach(Room room in rooms)
            {
                if (room.CurrentPlayerNum < room.GetCapacity)
                    return false;
            }
            return true;
        }
    }
}