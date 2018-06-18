namespace WitSharp
{
    public class WitClient
    {
        public Entities Entities { get; }
        public Training Training { get; }
        public Application Application { get; }
        internal static Config Config { get; private set; }
        public WitClient(Config config)
        {
            Config = config;
            Entities = new Entities();
            Training = new Training();
            Application = new Application();
        }
        internal WitClient() { }
    }
}