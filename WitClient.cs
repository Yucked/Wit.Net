namespace Wit.Net
{
    public class WitClient
    {
        public Account Account { get; }
        public Entities Entities { get; }
        public Training Training { get; }
        public Logger Logger => Logger.Logging;
        internal static Config Config { get; set; }
        public WitClient(Config config)
        {
            Config = config;
            Account = new Account();
            Entities = new Entities();
            Training = new Training();
        }
        internal WitClient() { }
    }
}