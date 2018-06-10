namespace Wit.Net
{
    using System;
    using System.Linq;
    using Wit.Net.Models;
    using Wit.Net.Objects;
    using System.Threading.Tasks;
    using System.Net.Http;

    public class Application : Base
    {
        internal Application() { }

        /// <summary>
        /// Returns an array of all apps that you own.
        /// </summary>
        /// <param name="Limit">Max number of apps to return.</param>
        /// <param name="Offset">Number of apps to skip.</param>
        public async Task<AppsObject[]> GetAllAsync(int Limit, int Offset = 0)
        {
            if (Limit <= 0 || Limit > 10000)
                Logger.Logging.Send(exception:
                    new Exception($"{nameof(Limit)} must be between 1 and 10000 inclusive (recommended max size is 500)."));
            else if (Offset < 0)
                Logger.Logging.Send(exception:
                    new Exception($"{nameof(Offset)} must be >= 0. Default is 0"));
            var Get = await RestClient.GetAsync($"apps?offset={Offset}&limit={Limit}");
            return await ProcessAsync<AppsObject[]>(Get, $"GET /apps");
        }

        /// <summary>
        /// Creates a new app for an existing user.
        /// </summary>
        /// <param name="Name">Name of the new app.</param>
        /// <param name="Language"></param>
        /// <param name="IsPrivate">Private if “true”</param>
        /// <param name="Description">Short sentence describing your app.</param>
        public async Task<CreationObject> CreateAsync(string Name, Language Language, bool IsPrivate,
            string Description = "My new Wit application VIA Wit.Net!")
        {
            var Content = "{" + string.Format("\"name\": \"{0}\", \"lang\": \"{1}\", \"private\": \"{2}\", \"desc\": \"{3}\"",
                Name, Language, IsPrivate.ToString().ToLower(), Description) + "}";
            var Post = await RestClient.PostAsync($"apps{Version}", new StringContent(Content));
            return await ProcessAsync<CreationObject>(Post, $"POST /apps");
        }

        /// <summary>
        /// Updates an existing application.
        /// </summary>
        /// <param name="Id">The ID of the application.</param>
        /// <param name="Name">Name of the new app.</param>
        /// <param name="Language">Language code</param>
        /// <param name="IsPrivate">Private if true</param>
        /// <param name="Timezone">Default timezone of the app. Must be a canonical ID. Example: “America/Los_Angeles”</param>
        /// <param name="Description">Short sentence describing your app</param>
        public async Task UpdateAsync(string Id, string Name, Language Language, bool? IsPrivate = null,
            string Timezone = null, string Description = "My new Wit application VIA Wit.Net!")
        {
            var Put = await RestClient.PutAsync($"apps/{Id}{Version}", CreateContent(new
            {
                name = Name,
                lang = Language,
                Private = IsPrivate.Value,
                timezone = Timezone,
                desc = Description
            }));
            Process(Put, $"PUT apps/{Id}");
        }

        /// <summary>
        /// Deletes an existing application.
        /// </summary>
        /// <param name="Id">The ID of the application.</param>
        public async Task DeleteAsync(string Id)
            => Process(await RestClient.DeleteAsync($"apps/{Id}{Version}"), $"DELETE /apps/{Id}");
    }
}