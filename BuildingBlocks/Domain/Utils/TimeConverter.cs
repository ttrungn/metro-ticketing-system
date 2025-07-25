namespace BuildingBlocks.Domain.Utils;

public static class TimeConverter
{
    public static DateTimeOffset GetCurrentVietNamTime()
    {
        var createdAtUtc = DateTimeOffset.UtcNow;

        TimeZoneInfo vietnamTimeZone;
        if (OperatingSystem.IsWindows())
        {
            vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        }
        else
        {
            vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
        }
        var createdAtInVietnam = TimeZoneInfo.ConvertTime(createdAtUtc, vietnamTimeZone);

        return createdAtInVietnam;
    }

    public static DateTimeOffset ToVietNamTime(DateTimeOffset time)
    {
        var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var timeAtVietNam = TimeZoneInfo.ConvertTime(time, vietnamTimeZone);

        return timeAtVietNam;
    }
}