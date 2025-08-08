import "../base/typespec/chat/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http";

using OpenAI;
using TypeSpec.Http;
using Azure.ClientGenerator.Core;

@@access(ChatCompletionFunctionChoice, Access.public);
@@usage(ChatCompletionFunctionChoice, Usage.input);

@@access(ChatCompletionToolChoice, Access.public);
@@usage(ChatCompletionToolChoice, Usage.input);

@@access(CreateChatCompletionStreamResponse, Access.public);
@@usage(CreateChatCompletionStreamResponse, Usage.output);

@@visibility(ChatCompletionResponseMessage.tool_calls, Lifecycle.Read);
@@visibility(ChatCompletionResponseMessage.annotations, Lifecycle.Read);
@@visibility(ChatCompletionStreamResponseDelta.tool_calls, Lifecycle.Read);

@@clientName(Chat.createChatCompletion, "CompleteChat");

op myOperationCustomization(@path completion_id: string, options: ChatCompletionMessageCollectionOptions): ChatCompletionMessageList | ErrorResponse;

@@override(Chat.getChatCompletionMessages, myOperationCustomization);