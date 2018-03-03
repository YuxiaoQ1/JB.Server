using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using Server;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServer gameServer = new GameServer("127.0.0.1", 52077);
            gameServer.Start();

            bool run = true;
            while(run)
            {
                Console.Write("console >> ");
                DoCommandProcess(Console.ReadLine(), out run, gameServer);
            }
            Console.ReadKey();
        }

        private static void DoCommandProcess(string cmd, out bool run, params object[] args)    //处理自定义控制台调试命令
        {
            run = true;
            if(Regex.IsMatch(cmd, "help"))
            {
                Console.WriteLine("[\"exit\"]:\t\tstop server and exit!");
                Console.WriteLine("[\"show -p\"]:\t\tshow online playerinfos!");
            }
            else if (Regex.IsMatch(cmd, "(exit)|(EXIT)"))
            {
                GameServer s = args[0] as GameServer;
                s.Close();
                run = false;
            }
            else if(Regex.IsMatch(cmd, "show -p"))
            {
                GameServer s = args[0] as GameServer;
                foreach(var p in s.GetonlinePlayers)
                {
                    if(p != null)
                    {
                        Console.WriteLine(p.GetSocket.RemoteEndPoint.ToString());
                    }
                }
            }
        }
    }
}
