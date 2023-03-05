using System.Globalization;
using UnityEngine;

public static class ColorExtension
{
    public static void SetAlpha(this Color color, float a)
    {
        var tmp = color;
        tmp.a = a;
        color = tmp;
    }

    public static Color HexToColor(string hex)
    {
        hex = hex.TrimStart('#');

        Color col = Color.black;

        if (hex.Length == 6)
        {
            col = new Color( // hardcoded opaque
                int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
                255f);
        }
        else // assuming length of 8
        {
            col = new Color(
                int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
                int.Parse(hex.Substring(6, 2), NumberStyles.HexNumber));
        }

        return col;
    }

    public static string GetHexRGB(this Color color)
    {
        return ColorUtility.ToHtmlStringRGB(color);
    }

    public static string GetHexRGBA(this Color color)
    {
        return ColorUtility.ToHtmlStringRGBA(color);
    }
}