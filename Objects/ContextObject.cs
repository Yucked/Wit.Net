namespace Wit.Net.Objects
{
    using System;
    using Newtonsoft.Json;
    /// <summary>
    ///[Optional] Context is key in natural language. For instance, at the same absolute instant, “today” will be resolved to a different value depending on the timezone of the user.
    /// </summary>
    public class ContextObject
    {
        /// <summary>Locale of the user. it will default to the “parent” language, with no locale-specific customization.<para/>Example: en_GB </summary>
        [JsonProperty("locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Local timezone of the user. Must be a canonical ID. <see cref="Timezone"/> is used only if you don’t provide a <see cref="ReferenceTime"/>.<para/>
        /// If neither <see cref="Timezone"/> or <see cref="ReferenceTime"/> are provided, default timezone will be used from your application.<para/>
        ///Example: America/Los_Angeles
        /// </summary>
        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        /// <summary>Local date and time of the user.
        /// Do not use UTC time, which would defeat the purpose of this field.<para/>Example: 2014-10-30T12:18:45-07:00</summary>
        [JsonProperty("reference_time")]
        public DateTimeOffset ReferenceTime { get; set; }

        /// <summary>
        /// If provided, intents that were defined for this or these state(s) will be matched with high priority.<para/>
        /// Example: "yes_or_no" or ["yes_or_no", "cancel"]
        /// </summary>
        [JsonProperty("state", DefaultValueHandling = DefaultValueHandling.Ignore),
            Obsolete("State is only available for older apps and is deprecated for current Wit apps.")]
        public string[] State { get; set; }
    }
}