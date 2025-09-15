using TypeSpec.OpenAPI;

namespace OpenAI;

union CreateFineTuningJobRequestHyperparametersBatchSizeChoiceEnum {
  auto: "auto",
}
union CreateFineTuningJobRequestHyperparametersBatchSizeOption {
  CreateFineTuningJobRequestHyperparametersBatchSizeChoiceEnum,
  integer,
}

union CreateFineTuningJobRequestHyperparametersLearningRateMultiplierChoiceEnum {
  auto: "auto",
}
union CreateFineTuningJobRequestHyperparametersLearningRateMultiplierOption {
  CreateFineTuningJobRequestHyperparametersLearningRateMultiplierChoiceEnum,
  float32,
}

union CreateFineTuningJobRequestHyperparametersNEpochsChoiceEnum {
  auto: "auto",
}
union CreateFineTuningJobRequestHyperparametersNEpochsOption {
  CreateFineTuningJobRequestHyperparametersNEpochsChoiceEnum,
  integer,
}

union CreateFineTuningJobRequestHyperparametersBetaChoiceEnum {
  auto: "auto",
}
union CreateFineTuningJobRequestHyperparametersBetaOption {
  CreateFineTuningJobRequestHyperparametersLearningRateMultiplierChoiceEnum,
  integer,
}

@discriminator("type")
model CreateFineTuningJobRequestIntegration {
  type: string | "wandb";
}

model CreateFineTuningJobRequestWandbIntegration
  extends CreateFineTuningJobRequestIntegration {
  type: "wandb";
  wandb: {
    project: string;
    name?: string | null;
    entity?: string | null;
    tags?: string[];
  };
}

@discriminator("type")
model FineTuningIntegration {
  type: string | "wandb";
}

model FineTuningJobsPageToken {
  limit?: integer;
  after?: string;
}
