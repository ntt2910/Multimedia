using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTablet
{
    public static float DeviceDiagonalSizeInInches()
    {
        float screenWidth = Screen.width / Screen.dpi;
        float screenHeight = Screen.height / Screen.dpi;
        float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
        // Debug.LogError("Screen.width " + Screen.width);
        // Debug.LogError("Screen.height " + Screen.height);
        // Debug.LogError("diagonalInches " + diagonalInches);
        // float aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
        // Debug.LogError("aspectRatio " + aspectRatio);
        return diagonalInches;
    }

    public static bool IsTable()
    {
#if UNITY_EDITOR
        return false;
#elif UNITY_IOS
        bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
        if (deviceIsIpad)
        {
            return true;
        }
        bool deviceIsIphone = UnityEngine.iOS.Device.generation.ToString().Contains("iPhone");
        if (deviceIsIphone)
        {
            return false;
        }
#elif UNITY_ANDROID
        float aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
        Debug.Log("Screen.width " + Screen.width);
        Debug.Log("Screen.height " + Screen.height);
        Debug.Log("aspectRatio " + aspectRatio);
        return DeviceDiagonalSizeInInches() >= 6.5f && aspectRatio < 2f;
#endif
        return false;
    }
}