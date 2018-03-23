using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Common;
using System.Timers;

namespace TestClient
{
    class Program
    {
        static private Socket serverSocket;
        static private MessageHelper messageHelper = new MessageHelper();
        static bool run = true;
        static void Main(string[] args)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 52077));

            NetCmdHandle.Init();
            serverSocket.BeginReceive(messageHelper.Buffer, messageHelper.GetStartIndex, messageHelper.GetRemainBytes, SocketFlags.None, _OnRecieveData, null);
            _StartHeartBeats();

            //注册测试 done
            //ST_LOGIN_REGISTER sT_REGISTER = new ST_LOGIN_REGISTER("lisi", "123456");
            //serverSocket.Send(MessageHelper.PackData(NetCmd.C_GS_REGISTER_REQ, MessageHelper.SerializeToBinary(sT_REGISTER)));
            //登录测试 done
            ST_LOGIN_REGISTER sT_LOGIN = new ST_LOGIN_REGISTER("zhangsan", "123456");
            serverSocket.Send(MessageHelper.PackData(NetCmd.C_GS_LOGIN_REQ, MessageHelper.SerializeToBinary(sT_LOGIN)));
            //匹配测试




            string s;
            while((s = Console.ReadLine()) != string.Empty && run)
            {
                byte[] data = MessageHelper.SerializeToBinary(new ST_BROADCAST(string.Empty, s));
                serverSocket.Send(MessageHelper.PackData(NetCmd.BROADCAST, data));
            }

            Console.ReadKey();
            serverSocket.Close();
        }

        static private Timer timer;
        static private void _StartHeartBeats()
        {
            timer = new Timer
            {
                Interval = 30000
            };
            timer.Elapsed += (s, e)=> { _OnSendHeartBeats(); };
            timer.Start();
        }

        static private void _OnSendHeartBeats()
        {
            if(serverSocket != null && serverSocket.Connected)
            {
                serverSocket.Send(MessageHelper.PackData(NetCmd.HEARTBEAT, new byte[0]));
            }
            else
            {
                timer.Close();
                timer = null;
            }
        }

        static private void _OnRecieveData(IAsyncResult ar)
        {
            try
            {
                run = false;
                if (serverSocket == null || serverSocket.Connected == false) return;
                int count = serverSocket.EndReceive(ar);
                if (count == 0)
                {
                    serverSocket.Close();
                    return;
                }
                messageHelper.AddCount(count);
                NetCmdHandle.Dispatch(messageHelper, serverSocket);
                serverSocket.BeginReceive(messageHelper.Buffer, messageHelper.GetStartIndex, messageHelper.GetRemainBytes, SocketFlags.None, _OnRecieveData, null);
                run = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
