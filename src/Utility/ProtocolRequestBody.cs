#nullable enable

namespace System.ClientModel
{
    public abstract class ProtocolBody<T>
    {
        protected ProtocolBody(T? body = default)
        {
            Body = body;
        }
        public abstract T? Body { get; set; }
    }
}