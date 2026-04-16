import "../../base/typespec/audio/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;

namespace OpenAI;

/** A log probability object. */
model LogProbProperties {
  /** The token that was used to generate the log probability. */
  token: string;

  /** The log probability of the token. */
  logprob: float32;

  /** The bytes that were used to generate the log probability. */
  bytes: int32[];
}

@access(Access.public)
@usage(Usage.output)
model DotNetAudioLogProbsProperties {
  ...LogProbProperties;
}

@access(Access.public)
@usage(Usage.output)
model DotNetCombinedJsonTranscriptionResponse {
  ...CreateTranscriptionResponseVerboseJson;
  logprobs?: LogProbProperties[];
}

// Remove this after https://github.com/microsoft/openai-openapi-pr/issues/4785 is fixed
@usage(Usage.output | Usage.json)
@discriminator("type")
model DotNetCreateTranscriptionStreamingResponse {
  type: DotNetCreateTranscriptionStreamingResponseType;
}

union DotNetCreateTranscriptionStreamingResponseType {
  `transcript.text.segment`: "transcript.text.segment",
  `transcript.text.delta`: "transcript.text.delta",
  `transcript.text.done`: "transcript.text.done",
  string
}

model DotNetTranscriptTextSegmentEvent extends DotNetCreateTranscriptionStreamingResponse {
  ...TranscriptTextSegmentEvent;
}

model DotNetTranscriptTextDeltaEvent extends DotNetCreateTranscriptionStreamingResponse {
  ...TranscriptTextDeltaEvent;
}

model DotNetTranscriptTextDoneEvent extends DotNetCreateTranscriptionStreamingResponse {
  ...TranscriptTextDoneEvent;
}

@usage(Usage.output | Usage.json)
@discriminator("type")
model DotNetCreateSpeechStreamingResponse {
  type: DotNetCreateSpeechStreamingResponseType;
}

union DotNetCreateSpeechStreamingResponseType {
  `speech.audio.delta`: "speech.audio.delta",
  `speech.audio.done`: "speech.audio.done",
  string
}

model DotNetSpeechAudioDeltaEvent extends DotNetCreateSpeechStreamingResponse {
  ...SpeechAudioDeltaEvent;
}

model DotNetSpeechAudioDoneEvent extends DotNetCreateSpeechStreamingResponse {
  ...SpeechAudioDoneEvent;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union DotNetTranscriptionDefaultChunkingStrategy {
  string,
  auto: "auto",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetTranscriptionChunkingStrategy {
  default_chunking_strategy?: DotNetTranscriptionDefaultChunkingStrategy;
  custom_chunking_strategy?: ChunkingStrategyConfig;
}
