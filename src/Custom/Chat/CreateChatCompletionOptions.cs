using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using OpenAI.Internal;

namespace OpenAI.Chat
{
    public partial class CreateChatCompletionOptions : JsonModel<CreateChatCompletionOptions>
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        public CreateChatCompletionOptions(IEnumerable<ChatMessage> messages, string model)
        {
            Argument.AssertNotNull(messages, nameof(messages));

            Metadata = new ChangeTrackingDictionary<string, string>();
            Messages = messages.ToList();
            Model = model;
            Modalities = new ChangeTrackingList<CreateChatCompletionRequestModality>();
            LogitBias = new ChangeTrackingDictionary<string, int>();
            Tools = new ChangeTrackingList<ChatTool>();
            Functions = new ChangeTrackingList<ChatFunction>();
        }

        internal CreateChatCompletionOptions(IDictionary<string, string> metadata, float? temperature, float? topP, string user, ChatServiceTier? serviceTier, IList<ChatMessage> messages, string model, IList<CreateChatCompletionRequestModality> modalities, ChatReasoningEffortLevel? reasoningEffort, int? maxCompletionTokens, float? frequencyPenalty, float? presencePenalty, ChatWebSearchOptions webSearchOptions, int? topLogprobs, ResponseFormat responseFormat, ChatAudioOptions audio, bool? store, bool? stream, BinaryData stop, IDictionary<string, int> logitBias, bool? logprobs, int? maxTokens, int? n, ChatOutputPrediction prediction, long? seed, ChatCompletionStreamOptions streamOptions, IList<ChatTool> tools, BinaryData toolChoice, bool? parallelToolCalls, BinaryData functionCall, IList<ChatFunction> functions, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            // Plugin customization: ensure initialization of collections
            Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
            Temperature = temperature;
            TopP = topP;
            User = user;
            ServiceTier = serviceTier;
            Messages = messages ?? new ChangeTrackingList<ChatMessage>();
            Model = model;
            Modalities = modalities ?? new ChangeTrackingList<CreateChatCompletionRequestModality>();
            ReasoningEffort = reasoningEffort;
            MaxCompletionTokens = maxCompletionTokens;
            FrequencyPenalty = frequencyPenalty;
            PresencePenalty = presencePenalty;
            WebSearchOptions = webSearchOptions;
            TopLogprobs = topLogprobs;
            ResponseFormat = responseFormat;
            Audio = audio;
            Store = store;
            Stream = stream;
            Stop = stop;
            LogitBias = logitBias ?? new ChangeTrackingDictionary<string, int>();
            Logprobs = logprobs;
            MaxTokens = maxTokens;
            N = n;
            Prediction = prediction;
            Seed = seed;
            StreamOptions = streamOptions;
            Tools = tools ?? new ChangeTrackingList<ChatTool>();
            ToolChoice = toolChoice;
            ParallelToolCalls = parallelToolCalls;
            FunctionCall = functionCall;
            Functions = functions ?? new ChangeTrackingList<ChatFunction>();
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public IDictionary<string, string> Metadata { get; }

        public float? Temperature { get; set; }

        public float? TopP { get; set; }

        public string User { get; set; }

        public ChatServiceTier? ServiceTier { get; set; }

        public IList<ChatMessage> Messages { get; }

        public string Model { get; }

        public IList<CreateChatCompletionRequestModality> Modalities { get; set; }

        public ChatReasoningEffortLevel? ReasoningEffort { get; set; }

        public int? MaxCompletionTokens { get; set; }

        public float? FrequencyPenalty { get; set; }

        public float? PresencePenalty { get; set; }

        public ChatWebSearchOptions WebSearchOptions { get; set; }

        public int? TopLogprobs { get; set; }

        public ResponseFormat ResponseFormat { get; set; }

        public ChatAudioOptions Audio { get; set; }

        public bool? Store { get; set; }

        public bool? Stream { get; set; }

        public BinaryData Stop { get; set; }

        public IDictionary<string, int> LogitBias { get; set; }

        public bool? Logprobs { get; set; }

        public int? MaxTokens { get; set; }

        public int? N { get; set; }

        public ChatOutputPrediction Prediction { get; set; }

        public long? Seed { get; set; }

        public ChatCompletionStreamOptions StreamOptions { get; set; }

        public IList<ChatTool> Tools { get; }

        public BinaryData ToolChoice { get; set; }

        public bool? ParallelToolCalls { get; set; }

        public BinaryData FunctionCall { get; set; }

        public IList<ChatFunction> Functions { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }

        public static CreateChatCompletionOptions Create(IEnumerable<ChatMessage> messages, ChatClient client, ChatCompletionOptions options = null, bool isStreaming = false)
        {
            Argument.AssertNotNull(messages, nameof(messages));
            options ??= new();
            client.CreateChatCompletionOptions(messages, ref options, isStreaming);

            var request = new CreateChatCompletionOptions(messages, options.Model);

            // Populate request properties from options
            request.Audio = options.AudioOptions;
            request.Temperature = options.Temperature;
            request.TopP = options.TopP;
            request.User = options.EndUserId;
            request.ServiceTier = options.ServiceTier;
            request.ReasoningEffort = options.ReasoningEffortLevel;
            request.MaxCompletionTokens = options.MaxOutputTokenCount;
            request.FrequencyPenalty = options.FrequencyPenalty;
            request.PresencePenalty = options.PresencePenalty;
            request.WebSearchOptions = options.WebSearchOptions;
            request.TopLogprobs = options.TopLogProbabilityCount;
            request.ResponseFormat = options.ResponseFormat switch
            {
                InternalDotNetChatResponseFormatText => new ResponseFormatText(),
                InternalDotNetChatResponseFormatJsonObject => new ResponseFormatJsonObject(),
                InternalDotNetChatResponseFormatJsonSchema js => new ResponseFormatJsonSchema()
                {
                    JsonSchema = new InternalResponseFormatJsonSchemaJsonSchema()
                    {
                        Schema = js.JsonSchema.Schema,
                        Name = js.JsonSchema.Name,
                        Description = js.JsonSchema.Description,
                        Strict = js.JsonSchema.Strict,
                        SerializedAdditionalRawData = js.JsonSchema.SerializedAdditionalRawData
                    }
                },
                _ => null,
            };
            request.Store = options.StoredOutputEnabled;
            request.Logprobs = options.IncludeLogProbabilities;
            request.Prediction = options.OutputPrediction;
            request.Seed = options.Seed;
            request.Stream = options.Stream;
            request.ParallelToolCalls = options.AllowParallelToolCalls;

            // Handle collections and complex types
            if (options.StopSequences != null && options.StopSequences.Count > 0)
            {
                request.Stop = SerializeStopSequences(options.StopSequences);
            }

            if (options.LogitBiases != null && options.LogitBiases.Count > 0)
            {
                foreach (var kvp in options.LogitBiases)
                {
                    request.LogitBias[kvp.Key.ToString()] = kvp.Value;
                }
            }

            if (options.Tools != null && options.Tools.Count > 0)
            {
                foreach (var tool in options.Tools)
                {
                    request.Tools.Add(tool);
                }
            }

            if (options.ToolChoice != null)
            {
                request.ToolChoice = ModelReaderWriter.Write(options.ToolChoice, ModelSerializationExtensions.WireOptions);
            }

            if (options.Functions != null && options.Functions.Count > 0)
            {
                foreach (var function in options.Functions)
                {
                    request.Functions.Add(function);
                }
            }

            if (options.FunctionChoice != null)
            {
                request.FunctionCall = ModelReaderWriter.Write(options.FunctionChoice, ModelSerializationExtensions.WireOptions);
            }

            if (options.Metadata != null && options.Metadata.Count > 0)
            {
                foreach (var kvp in options.Metadata)
                {
                    request.Metadata[kvp.Key] = kvp.Value;
                }
            }

            if (options.ResponseModalities.HasFlag(ChatResponseModalities.Audio))
            {
                request.Modalities.Add(CreateChatCompletionRequestModality.Audio);
            }
            if (options.ResponseModalities.HasFlag(ChatResponseModalities.Text))
            {
                request.Modalities.Add(CreateChatCompletionRequestModality.Text);
            }
            if (options.StreamOptions != null)
            {
                request.StreamOptions = new(options.StreamOptions.IncludeUsage, options.StreamOptions.SerializedAdditionalRawData);
            }

            return request;
        }

        private static BinaryData SerializeStopSequences(IList<string> stopSequences)
        {
            using var stream = new System.IO.MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            writer.WriteStartArray();
            foreach (var item in stopSequences)
            {
                writer.WriteStringValue(item);
            }
            writer.WriteEndArray();
            writer.Flush();
            return new BinaryData(stream.ToArray());
        }

        public BinaryContent Body { get; set; }

        protected override CreateChatCompletionOptions CreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        protected virtual CreateChatCompletionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<CreateChatCompletionOptions>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(CreateChatCompletionOptions)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeCreateChatCompletionRequest(document.RootElement, options);
        }

