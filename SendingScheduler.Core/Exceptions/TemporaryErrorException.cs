using System.Collections;

namespace SendingScheduler.Core.Exceptions;

public class TemporaryErrorException : Exception
{
    public sealed override IDictionary Data
    {
        get
        {
            var dict = new Dictionary<string, object> { { nameof(Message), Message } };
            AddData(dict);
            return dict;
        }
    }

    public TemporaryErrorException(string message) : base(message)
    {
    }

    public TemporaryErrorException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected virtual void AddData(Dictionary<string, object> dict)
    {
    }
}
