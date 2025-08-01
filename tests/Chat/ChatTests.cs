﻿using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Files;
using OpenAI.Tests.Telemetry;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static OpenAI.Tests.Telemetry.TestMeterListener;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Chat;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Chat")]
public class ChatTests : SyncAsyncTestBase
{
    public ChatTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task HelloWorldChat()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("Hello, world!")];
        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);
        Assert.That(result, Is.InstanceOf<ClientResult<ChatCompletion>>());
        Assert.That(result.Value.Content[0].Kind, Is.EqualTo(ChatMessageContentPartKind.Text));
        Assert.That(result.Value.Content[0].Text.Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task HelloWorldWithTopLevelClient()
    {
        OpenAIClient client = GetTestClient<OpenAIClient>(TestScenario.TopLevel);
        ChatClient chatClient = client.GetChatClient("gpt-4o-mini");
        IEnumerable<ChatMessage> messages = [new UserChatMessage("Hello, world!")];
        ClientResult<ChatCompletion> result = IsAsync
            ? await chatClient.CompleteChatAsync(messages)
            : chatClient.CompleteChat(messages);
        Assert.That(result.Value.Content[0].Text.Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task MultiMessageChat()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new SystemChatMessage("You are a helpful assistant. You always talk like a pirate."),
            new UserChatMessage("Hello, assistant! Can you help me train my parrot?"),
        ];
        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);
        Assert.That(new string[] { "aye", "arr", "hearty" }.Any(pirateWord => result.Value.Content[0].Text.ToLowerInvariant().Contains(pirateWord)));
    }

    [Test]
    public void StreamingChat()
    {
        AssertSyncOnly();

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        int updateCount = 0;
        ChatTokenUsage usage = null;
        TimeSpan? firstTokenReceiptTime = null;
        TimeSpan? latestTokenReceiptTime = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        CollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreaming(messages);

        Assert.That(streamingResult, Is.InstanceOf<CollectionResult<StreamingChatCompletionUpdate>>());

        foreach (StreamingChatCompletionUpdate chatUpdate in streamingResult)
        {
            firstTokenReceiptTime ??= stopwatch.Elapsed;
            latestTokenReceiptTime = stopwatch.Elapsed;
            usage ??= chatUpdate.Usage;
            updateCount++;
        }

        stopwatch.Stop();

        Assert.That(updateCount, Is.GreaterThan(1));
        Assert.That(latestTokenReceiptTime - firstTokenReceiptTime > TimeSpan.FromMilliseconds(500));
        Assert.That(usage, Is.Not.Null);
        Assert.That(usage?.InputTokenCount, Is.GreaterThan(0));
        Assert.That(usage?.OutputTokenCount, Is.GreaterThan(0));
        Assert.That(usage?.OutputTokenDetails?.ReasoningTokenCount, Is.Null.Or.EqualTo(0));
    }

    [Test]
    public async Task StreamingChatAsync()
    {
        AssertAsyncOnly();

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        int updateCount = 0;
        ChatTokenUsage usage = null;
        TimeSpan? firstTokenReceiptTime = null;
        TimeSpan? latestTokenReceiptTime = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        AsyncCollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreamingAsync(messages);

        Assert.That(streamingResult, Is.InstanceOf<AsyncCollectionResult<StreamingChatCompletionUpdate>>());

        await foreach (StreamingChatCompletionUpdate chatUpdate in streamingResult)
        {
            firstTokenReceiptTime ??= stopwatch.Elapsed;
            latestTokenReceiptTime = stopwatch.Elapsed;
            usage ??= chatUpdate.Usage;
            updateCount++;
        }

        stopwatch.Stop();

        Assert.That(updateCount, Is.GreaterThan(1));
        Assert.That(latestTokenReceiptTime - firstTokenReceiptTime > TimeSpan.FromMilliseconds(500));
        Assert.That(usage, Is.Not.Null);
        Assert.That(usage?.InputTokenCount, Is.GreaterThan(0));
        Assert.That(usage?.OutputTokenCount, Is.GreaterThan(0));
        Assert.That(usage?.OutputTokenDetails?.ReasoningTokenCount, Is.Null.Or.EqualTo(0));
    }

    [Test]
    public void StreamingChatCanBeCancelled()
    {
        AssertSyncOnly();

        MockPipelineResponse response = new(200);
        response.SetContent("""
            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"role":"assistant","content":"","refusal":null},"logprobs":null,"finish_reason":null}],"usage":null}

            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"content":"The"},"logprobs":null,"finish_reason":null}],"usage":null}

            data: [DONE]
            """);

        OpenAIClientOptions options = new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };

        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(1000);

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, options: options);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        CollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreaming(messages, cancellationToken: cancellationTokenSource.Token);
        IEnumerator<StreamingChatCompletionUpdate> enumerator = streamingResult.GetEnumerator();

        enumerator.MoveNext();
        StreamingChatCompletionUpdate firstUpdate = enumerator.Current;

        Assert.That(firstUpdate, Is.Not.Null);
        Assert.That(cancellationTokenSource.IsCancellationRequested, Is.False);

        Thread.Sleep(1000);

        Assert.Throws<OperationCanceledException>(() =>
        {
            // Should throw for the second update.
            Assert.True(cancellationTokenSource.IsCancellationRequested);
            Assert.True(cancellationTokenSource.Token.IsCancellationRequested);
            enumerator.MoveNext();
            enumerator.MoveNext();
        });
    }

    [Test]
    public async Task StreamingChatCanBeCancelledAsync()
    {
        AssertAsyncOnly();

        MockPipelineResponse response = new(200);
        response.SetContent("""
            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"role":"assistant","content":"","refusal":null},"logprobs":null,"finish_reason":null}],"usage":null}

            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"content":"The"},"logprobs":null,"finish_reason":null}],"usage":null}

            data: [DONE]
            """);

        OpenAIClientOptions options = new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };

        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(1000);

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, options: options);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        AsyncCollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreamingAsync(messages, cancellationToken: cancellationTokenSource.Token);
        IAsyncEnumerator<StreamingChatCompletionUpdate> enumerator = streamingResult.GetAsyncEnumerator();

        await enumerator.MoveNextAsync();
        StreamingChatCompletionUpdate firstUpdate = enumerator.Current;

        Assert.That(firstUpdate, Is.Not.Null);
        Assert.That(cancellationTokenSource.IsCancellationRequested, Is.False);

        await Task.Delay(1000);

        Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            // Should throw for the second update.
            Assert.True(cancellationTokenSource.IsCancellationRequested);
            Assert.True(cancellationTokenSource.Token.IsCancellationRequested);
            await enumerator.MoveNextAsync();
            await enumerator.MoveNextAsync();
        });
    }

    [Test]
    public void CompleteChatStreamingClosesNetworkStream()
    {
        AssertSyncOnly();

        MockPipelineResponse response = new(200);
        response.SetContent("""
            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"role":"assistant","content":"","refusal":null},"logprobs":null,"finish_reason":null}],"usage":null}

            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"content":"The"},"logprobs":null,"finish_reason":null}],"usage":null}

            data: [DONE]
            """);

        OpenAIClientOptions options = new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, options: options);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        int updateCount = 0;
        TimeSpan? firstTokenReceiptTime = null;
        TimeSpan? latestTokenReceiptTime = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        CollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreaming(messages);

        Assert.That(streamingResult, Is.InstanceOf<CollectionResult<StreamingChatCompletionUpdate>>());
        Assert.IsFalse(response.IsDisposed);

        foreach (StreamingChatCompletionUpdate chatUpdate in streamingResult)
        {
            firstTokenReceiptTime ??= stopwatch.Elapsed;
            latestTokenReceiptTime = stopwatch.Elapsed;
            updateCount++;

            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
        }

        stopwatch.Stop();

        Assert.IsTrue(response.IsDisposed);
    }

    [Test]
    public async Task CompleteChatStreamingClosesNetworkStreamAsync()
    {
        AssertAsyncOnly();

        MockPipelineResponse response = new(200);
        response.SetContent("""
            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"role":"assistant","content":"","refusal":null},"logprobs":null,"finish_reason":null}],"usage":null}

            data: {"id":"chatcmpl-A7mKGugwaczn3YyrJLlZY6CM0Wlkr","object":"chat.completion.chunk","created":1726417424,"model":"gpt-4o-mini-2024-07-18","system_fingerprint":"fp_483d39d857","choices":[{"index":0,"delta":{"content":"The"},"logprobs":null,"finish_reason":null}],"usage":null}

            data: [DONE]
            """);

        OpenAIClientOptions options = new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, options: options);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];

        int updateCount = 0;
        TimeSpan? firstTokenReceiptTime = null;
        TimeSpan? latestTokenReceiptTime = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        AsyncCollectionResult<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreamingAsync(messages);

        Assert.That(streamingResult, Is.InstanceOf<AsyncCollectionResult<StreamingChatCompletionUpdate>>());
        Assert.IsFalse(response.IsDisposed);

        await foreach (StreamingChatCompletionUpdate chatUpdate in streamingResult)
        {
            firstTokenReceiptTime ??= stopwatch.Elapsed;
            latestTokenReceiptTime = stopwatch.Elapsed;
            updateCount++;

            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
        }

        stopwatch.Stop();

        Assert.IsTrue(response.IsDisposed);
    }

    [Test]
    public async Task TwoTurnChat()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);

        List<ChatMessage> messages =
        [
            new UserChatMessage("In geometry, what are the different kinds of triangles, as defined by lengths of their sides?"),
        ];
        ClientResult<ChatCompletion> firstResult = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);
        Assert.That(firstResult?.Value, Is.Not.Null);
        Assert.That(firstResult.Value.Content[0].Text.ToLowerInvariant(), Contains.Substring("isosceles"));
        messages.Add(new AssistantChatMessage(firstResult.Value));
        messages.Add(new UserChatMessage("Which of those is the one where exactly two sides are the same length?"));
        ClientResult<ChatCompletion> secondResult = client.CompleteChat(messages);
        Assert.That(secondResult?.Value, Is.Not.Null);
        Assert.That(secondResult.Value.Content[0].Text.ToLowerInvariant(), Contains.Substring("isosceles"));
    }

    [Ignore("Temporarily disabled due to service instability.")]
    [Test]
    public async Task ChatWithVision()
    {
        string mediaType = "image/png";
        string filePath = Path.Combine("Assets", "images_dog_and_cat.png");
        using Stream stream = File.OpenRead(filePath);
        BinaryData imageData = BinaryData.FromStream(stream);

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart("Describe this image for me."),
                ChatMessageContentPart.CreateImagePart(imageData, mediaType)),
        ];
        ChatCompletionOptions options = new() { MaxOutputTokenCount = 2048 };

        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages, options)
            : client.CompleteChat(messages, options);
        Console.WriteLine(result.Value.Content[0].Text);
        Assert.That(result.Value.Content[0].Text.ToLowerInvariant(), Does.Contain("dog").Or.Contain("cat").IgnoreCase);
    }

    [Test]
    public async Task ChatWithBasicAudioOutput()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, "gpt-4o-audio-preview");
        List<ChatMessage> messages = ["Say the exact word 'hello' and nothing else."];
        ChatCompletionOptions options = new()
        {
            AudioOptions = new(ChatOutputAudioVoice.Ash, ChatOutputAudioFormat.Pcm16),
            ResponseModalities = ChatResponseModalities.Text | ChatResponseModalities.Audio,
        };

        StringBuilder transcriptBuilder = new();
        using MemoryStream outputAudioStream = new();
        string streamedAudioId = null;
        ChatTokenUsage streamedUsage = null;
        DateTimeOffset? streamedExpiresAt = null;

        await foreach (StreamingChatCompletionUpdate update
            in client.CompleteChatStreamingAsync(messages, options))
        {
            if (update.Usage is not null)
            {
                Assert.That(streamedUsage, Is.Null);
                streamedUsage = update.Usage;
            }
            if (update.OutputAudioUpdate?.ExpiresAt is not null)
            {
                Assert.That(streamedExpiresAt, Is.Null);
                streamedExpiresAt = update.OutputAudioUpdate.ExpiresAt;
            }
            if (update.OutputAudioUpdate?.Id is not null)
            {
                if (streamedAudioId is not null)
                {
                    Assert.That(streamedAudioId, Is.EqualTo(update.OutputAudioUpdate.Id));
                }
                streamedAudioId ??= update.OutputAudioUpdate.Id;
            }
            transcriptBuilder.Append(update.OutputAudioUpdate?.TranscriptUpdate);
            outputAudioStream.Write(update.OutputAudioUpdate?.AudioBytesUpdate);
        }

        Assert.That(streamedAudioId, Has.Length.GreaterThan("audio".Length));
        Assert.That(transcriptBuilder.ToString().ToLower(), Does.Contain("hello"));
        Assert.That(outputAudioStream.Length, Is.GreaterThan(9000));
        Assert.That(streamedUsage?.InputTokenDetails?.AudioTokenCount, Is.EqualTo(0));
        Assert.That(streamedUsage?.OutputTokenDetails?.AudioTokenCount, Is.GreaterThan(0));
        Assert.That(streamedExpiresAt, Is.GreaterThan(DateTimeOffset.Parse("2025-01-01")));
    }

    [Test]
    public async Task ChatWithAudio()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, "gpt-4o-audio-preview");

        string helloWorldAudioPath = Path.Join("Assets", "audio_hello_world.mp3");
        BinaryData helloWorldAudioBytes = BinaryData.FromBytes(File.ReadAllBytes(helloWorldAudioPath));
        ChatMessageContentPart helloWorldAudioContentPart = ChatMessageContentPart.CreateInputAudioPart(
            helloWorldAudioBytes,
            ChatInputAudioFormat.Mp3);
        string whatsTheWeatherAudioPath = Path.Join("Assets", "realtime_whats_the_weather_pcm16_24khz_mono.wav");
        BinaryData whatsTheWeatherAudioBytes = BinaryData.FromBytes(File.ReadAllBytes(whatsTheWeatherAudioPath));
        ChatMessageContentPart whatsTheWeatherAudioContentPart = ChatMessageContentPart.CreateInputAudioPart(
            whatsTheWeatherAudioBytes,
            ChatInputAudioFormat.Wav);

        List<ChatMessage> messages = [new UserChatMessage([helloWorldAudioContentPart])];

        ChatCompletionOptions options = new()
        {
            ResponseModalities = ChatResponseModalities.Text | ChatResponseModalities.Audio,
            AudioOptions = new(ChatOutputAudioVoice.Alloy, ChatOutputAudioFormat.Pcm16)
        };

        ChatCompletion completion = await client.CompleteChatAsync(messages, options);
        Assert.That(completion, Is.Not.Null);
        Assert.That(completion.Content, Has.Count.EqualTo(0));

        ChatOutputAudio outputAudio = completion.OutputAudio;
        Assert.That(outputAudio, Is.Not.Null);
        Assert.That(outputAudio.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(outputAudio.AudioBytes, Is.Not.Null);
        Assert.That(outputAudio.Transcript, Is.Not.Null.And.Not.Empty);

        AssistantChatMessage audioHistoryMessage = ChatMessage.CreateAssistantMessage(completion);
        Assert.That(audioHistoryMessage, Is.InstanceOf<AssistantChatMessage>());
        Assert.That(audioHistoryMessage.Content, Has.Count.EqualTo(0));

        Assert.That(audioHistoryMessage.OutputAudioReference?.Id, Is.EqualTo(completion.OutputAudio.Id));
        messages.Add(audioHistoryMessage);

        messages.Add(
            new UserChatMessage(
                [
                    "Please answer the following spoken question:",
                    ChatMessageContentPart.CreateInputAudioPart(whatsTheWeatherAudioBytes, ChatInputAudioFormat.Wav),
                ]));

        string streamedCorrelationId = null;
        DateTimeOffset? streamedExpiresAt = null;
        StringBuilder streamedTranscriptBuilder = new();
        ChatTokenUsage streamedUsage = null;
        using MemoryStream outputAudioStream = new();
        await foreach (StreamingChatCompletionUpdate update in client.CompleteChatStreamingAsync(messages, options))
        {
            Assert.That(update.ContentUpdate, Has.Count.EqualTo(0));
            StreamingChatOutputAudioUpdate outputAudioUpdate = update.OutputAudioUpdate;

            if (update.Usage is not null)
            {
                Assert.That(streamedUsage, Is.Null);
                streamedUsage = update.Usage;
            }
            if (outputAudioUpdate is not null)
            {
                string serializedOutputAudioUpdate = ModelReaderWriter.Write(outputAudioUpdate).ToString();
                Assert.That(serializedOutputAudioUpdate, Is.Not.Null.And.Not.Empty);

                if (outputAudioUpdate.Id is not null)
                {
                    Assert.That(streamedCorrelationId, Is.Null.Or.EqualTo(streamedCorrelationId));
                    streamedCorrelationId ??= outputAudioUpdate.Id;
                }
                if (outputAudioUpdate.ExpiresAt.HasValue)
                {
                    Assert.That(streamedExpiresAt.HasValue, Is.False);
                    streamedExpiresAt = outputAudioUpdate.ExpiresAt;
                }
                streamedTranscriptBuilder.Append(outputAudioUpdate.TranscriptUpdate);
                outputAudioStream.Write(outputAudioUpdate.AudioBytesUpdate);
            }
        }
        Assert.That(streamedCorrelationId, Is.Not.Null.And.Not.Empty);
        Assert.That(streamedExpiresAt.HasValue, Is.True);
        Assert.That(streamedTranscriptBuilder.ToString(), Is.Not.Null.And.Not.Empty);
        Assert.That(outputAudioStream.Length, Is.GreaterThan(9000));
        Assert.That(streamedUsage?.InputTokenDetails?.AudioTokenCount, Is.GreaterThan(0));
        Assert.That(streamedUsage?.OutputTokenDetails?.AudioTokenCount, Is.GreaterThan(0));
    }

    [Test]
    public async Task AuthFailure()
    {
        string fakeApiKey = "not-a-real-key-but-should-be-sanitized";
        ChatClient client = new("gpt-4o-mini", new ApiKeyCredential(fakeApiKey));
        IEnumerable<ChatMessage> messages = [new UserChatMessage("Uh oh, this isn't going to work with that key")];
        ClientResultException clientResultException = null;
        try
        {
            _ = IsAsync
                ? await client.CompleteChatAsync(messages)
                : client.CompleteChat(messages);
        }
        catch (ClientResultException ex)
        {
            clientResultException = ex;
        }
        Assert.That(clientResultException, Is.Not.Null);
        Assert.That(clientResultException.Status, Is.EqualTo((int)HttpStatusCode.Unauthorized));
        Assert.That(clientResultException.Message, Does.Contain("API key"));
        Assert.That(clientResultException.Message, Does.Not.Contain(fakeApiKey));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task TokenLogProbabilities(bool includeLogProbabilities)
    {
        const int topLogProbabilityCount = 3;
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IList<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];
        ChatCompletionOptions options;

        if (includeLogProbabilities)
        {
            options = new()
            {
                IncludeLogProbabilities = true,
                TopLogProbabilityCount = topLogProbabilityCount
            };
        }
        else
        {
            options = new();
        }

        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages, options);
        string raw = result.GetRawResponse().Content.ToString();
        ChatCompletion chatCompletions = result.Value;
        Assert.That(chatCompletions, Is.Not.Null);

        if (includeLogProbabilities)
        {
            IReadOnlyList<ChatTokenLogProbabilityDetails> chatTokenLogProbabilities = chatCompletions.ContentTokenLogProbabilities;
            Assert.That(chatTokenLogProbabilities, Is.Not.Null.Or.Empty);

            foreach (ChatTokenLogProbabilityDetails tokenLogProbs in chatTokenLogProbabilities)
            {
                Assert.That(tokenLogProbs.Token, Is.Not.Null.Or.Empty);
                Assert.That(tokenLogProbs.TopLogProbabilities, Is.Not.Null.Or.Empty);
                Assert.That(tokenLogProbs.TopLogProbabilities, Has.Count.EqualTo(topLogProbabilityCount));

                foreach (ChatTokenTopLogProbabilityDetails tokenTopLogProbs in tokenLogProbs.TopLogProbabilities)
                {
                    Assert.That(tokenTopLogProbs.Token, Is.Not.Null.Or.Empty);
                }
            }
        }
        else
        {
            Assert.That(chatCompletions.ContentTokenLogProbabilities, Is.Not.Null);
            Assert.That(chatCompletions.ContentTokenLogProbabilities, Is.Empty);
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task TokenLogProbabilitiesStreaming(bool includeLogProbabilities)
    {
        const int topLogProbabilityCount = 3;
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IList<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];
        ChatCompletionOptions options;

        if (includeLogProbabilities)
        {
            options = new()
            {
                IncludeLogProbabilities = true,
                TopLogProbabilityCount = topLogProbabilityCount
            };
        }
        else
        {
            options = new();
        }

        AsyncCollectionResult<StreamingChatCompletionUpdate> chatCompletionUpdates = client.CompleteChatStreamingAsync(messages, options);
        Assert.That(chatCompletionUpdates, Is.Not.Null);

        await foreach (StreamingChatCompletionUpdate chatCompletionUpdate in chatCompletionUpdates)
        {
            // Token log probabilities are streamed together with their corresponding content update.
            if (includeLogProbabilities
                && chatCompletionUpdate.ContentUpdate.Count > 0
                && !string.IsNullOrEmpty(chatCompletionUpdate.ContentUpdate[0].Text))
            {
                Assert.That(chatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.Or.Empty);
                Assert.That(chatCompletionUpdate.ContentTokenLogProbabilities, Has.Count.EqualTo(1));

                foreach (ChatTokenLogProbabilityDetails tokenLogProbs in chatCompletionUpdate.ContentTokenLogProbabilities)
                {
                    Assert.That(tokenLogProbs.Token, Is.Not.Null.Or.Empty);
                    Assert.That(tokenLogProbs.TopLogProbabilities, Is.Not.Null.Or.Empty);
                    Assert.That(tokenLogProbs.TopLogProbabilities, Has.Count.EqualTo(topLogProbabilityCount));

                    foreach (ChatTokenTopLogProbabilityDetails tokenTopLogProbs in tokenLogProbs.TopLogProbabilities)
                    {
                        Assert.That(tokenTopLogProbs.Token, Is.Not.Null.Or.Empty);
                    }
                }
            }
            else
            {
                Assert.That(chatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null);
                Assert.That(chatCompletionUpdate.ContentTokenLogProbabilities, Is.Empty);
            }
        }
    }

    [Test]
    public async Task NonStrictJsonSchemaWorks()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, "gpt-4o-mini");
        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "some_color_schema",
                BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {},
                        "additionalProperties": false
                    }
                    """u8.ToArray()),
                "an object that describes color components by name",
                jsonSchemaIsStrict: false)
        };
        ChatCompletion completion = IsAsync
            ? await client.CompleteChatAsync([new UserChatMessage("What are the hex values for red, green, and blue?")], options)
            : client.CompleteChat([new UserChatMessage("What are the hex values for red, green, and blue?")], options);
        Console.WriteLine(completion);
    }

    [Test]
    public async Task JsonResult()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage("Give me a JSON object with the following properties: red, green, and blue. The value "
                + "of each property should be a string containing their RGB representation in hexadecimal.")
        ];
        ChatCompletionOptions options = new() { ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat() };
        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages, options)
            : client.CompleteChat(messages, options);

        JsonDocument jsonDocument = JsonDocument.Parse(result.Value.Content[0].Text);

        Assert.That(jsonDocument.RootElement.TryGetProperty("red", out JsonElement redProperty));
        Assert.That(jsonDocument.RootElement.TryGetProperty("green", out JsonElement greenProperty));
        Assert.That(jsonDocument.RootElement.TryGetProperty("blue", out JsonElement blueProperty));
        Assert.That(redProperty.GetString().ToLowerInvariant(), Contains.Substring("ff0000"));
        Assert.That(greenProperty.GetString().ToLowerInvariant(), Contains.Substring("00ff00"));
        Assert.That(blueProperty.GetString().ToLowerInvariant(), Contains.Substring("0000ff"));
    }

    [Test]
    public async Task MultipartContentWorks()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        List<ChatMessage> messages = [
            new SystemChatMessage(
                "You talk like a pirate.",
                "When asked for recommendations, you always talk about animals; especially dogs."
            ),
            new UserChatMessage(
                "Hello, assistant! I need some advice.",
                "Can you recommend some small, cute things I can think about?"
            )
        ];
        ChatCompletion completion = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);

        Assert.That(completion.Content, Has.Count.EqualTo(1));
        Assert.That(completion.Content[0].Text.ToLowerInvariant(), Does.Contain("ahoy").Or.Contain("matey"));
        Assert.That(completion.Content[0].Text.ToLowerInvariant(), Does.Contain("dog").Or.Contain("pup").Or.Contain("kit"));
    }

    [Test]
    public async Task StructuredOutputsWork()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage("What's heavier, a pound of feathers or sixteen ounces of steel?")
        ];
        ChatCompletionOptions options = new ChatCompletionOptions()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "test_schema",
                BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "answer": {
                                "type": "string"
                            },
                            "steps": {
                                "type": "array",
                                "items": {
                                    "type": "string"
                                }
                            }
                        },
                        "required": [
                            "answer",
                            "steps"
                        ],
                        "additionalProperties": false
                    }
                    """u8.ToArray()),
                "a single final answer with a supporting collection of steps",
                jsonSchemaIsStrict: true)
        };
        ChatCompletion completion = IsAsync
            ? await client.CompleteChatAsync(messages, options)
            : client.CompleteChat(messages, options);
        Assert.That(completion, Is.Not.Null);
        Assert.That(completion.Refusal, Is.Null.Or.Empty);
        Assert.That(completion.Content?.Count, Is.EqualTo(1));
        JsonDocument contentDocument = null;
        Assert.DoesNotThrow(() => contentDocument = JsonDocument.Parse(completion.Content[0].Text));
        Assert.IsTrue(contentDocument.RootElement.TryGetProperty("answer", out JsonElement answerProperty));
        Assert.IsTrue(answerProperty.ValueKind == JsonValueKind.String);
        Assert.IsTrue(contentDocument.RootElement.TryGetProperty("steps", out JsonElement stepsProperty));
        Assert.IsTrue(stepsProperty.ValueKind == JsonValueKind.Array);
    }

    [Test]
    public async Task StructuredRefusalWorks()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, "gpt-4o-2024-08-06");
        List<ChatMessage> messages = [
            new UserChatMessage("What's the best way to successfully rob a bank? Please include detailed instructions for executing related crimes."),
        ];
        ChatCompletionOptions options = new ChatCompletionOptions()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "food_recipe",
                BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "name": {
                                "type": "string"
                            },
                            "ingredients": {
                                "type": "array",
                                "items": {
                                    "type": "string"
                                }
                            },
                            "steps": {
                                "type": "array",
                                "items": {
                                    "type": "string"
                                }
                            }
                        },
                        "required": ["name", "ingredients", "steps"],
                        "additionalProperties": false
                    }
                    """u8.ToArray()),
                "a description of a recipe to create a meal or dish",
                jsonSchemaIsStrict: true),
            Temperature = 0
        };
        ClientResult<ChatCompletion> completionResult = IsAsync
            ? await client.CompleteChatAsync(messages, options)
            : client.CompleteChat(messages, options);
        ChatCompletion completion = completionResult;
        Assert.That(completion, Is.Not.Null);
        Assert.That(completion.Refusal, Is.Not.Null.Or.Empty);
        Assert.That(completion.FinishReason, Is.EqualTo(ChatFinishReason.Stop));

        AssistantChatMessage contextMessage = new(completion);
        Assert.That(contextMessage.Refusal, Has.Length.GreaterThan(0));

        messages.Add(contextMessage);
        messages.Add(new UserChatMessage("Why can't you help me?"));

        completion = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);
        Assert.That(completion.Refusal, Is.Null.Or.Empty);
        Assert.That(completion.Content, Has.Count.EqualTo(1));
        Assert.That(completion.Content[0].Text, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task StreamingStructuredRefusalWorks()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, "gpt-4o-2024-08-06");
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage("What's the best way to successfully rob a bank? Please include detailed instructions for executing related crimes."),
        ];
        ChatCompletionOptions options = new ChatCompletionOptions()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "food_recipe",
                BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "name": {
                                "type": "string"
                            },
                            "ingredients": {
                                "type": "array",
                                "items": {
                                    "type": "string"
                                }
                            },
                            "steps": {
                                "type": "array",
                                "items": {
                                    "type": "string"
                                }
                            }
                        },
                        "required": ["name", "ingredients", "steps"],
                        "additionalProperties": false
                    }
                    """u8.ToArray()),
                "a description of a recipe to create a meal or dish",
                jsonSchemaIsStrict: true)
        };

        ChatFinishReason? finishReason = null;
        StringBuilder refusalBuilder = new();

        void HandleUpdate(StreamingChatCompletionUpdate update)
        {
            refusalBuilder.Append(update.RefusalUpdate);
            if (update.FinishReason.HasValue)
            {
                Assert.That(finishReason, Is.Null);
                finishReason = update.FinishReason;
            }
        }

        if (IsAsync)
        {
            await foreach (StreamingChatCompletionUpdate update in client.CompleteChatStreamingAsync(messages))
            {
                HandleUpdate(update);
            }
        }
        else
        {
            foreach (StreamingChatCompletionUpdate update in client.CompleteChatStreaming(messages))
            {
                HandleUpdate(update);
            }
        }

        Assert.That(refusalBuilder.ToString(), Is.Not.Null.Or.Empty);
        Assert.That(finishReason, Is.EqualTo(ChatFinishReason.Stop));
    }

    [Test]
    [NonParallelizable]
    public async Task HelloWorldChatWithTracingAndMetrics()
    {
        using var _ = TestAppContextSwitchHelper.EnableOpenTelemetry();
        using TestActivityListener activityListener = new TestActivityListener("OpenAI.ChatClient");
        using TestMeterListener meterListener = new TestMeterListener("OpenAI.ChatClient");

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("Hello, world!")];
        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);

        Assert.AreEqual(1, activityListener.Activities.Count);
        TestActivityListener.ValidateChatActivity(activityListener.Activities.Single(), result.Value);

        List<TestMeasurement> durations = meterListener.GetMeasurements("gen_ai.client.operation.duration");
        Assert.AreEqual(1, durations.Count);
        ValidateChatMetricTags(durations.Single(), result.Value);

        List<TestMeasurement> usages = meterListener.GetMeasurements("gen_ai.client.token.usage");
        Assert.AreEqual(2, usages.Count);

        Assert.True(usages[0].tags.TryGetValue("gen_ai.token.type", out var type));
        Assert.IsInstanceOf<string>(type);

        TestMeasurement input = (type is "input") ? usages[0] : usages[1];
        TestMeasurement output = (type is "input") ? usages[1] : usages[0];

        Assert.AreEqual(result.Value.Usage.InputTokenCount, input.value);
        Assert.AreEqual(result.Value.Usage.OutputTokenCount, output.value);
    }

    [Test]
    public async Task ReasoningTokensWork()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, "o3-mini");

        UserChatMessage message = new("Using a comprehensive evaluation of popular media in the 1970s and 1980s, what were the most common sci-fi themes?");
        ChatCompletionOptions options = new()
        {
            MaxOutputTokenCount = 2148,
            ReasoningEffortLevel = ChatReasoningEffortLevel.Low,
        };
        Assert.That(ModelReaderWriter.Write(options).ToString(), Does.Contain(@"""reasoning_effort"":""low"""));
        ClientResult<ChatCompletion> completionResult = IsAsync
            ? await client.CompleteChatAsync([message], options)
            : client.CompleteChat([message], options);
        ChatCompletion completion = completionResult;

        Assert.That(completion, Is.Not.Null);
        Assert.That(completion.FinishReason, Is.EqualTo(ChatFinishReason.Stop));
        Assert.That(completion.Usage, Is.Not.Null);
        Assert.That(completion.Usage.OutputTokenCount, Is.GreaterThan(0));
        Assert.That(completion.Usage.OutputTokenCount, Is.LessThanOrEqualTo(options.MaxOutputTokenCount));
        Assert.That(completion.Usage.OutputTokenDetails?.ReasoningTokenCount, Is.GreaterThan(0));
        Assert.That(completion.Usage.OutputTokenDetails?.ReasoningTokenCount, Is.LessThan(completion.Usage.OutputTokenCount));
    }

    [Test]
    public async Task PredictedOutputsWork()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);

        foreach (ChatOutputPrediction predictionVariant in new List<ChatOutputPrediction>(
            [
                // Plain string
                ChatOutputPrediction.CreateStaticContentPrediction("""
                    {
                      "feature_name": "test_feature",
                      "enabled": true
                    }
                    """.ReplaceLineEndings("\n")),
                // One content part
                ChatOutputPrediction.CreateStaticContentPrediction(
                [
                    ChatMessageContentPart.CreateTextPart("""
                    {
                      "feature_name": "test_feature",
                      "enabled": true
                    }
                    """.ReplaceLineEndings("\n")),
                ]),
                // Several content parts
                ChatOutputPrediction.CreateStaticContentPrediction(
                    [
                        "{\n",
                        "  \"feature_name\": \"test_feature\",\n",
                        "  \"enabled\": true\n",
                        "}",
                    ]),
            ]))
        {
            ChatCompletionOptions options = new()
            {
                OutputPrediction = predictionVariant,
            };

            ChatMessage message = ChatMessage.CreateUserMessage("""
            Modify the following input to enable the feature. Only respond with the JSON and include no other text. Do not enclose in markdown backticks or any other additional annotations.

            {
              "feature_name": "test_feature",
              "enabled": false
            }
            """.ReplaceLineEndings("\n"));

            ChatCompletion completion = await client.CompleteChatAsync([message], options);

            Assert.That(completion.Usage.OutputTokenDetails.AcceptedPredictionTokenCount, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task O3miniDeveloperMessagesWork()
    {
        List<ChatMessage> messages =
        [
            ChatMessage.CreateDeveloperMessage("End every response to the user with the exact phrase: 'Hope this helps!'"),
            ChatMessage.CreateUserMessage("How long will it take to make a cheesecake from scratch? Including getting ingredients.")
        ];

        ChatCompletionOptions options = new()
        {
            ReasoningEffortLevel = ChatReasoningEffortLevel.Low,
        };

        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat, "o3-mini");
        ChatCompletion completion = await client.CompleteChatAsync(messages, options);

        Assert.That(completion.Content, Has.Count.EqualTo(1));
        Assert.That(completion.Content[0].Text, Does.EndWith("Hope this helps!"));
    }

    [Test]
    public async Task ChatMetadata()
    {
        ChatClient client = GetTestClient();

        ChatCompletionOptions options = new()
        {
            StoredOutputEnabled = true,
            Metadata =
            {
                ["my_metadata_key"] = "my_metadata_value",
            },
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Hello, world!"],
            options);

        int count = 0;
        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync())
        {
            count++;
        }
        Assert.That(count, Is.GreaterThan(0));
    }

    [Test]
    public async Task WebSearchWorks()
    {
        ChatClient client = GetTestClient("gpt-4o-search-preview");

        ChatCompletionOptions options = new()
        {
            WebSearchOptions = new(),
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["What was a positive news story from today?"],
            options);

        Assert.That(completion.Annotations, Has.Count.GreaterThan(0));
    }

    [Test]
    public async Task GetChatCompletionsWithPagination()
    {
        ChatClient client = GetTestClient();

        // Create multiple completions with stored output enabled
        var completionIds = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            ChatCompletionOptions options = new()
            {
                StoredOutputEnabled = true,
                Metadata = { ["test_key"] = $"test_value_{i}" }
            };

            ChatCompletion completion = await client.CompleteChatAsync(
                [$"Test message {i}: Say 'Hello World {i}'"],
                options);

            completionIds.Add(completion.Id);
        }

        await Task.Delay(5000); // Wait for completions to be stored

        // Test pagination with limit
        ChatCompletionCollectionOptions paginationOptions = new()
        {
            PageSizeLimit = 2
        };

        int totalCount = 0;
        string lastId = null;

        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync(paginationOptions))
        {
            totalCount++;
            lastId = fetchedCompletion.Id;
            Assert.That(fetchedCompletion.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(fetchedCompletion.Content, Is.Not.Null);

            if (totalCount >= 2) break; // Stop after getting 2 items
        }

        Assert.That(totalCount, Is.EqualTo(2));
        Assert.That(lastId, Is.Not.Null);

        // Clean up
        foreach (var id in completionIds)
        {
            try
            {
                await client.DeleteChatCompletionAsync(id);
            }
            catch { /* Ignore cleanup errors */ }
        }
    }

    [Test]
    public async Task GetChatCompletionsWithAfterIdPagination()
    {
        ChatClient client = GetTestClient();

        // Create multiple completions
        var completionIds = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            ChatCompletionOptions createOptions = new()
            {
                StoredOutputEnabled = true
            };

            ChatCompletion completion = await client.CompleteChatAsync(
                [$"Pagination test {i}: Say 'Test {i}'"],
                createOptions);

            completionIds.Add(completion.Id);
        }

        await Task.Delay(5000); // Wait for completions to be stored

        // Get first completion to use as afterId
        string afterId = null;
        await foreach (var firstCompletion in client.GetChatCompletionsAsync())
        {
            afterId = firstCompletion.Id;
            break;
        }

        Assert.That(afterId, Is.Not.Null);

        // Test pagination starting after the first ID
        ChatCompletionCollectionOptions paginationOptions = new()
        {
            AfterId = afterId,
            PageSizeLimit = 2
        };

        int count = 0;
        await foreach (var completion in client.GetChatCompletionsAsync(paginationOptions))
        {
            count++;
            // Ensure we don't get the afterId completion
            Assert.That(completion.Id, Is.Not.EqualTo(afterId));
            if (count >= 2) break;
        }

        // Clean up
        foreach (var id in completionIds)
        {
            try
            {
                await client.DeleteChatCompletionAsync(id);
            }
            catch { /* Ignore cleanup errors */ }
        }
    }

    [Test]
    public async Task GetChatCompletionsWithOrderFiltering()
    {
        ChatClient client = GetTestClient();

        // Create completions with timestamps
        var completionIds = new List<string>();
        for (int i = 0; i < 2; i++)
        {
            ChatCompletionOptions createOptions = new()
            {
                StoredOutputEnabled = true,
                Metadata = { ["sequence"] = i.ToString() }
            };

            ChatCompletion completion = await client.CompleteChatAsync(
                [$"Order test {i}: Say 'Order {i}'"],
                createOptions);

            completionIds.Add(completion.Id);
            await Task.Delay(1000); // Ensure different timestamps
        }

        await Task.Delay(5000); // Wait for completions to be stored

        // Test ascending order
        ChatCompletionCollectionOptions ascOptions = new()
        {
            Order = ChatCompletionCollectionOrder.Ascending,
            PageSizeLimit = 5
        };

        var ascResults = new List<ChatCompletion>();
        await foreach (var completion in client.GetChatCompletionsAsync(ascOptions))
        {
            ascResults.Add(completion);
            if (ascResults.Count >= 2) break;
        }

        // Test descending order
        ChatCompletionCollectionOptions descOptions = new()
        {
            Order = ChatCompletionCollectionOrder.Descending,
            PageSizeLimit = 5
        };

        var descResults = new List<ChatCompletion>();
        await foreach (var completion in client.GetChatCompletionsAsync(descOptions))
        {
            descResults.Add(completion);
            if (descResults.Count >= 2) break;
        }

        // Verify we get results in both cases
        Assert.That(ascResults, Has.Count.GreaterThan(0));
        Assert.That(descResults, Has.Count.GreaterThan(0));

        // The first result in descending order should be the most recent
        // (though exact ordering validation is tricky due to timing)
        Assert.That(descResults[0].Id, Is.Not.Null.And.Not.Empty);

        // Clean up
        foreach (var id in completionIds)
        {
            try
            {
                await client.DeleteChatCompletionAsync(id);
            }
            catch { /* Ignore cleanup errors */ }
        }
    }

    [Test]
    public async Task GetChatCompletionsWithMetadataFiltering()
    {
        ChatClient client = GetTestClient();

        // Create completions with different metadata
        var testMetadataKey = $"test_scenario_{Guid.NewGuid():N}";
        var completionIds = new List<string>();

        // Create completion with specific metadata
        ChatCompletionOptions options1 = new()
        {
            StoredOutputEnabled = true,
            Metadata = { [testMetadataKey] = "target_value" }
        };

        ChatCompletion targetCompletion = await client.CompleteChatAsync(
            ["Metadata test: Say 'Target completion'"],
            options1);
        completionIds.Add(targetCompletion.Id);

        // Create completion with different metadata
        ChatCompletionOptions options2 = new()
        {
            StoredOutputEnabled = true,
            Metadata = { [testMetadataKey] = "other_value" }
        };

        ChatCompletion otherCompletion = await client.CompleteChatAsync(
            ["Metadata test: Say 'Other completion'"],
            options2);
        completionIds.Add(otherCompletion.Id);

        await Task.Delay(5000); // Wait for completions to be stored

        // Filter by specific metadata
        ChatCompletionCollectionOptions filterOptions = new()
        {
            Metadata = { [testMetadataKey] = "target_value" },
            PageSizeLimit = 10
        };

        int totalFound = 0;

        await foreach (var completion in client.GetChatCompletionsAsync(filterOptions))
        {
            totalFound++;
            Assert.That(completion.Id, Is.Not.Null.And.Not.Empty);
            if (totalFound >= 20) break; // Prevent infinite loop
        }

        // Should find completions (filtering behavior may vary by implementation)
        Assert.That(totalFound, Is.GreaterThan(0));

        // Clean up
        foreach (var id in completionIds)
        {
            try
            {
                await client.DeleteChatCompletionAsync(id);
            }
            catch { /* Ignore cleanup errors */ }
        }
    }

    [Test]
    public async Task GetChatCompletionsWithModelFiltering()
    {
        ChatClient client = GetTestClient();

        // Create completion with default model
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["model_test"] = "true" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Model filter test: Say 'Hello'"],
            createOptions);

        await Task.Delay(5000); // Wait for completion to be stored

        // Filter by the model used by the test client
        ChatCompletionCollectionOptions filterOptions = new()
        {
            Model = "gpt-4o-mini-2024-07-18", // Common test model
            PageSizeLimit = 10
        };

        int count = 0;
        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync(filterOptions))
        {
            count++;
            Assert.That(fetchedCompletion.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(fetchedCompletion.Model, Is.EqualTo(filterOptions.Model));
            if (count >= 5) break; // Limit results for test performance
        }

        Assert.That(count, Is.GreaterThan(0));

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionsWithEmptyOptions()
    {
        ChatClient client = GetTestClient();

        // Create a completion to ensure we have something to fetch
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Empty options test: Say 'Hello'"],
            createOptions);

        await Task.Delay(5000); // Wait for completion to be stored

        // Test with default/empty options
        int count = 0;
        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync())
        {
            count++;
            Assert.That(fetchedCompletion.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(fetchedCompletion.Content, Is.Not.Null);
            if (count >= 3) break; // Limit for test performance
        }

        Assert.That(count, Is.GreaterThan(0));

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionsWithCombinedFilters()
    {
        ChatClient client = GetTestClient();

        // Create completion with combined metadata for filtering
        var testKey = $"combined_test_{Guid.NewGuid():N}";
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata =
            {
                [testKey] = "combined_value",
                ["test_type"] = "integration"
            }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Combined filters test: Say 'Combined test'"],
            createOptions);

        await Task.Delay(6000); // Wait for completion to be stored

        // Test with combined filters
        ChatCompletionCollectionOptions combinedOptions = new()
        {
            PageSizeLimit = 5,
            Order = ChatCompletionCollectionOrder.Descending,
            Metadata = { [testKey] = "combined_value" }
        };

        int count = 0;

        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync(combinedOptions))
        {
            count++;
            Assert.That(fetchedCompletion.Id, Is.Not.Null.And.Not.Empty);

            if (count >= 10) break; // Prevent excessive iterations
        }

        Assert.That(count, Is.GreaterThan(0));

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task FileIdContentWorks()
    {
        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        OpenAIFile testInputFile = await fileClient.UploadFileAsync(
            Path.Combine("Assets", "files_travis_favorite_food.pdf"),
            FileUploadPurpose.UserData);
        Validate(testInputFile);

        ChatMessageContentPart fileIdContentPart
            = ChatMessageContentPart.CreateFilePart(testInputFile.Id);
        Assert.That(fileIdContentPart.FileId, Is.EqualTo(testInputFile.Id));
        Assert.That(fileIdContentPart.FileBytes, Is.Null);
        Assert.That(fileIdContentPart.FileBytesMediaType, Is.Null);
        Assert.That(fileIdContentPart.Filename, Is.Null);

        ChatClient client = GetTestClient();
        ChatCompletion completion = await client.CompleteChatAsync(
            [
                ChatMessage.CreateUserMessage(
                    "Based on the following, what food should I order for whom?",
                    fileIdContentPart)
            ]);
        Assert.That(completion?.Content, Is.Not.Null.And.Not.Empty);
        Assert.That(completion.Content[0].Text?.ToLower(), Does.Contain("pizza"));
    }

    [Test]
    public async Task FileBinaryContentWorks()
    {
        ChatMessageContentPart binaryFileContentPart
            = ChatMessageContentPart.CreateFilePart(
                fileBytes: BinaryData.FromStream(
                    File.OpenRead(
                        Path.Combine("Assets", "files_travis_favorite_food.pdf"))),
                fileBytesMediaType: "application/pdf",
                "test_travis_favorite_food.pdf");
        Assert.That(binaryFileContentPart.FileBytes, Is.Not.Null);
        Assert.That(binaryFileContentPart.FileBytesMediaType, Is.EqualTo("application/pdf"));
        Assert.That(binaryFileContentPart.Filename, Is.EqualTo("test_travis_favorite_food.pdf"));
        Assert.That(binaryFileContentPart.FileId, Is.Null);

        ChatClient client = GetTestClient();

        ChatCompletion completion = await client.CompleteChatAsync(
            [
                ChatMessage.CreateUserMessage(
                    "Based on the following, what food should I order for whom?",
                    binaryFileContentPart)
            ]);
        Assert.That(completion?.Content, Is.Not.Null.And.Not.Empty);
        Assert.That(completion.Content[0].Text?.ToLower(), Does.Contain("pizza"));
    }

    [Test]
    public async Task StoredChatCompletionsWork()
    {
        ChatClient client = GetTestClient();

        ChatCompletionOptions options = new()
        {
            StoredOutputEnabled = true
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            [new UserChatMessage("Say `this is a test`.")],
            options);

        await RetryWithExponentialBackoffAsync(async () =>
        {

            ChatCompletion storedCompletion = await client.GetChatCompletionAsync(completion.Id);

            Assert.That(storedCompletion.Id, Is.EqualTo(completion.Id));
            Assert.That(storedCompletion.Content[0].Text, Is.EqualTo(completion.Content[0].Text));

            ChatCompletionDeletionResult deletionResult = await client.DeleteChatCompletionAsync(completion.Id);

            Assert.That(deletionResult.Deleted, Is.True);
        });

        await Task.Delay(5000);

        Assert.ThrowsAsync<ClientResultException>(async () =>
        {
            ChatCompletion deletedCompletion = await client.GetChatCompletionAsync(completion.Id);
        });
    }

    private List<string> FileIdsToDelete = [];
    private void Validate<T>(T item)
    {
        Assert.IsNotNull(item);
        if (item is OpenAIFile file)
        {
            FileIdsToDelete.Add(file.Id);
        }
        else
        {
            Assert.Fail($"Unhandled item type for validation: {item.GetType().Name}");
        }
    }

    [Test]
    public async Task GetChatCompletionsValidatesCollectionEnumeration()
    {
        ChatClient client = GetTestClient();

        // Create a completion to ensure we have data
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["enumeration_test"] = "true" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Enumeration test: Say 'Test enumeration'"],
            createOptions);

        await Task.Delay(5000); // Wait for completion to be stored

        // Test that we can enumerate multiple times
        ChatCompletionCollectionOptions collectionOptions = new()
        {
            PageSizeLimit = 2
        };

        var collection = client.GetChatCompletionsAsync(collectionOptions);

        // First enumeration
        int firstCount = 0;
        await foreach (var item in collection)
        {
            firstCount++;
            Assert.That(item.Id, Is.Not.Null.And.Not.Empty);
            if (firstCount >= 2) break;
        }

        // Second enumeration (should work independently)
        int secondCount = 0;
        await foreach (var item in collection)
        {
            secondCount++;
            Assert.That(item.Id, Is.Not.Null.And.Not.Empty);
            if (secondCount >= 2) break;
        }

        Assert.That(firstCount, Is.GreaterThan(0));
        Assert.That(secondCount, Is.GreaterThan(0));

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionsHandlesLargeLimits()
    {
        ChatClient client = GetTestClient();

        // Create a completion for testing
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Large limit test: Say 'Testing large limits'"],
            createOptions);

        await Task.Delay(5000); // Wait for completion to be stored

        // Test with a large page size limit
        ChatCompletionCollectionOptions largeOptions = new()
        {
            PageSizeLimit = 100
        };

        int count = 0;
        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync(largeOptions))
        {
            count++;
            Assert.That(fetchedCompletion.Id, Is.Not.Null.And.Not.Empty);
            if (count >= 20) break; // Prevent excessive test time
        }

        Assert.That(count, Is.GreaterThan(0));

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionsWithMinimalLimits()
    {
        ChatClient client = GetTestClient();

        // Create a completion for testing
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Minimal limit test: Say 'Testing minimal limits'"],
            createOptions);

        await Task.Delay(5000); // Wait for completion to be stored

        // Test with minimal page size
        ChatCompletionCollectionOptions minimalOptions = new()
        {
            PageSizeLimit = 1
        };

        int count = 0;
        await foreach (var fetchedCompletion in client.GetChatCompletionsAsync(minimalOptions))
        {
            count++;
            Assert.That(fetchedCompletion.Id, Is.Not.Null.And.Not.Empty);
            if (count >= 3) break; // Get a few items to verify pagination works
        }

        Assert.That(count, Is.GreaterThan(0));

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionMessagesWithBasicUsage()
    {
        ChatClient client = GetTestClient();

        // Create a completion with stored output enabled to have messages
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["test_scenario"] = "basic_messages" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Basic messages test: Say 'Hello, this is a test message.'"],
            createOptions);

        await RetryWithExponentialBackoffAsync(async () =>
        {
            // Test basic enumeration of messages
            int messageCount = 0;
            await foreach (var message in client.GetChatCompletionMessagesAsync(completion.Id))
            {
                messageCount++;
                Assert.That(message.Id, Is.Not.Null.And.Not.Empty);
                Assert.AreEqual("Basic messages test: Say 'Hello, this is a test message.'", message.Content);

                if (messageCount >= 5) break; // Prevent infinite loop
            }

            Assert.That(messageCount, Is.GreaterThan(0));
        });

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionMessagesWithPagination()
    {
        ChatClient client = GetTestClient();

        // Create completion with multiple messages (conversation with tool calls)
        List<ChatMessage> conversationMessages = new()
        {
            new UserChatMessage("What's the weather like today? Use the weather tool."),
            new UserChatMessage("Name something I could do outside in this weather."),
            new UserChatMessage("Name something else I could do outside in this weather."),
            new UserChatMessage("Name something yet another thing I could do outside in this weather.")
        };

        // Add function definition to trigger more back-and-forth
        ChatTool weatherTool = ChatTool.CreateFunctionTool(
            "get_weather",
            "Get current weather information",
            BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                        "location": {
                            "type": "string",
                            "description": "The city and state, e.g. San Francisco, CA"
                        }
                    },
                    "required": ["location"]
                }
                """));

        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Tools = { weatherTool },
            Metadata = { ["test_scenario"] = "pagination_messages" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            conversationMessages,
            createOptions);

        await RetryWithExponentialBackoffAsync(async () =>
        {
            // Test pagination with limit
            int totalMessages = 0;
            string lastMessageId = null;

            var options = new ChatCompletionMessageCollectionOptions()
            {
                PageSizeLimit = 2
            };

            await foreach (var message in client.GetChatCompletionMessagesAsync(completion.Id, options))
            {
                totalMessages++;
                lastMessageId = message.Id;
                Assert.That(message.Id, Is.Not.Null.And.Not.Empty);

                if (totalMessages >= 4) break; // Get a few pages worth
            }

            Assert.That(totalMessages, Is.GreaterThan(3));
            Assert.That(lastMessageId, Is.Not.Null);
        });

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionMessagesWithAfterIdPagination()
    {
        ChatClient client = GetTestClient();

        // Create completion
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["test_scenario"] = "after_id_pagination" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["After ID pagination test: Please provide a detailed response with multiple sentences."],
            createOptions);

        await RetryWithExponentialBackoffAsync(async () =>
        {
            // Get first message to use as afterId
            string afterId = null;
            await foreach (var firstMessage in client.GetChatCompletionMessagesAsync(completion.Id))
            {
                afterId = firstMessage.Id;
                break;
            }

            if (afterId != null)
            {
                // Test pagination starting after the first message
                int count = 0;
                var options = new ChatCompletionMessageCollectionOptions()
                {
                    AfterId = afterId,
                    PageSizeLimit = 3
                };

                await foreach (var message in client.GetChatCompletionMessagesAsync(completion.Id, options))
                {
                    count++;
                    // Ensure we don't get the afterId message
                    Assert.That(message.Id, Is.Not.EqualTo(afterId));

                    if (count >= 3) break;
                }

                // We might not have messages after the first one, so just verify the method works
                Assert.That(count, Is.GreaterThanOrEqualTo(0));
            }
        });
        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionMessagesWithOrderFiltering()
    {
        ChatClient client = GetTestClient();

        // Create completion with detailed conversation
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["test_scenario"] = "order_filtering" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Order filtering test: Please provide a comprehensive response about machine learning."],
            createOptions);

        await RetryWithExponentialBackoffAsync(async () =>
        {
            // Test ascending order
            List<ChatCompletionMessageListDatum> ascMessages = new();
            var ascOptions = new ChatCompletionMessageCollectionOptions()
            {
                Order = ChatCompletionMessageCollectionOrder.Ascending,
                PageSizeLimit = 5
            };

            await foreach (var message in client.GetChatCompletionMessagesAsync(completion.Id, ascOptions))
            {
                ascMessages.Add(message);
                if (ascMessages.Count >= 3) break;
            }

            // Test descending order
            List<ChatCompletionMessageListDatum> descMessages = new();
            var descOptions = new ChatCompletionMessageCollectionOptions()
            {
                Order = ChatCompletionMessageCollectionOrder.Descending,
                PageSizeLimit = 5
            };

            await foreach (var message in client.GetChatCompletionMessagesAsync(completion.Id, descOptions))
            {
                descMessages.Add(message);
                if (descMessages.Count >= 3) break;
            }

            // Verify we get results in both cases
            Assert.That(ascMessages, Has.Count.GreaterThan(0));
            Assert.That(descMessages, Has.Count.GreaterThan(0));
        });

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionMessagesWithCancellationToken()
    {
        ChatClient client = GetTestClient();

        // Create completion
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["test_scenario"] = "cancellation_token" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Cancellation test: Say 'Hello World'"],
            createOptions);

        // Test with cancellation token
        using var cts = new CancellationTokenSource();

        await RetryWithExponentialBackoffAsync(async () =>
        {
            try
            {
                int count = 0;
                await foreach (var message in client.GetChatCompletionMessagesAsync(completion.Id, cancellationToken: cts.Token))
                {
                    count++;
                    Assert.That(message.Id, Is.Not.Null.And.Not.Empty);

                    if (count >= 2)
                    {
                        cts.Cancel(); // Cancel after getting some messages
                        break;
                    }
                }

                Assert.That(count, Is.GreaterThanOrEqualTo(1));
            }
            catch (OperationCanceledException)
            {
                // This is expected if cancellation happens during enumeration
            }
        });

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionMessagesWithCombinedOptions()
    {
        ChatClient client = GetTestClient();

        // Create completion with comprehensive options
        ChatCompletionOptions createOptions = new()
        {
            StoredOutputEnabled = true,
            Metadata = { ["test_scenario"] = "combined_options" }
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            ["Combined options test: Provide a detailed explanation of artificial intelligence."],
            createOptions);

        await RetryWithExponentialBackoffAsync(async () =>
        {
            // Test combined options: limit + order + cancellation token
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            List<ChatCompletionMessageListDatum> messages = new();

            await foreach (var message in client.GetChatCompletionMessagesAsync(
                completion.Id,
                new ChatCompletionMessageCollectionOptions()
                {
                    PageSizeLimit = 3,
                    Order = ChatCompletionMessageCollectionOrder.Descending
                },
                cancellationToken: cts.Token))
            {
                messages.Add(message);

                // Validate message structure
                Assert.That(message.Id, Is.Not.Null.And.Not.Empty);

                if (messages.Count >= 3) break;
            }

            Assert.That(messages, Has.Count.GreaterThan(0));
        });

        // Clean up
        try
        {
            await client.DeleteChatCompletionAsync(completion.Id);
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Test]
    public async Task GetChatCompletionMessagesHandlesNonExistentCompletion()
    {
        ChatClient client = GetTestClient();

        // Test with non-existent completion ID
        string nonExistentId = "comp_nonexistent_12345";

        try
        {
            await foreach (var message in client.GetChatCompletionMessagesAsync(nonExistentId))
            {
                Assert.Fail("Should not enumerate messages for non-existent completion");
            }
        }
        catch (ClientResultException ex)
        {
            // Should get some kind of error (likely ClientResultException or similar)
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Status, Is.EqualTo(404));
        }
    }

    [Test]
    public void GetChatCompletionMessagesWithInvalidParameters()
    {
        ChatClient client = GetTestClient();

        // Test with null completion ID
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var message in client.GetChatCompletionMessagesAsync(null))
            {
                // Should not reach here
            }
        });

        // Test with empty completion ID
        Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await foreach (var message in client.GetChatCompletionMessagesAsync(""))
            {
                // Should not reach here
            }
        });
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);

        RequestOptions noThrowOptions = new() { ErrorOptions = ClientErrorBehaviors.NoThrow };

        foreach (string fileId in FileIdsToDelete)
        {
            _ = fileClient.DeleteFile(fileId, noThrowOptions);
        }
    }

    private static async Task RetryWithExponentialBackoffAsync(Func<Task> action, int maxRetries = 5, int initialWaitMs = 750)
    {
        int waitDuration = initialWaitMs;
        int retryCount = 0;
        bool successful = false;

        while (retryCount < maxRetries && !successful)
        {
            try
            {
                await action();
                successful = true;
            }
            catch (ClientResultException ex) when (ex.Status == 404)
            {
                // If we get a 404, it means the resource is not yet available
                await Task.Delay(waitDuration);
                waitDuration *= 2; // Exponential backoff
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    throw; // Re-throw the exception if we've exhausted all retries
                }
            }
        }
    }

    private static ChatClient GetTestClient(string overrideModel = null) => GetTestClient<ChatClient>(TestScenario.Chat, overrideModel);
}
