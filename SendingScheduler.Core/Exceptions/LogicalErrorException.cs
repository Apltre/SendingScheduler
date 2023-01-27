using System.Collections;

namespace SendingScheduler.Core.Exceptions;

public class LogicalErrorException : Exception
{
    public sealed override IDictionary Data
    {
        get
        {
            var dict = new Dictionary<string, object>
            {
                { nameof(Message), Message }
            };
            AddData(dict);
            return dict;
        }
    }

    public LogicalErrorException(string message) : base(message)
    {
    }

    protected virtual void AddData(Dictionary<string, object> dict)
    {
    }
}
