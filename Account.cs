namespace Wit.Net
{
    using System;
    using System.Linq;
    using Wit.Net.Objects;
    using System.Threading.Tasks;
    public class Account : Base
    {
        internal Account() { }

        /// <summary>
        /// Returns an array of all apps that you own.
        /// </summary>
        /// <param name="Limit">Max number of apps to return.</param>
        /// <param name="Offset">Number of apps to skip.</param>
        public async Task<AppsModel[]> GetAppsAsync(int Limit, int Offset = 0)
        {
            if (Limit <= 0 || Limit > 10000)
                Logger.Logging.Send(exception:
                    new Exception($"{nameof(Limit)} must be between 1 and 10000 inclusive (recommended max size is 500)."));
            else if (Offset < 0)
                Logger.Logging.Send(exception:
                    new Exception($"{nameof(Offset)} must be >= 0. Default is 0"));
            var Get = await RestClient.GetAsync($"apps?offset={Offset}&limit={Limit}");
            return await ProcessAsync<AppsModel[]>(Get, $"GET /apps");
        }

        /// <summary>
        /// Creates a new app for an existing user.
        /// </summary>
        /// <param name="Name">Name of the new app.</param>
        /// <param name="IsPrivate">Private if “true”</param>
        /// <param name="Description">Short sentence describing your app.</param>
        public async Task CreateAppAsync(string Name, bool IsPrivate,
            string Description = "My new Wit application VIA Wit.Net!")
        {

        }
    }
}