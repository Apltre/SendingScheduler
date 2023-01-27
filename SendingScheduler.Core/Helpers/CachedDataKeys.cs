namespace SendingScheduler.Core.Helpers;

public static class CacheKeys
{
    public static string SendDataKey(this long id) => $"{id}-jobData";
    public static string ResponseDataKey(this long id) => $"{id}-responseData";
}
