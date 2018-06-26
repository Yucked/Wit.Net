using System;
using Newtonsoft.Json;

namespace WitSharp.Models
{
    public class AppModel
    {
        public AppModel()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentNullException(nameof(Name), "Please provide the name of your application.");
            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentNullException(nameof(Description), 
                    "Please provide a short sentence describing your app.");
            if (Language == 0)
                throw new ArgumentNullException(nameof(Language), "Please choose language.");            
            
        }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("private")]
        public bool IsPrivate { get; set; }
        [JsonProperty("lang")]
        public Language Language { get; set; }
        [JsonProperty("desc")]
        public string Description { get; set; }
    }
}