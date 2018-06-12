namespace Wit.Net.Models
{
    using Newtonsoft.Json;
    public class EntityModel
    {
        internal EntityModel() { }
        /// <summary>
        /// The entity name. This can be the name of an entity you created, or a builtin entity. i.e food or wit$on_off<para/>
        /// Entity must have been created previously. See: <see cref="Entities.CreateAsync(string, string)"./>
        /// </summary>
        [JsonProperty("entity")]
        public string Entity { get; set; }
        /// <summary>
        /// Canonical value of the entity or the text span.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
        /// <summary>
        /// If your entity appears in the text, the starting index within the text
        /// </summary>
        [JsonProperty("start")]
        public int? Start { get; set; }
        /// <summary>
        /// If your entity appears in the text, the ending index within the text
        /// </summary>
        [JsonProperty("end")]
        public int? End { get; set; }
    }
}