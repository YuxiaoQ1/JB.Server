using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    static class LogHelper
    {
        public static void DEBUGLOG(string logformat, params object[] args)
        {
            DateTime dateTime = DateTime.Now;
            bool tip = false;
            if(Console.CursorLeft > 0)
            {
                tip = true;
            }
            Console.CursorLeft = 0;
            Console.WriteLine(dateTime.ToString("[yyyy-MM-dd HH:mm:ss]: ") + logformat, args);
            if(tip)
            {
                Console.Write("console >> ");
            }
        }

        public static void ERRORLOG(object arg)
        {
            DateTime dateTime = DateTime.Now;
            bool tip = false;
            if (Console.CursorLeft > 0)
            {
                tip = true;
            }
            Console.CursorLeft = 0;
            ConsoleColor backup = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(dateTime.ToString("[yyyy-MM-dd HH:mm:ss]: ") + arg);
            Console.ForegroundColor = backup;
            if (tip)
            {
                Console.Write("console >> ");
            }
        }
    }
}
