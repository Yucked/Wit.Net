namespace Wit.Net.Objects
{
    using Newtonsoft.Json;
    public class EntityValuesObject
    {
        [JsonProperty("builtin")]
        public bool Builtin { get; internal set; }
        [JsonProperty("doc")]
        public string Description { get; internal set; }
        [JsonProperty("id")]
        public string Id { get; internal set; }
        [JsonProperty("lang")]
        public string Language { get; internal set; }
        [JsonProperty("lookups")]
        public string[] Lookups { get; internal set; }
        [JsonProperty("name")]
        public string Name { get; internal set; }
        [JsonProperty("values")]
        public Values[] Values { get; internal set; }
    }
    public partial class Values
    {
        [JsonProperty("value")]
        public string Value { get; internal set; }
        [JsonProperty("expressions")]
        public string[] Expressions { get; internal set; }
    }
}