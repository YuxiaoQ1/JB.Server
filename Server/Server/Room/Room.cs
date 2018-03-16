using Common;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Server.ROOM
{
    class Room
    {
        private const int capacity = 10;//房间容量
        public int GetCapacity => capacity;
        private UInt64 roomId;//房间ID
        public UInt64 RoomId { get; set; }
        private Player houseOwner;//房主
        public Player HouseOwner { get; set; }
        public int HouseOwnerNumber = 0;//房主编号，初始为0
        public Player[] players;
        public int CurrentPlayerNum = 0;//当前房间人数
        public bool isFighting = false; //当前房间是否在战斗中,false表示准备中，true为战斗中

        

        public Room(UInt64 rid, Player houseowner)
        {
            players = new Player[capacity];
            for (int i = 0; i < capacity; i++)
                players[i] = null;

            RoomId = rid;
            HouseOwner = houseowner;
            HouseOwnerNumber = 0;
            CurrentPlayerNum = 1;
        }

        public void RemovePlayer(Player player)
        {
            for (int i = 0; i < capacity; i++)
            {
                if (players[i] == player)
                {
                    players[i] = null;
                    player.InRoom = false;
                    CurrentPlayerNum--;
                    break;
                }
            }
        }

        public void AddPlayer(Player player)
        {
            for (int i = 0; i < capacity; i++)
            {
                if (players[i] == null)
                {
                    players[i] = player;//将当前玩家加入房间
                    player.InRoom = true;
                    player.roomId = RoomId;
                    CurrentPlayerNum++;
                    break;
                }
            }
        }

        public void SyncRoomInfoToAllPlayer()
        {
            ST_PLAYER_BASE_INFO[] sT_PLAYER_INFOs = new ST_PLAYER_BASE_INFO[capacity];
            for (int i = 0; i < capacity; i++)
            {
                if (players[i] == null)
                {
                    sT_PLAYER_INFOs[i] = null;
                    continue;
                }
                sT_PLAYER_INFOs[i] = new ST_PLAYER_BASE_INFO(players[i].Username,
                    players[i].CoinCounts, players[i].DiamondCounts, players[i].Level,
                    players[i].Exp, players[i].ClothId);
            }
            for (int i = 0; i < capacity; i++)
            {
                if (players[i] != null)
                    players[i].GetSocket.Send(MessageHelper.PackData(NetCmd.S2C_SYNC_ROOM_PLAYER_INFO, 
                        MessageHelper.SerializeToBinary(sT_PLAYER_INFOs)));
            }
        }

        public void Destory()
        {
            for (int i = 0; i < capacity; i++)
            {
                if (players[i] != null)
                {
                    players[i].InRoom = false;
                    players[i] = null;
                }
            }
            players = null;
        }

        public void ChangeHouseOwner()
        {
            for (int i = 0; i < capacity; i++)
            {
                if (players[i] != null)
                {
                    HouseOwnerNumber = i;
                    break;
                }
            }
        }
    }
}