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
        /// <exception cref="ArgumentNullException"><param name="sentence"/> was <c>null</c></exception>
        public async Task<MeaningObject> SentenceMeaningAsync(SentenceModel sentence)
        {
            if (sentence == null)
                throw new ArgumentNullException(nameof(sentence), "Sentence is a required argument.");
            var get = await RestClient.GetAsync(
                    $"message?q={sentence.Message}&context={JsonConvert.SerializeObject(sentence.Context ?? DefaultContext)}" +
                    $"&msg_id={SnowFlake}&thread_id={SnowFlake}&n={sentence.MaxTraits}&verbose={sentence.Verbose}")
                .ConfigureAwait(false);
            return await ProcessAsync<MeaningObject>(get);
        }

        /// <summary>Returns the meaning extracted from an audio file.</summary>
        /// <param name="audioType">What kind of audio is it.</param>
        /// <param name="audioFilePath">Path to audio file.</param>
        /// <param name="context"><see cref="ContextObject"/></param>
        /// <param name="bestOutcomes">The number of n-best outcomes you want to get back. default is 1</param>
        /// <exception cref="FileNotFoundException">Couldn't find the at <param name="audioFilePath"/> path.</exception>
        public async Task<MeaningObject> SpeechMeaningAsync(AudioType audioType, string audioFilePath,
            ContextObject context = null, int bestOutcomes = 1)
        {
            var audioHeaders = audioType == AudioType.Mpeg3
                ? "audio/mpeg3"
                : audioType == AudioType.Ulaw
                    ? "audio/ulaw"
                    : "audio/wav";
            if (!File.Exists(audioFilePath))
                throw new FileNotFoundException(nameof(audioFilePath),
                    "Please make sure the path to audio file is correct.");
            using (var stream = File.OpenRead(audioFilePath))
            {
                var reader = new BinaryReader(stream);
                var content = new ByteArrayContent(reader.ReadBytes((int) stream.Length));
                content.Headers.Remove("Content-Type");
                content.Headers.Add("Transfer-Encoding", "chunked");
                content.Headers.Add("Content-Type", audioHeaders);
                var post = await RestClient.PostAsync(
                    $"speech?context={context ?? DefaultContext}&msg_id={SnowFlake}&" +
                    $"thread_id={SnowFlake}&n={bestOutcomes}", content);
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
        /// <param name="bestOutcomes">The number of n-best outcomes you want to get back. default is 1</param>
        /// <exception cref="FileNotFoundException">Couldn't find the at <param name="audioFilePath"/> path.</exception>
        public async Task<MeaningObject> SpeechMeaningAsync(string audioFilePath, Encoding encoding, int bits,
            int rate, string endian, ContextObject context = null, int bestOutcomes = 1)
        {
            if (!File.Exists(audioFilePath))
                throw new FileNotFoundException(nameof(audioFilePath),
                    "Please make sure the path to audio file is correct.");
            using (var stream = File.OpenRead(audioFilePath))
            {
                var reader = new BinaryReader(stream);
                var content = new ByteArrayContent(reader.ReadBytes((int) stream.Length));
                content.Headers.Remove("Content-Type");
                content.Headers.Add("Transfer-Encoding", "chunked");
                content.Headers.Add("Content-Type",
                    $"audio/raw;encoding={encoding.ToString().Replace('_', '-').ToLower()};bits={bits};rate={rate};endian={endian}");
                var post = await RestClient.PostAsync(
                    $"speech?context={context ?? DefaultContext}&msg_id={SnowFlake}&" +
                    $"thread_id={SnowFlake}&n={bestOutcomes}", content);
                return await ProcessAsync<MeaningObject>(post);
            }
        }

        /// <summary>Validate samples (sentence + entities annotations) to train your app programmatically.</summary>
        /// <param name="text">The text (sentence) you want your app to understand.</param>
        /// <param name="entities">The list of entities appearing in this sentence, 
        /// that you want your app to extract once it is trained.</param>
        /// <exception cref="ArgumentNullException"><param name="text"/><param name="entities"/></exception>
        public async Task ValidateSamplesAsync(string text, EntityModel[] entities)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text),
                    "Please provide the sentence you want your app to understand.");
            if (entities.Length == 0)
                throw new ArgumentNullException(nameof(entities), "At least a single entitiy must be provided.");
            var post = await RestClient.PostAsync("samples", CreateContent(new
            {
                text,
                entities
            }));
            Process(post);
        }

        /// <summary>Delete validated samples from your app</summary>
        /// <param name="texts">The text of the sample you would like to be deleted.</param>
        /// <exception cref="ArgumentNullException"><param name="texts"/> cannot be empty.</exception>
        public async Task DeleteSampleAsync(string[] texts)
        {
            if (texts.Length == 0)
                throw new ArgumentNullException(nameof(texts), "At least a single text must be provided.");
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