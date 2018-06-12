namespace WitSharp.Objects
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class KeywordObject
    {
        internal KeywordObject() { }
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
        public List<string> Lookups { get; internal set; }
        [JsonProperty("name")]
        public string Name { get; internal set; }
    }
}