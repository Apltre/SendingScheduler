namespace SendingScheduler.Core.Helpers;

public class Reprocessing
{
    public static readonly Random random = new Random();

    public static TimeSpan GetDelay(int attempt)
    {
        var startingDelay = TimeSpan.FromMinutes(FastFibonacciSequence.ElementAtPlus3(attempt));
        var randomMove = TimeSpan.FromSeconds(random.Next(0, 120));

        return startingDelay.Add(randomMove);
    }
}