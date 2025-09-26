using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OpenAI.Chat;
using OpenAI.Internal;

namespace OpenAI
{
    [Experimental("OPENAI001")]
    public partial class CreateChatCompletionRequest
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        public CreateChatCompletionRequest(IEnumerable<ChatMessage> messages, string model)
        {
            Argument.AssertNotNull(messages, nameof(messages));

            Metadata = new ChangeTrackingDictionary<string, string>();
            Messages = messages.ToList();
            Model = model;
            Modalities = new ChangeTrackingList<InternalCreateChatCompletionRequestModality>();
            LogitBias = new ChangeTrackingDictionary<string, int>();
            Tools = new ChangeTrackingList<ChatTool>();
            Functions = new ChangeTrackingList<ChatFunction>();
        }

        internal CreateChatCompletionRequest(IDictionary<string, string> metadata, float? temperature, float? topP, string user, ChatServiceTier? serviceTier, IList<ChatMessage> messages, string model, IList<InternalCreateChatCompletionRequestModality> modalities, ChatReasoningEffortLevel? reasoningEffort, int? maxCompletionTokens, float? frequencyPenalty, float? presencePenalty, ChatWebSearchOptions webSearchOptions, int? topLogprobs, InternalResponseFormat responseFormat, ChatAudioOptions audio, bool? store, bool? stream, BinaryData stop, IDictionary<string, int> logitBias, bool? logprobs, int? maxTokens, int? n, ChatOutputPrediction prediction, long? seed, ChatCompletionStreamOptions streamOptions, IList<ChatTool> tools, BinaryData toolChoice, bool? parallelToolCalls, BinaryData functionCall, IList<ChatFunction> functions, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            // Plugin customization: ensure initialization of collections
            Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
            Temperature = temperature;
            TopP = topP;
            User = user;
            ServiceTier = serviceTier;
            Messages = messages ?? new ChangeTrackingList<ChatMessage>();
            Model = model;
            Modalities = modalities ?? new ChangeTrackingList<InternalCreateChatCompletionRequestModality>();
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

        public string Model { get; set; }

        internal IList<InternalCreateChatCompletionRequestModality> Modalities { get; set; }

        public ChatReasoningEffortLevel? ReasoningEffort { get; set; }

        public int? MaxCompletionTokens { get; set; }

        public float? FrequencyPenalty { get; set; }

        public float? PresencePenalty { get; set; }

        public ChatWebSearchOptions WebSearchOptions { get; set; }

        public int? TopLogprobs { get; set; }

        internal InternalResponseFormat ResponseFormat { get; set; }

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

        /// <summary>
        /// Creates a <see cref="CreateChatCompletionRequest"/> with the specified messages and the model from the provided <see cref="ChatClient"/>.
        /// </summary>
        /// <param name="messages">The messages to include in the request.</param>
        /// <param name="client">The chat client providing the model.</param>
        /// <returns>A new instance of <see cref="CreateChatCompletionRequest"/>.</returns>
        public static CreateChatCompletionRequest Create(IEnumerable<ChatMessage> messages, ChatClient client)
        {
            return new CreateChatCompletionRequest(messages, client.Model);
        }
    }
}
