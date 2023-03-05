using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScreenRatio
{
    None,
    R_3_4,
    R_3_2,
    R_16_9,
    R_18_9,
    R_19_9,
}

public static class ScreenDetection
{
    public static ScreenRatio GetRatio()
    {
        float ratio = (float)Screen.width / (float)Screen.height;
        if (ratio >= 0.7f)   //3:4
        {
            return ScreenRatio.R_3_4;
        }
        else
        if (ratio >= 0.6f)  //3:2
        {
            return ScreenRatio.R_3_2;
        }
        else
        if (ratio >= 0.56)  //16:9
        {
            return ScreenRatio.R_16_9;
        }
        else
        if (ratio >= 0.5)   //18:9
        {
            return ScreenRatio.R_18_9;
        }
        else
        if (ratio >= 0.47)   //19:9
        {
            return ScreenRatio.R_19_9;
        }
        else
        {
            return ScreenRatio.None;
        }
    }
}
