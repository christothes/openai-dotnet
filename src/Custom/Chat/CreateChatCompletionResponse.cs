using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public partial class CreateChatCompletionResponse
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal CreateChatCompletionResponse(string id, IEnumerable<CreateChatCompletionResponseChoice> choices, DateTimeOffset created, string model)
        {
            Id = id;
            Choices = choices.ToList();
            Created = created;
            Model = model;
        }

        internal CreateChatCompletionResponse(string id, IList<CreateChatCompletionResponseChoice> choices, DateTimeOffset created, string model, ChatServiceTier? serviceTier, string systemFingerprint, string @object, ChatTokenUsage usage, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            // Plugin customization: ensure initialization of collections
            Id = id;
            Choices = choices ?? new ChangeTrackingList<CreateChatCompletionResponseChoice>();
            Created = created;
            Model = model;
            ServiceTier = serviceTier;
            SystemFingerprint = systemFingerprint;
            Object = @object;
            Usage = usage;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public string Id { get; }

        public IList<CreateChatCompletionResponseChoice> Choices { get; }

        public DateTimeOffset Created { get; }

        public string Model { get; }

        public ChatServiceTier? ServiceTier { get; }

        public string SystemFingerprint { get; }

        public string Object { get; } = "chat.completion";

        public ChatTokenUsage Usage { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}