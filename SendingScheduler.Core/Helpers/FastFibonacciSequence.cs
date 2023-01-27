namespace SendingScheduler.Core.Helpers;

public static class FastFibonacciSequence
{
    private static readonly double sqrtFive;
    private static readonly double a;
    private static readonly double b;

    static FastFibonacciSequence()
    {
        sqrtFive = Math.Sqrt(5);
        a = (sqrtFive + 1) / 2;
        b = (-sqrtFive + 1) / 2;
    }

    public static int ElementAtPlus3(int index)
    {
        var n = index + 3;
        return (int)((Math.Pow(a, n) - Math.Pow(b, n)) / sqrtFive);
    }
}
