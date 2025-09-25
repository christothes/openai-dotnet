using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenAI.Chat;

namespace OpenAI
{
    [Experimental("OPENAI001")]
    public partial class CreateChatCompletionResponseChoiceLogprobs1
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal CreateChatCompletionResponseChoiceLogprobs1() : this(null, null, null)
        {
        }

        internal CreateChatCompletionResponseChoiceLogprobs1(IReadOnlyList<ChatTokenLogProbabilityDetails> content, IReadOnlyList<ChatTokenLogProbabilityDetails> refusal, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            // Plugin customization: ensure initialization of collections
            Content = content ?? new ChangeTrackingList<ChatTokenLogProbabilityDetails>();
            Refusal = refusal ?? new ChangeTrackingList<ChatTokenLogProbabilityDetails>();
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public IReadOnlyList<ChatTokenLogProbabilityDetails> Content { get; }

        public IReadOnlyList<ChatTokenLogProbabilityDetails> Refusal { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}
