using System;
using System.Collections;
using System.Collections.Generic;
//using Firebase.Analytics;
using GameAnalyticsSDK;
//using GameAnalyticsSDK;
using UnityEngine;

public static class AnalyticsEvent
{
    public static string LEVEL_START = "LEVEL_START_";
    public static string LEVEL_GIVE_UP = "LEVEL_GIVE_UP_";
    public static string LEVEL_WIN = "LEVEL_WIN_";
    public static string LEVEL_FAIL = "LEVEL_FAIL_";
    public static string LEVEL_REPLAY = "LEVEL_REPLAY_";
    public static string LEVEL_EXIT = "LEVEL_EXIT_";
    public static string WAIT_REVIVE_POPUP = "WAIT_REVIVE_POPUP_LEVEL";

    public static string LEVEL_COMPLETE = "level_complete";
    public static string TIMES_PER_FIRST_SECTION = "times_per_first_section";
    public static string COMPLETE_LEVEL = "complete_level_";
    public static string START_GAME = "start_game";
    public static string WIN_GAME = "win_game";
    public static string LOSE_GAME = "lose_game";

}
public class AnalyticsManager : MonoBehaviour
{
    private const string FIRSTTIME = "FIRSTTIME";
    private DateTime firstTimeOnTheGame;
    private DateTime timeStart;
    public static AnalyticsManager instance;
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
#if UNITY_EDITOR
        Application.runInBackground = false;
#endif
        GetDateTime();
        timeStart = System.DateTime.Now;
    }

    private void GetDateTime()
    {
        try
        {
            if (PlayerPrefs.HasKey(FIRSTTIME))
            {
                string dateTimeString = PlayerPrefs.GetString(FIRSTTIME);
                System.DateTime dateTime = System.DateTime.Parse(dateTimeString);
                firstTimeOnTheGame = dateTime;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (firstTimeOnTheGame.Year < 2021)
        {
            if (pauseStatus)
            {
                firstTimeOnTheGame = System.DateTime.Now;
                PlayerPrefs.SetString(FIRSTTIME, firstTimeOnTheGame.ToString());
                LogEventTimesInFirstSection((int)(System.DateTime.Now - timeStart).TotalSeconds);
            }
        }
    }
    public void LogLevelStartEvent(string level)
    {
        try
        {
            //Firebase.Analytics.FirebaseAnalytics.LogEvent($"{AnalyticsEvent.LEVEL_START}{level}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public void LogLevelGiveUp(string level)
    {
        try
        {
            //Firebase.Analytics.FirebaseAnalytics.LogEvent($"{AnalyticsEvent.LEVEL_GIVE_UP}{level}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public void LogLevelCompleteEvent(string level)
    {
        try
        {
            //Firebase.Analytics.FirebaseAnalytics.LogEvent($"{AnalyticsEvent.LEVEL_WIN}{level}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public void LogLevelFailEvent(string level)
    {
        try
        {
            // Firebase.Analytics.FirebaseAnalytics.LogEvent($"{AnalyticsEvent.LEVEL_FAIL}{level}",
            // new Firebase.Analytics.Parameter("Level", level));
            // ,new Firebase.Analytics.Parameter("failcount", failCount)
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public void LogWaitPopUpRevive(string level)
    {
        try
        {
            // Firebase.Analytics.FirebaseAnalytics.LogEvent($"{AnalyticsEvent.WAIT_REVIVE_POPUP}{level}",
            // new Firebase.Analytics.Parameter("Level", level));
            // ,new Firebase.Analytics.Parameter("failcount", failCount)
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public void LogLevelReplayEvent(string level, int replayAmount)
    {
        try
        {
            // Firebase.Analytics.FirebaseAnalytics.LogEvent($"{AnalyticsEvent.LEVEL_REPLAY}{level}",
            // new Firebase.Analytics.Parameter("Replay", replayAmount));
            // ,new Firebase.Analytics.Parameter("failcount", failCount)
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public void LogLevelExitEvent(string level)
    {
        try
        {
            //Firebase.Analytics.FirebaseAnalytics.LogEvent($"{AnalyticsEvent.LEVEL_EXIT}{level}");
            // ,new Firebase.Analytics.Parameter("failcount", failCount)
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void LogAdsEvent(string eventName, bool hasAds, bool internet, string placement)
    {
     
        // var parameters = new[] {
        //     new Firebase.Analytics.Parameter ("has_ads", hasAds.ToString()),
        //     new Firebase.Analytics.Parameter ("internet_available", internet.ToString()),
        //     new Firebase.Analytics.Parameter ("placement", placement),
        // };
        // FirebaseAnalytics.LogEvent(eventName, parameters);
        //try
        //{
        //    if (placement.Equals(string.Empty))
        //    {
        //        if (error.Equals(string.Empty))
        //        {
        //            FirebaseAnalytics.LogEvent(eventName, parameters);
        //        }
        //        else
        //        {
        //            FirebaseAnalytics.LogEvent(eventName);
        //        }
        //    }
        //    else
        //    {
        //        FirebaseAnalytics.LogEvent(eventName,
        //        new Parameter("placement", placement),
        //        new Parameter("error", error));
        //    }
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError(e);
        //}
    }


    public void LogEventTimesInFirstSection(long seconds)
    {
        try
        {
            //Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsEvent.TIMES_PER_FIRST_SECTION, "timeplayed", seconds.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void LogEventCheckPoint(string level)
    {
        try
        {
            //FirebaseAnalytics.LogEvent($"{AnalyticsEvent.COMPLETE_LEVEL}{level}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void SetUserProperties(string level)
    {
        try
        {
            //FirebaseAnalytics.SetUserProperty("level", level);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// ① Stage start: Send at the timing when the game starts (for example, the timing when you tap on the Tap to start screen)
    /// </summary>
    /// <param name="number_of_stages"></param>
    public void LogStageStart(int number_of_stages)
    {
        try
        {
            //FirebaseAnalytics.LogEvent ("StageStart" + number_of_stages.ToString ("000"));
            
            GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "Stage" + number_of_stages. ToString ("000"));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    /// <summary>
    /// Stage failure: Send at the timing of failure
    /// </summary>
    /// <param name="number_of_stages"></param>
    public void LogStageFailure(int number_of_stages)
    {
        try
        {
            //FirebaseAnalytics.LogEvent ("StageFail" + number_of_stages.ToString ("000"));
            
            GameAnalytics.NewProgressionEvent (GAProgressionStatus.Fail, "Stage" + number_of_stages.ToString ("000"));
            
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    /// <summary>
    /// ③ End of stage: Send when the stage is cleared
    /// </summary>
    /// <param name="number_of_stages"></param>
    public void LogStagClear(int number_of_stages)
    {
        try
        {
            //FirebaseAnalytics.LogEvent ("StageClear" + number_of_stages.ToString ("000"));
            
            GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "Stage" + number_of_stages.ToString ("000"));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    /// <summary>
    /// ④ When playing an insta advertisement
    /// </summary>
    /// <param name="number_of_stages"></param>
    public void LogShowInterAd(int integer_views)
    {
        try
        {
            //"Int_Impression" + 000 fills in the number of views of the inst
            //FirebaseAnalytics.LogEvent ("Int_Impression" + integer_views.ToString ("000"));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    /// <summary>
    /// ⑤ When playing reward advertisements
    /// </summary>
    /// <param name="integer_views"></param>
    public void LogShowRewardAd(int integer_views)
    {
        try
        {
            //"Int_Impression" + 000 fills in the number of views of the inst
            //FirebaseAnalytics.LogEvent ("Rwd_Impression" + integer_views.ToString ("000"));
        }
        catch (Exception e)
        {
            Debug.LogError(e);  
        }
    }
    
    /// <summary>
    /// ⑥ When playing advertisements on the monetization platform
    /// </summary>
    /// <param name="adInfo"></param>
    public void TrackAdRevenue (MaxSdk.AdInfo adInfo)
    {
        // var impressionParameters = new [] {
        //     new Firebase.Analytics.Parameter ("ad_platform", "AppLovin"),
        //     new Firebase.Analytics.Parameter ("ad_source", adInfo.NetworkName),
        //     new Firebase.Analytics.Parameter ("ad_unit_name", adInfo.AdUnitIdentifier),
        //     new Firebase.Analytics.Parameter ("ad_format", adInfo.Placement),
        //     new Firebase.Analytics.Parameter ("value", adInfo.Revenue),
        //     new Firebase.Analytics.Parameter ("currency", "USD"), // All AppLovin revenue is sent in USD
        // };;
        //FirebaseAnalytics.LogEvent ("ad_impression", impressionParameters);
    }
}
