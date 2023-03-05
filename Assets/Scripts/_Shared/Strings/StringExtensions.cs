using System;
using System.Globalization;
using System.Text.RegularExpressions;

public static partial class StringExtensions
{
    public static bool IsNotNullOrEmpty(this string str)
    {
        return !string.IsNullOrEmpty(str);
    }

    public static string ToTitleCase(this string title)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
    }
    
    public static string RemoveBetween(this string sourceString, string startTag, string endTag)
    {
        Regex regex = new Regex($"{Regex.Escape(startTag)}(.*?){Regex.Escape(endTag)}", RegexOptions.RightToLeft);
        return regex.Replace(sourceString, startTag+endTag);
    }
    public static DateTime ToBinaryUniversalDateTime(this string str)
    {
        if (string.IsNullOrEmpty(str)) return DateTime.UtcNow;
        return long.TryParse(str, out long binary) ? DateTime.FromBinary(binary).ToUniversalTime() : DateTime.UtcNow;
    }
}