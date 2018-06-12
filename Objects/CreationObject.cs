namespace WitSharp.Objects
{
    using Newtonsoft.Json;
    public class CreationObject
    {
        internal CreationObject() { }
        [JsonProperty("access_token")]
        public string AccessToken { get; internal set; }
        [JsonProperty("app_id")]
        public string AppId { get; internal set; }
    }
}