using System.ClientModel;

namespace OpenAI
{
    public partial class CreateChatCompletionRequest : ProtocolBody<CreateChatCompletionRequestBody>
    {
        public CreateChatCompletionRequest(CreateChatCompletionRequestBody body) : base(body)
        {
        }
        public override CreateChatCompletionRequestBody Body { get; set; }
    }
}
