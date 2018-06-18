using System.Globalization;

namespace WitSharp
{
    using System;
    using Objects;
    using System.Net;
    using System.Net.Http;
    using Newtonsoft.Json;    
    using System.Threading.Tasks;
    using System.Net.Http.Headers;
    public abstract class Base
    {
        internal HttpClient RestClient { get; }

        internal Base()
        {
            RestClient = new HttpClient { BaseAddress = new Uri("https://api.wit.ai") };
            RestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", WitClient.Key);
            RestClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            RestClient.DefaultRequestHeaders.Add("Accept", "application/vnd.wit.20200220+json");
        }

        protected static ContextObject DefaultContext
            => new ContextObject { Locale = "en_GB", ReferenceTime = DateTimeOffset.Now, Timezone = "Europe/Londer" };

        internal string SnowFlake
        {
            get
            {
                var random = new Random(Guid.NewGuid().GetHashCode());
                var date = new DateTime(2020, 02, 20, 06, 06, 06);
                var buffer = new byte[(int)Math.Round(Math.Log(Math.Sqrt(date.Ticks)))];
                random.NextBytes(buffer);
                return $"X0-{Math.Abs((decimal)BitConverter.ToUInt64(buffer, 0)).ToString(CultureInfo.InvariantCulture).Substring(0, 12)}";
            }
        }

        internal HttpContent CreateContent(object content)
            => new StringContent(JsonConvert.SerializeObject(content,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                System.Text.Encoding.UTF8, "application/json");

        internal void Process(HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
                throw new Exception($"HTTP ({message.StatusCode}): {Response(message.StatusCode)}");
        }

        internal async Task<T> ProcessAsync<T>(HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)            
                throw new Exception($"HTTP ({message.StatusCode}): {Response(message.StatusCode)}");
                                
                return JsonConvert.DeserializeObject<T>(await message.Content.ReadAsStringAsync(),
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });            
        }

        private string Response(HttpStatusCode code)
        {
            switch (code)
            {
                case HttpStatusCode.BadRequest: return "Missing Body/Content-Type | Unknown Content-Type | Speech Reconginition Failed | Invalid Parameters.";
                case HttpStatusCode.Unauthorized: return "Wrong authentication key.";
                case HttpStatusCode.RequestTimeout: return "Request timed out. Client was too slow to send data.";
                case HttpStatusCode.InternalServerError: return "Something went wrong on Wit's side, our experts are probably fixing it.";
                case HttpStatusCode.ServiceUnavailable: return "Something is very wrong on Wit's side.";
                default: return string.Empty;
            }
        }
    }
}