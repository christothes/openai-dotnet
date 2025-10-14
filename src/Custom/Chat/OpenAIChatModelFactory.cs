using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Chat;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIChatModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatCompletion"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatCompletion"/> instance for mocking. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ChatCompletion ChatCompletion(
        string id,
        ChatFinishReason finishReason,
        ChatMessageContent content,
        string refusal,
        IEnumerable<ChatToolCall> toolCalls,
        ChatMessageRole role,
        ChatFunctionCall functionCall,
        IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities,
        IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities,
        DateTimeOffset createdAt,
        string model,
        string systemFingerprint,
        ChatTokenUsage usage) =>
        ChatCompletion(
            id: id,
            finishReason: finishReason,
            content: content,
            refusal: refusal,
            toolCalls: toolCalls,
            role: role,
            functionCall: functionCall,
            contentTokenLogProbabilities: contentTokenLogProbabilities,
            refusalTokenLogProbabilities: refusalTokenLogProbabilities,
            createdAt: createdAt,
            model: model,
            systemFingerprint: systemFingerprint,
            usage: usage,
            outputAudio: default);

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatCompletion"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatCompletion"/> instance for mocking. </returns>
    [Experimental("OPENAI001")]
    public static ChatCompletion ChatCompletion(
        string id = null,
        ChatFinishReason finishReason = default,
        ChatMessageContent content = null,
        string refusal = null,
        IEnumerable<ChatToolCall> toolCalls = null,
        ChatMessageRole role = default,
        ChatFunctionCall functionCall = default,
        IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = null,
        IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = null,
        DateTimeOffset createdAt = default,
        string model = null,
        ChatServiceTier? serviceTier = default,
        string systemFingerprint = null,
        ChatTokenUsage usage = default,
        ChatOutputAudio outputAudio = default,
        IEnumerable<ChatMessageAnnotation> messageAnnotations = default)
    {
        content ??= new ChatMessageContent();
        toolCalls ??= new List<ChatToolCall>();
        contentTokenLogProbabilities ??= new List<ChatTokenLogProbabilityDetails>();
        refusalTokenLogProbabilities ??= new List<ChatTokenLogProbabilityDetails>();
        messageAnnotations ??= new List<ChatMessageAnnotation>();

        InternalChatCompletionResponseMessage message = new(
            refusal: refusal,
            toolCalls: toolCalls.ToList(),
            annotations: messageAnnotations.ToList(),
            audio: outputAudio,
            role: role,
            content: content,
            functionCall: functionCall,
            additionalBinaryDataProperties: null);

        InternalCreateChatCompletionResponseChoiceLogprobs logprobs = new InternalCreateChatCompletionResponseChoiceLogprobs(
            contentTokenLogProbabilities.ToList(),
            refusalTokenLogProbabilities.ToList(),
            additionalBinaryDataProperties: null);

        IReadOnlyList<InternalCreateChatCompletionResponseChoice> choices = [
            new InternalCreateChatCompletionResponseChoice(
                finishReason,
                index: 0,
                message,
                logprobs,
                additionalBinaryDataProperties: null)
        ];

        return new ChatCompletion(
            id: id,
            model: model,
            serviceTier: serviceTier,
            systemFingerprint: systemFingerprint,
            usage: usage,
            @object: "chat.completion",
            choices: choices,
            createdAt: createdAt,
            additionalBinaryDataProperties: null);
    }

    /// <summary>
    /// Creates a new instance of <see cref="ChatMessageAnnotation"/> for mocks and testing.
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    /// <param name="webResourceUri"></param>
    /// <param name="webResourceTitle"></param>
    /// <returns>A new <see cref="OpenAI.Chat.ChatMessageAnnotation"/> instance for mocking.</returns>
    [Experimental("OPENAI001")]
    public static ChatMessageAnnotation ChatMessageAnnotation(
        int startIndex = default,
        int endIndex = default,
        Uri webResourceUri = default,
        string webResourceTitle = default)
    {
        return new ChatMessageAnnotation(
            new InternalChatCompletionResponseMessageAnnotationUrlCitation(
                endIndex,
                startIndex,
                webResourceUri,
                webResourceTitle));
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatTokenLogProbabilityDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatTokenLogProbabilityDetails"/> instance for mocking. </returns>
    public static ChatTokenLogProbabilityDetails ChatTokenLogProbabilityDetails(string token = null, float logProbability = default, ReadOnlyMemory<byte>? utf8Bytes = null, IEnumerable<ChatTokenTopLogProbabilityDetails> topLogProbabilities = null)
    {
        topLogProbabilities ??= new List<ChatTokenTopLogProbabilityDetails>();

        return new ChatTokenLogProbabilityDetails(
            token,
            logProbability,
            utf8Bytes,
            topLogProbabilities.ToList(),
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatTokenTopLogProbabilityDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatTokenTopLogProbabilityDetails"/> instance for mocking. </returns>
    public static ChatTokenTopLogProbabilityDetails ChatTokenTopLogProbabilityDetails(string token = null, float logProbability = default, ReadOnlyMemory<byte>? utf8Bytes = null)
    {
        return new ChatTokenTopLogProbabilityDetails(
            token,
            logProbability,
            utf8Bytes,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatTokenUsage"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatTokenUsage"/> instance for mocking. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ChatTokenUsage ChatTokenUsage(int outputTokenCount, int inputTokenCount, int totalTokenCount, ChatOutputTokenUsageDetails outputTokenDetails) =>
        ChatTokenUsage(
            outputTokenCount: outputTokenCount,
            inputTokenCount: inputTokenCount,
            totalTokenCount: totalTokenCount,
            outputTokenDetails: outputTokenDetails,
            inputTokenDetails: default);

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatTokenUsage"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatTokenUsage"/> instance for mocking. </returns>
    public static ChatTokenUsage ChatTokenUsage(int outputTokenCount = default, int inputTokenCount = default, int totalTokenCount = default, ChatOutputTokenUsageDetails outputTokenDetails = null, ChatInputTokenUsageDetails inputTokenDetails = null)
    {
        return new ChatTokenUsage(
            outputTokenCount: outputTokenCount,
            inputTokenCount: inputTokenCount,
            totalTokenCount: totalTokenCount,
            outputTokenDetails: outputTokenDetails,
            inputTokenDetails: inputTokenDetails,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatInputTokenUsageDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatInputTokenUsageDetails"/> instance for mocking. </returns>
    public static ChatInputTokenUsageDetails ChatInputTokenUsageDetails(int audioTokenCount = default, int cachedTokenCount = default)
    {
        return new ChatInputTokenUsageDetails(
            audioTokenCount: audioTokenCount,
            cachedTokenCount: cachedTokenCount,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatOutputTokenUsageDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatOutputTokenusageDetails"/> instance for mocking. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ChatOutputTokenUsageDetails ChatOutputTokenUsageDetails(int reasoningTokenCount) =>
        ChatOutputTokenUsageDetails(
            reasoningTokenCount: reasoningTokenCount,
            audioTokenCount: default);

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatOutputTokenUsageDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatOutputTokenusageDetails"/> instance for mocking. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ChatOutputTokenUsageDetails ChatOutputTokenUsageDetails(int reasoningTokenCount, int audioTokenCount) =>
        ChatOutputTokenUsageDetails(
            reasoningTokenCount: reasoningTokenCount,
            audioTokenCount: audioTokenCount,
            acceptedPredictionTokenCount: default,
            rejectedPredictionTokenCount: default);

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatOutputTokenUsageDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatOutputTokenusageDetails"/> instance for mocking. </returns>
    [Experimental("OPENAI001")]
    public static ChatOutputTokenUsageDetails ChatOutputTokenUsageDetails(int reasoningTokenCount = default, int audioTokenCount = default, int acceptedPredictionTokenCount = default, int rejectedPredictionTokenCount = default)
    {
        return new ChatOutputTokenUsageDetails(
            audioTokenCount: audioTokenCount,
            reasoningTokenCount: reasoningTokenCount,
            acceptedPredictionTokenCount: acceptedPredictionTokenCount,
            rejectedPredictionTokenCount: rejectedPredictionTokenCount,
            additionalBinaryDataProperties: null);
    }

    [Experimental("OPENAI001")]
    public static ChatOutputAudio ChatOutputAudio(BinaryData audioBytes, string id = null, string transcript = null, DateTimeOffset expiresAt = default)
    {
        return new ChatOutputAudio(
            id: id,
            expiresAt: expiresAt,
            transcript: transcript,
            audioBytes: audioBytes,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatCompletionUpdate"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.StreamingChatCompletionUpdate"/> instance for mocking. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static StreamingChatCompletionUpdate StreamingChatCompletionUpdate(
        string completionId,
        ChatMessageContent contentUpdate,
        StreamingChatFunctionCallUpdate functionCallUpdate,
        IEnumerable<StreamingChatToolCallUpdate> toolCallUpdates,
        ChatMessageRole? role,
        string refusalUpdate,
        IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities,
        IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities,
        ChatFinishReason? finishReason,
        DateTimeOffset createdAt,
        string model,
        string systemFingerprint,
        ChatTokenUsage usage) =>
        StreamingChatCompletionUpdate(
            completionId: completionId,
            contentUpdate: contentUpdate,
            functionCallUpdate: functionCallUpdate,
            toolCallUpdates: toolCallUpdates,
            role: role,
            refusalUpdate: refusalUpdate,
            contentTokenLogProbabilities: contentTokenLogProbabilities,
            refusalTokenLogProbabilities: refusalTokenLogProbabilities,
            finishReason: finishReason,
            createdAt: createdAt,
            model: model,
            systemFingerprint: systemFingerprint,
            usage: usage,
            outputAudioUpdate: default);

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatCompletionUpdate"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.StreamingChatCompletionUpdate"/> instance for mocking. </returns>
    [Experimental("OPENAI001")]
    public static StreamingChatCompletionUpdate StreamingChatCompletionUpdate(
        string completionId = null,
        ChatMessageContent contentUpdate = null,
        StreamingChatFunctionCallUpdate functionCallUpdate = null,
        IEnumerable<StreamingChatToolCallUpdate> toolCallUpdates = null,
        ChatMessageRole? role = default,
        string refusalUpdate = null,
        IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = null,
        IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = null,
        ChatFinishReason? finishReason = default,
        DateTimeOffset createdAt = default,
        string model = null,
        ChatServiceTier? serviceTier = default,
        string systemFingerprint = null,
        ChatTokenUsage usage = default,
        StreamingChatOutputAudioUpdate outputAudioUpdate = default)
    {
        contentUpdate ??= new ChatMessageContent();
        toolCallUpdates ??= new List<StreamingChatToolCallUpdate>();
        contentTokenLogProbabilities ??= new List<ChatTokenLogProbabilityDetails>();
        refusalTokenLogProbabilities ??= new List<ChatTokenLogProbabilityDetails>();

        InternalChatCompletionStreamResponseDelta delta = new InternalChatCompletionStreamResponseDelta(
            audio: outputAudioUpdate,
            functionCall: functionCallUpdate,
            toolCalls: toolCallUpdates.ToList(),
            refusal: refusalUpdate,
            role: role,
            content: contentUpdate,
            additionalBinaryDataProperties: null);

        InternalCreateChatCompletionStreamResponseChoiceLogprobs logprobs = new InternalCreateChatCompletionStreamResponseChoiceLogprobs(
            contentTokenLogProbabilities.ToList(),
            refusalTokenLogProbabilities.ToList(),
            additionalBinaryDataProperties: null);

        IReadOnlyList<InternalCreateChatCompletionStreamResponseChoice> choices = [
            new InternalCreateChatCompletionStreamResponseChoice(
                delta: delta,
                logprobs: logprobs,
                index: 0,
                finishReason: finishReason,
                additionalBinaryDataProperties: null)
        ];

        return new StreamingChatCompletionUpdate(
            model: model,
            serviceTier: serviceTier,
            systemFingerprint: systemFingerprint,
            @object: "chat.completion.chunk",
            completionId: completionId,
            choices: choices,
            createdAt: createdAt,
            usage: usage,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatFunctionCallUpdate"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.StreamingChatFunctionCallUpdate"/> instance for mocking. </returns>
    [Obsolete($"This class is obsolete. Please use {nameof(StreamingChatToolCallUpdate)} instead.")]
    public static StreamingChatFunctionCallUpdate StreamingChatFunctionCallUpdate(string functionName = null, BinaryData functionArgumentsUpdate = null)
    {
        return new StreamingChatFunctionCallUpdate(
            functionName,
            functionArgumentsUpdate,
            additionalBinaryDataProperties: null);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatOutputAudioUpdate"/>.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="expiresAt"></param>
    /// <param name="transcriptUpdate"></param>
    /// <param name="audioBytesUpdate"></param>
    /// <returns>A new <see cref="OpenAI.Chat.StreamingChatOutputAudioUpdate"/> instance for mocking.</returns>
    [Experimental("OPENAI001")]
    public static StreamingChatOutputAudioUpdate StreamingChatOutputAudioUpdate(
        string id = null,
        DateTimeOffset? expiresAt = null,
        string transcriptUpdate = null,
        BinaryData audioBytesUpdate = null)
    {
        return new StreamingChatOutputAudioUpdate(
            id: id,
            expiresAt: expiresAt,
            transcriptUpdate: transcriptUpdate,
            audioBytesUpdate: audioBytesUpdate,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatToolCallUpdate"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.StreamingChatToolCallUpdate"/> instance for mocking. </returns>
    public static StreamingChatToolCallUpdate StreamingChatToolCallUpdate(int index = default, string toolCallId = null, ChatToolCallKind kind = default, string functionName = null, BinaryData functionArgumentsUpdate = null)
    {
        InternalChatCompletionMessageToolCallChunkFunction function = new InternalChatCompletionMessageToolCallChunkFunction(
            functionName,
            functionArgumentsUpdate,
            additionalBinaryDataProperties: null);

        return new StreamingChatToolCallUpdate(
            index: index,
            function: function,
            kind: kind,
            toolCallId: toolCallId,
            additionalBinaryDataProperties: null);
    }

    public static ChatCompletionList ChatCompletionList(string @object = default, IEnumerable<CreateChatCompletionResponse> data = default, string firstId = default, string lastId = default, bool hasMore = default)
    {
        data ??= new ChangeTrackingList<CreateChatCompletionResponse>();

        return new ChatCompletionList(
            @object,
            data.ToList(),
            firstId,
            lastId,
            hasMore,
            additionalBinaryDataProperties: null);
    }

    public static CreateChatCompletionResponse CreateChatCompletionResponse(string id = default, IEnumerable<CreateChatCompletionResponseChoice> choices = default, DateTimeOffset created = default, string model = default, ChatServiceTier? serviceTier = default, string systemFingerprint = default, string @object = default, ChatTokenUsage usage = default)
    {
        choices ??= new ChangeTrackingList<CreateChatCompletionResponseChoice>();

        return new CreateChatCompletionResponse(
            id,
            choices.ToList(),
            created,
            model,
            serviceTier,
            systemFingerprint,
            @object,
            usage,
            additionalBinaryDataProperties: null);
    }

    public static CreateChatCompletionResponseChoice CreateChatCompletionResponseChoice(ChatFinishReason finishReason = default, int index = default, ChatCompletionResponseMessage message = default, CreateChatCompletionResponseChoiceLogprobs logprobs = default)
    {
        return new CreateChatCompletionResponseChoice(finishReason, index, message, logprobs, additionalBinaryDataProperties: null);
    }

    public static CreateChatCompletionResponseChoiceLogprobs CreateChatCompletionResponseChoiceLogprobs(IEnumerable<ChatTokenLogProbabilityDetails> content = default, IEnumerable<ChatTokenLogProbabilityDetails> refusal = default)
    {
        content ??= new ChangeTrackingList<ChatTokenLogProbabilityDetails>();
        refusal ??= new ChangeTrackingList<ChatTokenLogProbabilityDetails>();

        return new CreateChatCompletionResponseChoiceLogprobs(content.ToList(), refusal.ToList(), additionalBinaryDataProperties: null);
    }

    public static ResponseFormatText ResponseFormatText()
    {
        return new ResponseFormatText(ResponseFormatType.Text, additionalBinaryDataProperties: null);
    }

    public static ResponseFormat ResponseFormat(string kind = default)
    {
        return new UnknownResponseFormat(kind.ToResponseFormatType(), additionalBinaryDataProperties: null);
    }

    public static ResponseFormatJsonObject ResponseFormatJsonObject()
    {
        return new ResponseFormatJsonObject(ResponseFormatType.JsonObject, additionalBinaryDataProperties: null);
    }

    public static CreateChatCompletionRequest CreateChatCompletionRequest(IDictionary<string, string> metadata = default, float? temperature = default, float? topP = default, string user = default, ChatServiceTier? serviceTier = default, IEnumerable<ChatMessage> messages = default, string model = default, IEnumerable<CreateChatCompletionRequestModality> modalities = default, ChatReasoningEffortLevel? reasoningEffort = default, int? maxCompletionTokens = default, float? frequencyPenalty = default, float? presencePenalty = default, ChatWebSearchOptions webSearchOptions = default, int? topLogprobs = default, ResponseFormat responseFormat = default, ChatAudioOptions audio = default, bool? store = default, bool? stream = default, BinaryData stop = default, IDictionary<string, int> logitBias = default, bool? logprobs = default, int? maxTokens = default, int? n = default, ChatOutputPrediction prediction = default, long? seed = default, ChatCompletionStreamOptions streamOptions = default, IEnumerable<ChatTool> tools = default, BinaryData toolChoice = default, bool? parallelToolCalls = default, BinaryData functionCall = default, IEnumerable<ChatFunction> functions = default)
    {
        metadata ??= new ChangeTrackingDictionary<string, string>();
        messages ??= new ChangeTrackingList<ChatMessage>();
        modalities ??= new ChangeTrackingList<CreateChatCompletionRequestModality>();
        logitBias ??= new ChangeTrackingDictionary<string, int>();
        tools ??= new ChangeTrackingList<ChatTool>();
        functions ??= new ChangeTrackingList<ChatFunction>();

        return new CreateChatCompletionRequest(
            metadata,
            temperature,
            topP,
            user,
            serviceTier,
            messages.ToList(),
            model,
            modalities.ToList(),
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
            logitBias,
            logprobs,
            maxTokens,
            n,
            prediction,
            seed,
            streamOptions,
            tools.ToList(),
            toolChoice,
            parallelToolCalls,
            functionCall,
            functions.ToList(),
            additionalBinaryDataProperties: null);
    }

    public static ChatCompletionStreamOptions ChatCompletionStreamOptions(bool? includeUsage = default)
    {
        return new ChatCompletionStreamOptions(includeUsage, additionalBinaryDataProperties: null);
    }

    public static ResponseFormat ChatResponseFormat(string kind = default)
    {
        return kind switch
        {
            nameof(ResponseFormatType.Text) => new ResponseFormatText(),
            nameof(ResponseFormatType.JsonObject) => new ResponseFormatJsonObject(),
            nameof(ResponseFormatType.JsonSchema) => new ResponseFormatJsonSchema(),
            _ => new UnknownResponseFormat(),
        };
    }

    public static ChatCompletionRequestSystemMessage ChatCompletionRequestSystemMessage(ChatMessageContent content = default, string name = default)
    {
        return new ChatCompletionRequestSystemMessage(default, content, additionalBinaryDataProperties: null, name);
    }

    public static ChatCompletionRequestDeveloperMessage ChatCompletionRequestDeveloperMessage(ChatMessageContent content = default, string name = default)
    {
        return new ChatCompletionRequestDeveloperMessage(default, content, additionalBinaryDataProperties: null, name);
    }

    public static ChatCompletionRequestUserMessage ChatCompletionRequestUserMessage(ChatMessageContent content = default, string name = default)
    {
        return new ChatCompletionRequestUserMessage(default, content, additionalBinaryDataProperties: null, name);
    }

    public static ChatCompletionRequestAssistantMessage ChatCompletionRequestAssistantMessage(ChatMessageContent content = default, string refusal = default, string name = default, ChatOutputAudioReference audio = default, IEnumerable<ChatToolCall> toolCalls = default, ChatFunctionCall functionCall = default)
    {
        toolCalls ??= new ChangeTrackingList<ChatToolCall>();

        return new ChatCompletionRequestAssistantMessage(
            default,
            content,
            additionalBinaryDataProperties: null,
            refusal,
            name,
            audio,
            toolCalls.ToList(),
            functionCall);
    }

    public static ChatCompletionRequestToolMessage ChatCompletionRequestToolMessage(ChatMessageContent content = default, string toolCallId = default)
    {
        return new ChatCompletionRequestToolMessage(default, content, additionalBinaryDataProperties: null, toolCallId);
    }

    public static ChatCompletionResponseMessageFunctionCall ChatCompletionResponseMessageFunctionCall(string name = default, string arguments = default)
    {
        return new ChatCompletionResponseMessageFunctionCall(name, arguments, additionalBinaryDataProperties: null);
    }

    public static ChatCompletionMessageListDatum ChatCompletionMessageListDatum(string content = default, string refusal = default, IEnumerable<ChatToolCall> toolCalls = default, IEnumerable<ChatMessageAnnotation> annotations = default, ChatMessageRole role = default, ChatCompletionResponseMessageFunctionCall functionCall = default, ChatOutputAudio outputAudio = default, string id = default)
    {
        toolCalls ??= new ChangeTrackingList<ChatToolCall>();
        annotations ??= new ChangeTrackingList<ChatMessageAnnotation>();

        return new ChatCompletionMessageListDatum(
            content,
            refusal,
            toolCalls.ToList(),
            annotations.ToList(),
            role,
            functionCall,
            outputAudio,
            id,
            additionalBinaryDataProperties: null);
    }

    public static ChatCompletionMessageList ChatCompletionMessageList(string @object = default, IEnumerable<ChatCompletionMessageListDatum> data = default, string firstId = default, string lastId = default, bool hasMore = default)
    {
        data ??= new ChangeTrackingList<ChatCompletionMessageListDatum>();

        return new ChatCompletionMessageList(
            @object,
            data.ToList(),
            firstId,
            lastId,
            hasMore,
            additionalBinaryDataProperties: null);
    }
}
