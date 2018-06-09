namespace Wit.Net
{
    using System;
    using Wit.Net.Models;
    using Wit.Net.Objects;
    using Newtonsoft.Json;
    using System.Threading.Tasks;
    public class WitClient : WitBase
    {
        public Entity Entity { get; }
        /// <summary>Initializes a new WitClient. Better to inject it.</summary>
        public WitClient(WitConfig config) : base(config)
        {
            if (string.IsNullOrWhiteSpace(Config.AccessToken))
                throw new Exception($"{nameof(Config.AccessToken)} is required.");
            Entity = new Entity();
        }

        /// <summary>Returns the meaning of a sentence.</summary>
        /// <param name="Sentence"><see cref="SentenceModel"/></param>
        public async Task<SentenceObject> SentenceMeaningAsync(SentenceModel Sentence)
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
    }
}