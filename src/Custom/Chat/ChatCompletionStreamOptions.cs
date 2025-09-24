using System;
using System.Collections.Generic;

namespace OpenAI.Chat
{
    public partial class ChatCompletionStreamOptions
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        public ChatCompletionStreamOptions()
        {
        }

        internal ChatCompletionStreamOptions(bool? includeUsage, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            IncludeUsage = includeUsage;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public bool? IncludeUsage { get; set; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}
