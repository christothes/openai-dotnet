// <auto-generated/>

#nullable disable

using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;
using OpenAI.Graders;

namespace OpenAI.Evals
{
    internal partial class InternalEvalGraderTextSimilarityParams : IJsonModel<InternalEvalGraderTextSimilarityParams>
    {
        internal InternalEvalGraderTextSimilarityParams() : this(GraderType.TextSimilarity, null, null, null, null, default, default)
        {
        }

        void IJsonModel<InternalEvalGraderTextSimilarityParams>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalEvalGraderTextSimilarityParams>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalEvalGraderTextSimilarityParams)} does not support writing '{format}' format.");
            }
            base.JsonModelWriteCore(writer, options);
            if (_additionalBinaryDataProperties?.ContainsKey("name") != true)
            {
                writer.WritePropertyName("name"u8);
                writer.WriteStringValue(Name);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("input") != true)
            {
                writer.WritePropertyName("input"u8);
                writer.WriteStringValue(Input);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("reference") != true)
            {
                writer.WritePropertyName("reference"u8);
                writer.WriteStringValue(Reference);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("evaluation_metric") != true)
            {
                writer.WritePropertyName("evaluation_metric"u8);
                writer.WriteStringValue(EvaluationMetric.ToString());
            }
            if (_additionalBinaryDataProperties?.ContainsKey("pass_threshold") != true)
            {
                writer.WritePropertyName("pass_threshold"u8);
                writer.WriteNumberValue(PassThreshold);
            }
        }

        InternalEvalGraderTextSimilarityParams IJsonModel<InternalEvalGraderTextSimilarityParams>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => (InternalEvalGraderTextSimilarityParams)JsonModelCreateCore(ref reader, options);

        protected override InternalEvalGraderParams JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalEvalGraderTextSimilarityParams>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalEvalGraderTextSimilarityParams)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeInternalEvalGraderTextSimilarityParams(document.RootElement, options);
        }

        internal static InternalEvalGraderTextSimilarityParams DeserializeInternalEvalGraderTextSimilarityParams(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            GraderType kind = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            string name = default;
            string input = default;
            string reference = default;
            GraderTextSimilarityEvaluationMetric evaluationMetric = default;
            float passThreshold = default;
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("type"u8))
                {
                    kind = new GraderType(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("name"u8))
                {
                    name = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("input"u8))
                {
                    input = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("reference"u8))
                {
                    reference = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("evaluation_metric"u8))
                {
                    evaluationMetric = new GraderTextSimilarityEvaluationMetric(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("pass_threshold"u8))
                {
                    passThreshold = prop.Value.GetSingle();
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new InternalEvalGraderTextSimilarityParams(
                kind,
                additionalBinaryDataProperties,
                name,
                input,
                reference,
                evaluationMetric,
                passThreshold);
        }

        BinaryData IPersistableModel<InternalEvalGraderTextSimilarityParams>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalEvalGraderTextSimilarityParams>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(InternalEvalGraderTextSimilarityParams)} does not support writing '{options.Format}' format.");
            }
        }

        InternalEvalGraderTextSimilarityParams IPersistableModel<InternalEvalGraderTextSimilarityParams>.Create(BinaryData data, ModelReaderWriterOptions options) => (InternalEvalGraderTextSimilarityParams)PersistableModelCreateCore(data, options);

        protected override InternalEvalGraderParams PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalEvalGraderTextSimilarityParams>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeInternalEvalGraderTextSimilarityParams(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(InternalEvalGraderTextSimilarityParams)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<InternalEvalGraderTextSimilarityParams>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
