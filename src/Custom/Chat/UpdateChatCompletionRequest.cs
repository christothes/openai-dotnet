using System.ClientModel;
using System.Collections.Generic;

namespace OpenAI.Chat;

public class UpdateChatCompletionRequest : ProtocolBody<UpdateChatCompletionRequestBody>
{
    public override UpdateChatCompletionRequestBody Body { get; set; }

    public string CompletionId { get; set; }

    public UpdateChatCompletionRequest(IDictionary<string, string> metadata, string completionId) : base(new UpdateChatCompletionRequestBody(metadata))
    {
        CompletionId = completionId;
    }
}