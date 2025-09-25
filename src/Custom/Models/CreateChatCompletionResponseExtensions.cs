using System.ClientModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Chat
{
    [Experimental("OPENAI001")]
    public static class CreateChatCompletionResponseExtensions
    {
        public static AsyncCollectionResult<StreamingChatCompletionUpdate> ToAsyncCollectionResult(this ClientResult result)
        {
            return new AsyncSseUpdateCollection<StreamingChatCompletionUpdate>(
            () => Task.FromResult(result),
            StreamingChatCompletionUpdate.DeserializeStreamingChatCompletionUpdate,
            CancellationToken.None);
        }
    }
}
