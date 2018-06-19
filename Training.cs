using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WitSharp.Objects;
using WitSharp.Models;

namespace WitSharp
{
    public class Training : Base
    {
        internal Training()
        {
        }

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
                    $"&msg_id={messageId}&thread_id={threadId}&n={sentence.MaxTraits}&verbose={sentence.Verbose}")
                .ConfigureAwait(false);
            return await ProcessAsync<MeaningObject>(get);
        }

        /// <summary>Returns the meaning extracted from an audio file.</summary>
        /// <param name="audioType">What kind of audio is it.</param>
        /// <param name="audioFilePath">Path to audio file.</param>
        /// <param name="Context"><see cref="ContextObject"/></param>
        /// <param name="BestOutcomes">The number of n-best outcomes you want to get back. default is 1</param>
        public async Task<MeaningObject> SpeechMeaningAsync(AudioType audioType, string audioFilePath,
            ContextObject Context = null, int BestOutcomes = 1)
        {
            var audioHeaders = audioType == AudioType.MPEG3
                ? "audio/mpeg3"
                : audioType == AudioType.ULAW
                    ? "audio/ulaw"
                    : "audio/wav";
            if (!File.Exists(audioFilePath)) throw new FileNotFoundException($"{nameof(audioFilePath)} not found.");
            using (var Stream = File.OpenRead(audioFilePath))
            {
                var Reader = new BinaryReader(Stream);
                var Content = new ByteArrayContent(Reader.ReadBytes((int) Stream.Length));
                Content.Headers.Remove("Content-Type");
                Content.Headers.Add("Transfer-Encoding", "chunked");
                Content.Headers.Add("Content-Type", audioHeaders);
                var post = await RestClient.PostAsync(
                    $"speech?context={Context ?? DefaultContext}&msg_id={SnowFlake}&" +
                    $"thread_id={SnowFlake}&n={BestOutcomes}", Content);
                return await ProcessAsync<MeaningObject>(post);
            }
        }
        
        /// <summary>Returns the meaning extracted from an raw audio file.</summary>
        /// <param name="audioFilePath">Path to audio file.</param>
        /// <param name="encoding"><see cref="Encoding"/></param>
        /// <param name="bits">8, 16 or 32.</param>
        /// <param name="rate">An integer value like 8000.</param>
        /// <param name="endian">big or little (usually little, see: http://en.wikipedia.org/wiki/Comparison_of_instruction_set_architectures#Instruction_sets</param>
        /// <param name="context"><see cref="ContextObject"/></param>
        /// <param name="BestOutcomes">The number of n-best outcomes you want to get back. default is 1</param>
        public async Task<MeaningObject> SpeechMeaningAsync(string audioFilePath, Encoding encoding, int bits,
            int rate, string endian, ContextObject context = null, int BestOutcomes = 1)
        {
            if (!File.Exists(audioFilePath)) throw new FileNotFoundException($"{nameof(audioFilePath)} not found.");
            using (var Stream = File.OpenRead(audioFilePath))
            {
                var Reader = new BinaryReader(Stream);
                var Content = new ByteArrayContent(Reader.ReadBytes((int) Stream.Length));
                Content.Headers.Remove("Content-Type");
                Content.Headers.Add("Transfer-Encoding", "chunked");
                Content.Headers.Add("Content-Type",
                    $"audio/raw;encoding={encoding.ToString().Replace('_', '-').ToLower()};bits={bits};rate={rate};endian={endian}");
                var post = await RestClient.PostAsync(
                    $"speech?context={context ?? DefaultContext}&msg_id={SnowFlake}&" +
                    $"thread_id={SnowFlake}&n={BestOutcomes}", Content);
                return await ProcessAsync<MeaningObject>(post);
            }
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
            foreach (var text in texts) samples.Add(new {text});
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