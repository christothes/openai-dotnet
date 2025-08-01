// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenAI;

namespace OpenAI.VectorStores
{
    [Experimental("OPENAI001")]
    public partial class VectorStore
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal VectorStore(string id, DateTimeOffset createdAt, string name, int usageBytes, VectorStoreFileCounts fileCounts, VectorStoreStatus status, DateTimeOffset? lastActiveAt)
        {
            Id = id;
            CreatedAt = createdAt;
            Name = name;
            UsageBytes = usageBytes;
            FileCounts = fileCounts;
            Status = status;
            LastActiveAt = lastActiveAt;
            Metadata = new ChangeTrackingDictionary<string, string>();
        }

        internal VectorStore(string id, DateTimeOffset createdAt, string name, int usageBytes, VectorStoreFileCounts fileCounts, VectorStoreStatus status, DateTimeOffset? expiresAt, DateTimeOffset? lastActiveAt, IReadOnlyDictionary<string, string> metadata, string @object, VectorStoreExpirationPolicy expirationPolicy, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            // Plugin customization: ensure initialization of collections
            Id = id;
            CreatedAt = createdAt;
            Name = name;
            UsageBytes = usageBytes;
            FileCounts = fileCounts;
            Status = status;
            ExpiresAt = expiresAt;
            LastActiveAt = lastActiveAt;
            Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
            Object = @object;
            ExpirationPolicy = expirationPolicy;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public string Id { get; }

        public DateTimeOffset CreatedAt { get; }

        public string Name { get; }

        public int UsageBytes { get; }

        public VectorStoreFileCounts FileCounts { get; }

        public VectorStoreStatus Status { get; }

        public DateTimeOffset? ExpiresAt { get; }

        public DateTimeOffset? LastActiveAt { get; }

        public IReadOnlyDictionary<string, string> Metadata { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}
