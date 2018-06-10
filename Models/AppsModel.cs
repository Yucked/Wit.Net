namespace Wit.Net.Models
{
    using System;
    using Newtonsoft.Json;
    public class AppsModel
    {
        [JsonProperty("name")]
        public string Name { get; internal set; }
        [JsonProperty("id")]
        public string Id { get; internal set; }
        [JsonProperty("description")]
        public string Description { get; internal set; }
        [JsonProperty("lang")]
        public string Language { get; internal set; }
        [JsonProperty("private")]
        public string Private { get; internal set; }
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; internal set; }
    }
}