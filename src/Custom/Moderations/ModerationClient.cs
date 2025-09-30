using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.Moderations.Models;

namespace OpenAI.Moderations;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Suppressed methods that only take the options parameter.
/// <summary> The service client for OpenAI moderation operations. </summary>
[CodeGenType("Moderations")]
[CodeGenSuppress("ModerationClient", typeof(ClientPipeline), typeof(Uri))]
[CodeGenSuppress("ClassifyTextAsync", typeof(CreateModerationRequest), typeof(CancellationToken))]
[CodeGenSuppress("ClassifyText", typeof(CreateModerationRequest), typeof(CancellationToken))]
public partial class ModerationClient
{
    private readonly string _model;

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ModerationClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="apiKey"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ModerationClient(string model, string apiKey) : this(model, new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ModerationClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ModerationClient(string model, ApiKeyCredential credential) : this(model, credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ModerationClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="credential"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public ModerationClient(string model, ApiKeyCredential credential, OpenAIClientOptions options) : this(model, OpenAIClient.CreateApiKeyAuthenticationPolicy(credential), options)
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ModerationClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="authenticationPolicy"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    [Experimental("OPENAI001")]
    public ModerationClient(string model, AuthenticationPolicy authenticationPolicy) : this(model, authenticationPolicy, new OpenAIClientOptions())
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ModerationClient"/>. </summary>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> or <paramref name="authenticationPolicy"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    [Experimental("OPENAI001")]
    public ModerationClient(string model, AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        Argument.AssertNotNull(authenticationPolicy, nameof(authenticationPolicy));
        options ??= new OpenAIClientOptions();

        _model = model;
        Pipeline = OpenAIClient.CreatePipeline(authenticationPolicy, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Added `model` parameter.
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="ModerationClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="model"> The name of the model to use in requests sent to the service. To learn more about the available models, see <see href="https://platform.openai.com/docs/models"/>. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> or <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    protected internal ModerationClient(ClientPipeline pipeline, string model, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        options ??= new OpenAIClientOptions();

        _model = model;
        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    /// <summary>
    /// Gets the name of the model used in requests sent to the service.
    /// </summary>
    [Experimental("OPENAI001")]
    public string Model => _model;

    /// <summary>
    /// Gets the endpoint URI for the service.
    /// </summary>
    [Experimental("OPENAI001")]
    public Uri Endpoint => _endpoint;

    /// <summary> Classifies if the text input is potentially harmful across several categories. </summary>
    /// <param name="input"> The text input to classify. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="input"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="input"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<ModerationResult>> ClassifyTextAsync(string input, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(input, nameof(input));

        CreateModerationRequest options = CreateModerationOptions(input);
        using BinaryContent content = BinaryContent.Create(options, ModelSerializationExtensions.WireOptions);
        ClientResult result = await ClassifyTextAsync(content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(((ModerationResultCollection)result).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Classifies if the text input is potentially harmful across several categories. </summary>
    /// <param name="input"> The text input to classify. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="input"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="input"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<ModerationResult> ClassifyText(string input, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(input, nameof(input));

        CreateModerationRequest options = CreateModerationOptions(input);
        using BinaryContent content = BinaryContent.Create(options, ModelSerializationExtensions.WireOptions);
        ClientResult result = ClassifyText(content, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(((ModerationResultCollection)result).FirstOrDefault(), result.GetRawResponse());
    }

    /// <summary> Classifies if the text inputs are potentially harmful across several categories. </summary>
    /// <param name="inputs"> The text inputs to classify. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="inputs"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="inputs"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<ModerationResultCollection>> ClassifyTextAsync(IEnumerable<string> inputs, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(inputs, nameof(inputs));

        CreateModerationRequest options = CreateModerationOptions(inputs);
        using BinaryContent content = BinaryContent.Create(options, ModelSerializationExtensions.WireOptions);
        ClientResult result = await ClassifyTextAsync(content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue((ModerationResultCollection)result, result.GetRawResponse());
    }

    /// <summary> Classifies if the text inputs are potentially harmful across several categories. </summary>
    /// <param name="inputs"> The text inputs to classify. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="inputs"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="inputs"/> is an empty collection, and was expected to be non-empty. </exception>
    public virtual ClientResult<ModerationResultCollection> ClassifyText(IEnumerable<string> inputs, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(inputs, nameof(inputs));

        CreateModerationRequest options = CreateModerationOptions(inputs);
        using BinaryContent content = BinaryContent.Create(options, ModelSerializationExtensions.WireOptions);
        ClientResult result = ClassifyText(content, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue((ModerationResultCollection)result, result.GetRawResponse());
    }

    private CreateModerationRequest CreateModerationOptions(string input)
    {
        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream);

        writer.WriteStringValue(input);
        writer.Flush();

        return new CreateModerationRequest(
            BinaryData.FromBytes(stream.GetBuffer().AsMemory(0, (int)stream.Length)),
            _model,
            null);
    }

    private CreateModerationRequest CreateModerationOptions(IEnumerable<string> inputs)
    {
        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream);

        writer.WriteStartArray();

        foreach (string input in inputs)
        {
            writer.WriteStringValue(input);
        }

        writer.WriteEndArray();
        writer.Flush();

        return new CreateModerationRequest(
            BinaryData.FromBytes(stream.GetBuffer().AsMemory(0, (int)stream.Length)),
            _model,
            null);
    }

    public BinaryContent CreateClassifyTextRequest(string input, CreateModerationRequestModel? model = null)
    {
        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream);

        var data = BinaryData.FromString($"\"{input.Replace("\"", "\\\"")}\"");

        var request = new CreateModerationRequest(
            data,
            _model,
            null);

        BinaryData bytes = ModelReaderWriter.Write(request);
        return BinaryContent.Create(bytes);
    }
}
