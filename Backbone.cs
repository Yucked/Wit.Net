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
    internal class Backbone : IDisposable
    {
        bool ThrowOnError { get; }
        HttpClient RestClient { get; set; }
        event Action<LogMessage> Logger;
        string Version => typeof(Backbone).GetTypeInfo().Assembly.GetName().Version.ToString(3);
        ContextObject DefaultContext => new ContextObject { Locale = "en_GB", ReferenceTime = DateTime.UtcNow, Timezone = "Europe/Londer" };
        long GenerateSnowflake => Convert.ToInt64(Math.Abs(DateTime.UtcNow.Subtract(new DateTime(2020, 02, 20, 20, 02, 22)).TotalMilliseconds)) + 20200220200222;

        public Backbone(WitConfig Config, Action<LogMessage> Log)
        {
            RestClient = new HttpClient { BaseAddress = new Uri("https://api.wit.ai") };
            RestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Config.AccessToken);
            RestClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ThrowOnError = Config.ThrowOnError;
        }

        internal async Task<SentenceObject> GetMeaning(SentenceModel Sentence)
        {
            if (string.IsNullOrWhiteSpace(Sentence.Message)) ProcessException($"{nameof(Sentence.Message)} cannot be null/whitespace.");
            else if (Sentence.Message.Length > 256) ProcessException($"{nameof(Sentence.Message)} length cannot be greater than 256.");
            else if (Sentence.MaxTraits > 8) ProcessException($"{nameof(Sentence.MaxTraits)} cannot be greater than 8.");
            else if (Sentence.MaxTraits < 0) ProcessException($"{nameof(Sentence.MaxTraits)} cannot be less than 0.");
            var Get = await RestClient.GetAsync(
                $"/message{Version}&q={Sentence.Message}&msg_id={Sentence.MessageId ?? $"{GenerateSnowflake}"}" +
                $"&thread_id={Sentence.ThreadId ?? $"{GenerateSnowflake}"}" +
                $"&n={Sentence.MaxTraits}&verbose={Sentence.Verbose}").ConfigureAwait(false);
            return await ProcessResponse<SentenceObject>(Get).ConfigureAwait(false);
        }


        internal void ProcessException(string Message)
        {
            if (ThrowOnError) throw new Exception(Message);
            else Logger.Invoke(new LogMessage(string.Empty, new Exception(Message)));
        }

        internal async Task<T> ProcessResponse<T>(HttpResponseMessage Message)
        {
            if (!Message.IsSuccessStatusCode) throw new Exception($"{Message.StatusCode}: {Message.ReasonPhrase}");
            return JsonConvert.DeserializeObject<T>(await Message.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        public void Dispose() { RestClient.Dispose(); RestClient = null; }
    }
}