using System.Collections;
using UnityEngine;
//using Firebase.Extensions;
using GameAnalyticsSDK;
using GoogleMobileAds.Api;

public class FirebaseManager : MonoBehaviour
{
    //Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    public bool isInitialized = false;
    public float callRate = 1f;
    public float maxTime = 60;
    float time = 0;
    public static FirebaseManager instance;
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    IEnumerator Start()
    {
        //TenjinConnect();
        
        GameAnalytics.Initialize();
        
        while (!isInitialized && (time < maxTime))
        {
            time += 1;
            try
            {
                // Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
                // {
                //     dependencyStatus = task.Result;
                //     if (dependencyStatus == Firebase.DependencyStatus.Available)
                //     {
                //         Debug.Log("Firebase is ready!");
                //         isInitialized = true;
                //         Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                //
                //         RemoteConfigControl.instance.InitializeFirebase();
                //     }
                //     else
                //     {
                //         Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                //     }
                // });
            }
            catch (System.Exception)
            {
                throw;
            }
            yield return new WaitForSeconds(callRate);
        }
    }
    
    public void OnPaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        // double revenue = impressionData.Revenue;
        // var impressionParameters = new[] {
        //     new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
        //     new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
        //     new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
        //     new Firebase.Analytics.Parameter("ad_format", impressionData.Placement),
        //     new Firebase.Analytics.Parameter("value", revenue),
        //     new Firebase.Analytics.Parameter("currency", "USD"), // All Applovin revenue is sent in USD
        // };
        // Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }
    
    public void sendRevenueFirebase(string nameAD , AdValueEventArgs args)
    {
        // AdValue adValue = args.AdValue;
        // double revenue =adValue.Value;
        // var impressionParameters = new[] {
        //     new Firebase.Analytics.Parameter("ad_platform", "ADMOD"),
        //     new Firebase.Analytics.Parameter("ad_source", "admod"),
        //     // new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
        //     // new Firebase.Analytics.Parameter("ad_format", impressionData.Placement),
        //     new Firebase.Analytics.Parameter("value", (revenue / 1000000f)),
        //     new Firebase.Analytics.Parameter("currency", "USD"), // All Applovin revenue is sent in USD
        // };
        // Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }


    void OnApplicationPause(bool pauseStatus) {
        //if (!pauseStatus) {
        //    TenjinConnect();
        //}
    }

    public void TenjinConnect() {
        //BaseTenjin instance = Tenjin.getInstance("WEEXFZLFST3KWAH62UZ79NO7DW4WPWUK");

        // Sends install/open event to Tenjin
        //instance.Connect();
    }
}
