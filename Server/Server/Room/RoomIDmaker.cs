using System;

namespace Server
{
    static class RoomIDmaker
    {
        static UInt64 lastID = 10000;
 
        public static UInt64 GetNewID()
        {
            return lastID += 1;
        }
    }
}
