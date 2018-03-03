using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    static class IDmaker
    {
        static UInt64 lastID = 10000;
 
        static UInt64 getNewID()
        {
            return lastID += 1;
        }
    }
}
