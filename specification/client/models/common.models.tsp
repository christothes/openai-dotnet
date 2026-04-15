import "../../base/typespec/common/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http";

using Azure.ClientGenerator.Core;
using TypeSpec.Http;

namespace OpenAI;

// Here, we create scenario-specific copies of types for the .NET SDK.

@access(Access.public)
@usage(Usage.input | Usage.output)
union DotNetResponseReasoningEffortLevel {
  ReasoningEffort,
}

// ResponseFormat

@access(Access.public)
@usage(Usage.input | Usage.output)
@discriminator("type")
model DotNetChatResponseFormat {
  ...ResponseFormat;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetChatResponseFormatText extends DotNetChatResponseFormat {
  @doc("""
    The type of response format being defined. Always `text`.
    """)
  type: "text";
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetChatResponseFormatJsonSchema extends DotNetChatResponseFormat {
  type: "json_schema";
  json_schema: {
    description?: string;
    name: string;
    schema?: unknown;
    strict?: boolean | null = false;
  };
}

// LogProbProperties

@access(Access.public)
@usage(Usage.output)
model DotNetAudioLogProbsProperties {
  ...LogProbProperties;
}

@access(Access.public)
@usage(Usage.output)
model DotNetRealtimeLogProbsProperties {
  ...LogProbProperties;
}

// VoiceIdsShared

@access(Access.public)
@usage(Usage.input | Usage.output)
union DotNetAudioVoiceIds {
  VoiceIdsShared,
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union DotNetChatVoiceIds {
  VoiceIdsShared,
}

// CollectionQueryParameters

alias DotNetCollectionLimitQueryParameter = {
  /**
   * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
   * default is 20.
   */
  @query pageSizeLimit?: int32 = 20;
};

alias DotNetCollectionAfterQueryParameter = {
  /**
   * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include after=obj_foo in order to fetch the next page of the list.
   */
  @query @continuationToken afterId?: string;
};

alias DotNetCollectionBeforeQueryParameter = {
  /**
   * A cursor for use in pagination. `before` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, starting with obj_foo, your
   * subsequent call can include before=obj_foo in order to fetch the previous page of the list.
   */
  @query beforeId?: string;
};

union DotNetChatServiceTier {
   ServiceTier
}

union DotNetResponseServiceTier {
  ServiceTier
}
