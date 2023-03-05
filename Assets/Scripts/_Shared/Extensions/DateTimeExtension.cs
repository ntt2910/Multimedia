using System;

public static class DateTimeExtension
{
    public static string ToISO8601(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:sszzz");
    }
    
    public static string ToUniversalBinaryString(this DateTime dateTime)
    {
        return dateTime.ToUniversalTime().ToBinary().ToString();
    }
    
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
    
    
    public static string ToReadableString(this TimeSpan span)
    {
        string formatted = string.Format("{0}{1}{2}{3}",
            span.Duration().Days > 0 ? $"{span.Days:0}d " : string.Empty,
            span.Duration().Hours > 0 ? string.Format("{0:0}h ", span.Hours) : string.Empty,
            span.Duration().Minutes > 0 ? string.Format("{0:0}m ", span.Minutes) : string.Empty,
            span.Duration().Seconds > 0 ? string.Format("{0:0}s ", span.Seconds) : string.Empty);

        if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

        if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

        return formatted;
    }
}