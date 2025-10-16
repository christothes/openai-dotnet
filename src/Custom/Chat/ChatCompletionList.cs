using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class ChatCompletionList
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal ChatCompletionList(IEnumerable<ChatCompletionResult> data, string firstId, string lastId, bool hasMore)
        {
            Data = data.ToList();
            FirstId = firstId;
            LastId = lastId;
            HasMore = hasMore;
        }

        internal ChatCompletionList(string @object, IList<ChatCompletionResult> data, string firstId, string lastId, bool hasMore, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            // Plugin customization: ensure initialization of collections
            Object = @object;
            Data = data ?? new ChangeTrackingList<ChatCompletionResult>();
            FirstId = firstId;
            LastId = lastId;
            HasMore = hasMore;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public string Object { get; } = "list";

        public IList<ChatCompletionResult> Data { get; }

        public string FirstId { get; }

        public string LastId { get; }

        public bool HasMore { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}
