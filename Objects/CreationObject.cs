namespace Wit.Net.Objects
{
    using Newtonsoft.Json;
    public class CreationObject
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; internal set; }
        [JsonProperty("app_id")]
        public string AppId { get; internal set; }
    }
}