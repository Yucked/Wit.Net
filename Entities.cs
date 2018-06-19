using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WitSharp.Objects;

namespace WitSharp
{
    public class Entities : Base
    {
        internal Entities()
        {
        }

        /// <summary>Returns a list of all entities.</summary>
        public async Task<IEnumerable<string>> GetAllAsync()
            => await ProcessAsync<IEnumerable<string>>(await RestClient.GetAsync("entities"));

        /// <summary>Creates a new entity.</summary>
        /// <param name="name">ID or name of the requested entity.</param>
        /// <param name="description">Short sentence describing this entity.</param>
        public async Task CreateAsync(string name, string description)
        {
            var get = await RestClient.PostAsync("entities", CreateContent(new
            {
                Id = name,
                doc = description
            }));
            Process(get);
        }

        /// <summary>Returns all the expressions validated for an entity.</summary>
        /// <param name="name">ID or name of the requested entity..</param>
        public async Task<EntityValuesObject> GetAsync(string name)
            => await ProcessAsync<EntityValuesObject>(await RestClient.GetAsync($"entities/{name}"));

        /// <summary>Updates an entity.</summary>
        /// <param name="id">ID or name of the  entity.</param>
        /// <param name="description">Short sentence describing this entity.</param>
        /// <param name="lookups">Short sentence describing this entity. Current lookup strategies are: free_text, keywords. 
        /// You can add both as well.</param>
        /// <param name="values">Possible values if this is a keyword entity.</param>
        public async Task UpdateAsync(string id, string description = null,
            string[] lookups = null, Values[] values = null)
        {
            var isKeyword =
                (lookups ?? throw new ArgumentNullException(nameof(lookups))).Any(x => x?.ToLower() == "keywords");
            if (values != null && !isKeyword)
                throw new Exception($"{nameof(values)} are only allowed if lookup method is keywords.");
            var get = await RestClient.PutAsync($"entities/{id}", CreateContent(
                new
                {
                    Id = id,
                    doc = description,
                    lookups,
                    values
                }));
            Process(get);
        }

        /// <summary>Permanently deletes the entity.</summary>
        /// <param name="id">ID or name of the entity.</param>
        public async Task DeleteAsync(string id)
            => Process(await RestClient.DeleteAsync($"entities/{id}"));

        /// <summary>Permanently delete the role associated to the entity.</summary>
        /// <param name="entityId">ID or name of the entity.</param>
        /// <param name="roleId">ID or name of the role associate to the entity.</param>
        public async Task DeleteRoleAsync(string entityId, string roleId)
            => Process(await RestClient.DeleteAsync($"entities/{entityId}:{roleId}"));

        /// <summary>Add a possible value into the list of values for the keyword entity.</summary>
        /// <param name="id">ID or name of the entity</param>
        /// <param name="value"><see cref="Values"/></param>
        public async Task<KeywordObject> AddValueAsync(string id, Values value)
        {
            if (string.IsNullOrWhiteSpace(value.Value))
                throw new Exception($"{nameof(value.Value)} can't be null.");
            var get = await RestClient.PostAsync($"entities/{id}/values", CreateContent(value));
            return await ProcessAsync<KeywordObject>(get);
        }

        /// <summary>Delete a canonical value from the entity</summary>
        /// <param name="id">ID or name of the entity</param>
        /// <param name="value">Id or name of the value.</param>
        public async Task DeleteValueAsync(string id, string value)
            => Process(await RestClient.DeleteAsync($"entities/{id}/values/{value}"));

        /// <summary>Create a new expression of the canonical value of the keyword entity</summary>
        /// <param name="id">ID or name of the entity</param>
        /// <param name="value">Id or name of associated entity value.</param>
        /// <param name="expression">new expression for the canonical value of the entity. 
        /// Must be shorter than 256 characters.</param>
        public async Task<KeywordObject> AddExpressionAsync(string id, string value, string expression)
        {
            if (expression.Length > 256)
                throw new Exception($"{nameof(expression)}'s length can't be more than 256 characters.");
            var get = await RestClient.PostAsync($"entities/{id}/values/{value}/expressions",
                CreateContent(new {expression}));
            return await ProcessAsync<KeywordObject>(get);
        }

        /// <summary>Delete an expression of the canonical value of the entity.</summary>
        /// <param name="id">ID or name of the entity</param>
        /// <param name="value">Id or name of associated entity value.</param>
        /// <param name="expression">new expression for the canonical value of the entity. Must be shorter than 256 characters.</param>
        public async Task DeleteExpressionAsync(string id, string value, string expression)
            => Process(await RestClient.DeleteAsync($"entities/{id}/values/{value}/expressions/{expression}"));
    }
}