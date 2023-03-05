using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GoogleMobileAds.Api;
using LTABase.DesignPattern;

#if UNITY_IOS
namespace AudienceNetwork
{
    public static class AdSettings
    {
        [DllImport("__Internal")]
        private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);

        public static void SetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled)
        {
            FBAdSettingsBridgeSetAdvertiserTrackingEnabled(advertiserTrackingEnabled);
        }
    }
}
#endif

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance = null;
    //private bool doneAdsInterval = true;    // True đã countdown xong và cho phép hiển thị, false thì chưa
    private WaitForSecondsRealtime waitRewarded;

    static Action actionRewarded;
    static Action actionFullAds;

    //static string forceAdsPosition;//ad start
    //static string rewardAdsPosition;

    const string MaxSdkKey = "lBnu4FPbTxGUi8cqgwNENfRfqINTVk8B9NxC2Japp2DqKPThsIhPCF7zcC6wr9_IN8OLTcG9SR4dT4OJwQPTBf"; //

#if UNITY_IOS
    const string InterstitialAdUnitId = "";
    const string RewardedAdUnitId = "";
    const string BannerAdUnitId = "";
#else
    const string InterstitialAdUnitId = "00d1ae5f153b22be";
    const string RewardedAdUnitId = "f3eaa6d464503eb7";
    const string BannerAdUnitId = "5d38689f99b726c3";
