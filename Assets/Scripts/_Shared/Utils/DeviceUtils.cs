using System;
using System.Globalization;
using UnityEngine;

namespace BW.Utils
{
    public static class DeviceUtils
    {
        /// <summary>
        /// Gets the device identifier and updates the static variables
        /// </summary>
        /// <returns><c>true</c>, if device identifier was obtained, <c>false</c> otherwise.</returns>
        public static string GetDeviceId()
        {
#if UNITY_EDITOR
            return SystemInfo.deviceUniqueIdentifier;
#elif UNITY_ANDROID
            var clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            var objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
            var clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
            return clsSecure.CallStatic<string>("getString", objResolver, "android_id");
#elif UNITY_IOS
            return UnityEngine.iOS.Device.vendorIdentifier;
#else
            return SystemInfo.deviceUniqueIdentifier;
#endif
        }

        public static string GetCurrentCountryCode()
        {
            try
            {
                var regionInfo = new RegionInfo(CultureInfo.CurrentCulture.LCID);
                return regionInfo.TwoLetterISORegionName;
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message + " " + e.StackTrace);
                return "US";
            }
        }
    }
}