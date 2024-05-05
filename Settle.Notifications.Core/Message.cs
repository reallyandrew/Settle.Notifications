using Settle.Notifications.Core.Primitives;

namespace Settle.Notifications.Core;
public abstract class Message<T> where T : ValueObject
{
    protected Message(T sender)
    {
        Sender = sender;
    }
    public IEnumerable<T> To { get; protected set; } = new List<T>();
    public string Body { get; protected set; } = string.Empty;
    public T Sender { get; protected set; }
}
