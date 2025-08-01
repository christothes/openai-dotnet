// <auto-generated/>

#nullable disable

using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Responses
{
    internal partial class InternalResponseImageGenCallPartialImageEvent : IJsonModel<InternalResponseImageGenCallPartialImageEvent>
    {
        internal InternalResponseImageGenCallPartialImageEvent() : this(InternalResponseStreamEventType.ResponseImageGenerationCallPartialImage, default, null, default, null, default, null)
        {
        }

        void IJsonModel<InternalResponseImageGenCallPartialImageEvent>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalResponseImageGenCallPartialImageEvent>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalResponseImageGenCallPartialImageEvent)} does not support writing '{format}' format.");
            }
            base.JsonModelWriteCore(writer, options);
            if (_additionalBinaryDataProperties?.ContainsKey("output_index") != true)
            {
                writer.WritePropertyName("output_index"u8);
                writer.WriteNumberValue(OutputIndex);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("item_id") != true)
            {
                writer.WritePropertyName("item_id"u8);
                writer.WriteStringValue(ItemId);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("partial_image_index") != true)
            {
                writer.WritePropertyName("partial_image_index"u8);
                writer.WriteNumberValue(PartialImageIndex);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("partial_image_b64") != true)
            {
                writer.WritePropertyName("partial_image_b64"u8);
                writer.WriteStringValue(PartialImageB64);
            }
        }

        InternalResponseImageGenCallPartialImageEvent IJsonModel<InternalResponseImageGenCallPartialImageEvent>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => (InternalResponseImageGenCallPartialImageEvent)JsonModelCreateCore(ref reader, options);

        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalResponseImageGenCallPartialImageEvent>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalResponseImageGenCallPartialImageEvent)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeInternalResponseImageGenCallPartialImageEvent(document.RootElement, options);
        }

        internal static InternalResponseImageGenCallPartialImageEvent DeserializeInternalResponseImageGenCallPartialImageEvent(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            InternalResponseStreamEventType kind = default;
            int sequenceNumber = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            int outputIndex = default;
            string itemId = default;
            int partialImageIndex = default;
            string partialImageB64 = default;
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("type"u8))
                {
                    kind = new InternalResponseStreamEventType(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("sequence_number"u8))
                {
                    sequenceNumber = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("output_index"u8))
                {
                    outputIndex = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("item_id"u8))
                {
                    itemId = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("partial_image_index"u8))
                {
                    partialImageIndex = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("partial_image_b64"u8))
                {
                    partialImageB64 = prop.Value.GetString();
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new InternalResponseImageGenCallPartialImageEvent(
                kind,
                sequenceNumber,
                additionalBinaryDataProperties,
                outputIndex,
                itemId,
                partialImageIndex,
                partialImageB64);
        }

        BinaryData IPersistableModel<InternalResponseImageGenCallPartialImageEvent>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalResponseImageGenCallPartialImageEvent>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(InternalResponseImageGenCallPartialImageEvent)} does not support writing '{options.Format}' format.");
            }
        }

        InternalResponseImageGenCallPartialImageEvent IPersistableModel<InternalResponseImageGenCallPartialImageEvent>.Create(BinaryData data, ModelReaderWriterOptions options) => (InternalResponseImageGenCallPartialImageEvent)PersistableModelCreateCore(data, options);

        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalResponseImageGenCallPartialImageEvent>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeInternalResponseImageGenCallPartialImageEvent(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(InternalResponseImageGenCallPartialImageEvent)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<InternalResponseImageGenCallPartialImageEvent>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
