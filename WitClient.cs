using System;

namespace WitSharp
{
    public class WitClient
    {
        public Entities Entities { get; }
        public Training Training { get; }
        public Application Application { get; }
        internal static string Token { get; private set; }

        /// <summary>
        /// Initialize a new instance of WitClient. Inject it.
        /// </summary>
        /// <param name="accessToken">Your application's access token.</param>
        /// <exception cref="ArgumentNullException"><param name="accessToken"> is <c>null</c></param></exception>
        public WitClient(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ArgumentNullException(nameof(accessToken), "Application's access token is required.");
            Token = accessToken;
            Entities = new Entities();
            Training = new Training();
            Application = new Application();
        }

        internal WitClient()
        {
        }
    }
}