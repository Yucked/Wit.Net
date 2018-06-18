namespace WitSharp
{
    public class WitClient
    {
        public Entities Entities { get; }
        public Training Training { get; }
        public Application Application { get; }
        internal static string Token { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken">Your applications access token.</param>
        public WitClient(string accessToken)
        {
            Token = accessToken;
            Entities = new Entities();
            Training = new Training();
            Application = new Application();
        }
        internal WitClient() { }
    }
}