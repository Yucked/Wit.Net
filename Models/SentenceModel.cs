using System;
using WitSharp.Objects;

namespace WitSharp.Models
{
    public class SentenceModel
    {
        public SentenceModel()
        {
            if (string.IsNullOrWhiteSpace(Message) || Message.Length < 1)
                throw new ArgumentNullException(nameof(Message),
                    "Message is a required argument and can't be empty or null.");
            if (Message.Length > 256)
                throw new ArgumentOutOfRangeException(nameof(Message),
                    "Message's content must be less than 256 characters.");
            if (MaxTraits > 8)
                throw new ArgumentOutOfRangeException(nameof(MaxTraits), "Max traits cannot be greater than 8.");
            if (MaxTraits < 0)
                throw new ArgumentOutOfRangeException(nameof(MaxTraits), "Max traits cannot be less than 0.");
        }

        /// <summary>User’s query. Length must be  between 0 and 256</summary>
        public string Message { get; set; }

        /// <summary>If no context is provided, default will be used.</summary>
        public ContextObject Context { get; set; }

        /// <summary>The number of n-best trait entities you want to get back. The Default is 1, and the maximum is 8.</summary>
        public int MaxTraits { get; set; } = 1;

        /// <summary>A flag to get auxiliary information about entities, like the location within the sentence.</summary>
        public bool Verbose { get; set; } = false;
    }
}