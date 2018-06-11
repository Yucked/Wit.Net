namespace Wit.Net
{
    using System;
    using System.Linq;
    using Wit.Net.Objects;
    using System.Threading.Tasks;
    public class Entities : Base
    {
        internal Entities() { }
        /// <summary>Returns a list of all entities.</summary>
        public async Task<string[]> GetAllAsync()
        {
            var Get = await RestClient.GetAsync($"entities");
            return await ProcessAsync<string[]>(Get, $"GET /entities");
        }

        /// <summary>Creates a new entity.</summary>
        /// <param name="Name">ID or name of the requested entity.</param>
        /// <param name="Description">Short sentence describing this entity.</param>
        public async Task CreateAsync(string Name, string Description)
        {
            var Get = await RestClient.PostAsync($"entities", CreateContent(new
            {
                Id = Name,
                doc = Description
            }));
            Process(Get, $"POST /entities");
        }

        /// <summary>Returns all the expressions validated for an entity.</summary>
        /// <param name="Name">ID or name of the requested entity..</param>
        public async Task<EntityValuesObject> GetAsync(string Name)
        {
            var Get = await RestClient.GetAsync($"entities/{Name}");
            return await ProcessAsync<EntityValuesObject>(Get, $"GET /entities/{Name}");
        }

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="Id">ID or name of the  entity.</param>
        /// <param name="Description">Short sentence describing this entity.</param>
        /// <param name="Lookups">Short sentence describing this entity. Current lookup strategies are: free_text, keywords. 
        /// You can add both as well.</param>
        /// <param name="Values">Possible values if this is a keyword entity.</param>
        /// <returns></returns>
        public async Task UpdateAsync(string Id, string Description = null, string[] Lookups = null, Values[] Values = null)
        {
            var IsKeyword = Lookups.Any(x => x?.ToLower() == "keywords");
            if (Values.Length != 0 && !IsKeyword)
                Logger.Logging.Send(exception: new Exception("Values are only allowed if lookup method is keywords."));
            var Get = await RestClient.PutAsync($"entities/{Id}", CreateContent(
              new
              {
                  Id,
                  doc = Description,
                  lookups = Lookups,
                  values = Values
              }));
            Process(Get, $"PUT /entities/{Id}");
        }

        /// <summary>Permanently deletes the entity.</summary>
        /// <param name="Id">ID or name of the entity.</param>
        public async Task DeleteAsync(string Id)
            => Process(await RestClient.DeleteAsync($"entities/{Id}"), $"DELETE /entities/{Id}");

        /// <summary>Permanently delete the role associated to the entity.</summary>
        /// <param name="EntityId">ID or name of the entity.</param>
        /// <param name="RoleId">ID or name of the role associate to the entity.</param>
        public async Task DeleteRoleAsync(string EntityId, string RoleId)
            => Process(await RestClient.DeleteAsync($"entities/{EntityId}:{RoleId}"),
                $"DELETE /entities/{EntityId}:{RoleId}");

        /// <summary>Add a possible value into the list of values for the keyword entity.</summary>
        /// <param name="Id">ID or name of the entity</param>
        /// <param name="Value"><see cref="Values"/></param>
        public async Task<KeywordObject> AddValueAsync(string Id, Values Value)
        {
            if (string.IsNullOrWhiteSpace(Value.Value))
                Logger.Logging.Send(exception: new Exception($"{nameof(Value.Value)} can't be null."));
            var Get = await RestClient.PostAsync($"entities/{Id}/values", CreateContent(Value));
            return await ProcessAsync<KeywordObject>(Get, $"POST /entities/{Id}/values");
        }

        /// <summary>Delete a canonical value from the entity</summary>
        /// <param name="Id">ID or name of the entity</param>
        /// <param name="Value">Id or name of the value.</param>
        public async Task DeleteValueAsync(string Id, string Value)
            => Process(await RestClient.DeleteAsync($"entities/{Id}/values/{Value}"),
                $"DELETE /entities/{Id}/values/{Value}");

        /// <summary>Create a new expression of the canonical value of the keyword entity</summary>
        /// <param name="Id">ID or name of the entity</param>
        /// <param name="Value">Id or name of associated entity value.</param>
        /// <param name="Expression">new expression for the canonical value of the entity. 
        /// Must be shorter than 256 characters.</param>
        public async Task<KeywordObject> AddExpressionAsync(string Id, string Value, string Expression)
        {
            if (Expression.Length > 256)
                Logger.Logging.Send(exception: new Exception($"{nameof(Expression)} can't be null."));
            var Get = await RestClient.PostAsync($"entities/{Id}/values/{Value}/expressions",
                CreateContent(new { expression = Expression }));
            return await ProcessAsync<KeywordObject>(Get, $"POST /entities/{Id}/values/{Value}/expressions");
        }

        /// <summary>Delete an expression of the canonical value of the entity.</summary>
        /// <param name="Id">ID or name of the entity</param>
        /// <param name="Value">Id or name of associated entity value.</param>
        /// <param name="Expression">new expression for the canonical value of the entity. Must be shorter than 256 characters.</param>
        public async Task DeleteExpressionAsync(string Id, string Value, string Expression)
            => Process(await RestClient.DeleteAsync($"entities/{Id}/values/{Value}/expressions/{Expression}"),
                $"DELETE /entities/{Id}/values/{Value}/expressions");
    }
}