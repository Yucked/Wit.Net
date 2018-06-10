namespace Wit.Net.Objects
{
    using System;
    using Newtonsoft.Json;
    public class AppsModel
    {
        /// <summary>The name of your app.</summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }
        /// <summary>A unique uuid that identifies your app.</summary>
        [JsonProperty("id")]
        public string Id { get; internal set; }
        /// <summary>The description of your app.</summary>
        [JsonProperty("description")]
        public string Description { get; internal set; }
        /// <summary>Language code.</summary>
        [JsonProperty("lang")]
        public string Language { get; internal set; }
        /// <summary>Private if true.</summary>
        [JsonProperty("private")]
        public string Private { get; internal set; }
        /// <summary>Datetime of when your app was created.</summary>
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; internal set; }
    }
}