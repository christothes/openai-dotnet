// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using OpenAI;

namespace OpenAI.Evals
{
    internal partial class InternalEvalCustomDataSourceConfigParams : InternalEvalDataSourceConfigParams
    {
        public InternalEvalCustomDataSourceConfigParams(IDictionary<string, BinaryData> itemSchema) : base(InternalEvalDataSourceConfigType.Custom)
        {
            // Plugin customization: ensure initialization of collections
            Argument.AssertNotNull(itemSchema, nameof(itemSchema));

            ItemSchema = itemSchema ?? new ChangeTrackingDictionary<string, BinaryData>();
        }

        internal InternalEvalCustomDataSourceConfigParams(InternalEvalDataSourceConfigType kind, IDictionary<string, BinaryData> additionalBinaryDataProperties, IDictionary<string, BinaryData> itemSchema, bool? includeSampleSchema) : base(kind, additionalBinaryDataProperties)
        {
            // Plugin customization: ensure initialization of collections
            ItemSchema = itemSchema ?? new ChangeTrackingDictionary<string, BinaryData>();
            IncludeSampleSchema = includeSampleSchema;
        }

        public IDictionary<string, BinaryData> ItemSchema { get; }

        public bool? IncludeSampleSchema { get; set; }
    }
}
