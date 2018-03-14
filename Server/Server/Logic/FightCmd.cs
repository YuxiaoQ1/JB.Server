namespace Server.Logic
{
    class FightCmd
    {
        public static void Match(Player player, byte[] data)
        {
            //玩家战力公式：3*Exp + 3*Level + 4*FightCount
            //获取玩家Exp、Level、FightCount 计算战力 FP1
            //遍历房间中所有房主的Exp、Level、FightCount 
            //
        }

        /**
         * 用于休闲模式下：点击休闲模式，立刻进行匹配，如果当前没有合适的房间就创建新房间
         */
        public static void QuickMatch(Player player, byte[] data)
        {

        }
    }
}