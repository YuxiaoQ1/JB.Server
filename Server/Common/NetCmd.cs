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

}
