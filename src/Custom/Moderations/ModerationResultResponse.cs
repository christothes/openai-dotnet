#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Moderations
{
    [CodeGenType("CreateModerationResponseResultCategories")] public partial class ModerationCategories { }
    [CodeGenType("CreateModerationResponseResultCategoryScores")] public partial class ModerationCategoryScores { }

    public partial class ModerationResultResponse
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal ModerationResultResponse(bool flagged, ModerationCategories categories, ModerationCategoryScores categoryScores, CreateModerationResponseResultCategoryAppliedInputTypes categoryAppliedInputTypes)
        {
            Flagged = flagged;
            Categories = categories;
            CategoryScores = categoryScores;
            CategoryAppliedInputTypes = categoryAppliedInputTypes;
        }

        internal ModerationResultResponse(bool flagged, ModerationCategories categories, ModerationCategoryScores categoryScores, CreateModerationResponseResultCategoryAppliedInputTypes categoryAppliedInputTypes, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            Flagged = flagged;
            Categories = categories;
            CategoryScores = categoryScores;
            CategoryAppliedInputTypes = categoryAppliedInputTypes;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public bool Flagged { get; }

        public ModerationCategories Categories { get; }

        public ModerationCategoryScores CategoryScores { get; }

        public CreateModerationResponseResultCategoryAppliedInputTypes CategoryAppliedInputTypes { get; }

        public IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}
