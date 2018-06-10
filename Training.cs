namespace Wit.Net
{
    using System;
    using System.Linq;
    using Wit.Net.Models;
    using Newtonsoft.Json;
    using System.Net.Http;
    using Wit.Net.Objects;
    using System.Threading.Tasks;
    public class Training : Base
    {
        internal Training() { }

        /// <summary>Returns the meaning of a sentence.</summary>
        /// <param name="Sentence"><see cref="SentenceModel"/></param>
        public async Task<SentenceObject> SentenceMeaningAsync(SentenceModel Sentence)
        {
            var v = RestClient;
            var z = v.BaseAddress;
            if (Sentence == null)
                Logger.Logging.Send(exception: new NullReferenceException($"{nameof(Sentence)} can't be null."));
            else if (string.IsNullOrWhiteSpace(Sentence.Message))
                Logger.Logging.Send(exception: new Exception($"{nameof(Sentence.Message)} cannot be null/whitespace."));
            else if (Sentence.Message.Length > 256)
                Logger.Logging.Send(exception: new Exception($"{nameof(Sentence.Message)} length cannot be greater than 256."));
            else if (Sentence.MaxTraits > 8)
                Logger.Logging.Send(exception: new Exception($"{nameof(Sentence.MaxTraits)} cannot be greater than 8."));
            else if (Sentence.MaxTraits < 0)
                Logger.Logging.Send(exception: new Exception($"{nameof(Sentence.MaxTraits)} cannot be less than 0."));
            var ThreadId = Sentence.ThreadId ?? $"{SnowFlake}";
            var MessageId = Sentence.MessageId ?? $"{SnowFlake}";
            var Get = await RestClient.GetAsync(
                $"/message{Version}&q={Sentence.Message}&context={JsonConvert.SerializeObject(Sentence.Context ?? DefaultContext)}" +
                $"&msg_id={MessageId}&thread_id={ThreadId}&n={Sentence.MaxTraits}&verbose={Sentence.Verbose}").ConfigureAwait(false);
            return await ProcessAsync<SentenceObject>(Get, $"GET | Message Id {MessageId} : Thread Id {ThreadId}");
        }
    }
}