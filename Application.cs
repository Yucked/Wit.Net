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
                throw new ArgumentOutOfRangeException(nameof(limit),
                    "Value must be between 1 and 10000 inclusive (recommended max size is 500).");
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "Must be >= 0. Default is 0");
            var get = await RestClient.GetAsync($"apps?offset={offset}&limit={limit}");
            return await ProcessAsync<AppsObject[]>(get);
        }

        /// <summary>
        /// Creates a new app for an existing user.
        /// </summary>
        /// <param name="applicationInfo"><see cref="applicationInfo"/></param>
        public async Task<CreationObject> CreateAsync(AppModel applicationInfo)
        {
            if (applicationInfo == null)
                throw new ArgumentNullException(nameof(applicationInfo));
            var post = await RestClient.PostAsync("apps", CreateContent(applicationInfo));
            return await ProcessAsync<CreationObject>(post);
        }

        /// <summary>
        /// Updates an existing application.
        /// </summary>
        /// <param name="id">The ID of the application.</param>
        /// <param name="applicationInfo"><see cref="applicationInfo"/></param>
        public async Task UpdateAsync(string id, AppModel applicationInfo)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));
            if (applicationInfo == null)
                throw new ArgumentNullException(nameof(applicationInfo));
            Process(await RestClient.PutAsync($"apps/{id}", CreateContent(applicationInfo)));
        }

        /// <summary>
        /// Deletes an existing application.
        /// </summary>
        /// <param name="id">The ID of the application.</param>
        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));
            Process(await RestClient.DeleteAsync($"apps/{id}"));
        }
    }
}