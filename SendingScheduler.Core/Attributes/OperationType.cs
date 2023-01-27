namespace SendingScheduler.Core.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class OperationType : Attribute
{
    public OperationType(int type)
    {
        Type = type;
    }

    public OperationType(object type)
    {
        Type = (int)type;
    }

    public int Type { get; }
}
