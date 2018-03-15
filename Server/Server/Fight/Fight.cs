using Common;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Server
{
    //战斗
    class Fight
    {
        public const int Capacity = 10;
        public UInt64 fightID;
        private List<Player> players;
        public List<Player> GetPlayers => players;
        private Timer jobTimer;
        private const double  gameDuration = 10 * 60 * 1000; //游戏时长倒计时 10分钟
        private const double createBossDelay = 2 * 60 * 1000; //生成Boss计时器 2分钟
        private const double createJewelInterval = 30 * 1000; //生成宝石计时器 30s
        private const double syncPlayerInfoInterval = 50; //玩家信息同步计时器 0.05s
        private const double baseInterval = syncPlayerInfoInterval;

        private double startTime = 0; //游戏启动时间
        private double lastNow = 0;
        private double nextTimeCreateJewel;//下次生成宝石的时间
        private bool isBossCreated = false;

        //
        private const int jewelNum = 50;
        //key：编号， value：类型
        Dictionary<int, JewelType> jewelMap = new Dictionary<int, JewelType>();

        public Fight(UInt64 fid)
        {
            fightID = fid;
            players = new List<Player>();
            for (int i = 1; i <= jewelNum; i++)
                jewelMap.Add(i, JewelType.NONE);
        }
        public void Start()
        {
            jobTimer = new Timer
            {
                Interval = baseInterval
            };
            jobTimer.Elapsed += (s, e) => _TimerJob();
            startTime = DateTime.Now.Millisecond; //开始启动时间
            nextTimeCreateJewel = startTime;
            lastNow = startTime;
            jobTimer.Start();
        }

        private void _TimerJob()
        {
            double now = DateTime.Now.Millisecond;
            if (now - startTime >= gameDuration)
            {
                //GameOver
                //1 向玩家广播游戏结束消息
                SimpleSync(NetCmd.S2C_GAME_OVER, new byte[0]);
                //2 结算并广播结果
                jobTimer.Close();
                return;
            }

            //TODO：同步玩家信息

            if(now - lastNow >= 1000)
            {
                //1 向客户端同步剩余时间 按秒计算
                int left = (int)((startTime + gameDuration - now) / 1000);
                
                lastNow += 1000;
            }
            

            if(now - startTime >= createBossDelay && !isBossCreated)
            {
                isBossCreated = true;
                //向客户端同步 通知创建boss
                SimpleSync(NetCmd.S2C_CREATE_BOSS, new byte[0]);
            }

            if(now >= nextTimeCreateJewel)
            {
                nextTimeCreateJewel = now + createJewelInterval;
                //1 CreateJewel
                //2 向玩家广播jewel的位置：将宝石编号同步
            }
        }
        public void AddPlayer(Player player)
        {
            lock (players)
            {
                if (players.Count < Capacity)
                {
                    player.InGame = true;
                    players.Add(player);
                }
            }
        }

        public void RemovePlayer(Player player)
        {
            lock (players)
            {
                if(players.Contains(player))
                {
                    player.InGame = false;
                    players.Remove(player);
                }
            }
        }

        private void SimpleSync(NetCmd cmd, byte[] data)
        {
            foreach (Player player in players)
                player.GetSocket.Send(MessageHelper.PackData(cmd, data));
        }

        //为了方便，就算房间只有一个人，也进行同步
        private void SyncFightingPlayerInfo()
        {
            //同步位置、血量、当前分数,玩家当前动画类型:S2C_SYNC_FIGHT_INFO
            //TODO:同步更多
        }

        public byte[] PackFightData()
        {
            //将战斗内的信息，通过MessageHelper打包成可序列化的数据
            return null;
        }
    }
}