        internal static CreateChatCompletionOptions DeserializeCreateChatCompletionRequest(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            IDictionary<string, string> metadata = default;
            float? temperature = default;
            float? topP = default;
            string user = default;
            ChatServiceTier? serviceTier = default;
            IList<ChatMessage> messages = default;
            string model = default;
            IList<CreateChatCompletionRequestModality> modalities = default;
            ChatReasoningEffortLevel? reasoningEffort = default;
            int? maxCompletionTokens = default;
            float? frequencyPenalty = default;
            float? presencePenalty = default;
            ChatWebSearchOptions webSearchOptions = default;
            int? topLogprobs = default;
            ResponseFormat responseFormat = default;
            ChatAudioOptions audio = default;
            bool? store = default;
            bool? stream = default;
            BinaryData stop = default;
            IDictionary<string, int> logitBias = default;
            bool? logprobs = default;
            int? maxTokens = default;
            int? n = default;
            ChatOutputPrediction prediction = default;
            long? seed = default;
            ChatCompletionStreamOptions streamOptions = default;
            IList<ChatTool> tools = default;
            BinaryData toolChoice = default;
            bool? parallelToolCalls = default;
            BinaryData functionCall = default;
            IList<ChatFunction> functions = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("metadata"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (var prop0 in prop.Value.EnumerateObject())
                    {
                        if (prop0.Value.ValueKind == JsonValueKind.Null)
                        {
                            dictionary.Add(prop0.Name, null);
                        }
                        else
                        {
                            dictionary.Add(prop0.Name, prop0.Value.GetString());
                        }
                    }
                    metadata = dictionary;
                    continue;
                }
                if (prop.NameEquals("temperature"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        temperature = null;
                        continue;
                    }
                    temperature = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("top_p"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        topP = null;
                        continue;
                    }
                    topP = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("user"u8))
                {
                    user = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("service_tier"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    serviceTier = new ChatServiceTier(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("messages"u8))
                {
                    List<ChatMessage> array = new List<ChatMessage>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ChatMessage.DeserializeChatMessage(item, options));
                    }
                    messages = array;
                    continue;
                }
                if (prop.NameEquals("model"u8))
                {
                    model = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("modalities"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    List<CreateChatCompletionRequestModality> array = new List<CreateChatCompletionRequestModality>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(item.GetString().ToCreateChatCompletionRequestModality());
                    }
                    modalities = array;
                    continue;
                }
                if (prop.NameEquals("reasoning_effort"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        reasoningEffort = null;
                        continue;
                    }
                    reasoningEffort = new ChatReasoningEffortLevel(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("max_completion_tokens"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        maxCompletionTokens = null;
                        continue;
                    }
                    maxCompletionTokens = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("frequency_penalty"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        frequencyPenalty = null;
                        continue;
                    }
                    frequencyPenalty = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("presence_penalty"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        presencePenalty = null;
                        continue;
                    }
                    presencePenalty = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("web_search_options"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    webSearchOptions = ChatWebSearchOptions.DeserializeChatWebSearchOptions(prop.Value, options);
                    continue;
                }
                if (prop.NameEquals("top_logprobs"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        topLogprobs = null;
                        continue;
                    }
                    topLogprobs = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("response_format"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    responseFormat = ResponseFormat.DeserializeResponseFormat(prop.Value, options);
                    continue;
                }
                if (prop.NameEquals("audio"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        audio = null;
                        continue;
                    }
                    audio = ChatAudioOptions.DeserializeChatAudioOptions(prop.Value, options);
                    continue;
                }
                if (prop.NameEquals("store"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        store = null;
                        continue;
                    }
                    store = prop.Value.GetBoolean();
                    continue;
                }
                if (prop.NameEquals("stream"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        stream = null;
                        continue;
                    }
                    stream = prop.Value.GetBoolean();
                    continue;
                }
                if (prop.NameEquals("stop"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        stop = null;
                        continue;
                    }
                    stop = BinaryData.FromString(prop.Value.GetRawText());
                    continue;
                }
                if (prop.NameEquals("logit_bias"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    Dictionary<string, int> dictionary = new Dictionary<string, int>();
                    foreach (var prop0 in prop.Value.EnumerateObject())
                    {
                        dictionary.Add(prop0.Name, prop0.Value.GetInt32());
                    }
                    logitBias = dictionary;
                    continue;
                }
                if (prop.NameEquals("logprobs"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        logprobs = null;
                        continue;
                    }
                    logprobs = prop.Value.GetBoolean();
                    continue;
                }
                if (prop.NameEquals("max_tokens"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        maxTokens = null;
                        continue;
                    }
                    maxTokens = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("n"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        n = null;
                        continue;
                    }
                    n = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("prediction"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        prediction = null;
                        continue;
                    }
                    prediction = ChatOutputPrediction.DeserializeChatOutputPrediction(prop.Value, options);
                    continue;
                }
                if (prop.NameEquals("seed"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        seed = null;
                        continue;
                    }
                    seed = prop.Value.GetInt64();
                    continue;
                }
                if (prop.NameEquals("stream_options"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        streamOptions = null;
                        continue;
                    }
                    streamOptions = ChatCompletionStreamOptions.DeserializeChatCompletionStreamOptions(prop.Value, options);
                    continue;
                }
                if (prop.NameEquals("tools"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    List<ChatTool> array = new List<ChatTool>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ChatTool.DeserializeChatTool(item, options));
                    }
                    tools = array;
                    continue;
                }
                if (prop.NameEquals("tool_choice"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    toolChoice = BinaryData.FromString(prop.Value.GetRawText());
                    continue;
                }
                if (prop.NameEquals("parallel_tool_calls"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    parallelToolCalls = prop.Value.GetBoolean();
                    continue;
                }
                if (prop.NameEquals("function_call"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    functionCall = BinaryData.FromString(prop.Value.GetRawText());
                    continue;
                }
                if (prop.NameEquals("functions"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    List<ChatFunction> array = new List<ChatFunction>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ChatFunction.DeserializeChatFunction(item, options));
                    }
                    functions = array;
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new CreateChatCompletionOptions(
                metadata ?? new ChangeTrackingDictionary<string, string>(),
                temperature,
                topP,
                user,
                serviceTier,
                messages,
                model,
                modalities ?? new ChangeTrackingList<CreateChatCompletionRequestModality>(),
                reasoningEffort,
                maxCompletionTokens,
                frequencyPenalty,
                presencePenalty,
                webSearchOptions,
                topLogprobs,
                responseFormat,
                audio,
                store,
                stream,
                stop,
                logitBias ?? new ChangeTrackingDictionary<string, int>(),
                logprobs,
                maxTokens,
                n,
                prediction,
                seed,
                streamOptions,
                tools ?? new ChangeTrackingList<ChatTool>(),
                toolChoice,
                parallelToolCalls,
                functionCall,
                functions ?? new ChangeTrackingList<ChatFunction>(),
                additionalBinaryDataProperties);
        }

        protected override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            string format = options.Format == "W" ? ((IPersistableModel<CreateChatCompletionOptions>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(CreateChatCompletionOptions)} does not support writing '{format}' format.");
            }
            if (Optional.IsCollectionDefined(Metadata) && _additionalBinaryDataProperties?.ContainsKey("metadata") != true)
            {
                writer.WritePropertyName("metadata"u8);
                writer.WriteStartObject();
                foreach (var item in Metadata)
                {
                    writer.WritePropertyName(item.Key);
                    if (item.Value == null)
                    {
                        writer.WriteNullValue();
                        continue;
                    }
                    writer.WriteStringValue(item.Value);
                }
                writer.WriteEndObject();
            }
            if (Optional.IsDefined(Temperature) && _additionalBinaryDataProperties?.ContainsKey("temperature") != true)
            {
                writer.WritePropertyName("temperature"u8);
                writer.WriteNumberValue(Temperature.Value);
            }
            if (Optional.IsDefined(TopP) && _additionalBinaryDataProperties?.ContainsKey("top_p") != true)
            {
                writer.WritePropertyName("top_p"u8);
                writer.WriteNumberValue(TopP.Value);
            }
            if (Optional.IsDefined(User) && _additionalBinaryDataProperties?.ContainsKey("user") != true)
            {
                writer.WritePropertyName("user"u8);
                writer.WriteStringValue(User);
            }
            if (Optional.IsDefined(ServiceTier) && _additionalBinaryDataProperties?.ContainsKey("service_tier") != true)
            {
                writer.WritePropertyName("service_tier"u8);
                writer.WriteStringValue(ServiceTier.Value.ToString());
            }
            if (_additionalBinaryDataProperties?.ContainsKey("messages") != true)
            {
                writer.WritePropertyName("messages"u8);
                writer.WriteStartArray();
                foreach (ChatMessage item in Messages)
                {
                    writer.WriteObjectValue(item, options);
                }
                writer.WriteEndArray();
            }
            if (_additionalBinaryDataProperties?.ContainsKey("model") != true)
            {
                writer.WritePropertyName("model"u8);
                writer.WriteStringValue(Model.ToString());
            }
            if (Optional.IsCollectionDefined(Modalities) && _additionalBinaryDataProperties?.ContainsKey("modalities") != true)
            {
                writer.WritePropertyName("modalities"u8);
                writer.WriteStartArray();
                foreach (CreateChatCompletionRequestModality item in Modalities)
                {
                    writer.WriteStringValue(item.ToSerialString());
                }
                writer.WriteEndArray();
            }
            if (Optional.IsDefined(ReasoningEffort) && _additionalBinaryDataProperties?.ContainsKey("reasoning_effort") != true)
            {
                writer.WritePropertyName("reasoning_effort"u8);
                writer.WriteStringValue(ReasoningEffort.Value.ToString());
            }
            if (Optional.IsDefined(MaxCompletionTokens) && _additionalBinaryDataProperties?.ContainsKey("max_completion_tokens") != true)
            {
                writer.WritePropertyName("max_completion_tokens"u8);
                writer.WriteNumberValue(MaxCompletionTokens.Value);
            }
            if (Optional.IsDefined(FrequencyPenalty) && _additionalBinaryDataProperties?.ContainsKey("frequency_penalty") != true)
            {
                writer.WritePropertyName("frequency_penalty"u8);
                writer.WriteNumberValue(FrequencyPenalty.Value);
            }
            if (Optional.IsDefined(PresencePenalty) && _additionalBinaryDataProperties?.ContainsKey("presence_penalty") != true)
            {
                writer.WritePropertyName("presence_penalty"u8);
                writer.WriteNumberValue(PresencePenalty.Value);
            }
            if (Optional.IsDefined(WebSearchOptions) && _additionalBinaryDataProperties?.ContainsKey("web_search_options") != true)
            {
                writer.WritePropertyName("web_search_options"u8);
                writer.WriteObjectValue(WebSearchOptions, options);
            }
            if (Optional.IsDefined(TopLogprobs) && _additionalBinaryDataProperties?.ContainsKey("top_logprobs") != true)
            {
                writer.WritePropertyName("top_logprobs"u8);
                writer.WriteNumberValue(TopLogprobs.Value);
            }
            if (Optional.IsDefined(ResponseFormat) && _additionalBinaryDataProperties?.ContainsKey("response_format") != true)
            {
                writer.WritePropertyName("response_format"u8);
                writer.WriteObjectValue(ResponseFormat, options);
            }
            if (Optional.IsDefined(Audio) && _additionalBinaryDataProperties?.ContainsKey("audio") != true)
            {
                writer.WritePropertyName("audio"u8);
                writer.WriteObjectValue(Audio, options);
            }
            if (Optional.IsDefined(Store) && _additionalBinaryDataProperties?.ContainsKey("store") != true)
            {
                writer.WritePropertyName("store"u8);
                writer.WriteBooleanValue(Store.Value);
            }
            if (Optional.IsDefined(Stream) && _additionalBinaryDataProperties?.ContainsKey("stream") != true)
            {
                writer.WritePropertyName("stream"u8);
                writer.WriteBooleanValue(Stream.Value);
            }
            if (Optional.IsDefined(Stop) && _additionalBinaryDataProperties?.ContainsKey("stop") != true)
            {
                writer.WritePropertyName("stop"u8);
#if NET6_0_OR_GREATER
                writer.WriteRawValue(Stop);
#else
                using (JsonDocument document = JsonDocument.Parse(Stop))
                {
                    JsonSerializer.Serialize(writer, document.RootElement);
                }
#endif
            }
            if (Optional.IsCollectionDefined(LogitBias) && _additionalBinaryDataProperties?.ContainsKey("logit_bias") != true)
            {
                writer.WritePropertyName("logit_bias"u8);
                writer.WriteStartObject();
                foreach (var item in LogitBias)
                {
                    writer.WritePropertyName(item.Key);
                    writer.WriteNumberValue(item.Value);
                }
                writer.WriteEndObject();
            }
            if (Optional.IsDefined(Logprobs) && _additionalBinaryDataProperties?.ContainsKey("logprobs") != true)
            {
                writer.WritePropertyName("logprobs"u8);
                writer.WriteBooleanValue(Logprobs.Value);
            }
            if (Optional.IsDefined(MaxTokens) && _additionalBinaryDataProperties?.ContainsKey("max_tokens") != true)
            {
                writer.WritePropertyName("max_tokens"u8);
                writer.WriteNumberValue(MaxTokens.Value);
            }
            if (Optional.IsDefined(N) && _additionalBinaryDataProperties?.ContainsKey("n") != true)
            {
                writer.WritePropertyName("n"u8);
                writer.WriteNumberValue(N.Value);
            }
            if (Optional.IsDefined(Prediction) && _additionalBinaryDataProperties?.ContainsKey("prediction") != true)
            {
                writer.WritePropertyName("prediction"u8);
                writer.WriteObjectValue(Prediction, options);
            }
            if (Optional.IsDefined(Seed) && _additionalBinaryDataProperties?.ContainsKey("seed") != true)
            {
                writer.WritePropertyName("seed"u8);
                writer.WriteNumberValue(Seed.Value);
            }
            if (Optional.IsDefined(StreamOptions) && _additionalBinaryDataProperties?.ContainsKey("stream_options") != true)
            {
                writer.WritePropertyName("stream_options"u8);
                writer.WriteObjectValue(StreamOptions, options);
            }
            if (Optional.IsCollectionDefined(Tools) && _additionalBinaryDataProperties?.ContainsKey("tools") != true)
            {
                writer.WritePropertyName("tools"u8);
                writer.WriteStartArray();
                foreach (ChatTool item in Tools)
                {
                    writer.WriteObjectValue(item, options);
                }
                writer.WriteEndArray();
            }
            if (Optional.IsDefined(ToolChoice) && _additionalBinaryDataProperties?.ContainsKey("tool_choice") != true)
            {
                writer.WritePropertyName("tool_choice"u8);
#if NET6_0_OR_GREATER
                writer.WriteRawValue(ToolChoice);
#else
                using (JsonDocument document = JsonDocument.Parse(ToolChoice))
                {
                    JsonSerializer.Serialize(writer, document.RootElement);
                }
#endif
            }
            if (Optional.IsDefined(ParallelToolCalls) && _additionalBinaryDataProperties?.ContainsKey("parallel_tool_calls") != true)
            {
                writer.WritePropertyName("parallel_tool_calls"u8);
                writer.WriteBooleanValue(ParallelToolCalls.Value);
            }
            if (Optional.IsDefined(FunctionCall) && _additionalBinaryDataProperties?.ContainsKey("function_call") != true)
            {
                writer.WritePropertyName("function_call"u8);
#if NET6_0_OR_GREATER
                writer.WriteRawValue(FunctionCall);
#else
                using (JsonDocument document = JsonDocument.Parse(FunctionCall))
                {
                    JsonSerializer.Serialize(writer, document.RootElement);
                }
#endif
            }
            if (Optional.IsCollectionDefined(Functions) && _additionalBinaryDataProperties?.ContainsKey("functions") != true)
            {
                writer.WritePropertyName("functions"u8);
                writer.WriteStartArray();
                foreach (ChatFunction item in Functions)
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
            writer.WriteEndObject();
        }

        public static implicit operator BinaryContent(CreateChatCompletionOptions createCompletionRequest)
        {
            if (createCompletionRequest == null)
            {
                return null;
            }
            return BinaryContent.Create(createCompletionRequest, ModelSerializationExtensions.WireOptions);
        }
    }
}
