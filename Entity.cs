namespace Wit.Net
{
    using System;
    using System.Linq;
    using Wit.Net.Objects;
    using System.Net.Http;
    using Newtonsoft.Json;
    using System.Threading.Tasks;
    public class Entity : WitBase
    {
        internal Entity() { }
        /// <summary>Returns a list of all entities.</summary>
        public async Task<string[]> GetAllAsync()
            => await ProcessResponse<string[]>(await RestClient.GetAsync($"entities{Version}"));

        /// <summary>Creates a new entity.</summary>
        /// <param name="Name">ID or name of the requested entity.</param>
        /// <param name="Description">Short sentence describing this entity.</param>
        public async Task CreateAsync(string Name, string Description)
            => ProcessResponse(await RestClient.PostAsync($"entities{Version}", new StringContent(JsonConvert.SerializeObject
                (new { Id = Name, doc = Description }))));

        /// <summary>Returns all the expressions validated for an entity.</summary>
        /// <param name="Name">ID or name of the requested entity..</param>
        public async Task<EntityValuesObject> GetAsync(string Name)
            => await ProcessResponse<EntityValuesObject>(await RestClient.GetAsync($"entities/{Name}{Version}"));

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="Id">ID or name of the  entity.</param>
        /// <param name="Description">Short sentence describing this entity.</param>
        /// <param name="Lookups">Short sentence describing this entity. Current lookup strategies are: free_text, keywords. You can add both as well.</param>
        /// <param name="Values">Possible values if this is a keyword entity.</param>
        /// <returns></returns>
        public async Task UpdateAsync(string Id, string Description = null, string[] Lookups = null, Values[] Values = null)
        {
            var IsKeyword = Lookups.Any(x => x?.ToLower() == "keywords");
            if (Values.Length != 0 && !IsKeyword) Logger(exception: new Exception("Values are only allowed if lookup method is keywords."));
            ProcessResponse(await RestClient.PutAsync($"entities/{Id}{Version}", new StringContent(JsonConvert.SerializeObject(
              new
              {
                  Id,
                  doc = Description,
                  lookups = Lookups,
                  values = Values
              }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }))));
        }

        /// <summary>Permanently deletes the entity.</summary>
        /// <param name="Id">ID or name of the entity.</param>
        public async Task DeleteAsync(string Id)
            => ProcessResponse(await RestClient.DeleteAsync($"entities/{Id}"));

        /// <summary>Permanently delete the role associated to the entity.</summary>
        /// <param name="EntityId">ID or name of the entity.</param>
        /// <param name="RoleId">ID or name of the role associate to the entity.</param>
        public async Task DeleteRoleAsync(string EntityId, string RoleId)
            => ProcessResponse(await RestClient.DeleteAsync($"entities/{EntityId}:{RoleId}{Version}"));

        /// <summary>Add a possible value into the list of values for the keyword entity.</summary>
        /// <param name="Id">ID or name of the entity</param>
        /// <param name="Value"><see cref="Values"/></param>
        public async Task<KeywordObject> AddValueAsync(string Id, Values Value)
        {
            if (string.IsNullOrWhiteSpace(Value.Value)) Logger(exception: new Exception($"{nameof(Value.Value)} can't be null."));
            return await ProcessResponse<KeywordObject>(
                await RestClient.PostAsync($"entities/{Id}/values{Version}", new StringContent(JsonConvert.SerializeObject(Value))));
        }

        /// <summary>Delete a canonical value from the entity</summary>
        /// <param name="Id">ID or name of the entity</param>
        /// <param name="Value">Id or name of the value.</param>
        public async Task DeleteValueAsync(string Id, string Value)
            => ProcessResponse(await RestClient.DeleteAsync($"entities/{Id}/values/{Value}{Version}"));

        /// <summary>Create a new expression of the canonical value of the keyword entity</summary>
        /// <param name="Id">ID or name of the entity</param>
        /// <param name="Value">Id or name of associated entity value.</param>
        /// <param name="Expression">new expression for the canonical value of the entity. Must be shorter than 256 characters.</param>
        public async Task<KeywordObject> AddExpressionAsync(string Id, string Value, string Expression)
        {
            if (Expression.Length > 256) Logger(exception: new Exception($"{nameof(Expression)} can't be null."));
            return await ProcessResponse<KeywordObject>(
                await RestClient.PostAsync($"entities/{Id}/values/{Value}/expressions{Version}",
                new StringContent(JsonConvert.SerializeObject(new { expression = Expression }))));
        }

        /// <summary>Delete an expression of the canonical value of the entity.</summary>
        /// <param name="Id">ID or name of the entity</param>
        /// <param name="Value">Id or name of associated entity value.</param>
        /// <param name="Expression">new expression for the canonical value of the entity. Must be shorter than 256 characters.</param>
        public async Task DeleteExpressionAsync(string Id, string Value, string Expression)
            => ProcessResponse(await RestClient.DeleteAsync($"entities/{Id}/values/{Value}/expressions/{Expression}"));
    }
}