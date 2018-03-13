using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public enum NetCmd  //网络数据操作码
    {
        NetCmdMin,

        RAWSTRING               =   0,      //无操作, 字符串
        HEARTBEAT               =   1,      //心跳包，测试
        BROADCAST               =   2,      //广播消息
        C_GS_REGISTER_REQ       =   3,      //注册请求
        C_GS_LOGIN_REQ          =   4,      //登录请求
        C_GS_EXIT_REQ           =   5,      //主动退出请求

        //S2C
        //登录、注册的结果
        S2C_LOGIN_FAILED        =   6,
        S2C_LOGIN_SUCCESS       =   7,
        S2C_REGISTER_FAILED     =   8,      //注册失败:Username已存在, 或连接断开
        S2C_REGISTER_SUCCESS    =   9,
        S2C_CREATE_ROOM_FAILED  =   10,     //创建房间失败


        //C2S
        C2S_EXIT_GAME           =   100,    //玩家退出战斗
        C2S_EXIT_ROOM           =   101,    //玩家退出房间
        C2S_CREATE_ROOM         =   102,    //玩家创建房间
        C2S_ENTER_ROOM          =   103,    //玩家进入房间

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
            this.username = n;
            this.password = p;
        }
        public string username;
        public string password;
    }
}
