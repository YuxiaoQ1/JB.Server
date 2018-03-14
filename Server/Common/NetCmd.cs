using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public enum NetCmd  //网络数据操作码
    {
        NetCmdMin,

        RAWSTRING = 0,      //无操作, 字符串
        HEARTBEAT = 1,      //心跳包，测试
        BROADCAST = 2,      //广播消息
        C_GS_REGISTER_REQ = 3,      //注册请求
        C_GS_LOGIN_REQ = 4,      //登录请求
        C_GS_EXIT_REQ = 5,      //主动退出请求

        //S2C
        //登录、注册的结果
        S2C_LOGIN_FAILED = 56,
        S2C_LOGIN_SUCCESS = 57,
        S2C_REGISTER_FAILED = 58,      //注册失败:Username已存在, 或连接断开
        S2C_REGISTER_SUCCESS = 59,
        S2C_CREATE_ROOM_FAILED = 60,     //创建房间失败
        S2C_CREATE_ROOM_SUCCESS = 61,   //成功创建房间
        S2C_ROOM_INFO_RES = 62,     //服务器返回房间信息，用于进入房间同步房间内玩家信息
        S2C_ROOM_LIST_INFO_RES = 63, //服务器返回房间列表信息
        S2C_ENTER_ROOM_SUCCESS = 64,
        S2C_ENTER_ROOM_FAILED = 65,
        S2C_SYNC_ROOM_PLAYER_INFO = 66, //同步房间内玩家信息


        //C2S
        C2S_EXIT_GAME = 200,    //玩家退出战斗
        C2S_EXIT_ROOM = 201,    //玩家退出房间
        C2S_CREATE_ROOM = 202,    //玩家创建房间
        C2S_ENTER_ROOM = 203,       //玩家进入房间
        C2S_ROOM_INFO_REQ = 204,    //客户端请求房间信息
        C2S_ROOM_LIST_INFO_REQ = 205,   //请求房间信息列表

        NetCmdMax
    }

    [Serializable]      //协议结构体，每条协议在这里自定义
    public class ST_RAWSTRING
    {
        public ST_RAWSTRING(string s)
        {
            data = s;
        }
        public string data;
    }
    [Serializable]
    public class ST_BROADCAST
    {
        public ST_BROADCAST(string ID, string data)
        {
            this.ID = ID;
            this.data = data;
        }
        public string ID;
        public string data;
    }
    [Serializable]
    public class ST_LOGIN_REGISTER
    {
        public ST_LOGIN_REGISTER(string n, string p)
        {
            username = n;
            password = p;
        }
        public string username;
        public string password;
    }
    [Serializable]
    public class ST_PLAYER_INFO
    {
        public ST_PLAYER_INFO(string uname, int cc, int dc, int l, int e, int cid)
        {
            Username = uname;
            CoinCounts = cc;
            DiamondCounts = dc;
            Level = l;
            Exp = e;
            ClothId = cid;
        }
        public string Username;
        public int CoinCounts;
        public int DiamondCounts;
        public int Level;
        public int Exp;
        public int ClothId;
    }
    
    /**
     * 房间信息列表：房间ID，房间容量
     * 选择某模式之后，服务器仅返回所有房间的这三个信息,由客户端构造房间列表
     */
    [Serializable]
    public class ST_ROOM_LIST_INFO
    {
        public ST_ROOM_LIST_INFO(UInt64 rid, int curCap, bool isF)
        {
            roomId = rid;
            CurrentCapacity = curCap;
            isFighting = isF;
        }
        public UInt64 roomId;
        public int CurrentCapacity;
        public bool isFighting;
    }

    [Serializable]
    public class ST_ROOM_INFO
    {
        public ST_ROOM_INFO(UInt64 rid, int cap, int cpn, int hon, ST_PLAYER_INFO[] pi)
        {
            roomID = rid;
            Capacity = cap;
            CurrentPlayerNum = cpn;
            HouseOwnerNumber = hon;
            _PLAYER_INFOs = pi;
        }
        public UInt64 roomID;
        public int Capacity;
        public int CurrentPlayerNum;
        public int HouseOwnerNumber;//房主编号
        public ST_PLAYER_INFO[] _PLAYER_INFOs;//房间内玩家信息

    }
}
