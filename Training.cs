namespace Wit.Net
{
    using System;
    using System.Linq;
    using Wit.Net.Models;
    using Newtonsoft.Json;
    using System.Net.Http;
    using Wit.Net.Objects;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    public class Training : Base
    {
        internal Training() { }

        /// <summary>Returns the meaning of a sentence.</summary>
        /// <param name="Sentence"><see cref="SentenceModel"/></param>
        public async Task<SentenceObject> SentenceMeaningAsync(SentenceModel Sentence)
        {
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
            var ThreadId = Sentence.ThreadId ?? SnowFlake;
            var MessageId = Sentence.MessageId ?? SnowFlake;
            var Get = await RestClient.GetAsync(
                $"message?q={Sentence.Message}&context={JsonConvert.SerializeObject(Sentence.Context ?? DefaultContext)}" +
                $"&msg_id={MessageId}&thread_id={ThreadId}&n={Sentence.MaxTraits}&verbose={Sentence.Verbose}").ConfigureAwait(false);
            return await ProcessAsync<SentenceObject>(Get, $"GET | Message Id {MessageId} : Thread Id {ThreadId}");
        }

        /// <summary>Validate samples (sentence + entities annotations) to train your app programmatically.</summary>
        /// <param name="Text">The text (sentence) you want your app to understand.</param>
        /// <param name="Entities">The list of entities appearing in this sentence, 
        /// that you want your app to extract once it is trained.</param>
        public async Task ValidateSamplesAsync(string Text, EntityModel[] Entities)
        {
            if (string.IsNullOrWhiteSpace(Text) || Entities.Length == 0)
            {
                Logger.Logging.Send(exception: new Exception($"{nameof(Text)} or {nameof(Entities)} can't be null or empty."));
                return;
            }
            var Post = await RestClient.PostAsync("samples", CreateContent(new
            {
                text = Text,
                entities = Entities
            }));
            Process(Post, "POST /samples");
        }

        /// <summary>Delete validated samples from your app</summary>
        /// <param name="Texts">The text of the sample you would like deleted.</param>
        public async Task DeleteSampleAsync(string[] Texts)
        {
            var Samples = new List<object>(Texts.Length);
            foreach (var Text in Texts) Samples.Add(new { text = Text });
            var Request = new HttpRequestMessage
            {
                Content = CreateContent(Samples),
                Method = HttpMethod.Delete,
                RequestUri = new Uri("samples")
            };
            Process(await RestClient.SendAsync(Request), "DELETE /samples");
        }
    }
}