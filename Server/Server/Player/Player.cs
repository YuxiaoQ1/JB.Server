using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Server;
using Common;

namespace Server
{
    class Player
    {
        Socket clientSocket;
        GameServer server;
        MessageHelper msgHelper = new MessageHelper();

        public Player(Socket clientSocket, GameServer server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
        }

        public Socket GetSocket => clientSocket;

        public GameServer GetGameServer => server;

        public void Start()
        {
            if(clientSocket == null)
            {
                LogHelper.ERRORLOG("clientsocket is null!");
            }
            else if(!clientSocket.Connected)
            {
                LogHelper.ERRORLOG("clientsocket disconnected!");
            }
            else
            {
                clientSocket.BeginReceive(msgHelper.Buffer, msgHelper.GetStartIndex, msgHelper.GetRemainBytes, SocketFlags.None, _RecieveCallBack, null);
            }
        }

        private void _RecieveCallBack(IAsyncResult ar)
        {
            try
            {
                if (clientSocket == null)
                {
                    LogHelper.ERRORLOG("clientsocket is null!");
                }
                else if (!clientSocket.Connected)
                {
                    LogHelper.ERRORLOG("clientsocket disconnected!");
                }
                else
                {
                    int count = clientSocket.EndReceive(ar);
                    if (count == 0)
                    {
                        Close();
                        return;
                    }
                    msgHelper.AddCount(count);
                    NetCmdHandle.Dispatch(msgHelper, this);
                    Start();
                }
            }
            catch (Exception e)
            {
                LogHelper.ERRORLOG(e);
                Close();
            }
        }

        public void Close()
        {
            server.RemovePlayer(this);
            //ConnHelper.CloseConnection(mysqlConn);
            if (clientSocket != null)
                clientSocket.Close();
            /*if (room != null)
            {
                room.QuitRoom(this);
            }*/
        }
    }
}
