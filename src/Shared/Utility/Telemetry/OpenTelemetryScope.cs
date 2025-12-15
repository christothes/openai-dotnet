using System;
using System.ClientModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;

using static OpenAI.Telemetry.OpenTelemetryConstants;

#nullable enable

namespace OpenAI.Telemetry;

internal class OpenTelemetryScope : IDisposable
{
    private static readonly ActivitySource s_chatSource = new ActivitySource("OpenAI.ChatClient");
    private static readonly Meter s_chatMeter = new Meter("OpenAI.ChatClient");

    // TODO: add explicit histogram buckets once System.Diagnostics.DiagnosticSource 9.0 is used
    private static readonly Histogram<double> s_duration = s_chatMeter.CreateHistogram<double>(GenAiClientOperationDurationMetricName, "s", "Measures GenAI operation duration.");
    private static readonly Histogram<long> s_tokens = s_chatMeter.CreateHistogram<long>(GenAiClientTokenUsageMetricName, "{token}", "Measures the number of input and output token used.");

    private readonly string _operationName;
    private readonly string _serverAddress;
    private readonly int _serverPort;
    private readonly string _requestModel;

    private Stopwatch? _duration;
    private Activity? _activity;
    private TagList _commonTags;

    private OpenTelemetryScope(
        string model, string operationName,
        string serverAddress, int serverPort)
    {
        _requestModel = model;
        _operationName = operationName;
        _serverAddress = serverAddress;
        _serverPort = serverPort;
    }

    private static bool IsChatEnabled => s_chatSource.HasListeners() || s_tokens.Enabled || s_duration.Enabled;

    public static OpenTelemetryScope? StartChat(
        string model,
        string operationName,
        string serverAddress,
        int serverPort,
        int? maxOutputTokenCount,
        float? temperature,
        float? topP)
    {
        if (IsChatEnabled)
        {
            var scope = new OpenTelemetryScope(model, operationName, serverAddress, serverPort);
            scope.StartChat(maxOutputTokenCount, temperature, topP);
            return scope;
        }

        return null;
    }

    private void StartChat(int? maxOutputTokenCount, float? temperature, float? topP)
    {
        _duration = Stopwatch.StartNew();
        _commonTags = new TagList
        {
            { GenAiSystemKey, GenAiSystemValue },
            { GenAiRequestModelKey, _requestModel },
            { ServerAddressKey, _serverAddress },
            { ServerPortKey, _serverPort },
            { GenAiOperationNameKey, _operationName },
        };

        _activity = s_chatSource.StartActivity(string.Concat(_operationName, " ", _requestModel), ActivityKind.Client);
        if (_activity?.IsAllDataRequested == true)
        {
            RecordCommonAttributes();
            SetActivityTagIfNotNull(GenAiRequestMaxTokensKey, maxOutputTokenCount);
            SetActivityTagIfNotNull(GenAiRequestTemperatureKey, temperature);
            SetActivityTagIfNotNull(GenAiRequestTopPKey, topP);
        }

        return;
    }

    public void RecordChatCompletion(
        string responseId,
        string? responseModel = null,
        string? finishReason = null,
        int? inputTokenCount = null,
        int? outputTokenCount = null)
    {
        RecordMetrics(responseModel, null, inputTokenCount, outputTokenCount);

        if (_activity?.IsAllDataRequested == true)
        {
            RecordResponseAttributes(responseId, responseModel, finishReason, inputTokenCount, outputTokenCount);
        }
    }

    public void RecordException(Exception ex)
    {
        var errorType = GetErrorType(ex);
        RecordMetrics(null, errorType, null, null);
        if (_activity?.IsAllDataRequested == true)
        {
            _activity?.SetTag(OpenTelemetryConstants.ErrorTypeKey, errorType);
            _activity?.SetStatus(ActivityStatusCode.Error, ex?.Message ?? errorType);
        }
    }

    public void Dispose()
    {
        _activity?.Stop();
    }

    private void RecordCommonAttributes()
    {
        Activity? activity = _activity;
        if (activity is null)
        {
            return;
        }

        activity.SetTag(GenAiSystemKey, GenAiSystemValue);
        activity.SetTag(GenAiRequestModelKey, _requestModel);
        activity.SetTag(ServerAddressKey, _serverAddress);
        activity.SetTag(ServerPortKey, _serverPort);
        activity.SetTag(GenAiOperationNameKey, _operationName);
    }

    private void RecordMetrics(string? responseModel, string? errorType, int? inputTokensUsage, int? outputTokensUsage)
    {
        // tags is a struct, let's copy and modify them
        var tags = _commonTags;

        if (responseModel != null)
        {
            tags.Add(GenAiResponseModelKey, responseModel);
        }

        if (inputTokensUsage != null)
        {
            var inputUsageTags = tags;
            inputUsageTags.Add(GenAiTokenTypeKey, "input");
            s_tokens.Record(inputTokensUsage.Value, inputUsageTags);
        }

        if (outputTokensUsage != null)
        {
            var outputUsageTags = tags;
            outputUsageTags.Add(GenAiTokenTypeKey, "output");
            s_tokens.Record(outputTokensUsage.Value, outputUsageTags);
        }

        if (errorType != null)
        {
            tags.Add(ErrorTypeKey, errorType);
        }

        s_duration.Record(_duration!.Elapsed.TotalSeconds, tags);
    }

    private void RecordResponseAttributes(
        string responseId,
        string? model,
        string? finishReason,
        int? inputTokenCount,
        int? outputTokenCount)
    {
        SetActivityTagIfNotNull(GenAiResponseIdKey, responseId);
        SetActivityTagIfNotNull(GenAiResponseModelKey, model);
        SetActivityTagIfNotNull(GenAiUsageInputTokensKey, inputTokenCount);
        SetActivityTagIfNotNull(GenAiUsageOutputTokensKey, outputTokenCount);
        SetFinishReasonAttribute(finishReason);
    }

    private void SetFinishReasonAttribute(string? finishReason)
    {
        if (string.IsNullOrEmpty(finishReason))
        {
            return;
        }

        // There could be multiple finish reasons, so semantic conventions use array type for the corrresponding attribute.
        // It's likely to change, but for now let's report it as array.
        Activity? activity = _activity;
        if (activity is null)
        {
            return;
        }

        activity.SetTag(GenAiResponseFinishReasonKey, new[] { finishReason });
    }

    private string GetErrorType(Exception exception)
    {
        if (exception is ClientResultException requestFailedException)
        {
            // TODO (lmolkova) when we start targeting .NET 8 we should put
            // requestFailedException.InnerException.HttpRequestError into error.type
            return requestFailedException.Status.ToString();
        }

        return exception?.GetType()?.FullName ?? "unknown";
    }

    private void SetActivityTagIfNotNull(string name, object? value)
    {
        Activity? activity = _activity;
        if (value is null || activity is null)
        {
            return;
        }

        activity.SetTag(name, value);
    }

    private void SetActivityTagIfNotNull(string name, int? value)
    {
        Activity? activity = _activity;
        if (value.HasValue && activity is not null)
        {
            activity.SetTag(name, value.Value);
        }
    }

    private void SetActivityTagIfNotNull(string name, float? value)
    {
        Activity? activity = _activity;
        if (value.HasValue && activity is not null)
        {
            activity.SetTag(name, value.Value);
        }
    }
}
