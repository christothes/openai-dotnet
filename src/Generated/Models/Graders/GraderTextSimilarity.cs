// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenAI;

namespace OpenAI.Graders
{
    [Experimental("OPENAI001")]
    public partial class GraderTextSimilarity : Grader
    {
        public GraderTextSimilarity(string name, string input, string reference, GraderTextSimilarityEvaluationMetric evaluationMetric) : base(GraderType.TextSimilarity)
        {
            Argument.AssertNotNull(name, nameof(name));
            Argument.AssertNotNull(input, nameof(input));
            Argument.AssertNotNull(reference, nameof(reference));

            Name = name;
            Input = input;
            Reference = reference;
            EvaluationMetric = evaluationMetric;
        }

        internal GraderTextSimilarity(GraderType kind, IDictionary<string, BinaryData> additionalBinaryDataProperties, string name, string input, string reference, GraderTextSimilarityEvaluationMetric evaluationMetric) : base(kind, additionalBinaryDataProperties)
        {
            Name = name;
            Input = input;
            Reference = reference;
            EvaluationMetric = evaluationMetric;
        }

        public string Name { get; set; }

        public string Input { get; set; }

        public string Reference { get; set; }

        public GraderTextSimilarityEvaluationMetric EvaluationMetric { get; set; }
    }
}
