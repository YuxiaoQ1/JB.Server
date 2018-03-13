using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Server;
using Server.Logic;

namespace Server
{
    enum ServerState
    {
        Uninit      =   0,
        Ready       =   1,
        Running     =   2,
        Stopped     =   3,    
    }

    class GameServer
    {
        private ServerState state = ServerState.Uninit;
        private IPEndPoint IpEndpoint { get; set; }
        private Socket serverSocket;
        private List<Player> onlinePlayerList = new List<Player>();
        private List<Room> roomList = new List<Room>();

        public List<Player> GetonlinePlayers => onlinePlayerList;

        public bool AddRoom(Room room)
        {
            if(!roomList.Contains(room))
            {
                roomList.Add(room);
                return true;
            }
            return false;
        }

        public GameServer() { }
        public GameServer(string ipStr, UInt16 port)
        {
            _SetIPEndPoint(ipStr, port);
            onlinePlayerList.Clear();
        }

        public void Start()
        {
            NetCmdHandle.Init();
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(IpEndpoint);
            serverSocket.Listen(0);
            serverSocket.BeginAccept(_AcceptCallBack, null);
            _SetServerState(ServerState.Running);
        }

        public void Stop()
        {
            //TODO: 暂停
            _SetServerState(ServerState.Stopped);
        }

        public void Run()
        {
            if(state == ServerState.Stopped)
            {
                //TODO: 继续
            }
            else if(state == ServerState.Ready)
            {
                this.Start();
            }
        }

        public void Close()
        {
            if(serverSocket != null)
            {
                serverSocket.Close();
                serverSocket = null;    //GC
            }
            //TODO, do other process
            while(onlinePlayerList.Count > 0)
            {
                onlinePlayerList.Last().Close();    //倒序删除
            }
            LogHelper.DEBUGLOG("GameServer Closed!");
        }

        public void RemovePlayer(Player player)
        {
            LogHelper.DEBUGLOG("player[{0}] disconnected!", player.GetSocket.RemoteEndPoint.ToString());
            lock (onlinePlayerList)
            {
                onlinePlayerList.Remove(player);
            }
        }

        private void _SetIPEndPoint(string ipStr, UInt16 port)
        {
            IpEndpoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
            LogHelper.DEBUGLOG("Set Server IP: {0} Port: {1} Success!", ipStr, port);
            _SetServerState(ServerState.Ready);
        }

        private void _AcceptCallBack(IAsyncResult ar)
        {
            try
            {
                if(serverSocket != null)
                {
                    Socket clientSocket = serverSocket.EndAccept(ar);
                    Player player = new Player(clientSocket, this);
                    player.Start();
                    onlinePlayerList.Add(player);
                    LogHelper.DEBUGLOG("Accept Client:[{0}]", clientSocket.RemoteEndPoint.ToString());
                    serverSocket.BeginAccept(_AcceptCallBack, null);
                }
            }
            catch (Exception e)
            {
                LogHelper.ERRORLOG(e);
            }
        }

        private void _SetServerState(ServerState state)
        {
            this.state = state;
            LogHelper.DEBUGLOG("GameServer "+ state.ToString() + "!");
        }
    }
}
