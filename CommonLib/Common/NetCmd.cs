using System;

namespace Common
{
    public enum NetCmd
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
        S2C_START_NEW_FIGHT = 67,
        S2C_ENTER_FIGHT = 68,
        S2C_SYNC_FIGHT_INFO = 69,   //同步战斗内玩家信息
        S2C_GAME_OVER = 70, //向客户端广播游戏结束
        S2C_CREATE_BOSS = 71,
        S2C_SYNC_REMAINING_TIME = 72, //同步战斗剩余时间
        S2C_CREATE_JEWEL = 73,

        //C2S
        C2S_EXIT_GAME = 200,    //玩家退出战斗
        C2S_EXIT_ROOM = 201,    //玩家退出房间
        C2S_CREATE_ROOM = 202,    //玩家创建房间
        C2S_ENTER_ROOM = 203,       //玩家进入房间
        C2S_ROOM_INFO_REQ = 204,    //客户端请求房间信息
        C2S_ROOM_LIST_INFO_REQ = 205,   //请求房间信息列表
        C2S_QUICK_MATCH = 206, //休闲模式下使用
        C2S_START_FIGHT = 207,  //玩家发起战斗 ，创建战斗时

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
    //全服广播的字符串消息：可能用于聊天大喇叭
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
    //通用物件三维坐标，z轴默认为0
    [Serializable]
    public class ST_POSITION
    {
        public ST_POSITION(int px, int py)
        {
            Px = px;
            Py = py;
            Pz = 0;
        }
        public int Px;
        public int Py;
        public int Pz;
    }
    //用于登录、注册传递用户名和密码
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
    //玩家基础信息
    public class ST_PLAYER_BASE_INFO
    {
        public ST_PLAYER_BASE_INFO(string uname, int cc, int dc, int l, int e, int cid)
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
     * 房间列表信息：房间ID，房间容量，是否在战斗
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
    //房间内部信息：包括所有玩家信息
    [Serializable]
    public class ST_ROOM_INFO
    {
        public ST_ROOM_INFO(UInt64 rid, int cap, int cpn, int hon, ST_PLAYER_BASE_INFO[] pi)
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
        public ST_PLAYER_BASE_INFO[] _PLAYER_INFOs;//房间内玩家信息

    }
    //战斗ID
    [Serializable]
    public class ST_FIGHT_ID
    {
        public ST_FIGHT_ID(UInt64 fid)
        {
            FigthID = fid;
        }
        public UInt64 FigthID;
    }
    //玩家战斗内信息
    [Serializable]
    public class ST_PLAYER_FIGHT_INFO
    {
        public ST_PLAYER_FIGHT_INFO(string username, ST_POSITION pos, int hp, int score, int animationType, ST_POSITION skillPos)
        {
            Username = username;
            Position = pos;
            Hp = hp;
            Score = score;
            AnimationType = animationType;
            SkillPosition = skillPos;
        }
        public string Username;
        public ST_POSITION Position;
        public int Hp;
        public int Score;
        public int AnimationType; //技能动画、移动动画，表示技能动画时和玩家技能类型一致
        public ST_POSITION SkillPosition; //结合玩家pos可以绘制出技能
    }
    //宝石信息
    [Serializable]
    public class ST_JEWEL_INFO
    {
        public ST_JEWEL_INFO(int jewelNum, JewelType jewelType)
        {
            Number = jewelNum;
            Type = jewelType;
        }
        public int Number;
        public JewelType Type;
    }
    //Boss信息
    [Serializable]
    public class ST_BOSS_INFO
    {
        public ST_BOSS_INFO(int hp)
        {
            Hp = hp;
        }
        int Hp; //boss血量
    }
    //战斗Fight信息
    [Serializable]
    public class ST_FIGHT_INFO
    {
        public ST_FIGHT_INFO(UInt64 fid, ST_BOSS_INFO bd, ST_PLAYER_FIGHT_INFO[] pd, ST_JEWEL_INFO[] jd)
        {
            FightID = fid;
            BossData = bd;
            PlayerData = pd;
            JewelData = jd;
        }
        public UInt64 FightID;
        public ST_BOSS_INFO BossData;
        public ST_PLAYER_FIGHT_INFO[] PlayerData;
        public ST_JEWEL_INFO[] JewelData;
    }
    //玩家战绩
    [Serializable]
    public class ST_PLAYER_RECORD
    {
        public ST_PLAYER_RECORD(string username, int score)
        {
            Username = username;
            Score = score;
        }
        public string Username;
        public int Score;
    }
    //战斗结束结果
    [Serializable]
    public class ST_FIGHT_OVER_RESULT
    {
        public ST_FIGHT_OVER_RESULT(ST_PLAYER_RECORD[] records)
        {
            PlayerRecords = records;
        }
        public ST_PLAYER_RECORD[] PlayerRecords;
    }
    //剩余时间
    [Serializable]
    public class ST_REMAINING_TIME
    {
        public ST_REMAINING_TIME(int remaining)
        {
            RemainingTime = remaining;
        }
        public int RemainingTime;
    }

}
