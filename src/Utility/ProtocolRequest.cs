using System.Collections.Generic;

namespace System.ClientModel
{
    public class ProtocolRequest : ProtocolBody<BinaryContent>
    {
        public ProtocolRequest(BinaryContent body = null) : base(body)
        {
        }
        public override BinaryContent Body { get; set; }
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();
    }
}