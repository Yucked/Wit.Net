using System;
using System.Threading.Tasks;
using WitSharp.Models;
using WitSharp.Objects;

namespace WitSharp
{
    public class Application : Base
    {
        internal Application()
        {
        }

        /// <summary>
        /// Returns an array of all apps that you own.
        /// </summary>
        /// <param name="limit">Max number of apps to return.</param>
        /// <param name="offset">Number of apps to skip.</param>
        public async Task<AppsObject[]> GetAllAsync(int limit, int offset = 0)
        {
            if (limit <= 0 || limit > 10000)
                throw new Exception(
                    $"{nameof(limit)} must be between 1 and 10000 inclusive (recommended max size is 500).");
            if (offset < 0)
                throw new Exception($"{nameof(offset)} must be >= 0. Default is 0");
            var get = await RestClient.GetAsync($"apps?offset={offset}&limit={limit}");
            return await ProcessAsync<AppsObject[]>(get);
        }

        /// <summary>
        /// Creates a new app for an existing user.
        /// </summary>
        /// <param name="name">Name of the new app.</param>
        /// <param name="lang"></param>
        /// <param name="isPrivate">Private if “true”</param>
        /// <param name="description">Short sentence describing your app.</param>
        public async Task<CreationObject> CreateAsync(string name, Language lang, bool isPrivate,
            string description = "My new Wit application VIA WitSharp!")
        {
            var data = new AppModel
            {
                Description = description,
                IsPrivate = isPrivate,
                Language = lang.ToString(),
                Name = name
            };
            var post = await RestClient.PostAsync("apps", CreateContent(data));
            return await ProcessAsync<CreationObject>(post);
        }

        /// <summary>
        /// Updates an existing application.
        /// </summary>
        /// <param name="id">The ID of the application.</param>
        /// <param name="name">Name of the new app.</param>
        /// <param name="lang">Language code</param>
        /// <param name="isPrivate">Private if true</param>
        /// <param name="timezone">Default timezone of the app. Must be a canonical ID. Example: “America/Los_Angeles”</param>
        /// <param name="description">Short sentence describing your app</param>
        public async Task UpdateAsync(string id, string name = null, Language? lang = Language.En,
            bool? isPrivate = null,
            string timezone = null, string description = "My new Wit application VIA WitSharp!")
        {
            if (lang != null)
            {
                var put = await RestClient.PutAsync($"apps/{id}", CreateContent(new AppModel
                {
                    Name = name,
                    Timezone = timezone,
                    Description = description,
                    IsPrivate = isPrivate != null && isPrivate.Value,
                    Language = lang.Value.ToString()
                }));
                Process(put);
            }
        }

        /// <summary>
        /// Deletes an existing application.
        /// </summary>
        /// <param name="id">The ID of the application.</param>
        public async Task DeleteAsync(string id)
            => Process(await RestClient.DeleteAsync($"apps/{id}"));
    }
}