namespace Wit.Net
{
    using System;
    using Wit.Net.Models;
    using Wit.Net.Objects;
    using System.Net.Http;
    using Newtonsoft.Json;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Net.Http.Headers;
    /// <summary></summary>
    public class WitClient
    {
        /// <summary>Log event for everything.</summary>
        public event Action<string, Exception> Log;

        #region Privates
        LogSeverity Severity { get; }
        HttpClient RestClient { get; }
        string Version => $"?v={typeof(WitClient).GetTypeInfo().Assembly.GetName().Version.ToString(3)}";
        ContextObject DefaultContext => new ContextObject { Locale = "en_GB", ReferenceTime = DateTimeOffset.Now, Timezone = "Europe/Londer" };
        long GenerateSnowflake => Convert.ToInt64(Math.Abs(DateTime.UtcNow.Subtract(new DateTime(2020, 02, 20, 20, 02, 22)).TotalMilliseconds)) + 20200220200222;
        #endregion

        /// <summary></summary>
        public WitClient(WitConfig Config)
        {
            if (string.IsNullOrWhiteSpace(Config.AccessToken)) throw new Exception($"{nameof(Config.AccessToken)} is required.");
            RestClient = new HttpClient { BaseAddress = new Uri("https://api.wit.ai") };
            RestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Config.AccessToken);
            RestClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Severity = Config.LogSeverity;
        }


        /// <summary>
        /// Returns the meaning of a sentence.
        /// </summary>
        /// <param name="Sentence"><see cref="SentenceModel"/></param>
        /// <returns></returns>
        public async Task<SentenceObject> GetMeaningAsync(SentenceModel Sentence)
        {
            if (Sentence == null)
                Logger(exception: new NullReferenceException($"{nameof(Sentence)} can't be null."));
            else if (string.IsNullOrWhiteSpace(Sentence.Message)) Logger(exception: new Exception($"{nameof(Sentence.Message)} cannot be null/whitespace."));
            else if (Sentence.Message.Length > 256) Logger(exception: new Exception($"{nameof(Sentence.Message)} length cannot be greater than 256."));
            else if (Sentence.MaxTraits > 8) Logger(exception: new Exception($"{nameof(Sentence.MaxTraits)} cannot be greater than 8."));
            else if (Sentence.MaxTraits < 0) Logger(exception: new Exception($"{nameof(Sentence.MaxTraits)} cannot be less than 0."));
            var Get = await RestClient.GetAsync(
                $"message{Version}&q={Sentence.Message}&context={JsonConvert.SerializeObject(Sentence.Context ?? DefaultContext)}" +
                $"&msg_id={Sentence.MessageId ?? $"{GenerateSnowflake}"}" +
                $"&thread_id={Sentence.ThreadId ?? $"{GenerateSnowflake}"}" +
                $"&n={Sentence.MaxTraits}&verbose={Sentence.Verbose}").ConfigureAwait(false);
            return await ProcessResponse<SentenceObject>(Get).ConfigureAwait(false);
        }




        #region Internal Methods

        void Logger(string message = null, Exception exception = null)
        {
            switch (Severity)
            {
                case LogSeverity.EXCEPTIONS: throw exception;
                case LogSeverity.INFO: Log.Invoke(message, exception); break;
            }
        }

        internal async Task<T> ProcessResponse<T>(HttpResponseMessage Message)
        {
            if (!Message.IsSuccessStatusCode) Logger(exception: new Exception($"HTTP Error {(int)Message.StatusCode}: {Message.ReasonPhrase}"));
            return JsonConvert.DeserializeObject<T>(await Message.Content.ReadAsStringAsync().ConfigureAwait(false));
        }
        #endregion
    }
}