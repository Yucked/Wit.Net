namespace Wit.Net
{
    using System;
    using Wit.Net.Objects;
    using System.Net.Http;
    using Newtonsoft.Json;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Net.Http.Headers;
    public class WitBase : ILog
    {
        internal WitBase() { }
        internal WitConfig Config { get; }
        internal LogSeverity Severity { get; }
        public event Action<string, Exception> Log;
        internal HttpClient RestClient => new HttpClient { BaseAddress = new Uri("https://api.wit.ai") };
        internal WitBase(WitConfig config)
        {
            Config = config;
            Severity = config.LogSeverity;
            RestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Config.AccessToken);
            RestClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        internal string Version
            => $"?v={typeof(WitClient).GetTypeInfo().Assembly.GetName().Version.ToString(3)}";

        internal ContextObject DefaultContext
            => new ContextObject { Locale = "en_GB", ReferenceTime = DateTimeOffset.Now, Timezone = "Europe/Londer" };

        internal long GenerateSnowflake
            => Convert.ToInt64(Math.Abs(DateTime.UtcNow.Subtract(new DateTime(2020, 02, 20, 20, 02, 22)).TotalMilliseconds)) + 20200220200222;

        internal void ProcessResponse(HttpResponseMessage Message)
        {
            if (!Message.IsSuccessStatusCode) Logger(exception: new Exception($"HTTP Error {(int)Message.StatusCode}: {Message.ReasonPhrase}"));
        }

        internal async Task<T> ProcessResponse<T>(HttpResponseMessage Message)
        {
            if (!Message.IsSuccessStatusCode) Logger(exception: new Exception($"HTTP Error {(int)Message.StatusCode}: {Message.ReasonPhrase}"));
            return JsonConvert.DeserializeObject<T>(await Message.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        internal void Logger(string message = null, Exception exception = null)
        {
            switch (Severity)
            {
                case LogSeverity.EXCEPTIONS: throw exception;
                case LogSeverity.INFO: Log.Invoke(message, exception); break;
            }
        }
    }

    public interface ILog
    {
        event Action<string, Exception> Log;
    }
}