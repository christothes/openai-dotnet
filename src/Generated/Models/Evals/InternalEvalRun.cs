// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using OpenAI;

namespace OpenAI.Evals
{
    internal partial class InternalEvalRun
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal InternalEvalRun(string id, string evalId, string status, string model, string name, DateTimeOffset createdAt, string reportUrl, InternalEvalRunResultCounts resultCounts, IEnumerable<InternalEvalRunPerModelUsage> perModelUsage, IEnumerable<InternalEvalRunPerTestingCriteriaResult> perTestingCriteriaResults, InternalEvalRunDataSourceResource dataSource, IDictionary<string, string> metadata, InternalEvalApiError error)
        {
            // Plugin customization: ensure initialization of collections
            Id = id;
            EvalId = evalId;
            Status = status;
            Model = model;
            Name = name;
            CreatedAt = createdAt;
            ReportUrl = reportUrl;
            ResultCounts = resultCounts;
            PerModelUsage = perModelUsage.ToList();
            PerTestingCriteriaResults = perTestingCriteriaResults.ToList();
            DataSource = dataSource;
            Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
            Error = error;
        }

        internal InternalEvalRun(string @object, string id, string evalId, string status, string model, string name, DateTimeOffset createdAt, string reportUrl, InternalEvalRunResultCounts resultCounts, IList<InternalEvalRunPerModelUsage> perModelUsage, IList<InternalEvalRunPerTestingCriteriaResult> perTestingCriteriaResults, InternalEvalRunDataSourceResource dataSource, IDictionary<string, string> metadata, InternalEvalApiError error, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            // Plugin customization: ensure initialization of collections
            Object = @object;
            Id = id;
            EvalId = evalId;
            Status = status;
            Model = model;
            Name = name;
            CreatedAt = createdAt;
            ReportUrl = reportUrl;
            ResultCounts = resultCounts;
            PerModelUsage = perModelUsage ?? new ChangeTrackingList<InternalEvalRunPerModelUsage>();
            PerTestingCriteriaResults = perTestingCriteriaResults ?? new ChangeTrackingList<InternalEvalRunPerTestingCriteriaResult>();
            DataSource = dataSource;
            Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
            Error = error;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public string Object { get; } = "eval.run";

        public string Id { get; }

        public string EvalId { get; }

        public string Status { get; }

        public string Model { get; }

        public string Name { get; }

        public DateTimeOffset CreatedAt { get; }

        public string ReportUrl { get; }

        internal InternalEvalRunResultCounts ResultCounts { get; }

        internal IList<InternalEvalRunPerModelUsage> PerModelUsage { get; }

        internal IList<InternalEvalRunPerTestingCriteriaResult> PerTestingCriteriaResults { get; }

        internal InternalEvalRunDataSourceResource DataSource { get; }

        public IDictionary<string, string> Metadata { get; }

        internal InternalEvalApiError Error { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}
