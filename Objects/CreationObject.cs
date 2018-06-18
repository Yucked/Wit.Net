using Newtonsoft.Json;

namespace WitSharp.Objects
{
    public class CreationObject
    {
        internal CreationObject() { }
        [JsonProperty("access_token")]
        public string AccessToken { get; internal set; }
        [JsonProperty("app_id")]
        public string AppId { get; internal set; }
    }
}