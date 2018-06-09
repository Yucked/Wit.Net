namespace Wit.Net
{
    using System;
    using Wit.Net.Models;
    using System.Threading.Tasks;
    /// <summary></summary>
    public class WitClient
    {
        WitConfig Config { get; }
        Backbone Backbone { get; }
        /// <summary>Log event for everything.</summary>
        public static event Action<LogMessage> Log;
        /// <summary></summary>
        public WitClient(WitConfig Config)
        {
            if (string.IsNullOrWhiteSpace(Config.AccessToken)) throw new Exception($"{nameof(Config.AccessToken)} is required.");
            using (Backbone = new Backbone(Config, Log)) { }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sentence"></param>
        /// <returns></returns>
        public Task GetMeaningAsync(SentenceModel Sentence)
        {
            if (Sentence == null) throw new NullReferenceException($"{nameof(Sentence)} can't be null.");
            return Backbone.GetMeaning(Sentence);
        }
    }
}