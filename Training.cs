using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WitSharp.Models;
using WitSharp.Objects;

namespace WitSharp
{
    public class Training : Base
    {
        internal Training() { }

        /// <summary>Returns the meaning of a sentence.</summary>
        /// <param name="sentence"><see cref="SentenceModel"/></param>
        public async Task<MeaningObject> SentenceMeaningAsync(SentenceModel sentence)
        {
            if (sentence == null)
                throw new NullReferenceException($"{nameof(sentence)} can't be null.");
            if (string.IsNullOrWhiteSpace(sentence.Message))
                throw new Exception($"{nameof(sentence.Message)} cannot be null/whitespace.");
            if (sentence.Message.Length > 256)
                throw new Exception($"{nameof(sentence.Message)} length cannot be greater than 256.");
            if (sentence.MaxTraits > 8)
                throw new Exception($"{nameof(sentence.MaxTraits)} cannot be greater than 8.");
            if (sentence.MaxTraits < 0)
                throw new Exception($"{nameof(sentence.MaxTraits)} cannot be less than 0.");
            var threadId = sentence.ThreadId ?? SnowFlake;
            var messageId = sentence.MessageId ?? SnowFlake;
            var get = await RestClient.GetAsync(
                $"message?q={sentence.Message}&context={JsonConvert.SerializeObject(sentence.Context ?? DefaultContext)}" +
                $"&msg_id={messageId}&thread_id={threadId}&n={sentence.MaxTraits}&verbose={sentence.Verbose}").ConfigureAwait(false);
            return await ProcessAsync<MeaningObject>(get);
        }

        /// <summary>Validate samples (sentence + entities annotations) to train your app programmatically.</summary>
        /// <param name="text">The text (sentence) you want your app to understand.</param>
        /// <param name="entities">The list of entities appearing in this sentence, 
        /// that you want your app to extract once it is trained.</param>
        public async Task ValidateSamplesAsync(string text, EntityModel[] entities)
        {
            if (string.IsNullOrWhiteSpace(text) || entities.Length == 0)
                throw new Exception($"{nameof(text)} or {nameof(entities)} can't be null or empty.");          
            var post = await RestClient.PostAsync("samples", CreateContent(new
            {
                text,
                entities
            }));
            Process(post);
        }

        /// <summary>Delete validated samples from your app</summary>
        /// <param name="texts">The text of the sample you would like deleted.</param>
        public async Task DeleteSampleAsync(string[] texts)
        {
            var samples = new List<object>(texts.Length);
            foreach (var text in texts) samples.Add(new {text });
            var request = new HttpRequestMessage
            {
                Content = CreateContent(samples),
                Method = HttpMethod.Delete,
                RequestUri = new Uri("samples")
            };
            Process(await RestClient.SendAsync(request));
        }
    }
}