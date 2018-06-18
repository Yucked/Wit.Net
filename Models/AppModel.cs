using Newtonsoft.Json;

namespace WitSharp.Models
{
    internal class AppModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("private")]
        public bool IsPrivate { get; set; }
        [JsonProperty("lang")]
        public string Language { get; set; }
        [JsonProperty("desc")]
        public string Description { get; set; }
        [JsonProperty("timezone")]
        public string Timezone { get; set; }
    }
}