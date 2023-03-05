using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using HyperCasualTemplate;
using UnityEngine;

public static class Utilities
{
    private static System.Random rand = new System.Random();
    
    public static int RandomInt(int min, int max)
    {
        return rand.Next(min, max);
    }
    public static int RandomInt(int max)
    {
        return rand.Next(0, max);
    }
    
    public static string NumberToString(Int64 num)
    {
        if (num > 999999999 || num < -999999999)
        {
            num = num / 1000000000;
            num = num * 1000000000;
            return num.ToString("0,,,.#B", CultureInfo.InvariantCulture);
        }
        else if (num > 999999 || num < -999999)
        {
            num = num / 1000000;
            num = num * 1000000;
            return num.ToString("0,,.#M", CultureInfo.InvariantCulture);
        }
        else if (num > 99999 || num < -99999)
        {
            num = num / 1000;
            num = num * 1000;
            return num.ToString("0,.#K", CultureInfo.InvariantCulture);
        }
        else
        {
            return num.ToString(CultureInfo.InvariantCulture);
        }
    }
    
    public static string StringColor(Enums.ColorString color, string str)
    {
        switch (color)
        {
            case Enums.ColorString.yellow:
                return $"<color=yellow>{str}</color>";
            case Enums.ColorString.green:
                return $"<color=green>{str}</color>";
            case Enums.ColorString.red:
                return $"<color=red>{str}</color>";
            default:
                return str;
        }
    }
    
    public static void DelayCall(this MonoBehaviour mono, float waitTime, Action callback)
    {
        mono.StartCoroutine(CoroutineDelay(callback, waitTime));
    }

    private static IEnumerator CoroutineDelay(Action callback, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        callback.Invoke();
    }
}
