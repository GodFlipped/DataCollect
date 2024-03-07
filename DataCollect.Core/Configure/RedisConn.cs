using CSRedis;

namespace DataCollect.Core.Configure
{
    public sealed class RedisConn
    {
        private static RedisConn instance = null;
        public CSRedisClient rds =null;
        private static readonly object padlock = new object();
        private RedisConn()
        {
            //todu配置文件
             rds = new CSRedisClient("127.0.0.1:6379,defaultDatabase=2,poolsize=500,ssl=false,writeBuffer=10240");
           
        }

        public static RedisConn Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new RedisConn();
                    }
                    return instance;
                }
            }
        }

   
    }
}
