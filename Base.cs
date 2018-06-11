namespace Wit.Net
{
    using System;
    using System.Net;
    using System.Net.Http;
    using Newtonsoft.Json;
    using Wit.Net.Objects;
    using System.Reflection;
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
            RestClient.DefaultRequestHeaders.Add("Accept", "application/vnd.wit.20200202+json");
        }

        internal ContextObject DefaultContext
            => new ContextObject { Locale = "en_GB", ReferenceTime = DateTimeOffset.Now, Timezone = "Europe/Londer" };

        internal long SnowFlake => GenerateRandom();

        long GenerateRandom()
        {
            var Buffer = new byte[8];
            var Random = new Random(Guid.NewGuid().GetHashCode());
            Random.NextBytes(Buffer);
            return (Math.Abs(BitConverter.ToInt64(Buffer, 0) % (10000000000000 - 10000000000500)) + 10000000000500);
        }

        internal HttpContent CreateContent(object Content)
            => new StringContent(JsonConvert.SerializeObject(Content),
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
                return JsonConvert.DeserializeObject<T>(await message.Content.ReadAsStringAsync());
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