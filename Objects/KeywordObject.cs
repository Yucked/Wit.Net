namespace Wit.Net.Objects
{
    using Newtonsoft.Json;
    public class KeywordObject
    {
        [JsonProperty("builtin")]
        public bool Builtin { get; internal set; }
        [JsonProperty("doc")]
        public string Description { get; internal set; }
        [JsonProperty("exotic")]
        public bool Exotic { get; internal set; }
        [JsonProperty("id")]
        public string Id { get; internal set; }
        [JsonProperty("lang")]
        public string Language { get; internal set; }
        [JsonProperty("lookups")]
        public string[] Lookups { get; internal set; }
        [JsonProperty("name")]
        public string Name { get; internal set; }
    }
}