#endif

    private bool isBannerShowing;
    private bool isMRecShowing;
    private bool isAdsShowing;

    private int interstitialRetryAttempt;
    private int bannerRetryAttempt;
    private int rewardedRetryAttempt;
    private int rewardedInterstitialRetryAttempt;

    private int tracking_timeLoadAds, tracking_timeLoadAds2;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(instance);

        waitRewarded = new WaitForSecondsRealtime(0.2f);

        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            // AppLovin SDK is initialized, configure and start loading ads.
            Debug.Log("MAX SDK Initialized");

            InitializeInterstitialAds();
            InitializeRewardedAds();
            //InitializeRewardedInterstitialAds();
            InitializeBannerAds();
            //InitializeMRecAds();
        };

        MaxSdk.SetSdkKey(MaxSdkKey);
        MaxSdk.InitializeSdk();

        //MaxSdk.ShowMediationDebugger();
    }
    
    #region Retry

    private void ReinitAdSdk()
    {
        if (CheckInternet() && !MaxSdk.IsInitialized())
        {
            MaxSdk.SetSdkKey(MaxSdkKey);
            MaxSdk.InitializeSdk();
        }
    }
    public void TryInitAndRequestAd()
    {
        ReinitAdSdk();
        LoadRewardedAd();
        LoadInterstitial();
        InitializeBannerAds();
    }
    #endregion

    #region Interstitial Ad Methods

    private void InitializeInterstitialAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += InterstitialOnAdLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += InterstitialOnAdLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += InterstitialOnAdDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialOnAdDisplayFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += InterstitialOnAdClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += InterstitialOnAdHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += InterstitialOnAdRevenuePaidEvent;
        //fix anr
        LoadFullAds();
    }

    // Load the first interstitial
    void LoadFullAds()
    {
        LoadInterstitial();
    }

    public void LoadInterstitial()
    {
        Debug.Log("Load Full Ads");
        if (MaxSdk.IsInitialized())
        {
            Debug.Log("Load Full Ads - Max Ready");
            if (!MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
            {
                MaxSdk.LoadInterstitial(InterstitialAdUnitId);
                tracking_timeLoadAds = (int)Time.time;
            }
            else
            {
                Debug.Log("Load Full Ads - AdsIsReady - Not Load");
            }
        }
        else
        {
            interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

            Debug.Log("ReLoad Full Ads - Max Not Ready");

            Invoke("LoadInterstitial", (float)retryDelay);
        }
    }

    public void ShowAdUnit(Action actionDone, string position)
    {
#if UNITY_EDITOR
        Debug.Log($"Show Full Ads: {position}");
        actionDone?.Invoke();
        DataManager.LastTimeShowInterAds = DateTime.Now;
        //DataManager.LastTimeShowAds = DateTime.Now;
        return;
#else
        // forceAdsPosition = position;
        // if (IsNoAds())
        // {
        //     Debug.Log("check_noads_" + IsNoAds());
        //     actionDone?.Invoke();
        //     return;
        // }
        
        if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
        {
        DataManager.LastTimeShowInterAds = DateTime.Now;
            //AdjustManager.instance.TrackAdEvent(AdjustEventKeys.ad_inters_api_called);
            Debug.Log("ad ready, show ad");
            if(actionDone != null) actionFullAds = actionDone;
            isAdsShowing = true;
            DataManager.LastTimeShowAds = DateTime.Now;
            MaxSdk.ShowInterstitial(InterstitialAdUnitId);
            AnalyticsManager.instance.LogAdsEvent("show_interstitial_ads", true, true, position);
        }
        else
        {
            Debug.Log("ad not ready");
            actionDone.Invoke();
            if (CheckInternet())
            {
                //LogAdEvent("inter_ad", "InterstitialNotReady_InternetReady", string.Empty);
                AnalyticsManager.instance.LogAdsEvent("show_interstitial_ads", false, true, position);
            }
            else
            {
                // show no internet
                AnalyticsManager.instance.LogAdsEvent("show_interstitial_ads", false, false, position);
            }
            //
            // LogAdEvent("inter_ad", "InterstitialNotReady", string.Empty);
        }
#endif
    }
    
    private void InterstitialOnOnPaidEvent(object sender, AdValueEventArgs args)
    {
        if (FirebaseManager.instance != null)
        {
            //Debug.LogError("GetMediationAdapterClassName: "+
            FirebaseManager.instance.sendRevenueFirebase("INTERSTITIAL_ADS" , args);
        }
        AdjustManager.instance.sendRevAdjust("INTERSTITIAL_ADS" , args);
    }
    
    private void InterstitialOnPaidEvent(object sender, AdValueEventArgs args)
    {
        if (AdjustManager.instance != null)
        {
            //Debug.LogError("GetMediationAdapterClassName: "+
            AdjustManager.instance.sendRevAdjust("INTERSTITIAL_ADS" , args);
        }
    }



    private void InterstitialOnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
        interstitialRetryAttempt = 0;

        int time = (int)Time.time - tracking_timeLoadAds;

        // LogAdEvent("inter_ad", "InterstitialAdLoadedEvent", string.Empty);
    }

    private void InterstitialOnAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        // Debug.Log("Interstitial failed to load with error code: " + errorInfo);
        // Debug.Log("Load AdInter Fail:" + adUnitId);

        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
        Invoke("LoadInterstitial", (float)retryDelay);

        //LogAdEvent("inter_ad", "InterstitialAdLoadFailedEvent", string.Empty);
    }

    private void InterstitialOnAdDisplayedEvent(string adUnitID, MaxSdkBase.AdInfo adInfo)
    {
        //AdjustManager.instance.TrackAdEvent(AdjustEventKeys.ad_inters_displayed);
        AnalyticsManager.instance.TrackAdRevenue(adInfo);

        Debug.Log("InterstitialOnAdDisplayedEvent AdInfo :" + adInfo);
        Debug.Log("InterstitialOnAdDisplayedEvent AdInfo revenue :" + adInfo.Revenue);
        // Debug.Log("Interstitial OnIterstitailDisplayEvent - pause game show ads");
        // GameManager.Instance.PauseGameShowAds(true);
        
        //LogAdEvent("inter_ad", "Interstitial_Ad_DisplayedEvent", string.Empty);
    }

    private void InterstitialOnAdDisplayFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        Debug.Log("Interstitial failed to display with error code: " + errorInfo);

        if (actionFullAds != null)
        {
            actionFullAds.Invoke();
            actionFullAds = null;

            //SetAdsInterval();
        }

        //fix anr
        Debug.Log("Show AdInter Fail:" + adUnitId);
        Invoke("LoadInterstitial", 0.5f);

        //LogAdEvent("inter_ad", "InterstitialAdShowFailedEvent", string.Empty);
    }

    private void InterstitialOnAdClickedEvent(string adUnitID, MaxSdkBase.AdInfo adInfo)
    {
        // LogAdEvent("inter_ad", "InterstitialAdClickEvent", string.Empty);
        // LogAdEvent("inter_ad", $"InterstitialAdClickEvent_{forceAdsPosition}", string.Empty);
    }

    private void InterstitialOnAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        Debug.Log("Interstitial dismissed - end pause game");

        if (actionFullAds != null)
        {
            actionFullAds.Invoke();
            actionFullAds = null;
            //SetAdsInterval();
        }

        //fix anr
        // Debug.Log("Show AdInter Success:" + adUnitId);
        // Debug.Log("Show AdInter Success AdInfo :" + adInfo);
        Invoke("LoadInterstitial", 0.5f);

        //LogAdEvent("inter_ad", "InterstitialAdClosedEvent", string.Empty);
        //LogAdEvent("inter_ad", $"InterstitialAdClosedEvent_{forceAdsPosition}", string.Empty);
    }

    private void InterstitialOnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
      Debug.LogError("InterstitialOnAdRevenuePaidEvent");
    }

    #endregion

    #region Rewarded Ad Methods

    private void InitializeRewardedAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += RewardedOnAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += RewardedOnAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += RewardedOnAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += RewardedOnAdDisplayFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += RewardedOnAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += RewardedOnAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += RewardedOnAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += RewardedOnAdReceivedRewardEvent;

        // Load the first RewardedAd
        //fix anr
        Invoke("LoadRewardedAd", 0.3f);
    }

    private void LoadRewardedAd()
    {
        if (MaxSdk.IsInitialized())
        {
            MaxSdk.LoadRewardedAd(RewardedAdUnitId);
        }
    }

    public void ShowRewardVideo(Action actionDone, string position = "")
    {
#if UNITY_EDITOR
        Debug.Log($"Show Reward Ads: {position}");
        actionDone?.Invoke();
        //Observer.Instance.Notify(ObserverName.WATCH_ADS);
        DataManager.LastTimeShowAds = DateTime.Now;
        return;
#else
        // rewardAdsPosition = position;
        //AdjustManager.instance.TrackAdEvent(AdjustEventKeys.ad_rewarded_ad_eligible);
        if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
        {
            //AdjustManager.instance.TrackAdEvent(AdjustEventKeys.ad_rewarded_api_called);
            if(actionDone != null) actionRewarded = actionDone;
            Observer.Instance.Notify(ObserverName.WATCH_ADS);
            isAdsShowing = true;
            DataManager.LastTimeShowAds = DateTime.Now;
            MaxSdk.ShowRewardedAd(RewardedAdUnitId);
            AnalyticsManager.instance.LogAdsEvent("show_rewarded_ads", true, true, position);
        }
        else
        {
            ShowMessage("Ads not ready yet!");
            if (CheckInternet())
            {
                //LogAdEvent("reward_ad", "RewardedVideoAdNotReady_InternetReady", string.Empty);
                AnalyticsManager.instance.LogAdsEvent("show_rewarded_ads", false, true, position);
            }
            else
            {
                ShowMessage("No internet connection!");
                AnalyticsManager.instance.LogAdsEvent("show_rewarded_ads", false, false, position);
                //LogAdEvent("reward_ad", "RewardedVideoAdNotReady_InternetNotReady", string.Empty);
            }

            //LogAdEvent("reward_ad", "RewardedVideoAdNotReady", string.Empty);
        }
#endif
    }

    private void RewardedOnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        // Debug.Log("Rewarded ad loaded");

        // Reset retry attempt
        rewardedRetryAttempt = 0;

        //LogAdEvent("reward_ad", "RewardedVideoAdLoadSuccessEvent", string.Empty);
    }

    private void RewardedOnAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        // Debug.Log("Rewarded ad failed to load with error code: " + errorInfo);

        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));
        Invoke("LoadRewardedAd", (float)retryDelay);

        //LogAdEvent("reward_ad", "RewardedVideoAdLoadFailedEvent", string.Empty);
    }

    private void RewardedOnAdDisplayFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        // Debug.Log("Rewarded ad failed to display with error code: " + errorInfo);
        // Debug.Log("Rewarded ad failed to display with adinfo : " + adInfo);

        Invoke("LoadRewardedAd", 0.5f);

        //LogAdEvent("reward_ad", "RewardedVideoAdShowFailedEvent", string.Empty);
    }

    private void RewardedOnAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        //AdjustManager.instance.TrackAdEvent(AdjustEventKeys.ad_rewarded_displayed);
        // Debug.Log("Rewarded ad displayed - pause game show ads reward");
        // Debug.Log("RewardedOnAdDisplayedEvent AdInfo :" + adInfo);
        // Debug.Log("RewardedOnAdDisplayedEvent AdInfo revenue :" + adInfo.Revenue);
        
        AnalyticsManager.instance.TrackAdRevenue(adInfo);

        //fix anr
        Invoke("LoadRewardedAd", 0.5f);
    }

    private void RewardedOnAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad clicked");
    }

    private void RewardedOnAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        // Debug.Log("Rewarded ad dismissed - end pause game");

        Invoke("LoadRewardedAd", 0.5f);

        //LogAdEvent("reward_ad", "RewardedVideoAdDismissedEvent", string.Empty);
    }

    private void RewardedOnAdReceivedRewardEvent(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad was displayed and user should receive the reward
        // Debug.Log("Rewarded ad received reward - end pause game");

        if (actionRewarded != null)
        {
            StartCoroutine(WaitRewarded());
        }

        //SetAdsInterval();
  
        // LogAdEvent("reward_ad", "RewardedVideoAdRewardedEvent", string.Empty);
        // LogAdEvent("reward_ad", $"RewardedVideoAdRewardedEvent_{rewardAdsPosition}", string.Empty);

    }

    private void RewardedOnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        AdjustController.Instance.sendPaidToFirebase(adUnitId, adInfo);
        AdjustController.Instance.sendRevAdjust(adUnitId, adInfo);
    }

    IEnumerator WaitRewarded()
    {
        yield return waitRewarded;
        actionRewarded.Invoke();
    }

    #endregion

    #region Banner Ad Methods

    private void InitializeBannerAds()
    {
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += BannerOnAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += BannerOnAdLoadFailedEvent;

        Invoke("DelayInitBanner", 0.5f);
    }

    private void DelayInitBanner()
    {
        // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
        // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
        MaxSdk.CreateBanner(BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerExtraParameter(BannerAdUnitId, "adaptive_banner", "false");
        // Set background or background color for banners to be fully functional.
        MaxSdk.SetBannerBackgroundColor(BannerAdUnitId, Color.black);
        MaxSdk.SetBannerWidth(BannerAdUnitId, (float)Screen.width);
        //ShowBanner();
    }

    public void ShowBanner()
    {
        // #if !UNITY_EDITOR
        if (!IsNoAds())
        {
            MaxSdk.ShowBanner(BannerAdUnitId);
        }
        // #endif
    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(BannerAdUnitId);
    }

    private void ToggleBannerVisibility()
    {
        if (!isBannerShowing)
        {
            MaxSdk.ShowBanner(BannerAdUnitId);
        }
        else
        {
            MaxSdk.HideBanner(BannerAdUnitId);
        }

        isBannerShowing = !isBannerShowing;
    }

    // Fired when a banner is loaded
    private void BannerOnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        bannerRetryAttempt = 0;
        Debug.Log("Banner Loaded:" + adUnitId);
        //LogAdEvent("banner_ad", "banner_loaded", string.Empty);
    }

    // Fired when a banner has failed to load
    private void BannerOnAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        bannerRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, bannerRetryAttempt));

        // Debug.Log("Banner failed to load with error code: " + errorInfo);
        // Debug.Log("Load Banner Fail:" + adUnitId);

        Invoke("DelayInitBanner", (float)retryDelay);
        //LogAdEvent("banner_ad", "banner_load_fail", errorInfo.Message);
    }

    #endregion

    public void ShowMessage(string msg)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject androidJavaObject = new AndroidJavaClass("android.widget.Toast");
        androidJavaObject.CallStatic<AndroidJavaObject>("makeText", new object[]
        {
                @static,
                msg,
                androidJavaObject.GetStatic<int>("LENGTH_SHORT")
        }).Call("show", Array.Empty<object>());
#endif
    }

    public bool CheckInternet()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public bool IsNoAds()
    {
        return PlayerPrefs.GetInt(StringHelper.REMOVE_ADS, 0) == 1;
    }
}
