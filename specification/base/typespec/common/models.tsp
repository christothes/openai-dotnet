/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "./custom.tsp";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

model Error {
  code: string | null;
  message: string;
  param: string | null;
  type: string;
}

// Tool customization: apply error decorator
@error
model ErrorResponse {
  error: Error;
}

@doc("""
  Not supported with latest reasoning models `o3` and `o4-mini`.
  
  Up to 4 sequences where the API will stop generating further tokens. The
  returned text will not contain the stop sequence.
  """)
union StopConfiguration {
  string,
  string[],
}

@doc("""
  High level guidance for the amount of context window space to use for the
  search. One of `low`, `medium`, or `high`. `medium` is the default.
  """)
union WebSearchContextSize {
  "low",
  "medium",
  "high",
}

@doc("""
  The ranker to use for the file search. If not specified will use the `auto` ranker.
  """)
union FileSearchRanker {
  "auto",
  "default_2024_08_21",
}

union ModelIdsShared {
  string,
  "gpt-4.1",
  "gpt-4.1-mini",
  "gpt-4.1-nano",
  "gpt-4.1-2025-04-14",
  "gpt-4.1-mini-2025-04-14",
  "gpt-4.1-nano-2025-04-14",
  "o4-mini",
  "o4-mini-2025-04-16",
  "o3",
  "o3-2025-04-16",
  "o3-mini",
  "o3-mini-2025-01-31",
  "o1",
  "o1-2024-12-17",
  "o1-preview",
  "o1-preview-2024-09-12",
  "o1-mini",
  "o1-mini-2024-09-12",
  "gpt-4o",
  "gpt-4o-2024-11-20",
  "gpt-4o-2024-08-06",
  "gpt-4o-2024-05-13",
  "gpt-4o-audio-preview",
  "gpt-4o-audio-preview-2024-10-01",
  "gpt-4o-audio-preview-2024-12-17",
  "gpt-4o-audio-preview-2025-06-03",
  "gpt-4o-mini-audio-preview",
  "gpt-4o-mini-audio-preview-2024-12-17",
  "gpt-4o-search-preview",
  "gpt-4o-mini-search-preview",
  "gpt-4o-search-preview-2025-03-11",
  "gpt-4o-mini-search-preview-2025-03-11",
  "chatgpt-4o-latest",
  "codex-mini-latest",
  "gpt-4o-mini",
  "gpt-4o-mini-2024-07-18",
  "gpt-4-turbo",
  "gpt-4-turbo-2024-04-09",
  "gpt-4-0125-preview",
  "gpt-4-turbo-preview",
  "gpt-4-1106-preview",
  "gpt-4-vision-preview",
  "gpt-4",
  "gpt-4-0314",
  "gpt-4-0613",
  "gpt-4-32k",
  "gpt-4-32k-0314",
  "gpt-4-32k-0613",
  "gpt-3.5-turbo",
  "gpt-3.5-turbo-16k",
  "gpt-3.5-turbo-0301",
  "gpt-3.5-turbo-0613",
  "gpt-3.5-turbo-1106",
  "gpt-3.5-turbo-0125",
  "gpt-3.5-turbo-16k-0613",
}

/** A log probability object. */
model LogProbProperties {
  /** The token that was used to generate the log probability. */
  token: string;

  /** The log probability of the token. */
  logprob: float32;

  /** The bytes that were used to generate the log probability. */
  bytes: int32[];
}

union VoiceIdsShared {
  string,
  "alloy",
  "ash",
  "ballad",
  "coral",
  "echo",
  "fable",
  "onyx",
  "nova",
  "sage",
  "shimmer",
  "verse",
}

@doc("""
  Specifies the latency tier to use for processing the request. This parameter is relevant for customers subscribed to the scale tier service:
    - If set to 'auto', and the Project is Scale tier enabled, the system
      will utilize scale tier credits until they are exhausted.
    - If set to 'auto', and the Project is not Scale tier enabled, the request will be processed using the default service tier with a lower uptime SLA and no latency guarantee.
    - If set to 'default', the request will be processed using the default service tier with a lower uptime SLA and no latency guarantee.
    - If set to 'flex', the request will be processed with the Flex Processing service tier. [Learn more](/docs/guides/flex-processing).
    - When not set, the default behavior is 'auto'.
  
    When this parameter is set, the response body will include the `service_tier` utilized.
  """)
union ServiceTier {
  "auto",
  "default",
  "flex",
  "scale",
}

#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
enum OrderEnum {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  asc,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  desc,
}

/**Set of 16 key-value pairs that can be attached to an object. This can be
useful for storing additional information about the object in a structured
format, and querying for objects via API or the dashboard.
Keys are strings with a maximum length of 64 characters. Values are strings
with a maximum length of 512 characters.*/
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@extension("x-oaiTypeLabel", "map")
model Metadata {
  ...Record<string>;
}