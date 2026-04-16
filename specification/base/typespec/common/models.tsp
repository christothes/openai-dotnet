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