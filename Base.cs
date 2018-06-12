namespace WitSharp
{
    using System;
    using System.Net;
    using System.Net.Http;
    using Newtonsoft.Json;
    using WitSharp.Objects;
    using System.Threading.Tasks;
    using System.Net.Http.Headers;
    public abstract class Base
    {
        internal HttpClient RestClient { get; }
        internal LogSeverity Severity { get => WitClient.Config.LogSeverity; }

        internal Base()
        {
            RestClient = new HttpClient { BaseAddress = new Uri("https://api.wit.ai") };
            RestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", WitClient.Config.AccessToken);
            RestClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            RestClient.DefaultRequestHeaders.Add("Accept", "application/vnd.wit.20200220+json");
        }

        internal ContextObject DefaultContext
            => new ContextObject { Locale = "en_GB", ReferenceTime = DateTimeOffset.Now, Timezone = "Europe/Londer" };

        internal string SnowFlake
        {
            get
            {
                var Random = new Random(Guid.NewGuid().GetHashCode());
                var Date = new DateTime(2020, 02, 20, 06, 06, 06);
                var Buffer = new byte[(int)Math.Round(Math.Log(Math.Sqrt(Date.Ticks)))];
                Random.NextBytes(Buffer);
                return $"X0-{Math.Abs((decimal)BitConverter.ToUInt64(Buffer, 0)).ToString().Substring(0, 12)}";
            }
        }

        internal HttpContent CreateContent(object Content)
            => new StringContent(JsonConvert.SerializeObject(Content,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                System.Text.Encoding.UTF8, "application/json");

        internal void Process(HttpResponseMessage message, string LogMessage)
        {
            if (!message.IsSuccessStatusCode)
                Logger.Logging.Send(exception: new Exception($"HTTP ({message.StatusCode}): {Response(message.StatusCode)}"));
            else Logger.Logging.Send(LogMessage);
        }

        internal async Task<T> ProcessAsync<T>(HttpResponseMessage message, string LogMessage)
        {
            if (!message.IsSuccessStatusCode)
            {
                Logger.Logging.Send(exception: new Exception($"HTTP ({message.StatusCode}): {Response(message.StatusCode)}"));
                return default;
            }
            else
            {
                Logger.Logging.Send(LogMessage);
                return JsonConvert.DeserializeObject<T>(await message.Content.ReadAsStringAsync(),
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }

        internal string Response(HttpStatusCode Code)
        {
            switch (Code)
            {
                case HttpStatusCode.BadRequest: return $"Missing Body/Content-Type | Unknown Content-Type | Speech Reconginition Failed | Invalid Parameters.";
                case HttpStatusCode.Unauthorized: return $"Wrong authentication key.";
                case HttpStatusCode.RequestTimeout: return $"Request timed out. Client was too slow to send data.";
                case HttpStatusCode.InternalServerError: return $"Something went wrong on Wit's side, our experts are probably fixing it.";
                case HttpStatusCode.ServiceUnavailable: return $"Something is very wrong on Wit's side.";
                default: return string.Empty;
            }
        }
    }
}