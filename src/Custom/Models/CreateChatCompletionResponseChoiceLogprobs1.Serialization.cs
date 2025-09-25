using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI.Chat;

namespace OpenAI
{
    public partial class CreateChatCompletionResponseChoiceLogprobs1 : IJsonModel<CreateChatCompletionResponseChoiceLogprobs1>
    {
        void IJsonModel<CreateChatCompletionResponseChoiceLogprobs1>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<CreateChatCompletionResponseChoiceLogprobs1>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(CreateChatCompletionResponseChoiceLogprobs1)} does not support writing '{format}' format.");
            }
            // Plugin customization: remove options.Format != "W" check
            if (_additionalBinaryDataProperties?.ContainsKey("content") != true)
            {
                writer.WritePropertyName("content"u8);
                writer.WriteStartArray();
                foreach (ChatTokenLogProbabilityDetails item in Content)
                {
                    writer.WriteObjectValue(item, options);
                }
                writer.WriteEndArray();
            }
            // Plugin customization: remove options.Format != "W" check
            if (_additionalBinaryDataProperties?.ContainsKey("refusal") != true)
            {
                writer.WritePropertyName("refusal"u8);
                writer.WriteStartArray();
                foreach (ChatTokenLogProbabilityDetails item in Refusal)
                {
                    writer.WriteObjectValue(item, options);
                }
                writer.WriteEndArray();
            }
            // Plugin customization: remove options.Format != "W" check
            if (_additionalBinaryDataProperties != null)
            {
                foreach (var item in _additionalBinaryDataProperties)
                {
                    if (ModelSerializationExtensions.IsSentinelValue(item.Value))
                    {
                        continue;
                    }
                    writer.WritePropertyName(item.Key);
#if NET6_0_OR_GREATER
                    writer.WriteRawValue(item.Value);
#else
                    using (JsonDocument document = JsonDocument.Parse(item.Value))
                    {
                        JsonSerializer.Serialize(writer, document.RootElement);
                    }
#endif
                }
            }
        }

        CreateChatCompletionResponseChoiceLogprobs1 IJsonModel<CreateChatCompletionResponseChoiceLogprobs1>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        protected virtual CreateChatCompletionResponseChoiceLogprobs1 JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<CreateChatCompletionResponseChoiceLogprobs1>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(CreateChatCompletionResponseChoiceLogprobs1)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeCreateChatCompletionResponseChoiceLogprobs1(document.RootElement, options);
        }

        internal static CreateChatCompletionResponseChoiceLogprobs1 DeserializeCreateChatCompletionResponseChoiceLogprobs1(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            IReadOnlyList<ChatTokenLogProbabilityDetails> content = default;
            IReadOnlyList<ChatTokenLogProbabilityDetails> refusal = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("content"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        content = new ChangeTrackingList<ChatTokenLogProbabilityDetails>();
                        continue;
                    }
                    List<ChatTokenLogProbabilityDetails> array = new List<ChatTokenLogProbabilityDetails>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ChatTokenLogProbabilityDetails.DeserializeChatTokenLogProbabilityDetails(item, options));
                    }
                    content = array;
                    continue;
                }
                if (prop.NameEquals("refusal"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        refusal = new ChangeTrackingList<ChatTokenLogProbabilityDetails>();
                        continue;
                    }
                    List<ChatTokenLogProbabilityDetails> array = new List<ChatTokenLogProbabilityDetails>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ChatTokenLogProbabilityDetails.DeserializeChatTokenLogProbabilityDetails(item, options));
                    }
                    refusal = array;
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new CreateChatCompletionResponseChoiceLogprobs1(content, refusal, additionalBinaryDataProperties);
        }

        BinaryData IPersistableModel<CreateChatCompletionResponseChoiceLogprobs1>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<CreateChatCompletionResponseChoiceLogprobs1>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(CreateChatCompletionResponseChoiceLogprobs1)} does not support writing '{options.Format}' format.");
            }
        }

        CreateChatCompletionResponseChoiceLogprobs1 IPersistableModel<CreateChatCompletionResponseChoiceLogprobs1>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

        protected virtual CreateChatCompletionResponseChoiceLogprobs1 PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<CreateChatCompletionResponseChoiceLogprobs1>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeCreateChatCompletionResponseChoiceLogprobs1(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(CreateChatCompletionResponseChoiceLogprobs1)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<CreateChatCompletionResponseChoiceLogprobs1>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
