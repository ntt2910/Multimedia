using System;
using System.Collections;
using System.Collections.Generic;
using com.adjust.sdk;
using com.adjust.sdk.purchase;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdjustEventKeys
{
    public static string complete_level_1 = "";
    public static string complete_level_2 = "";
    public static string complete_level_3 = "";
    public static string complete_level_4 = "";
    public static string complete_level_5 = "";
    public static string ad_inters_ad_eligible = "ashbh2";
    public static string ad_inters_api_called = "yq2o3y";
    public static string ad_inters_displayed = "ogffcu";
    public static string ad_rewarded_ad_eligible = "1gykw5";
    public static string ad_rewarded_api_called = "byknlg";
    public static string ad_rewarded_displayed = "uys0mo";
    
    public const string purchase = "m96foh";
    public const string purchase_failed = "lbmdzl";
    public const string purchase_notverified = "1r60yr";
    public const string purchase_unknown = "dhyz79";
}

public class AdjustManager : MonoBehaviour
{
    public static AdjustManager instance;
    
    static string currency, TransactionId, productID;
    static double price;
    public static void validateAndSendInAppPurchase_Android(string ItemSKU, string ItemToken, string DeveloperPayload, double price, string currency, string TransactionID, string ProductId)
    {
#if !UNITY_ANDROID
     return;
#endif

        AdjustManager.price = price;
        AdjustManager.currency = currency;
        AdjustManager.TransactionId = TransactionID;
        AdjustManager.productID = productID;
#if DEVELOPMENT
		Debug.Log("Adjust validate " + ItemSKU + " " + ItemToken + " " + DeveloperPayload);
		LogPurchaseTest();
#endif

        AdjustPurchase.VerifyPurchaseAndroid(ItemSKU, ItemToken, DeveloperPayload, VerificationInfoDelegate);
    }
    
    private static void VerificationInfoDelegate(ADJPVerificationInfo verificationInfo)
    {
        if (verificationInfo.VerificationState == ADJPVerificationState.ADJPVerificationStatePassed)
        {

            if (AdjustManager.price != 0)
            {
                AdjustEvent adjustEvent = new AdjustEvent(AdjustEventKeys.purchase);
                adjustEvent.setRevenue(price, AdjustManager.currency);
                adjustEvent.setTransactionId(AdjustManager.TransactionId);
                Adjust.trackEvent(adjustEvent);

                try
                {
                    //	int level = GameData.Instance.MissionsGroup.CurrentMissionId;
                    //	Config.LogEvent(TrackingConstants.IAP_EVENT, "pack", AdjustManager.productID, "level", level);
                }
                catch (Exception)
                {

                    //throw;
                }

            }
        }
        else if (verificationInfo.VerificationState == ADJPVerificationState.ADJPVerificationStateFailed)
        {
            AdjustEvent adjustEvent = new AdjustEvent(AdjustEventKeys.purchase_failed);
            Adjust.trackEvent(adjustEvent);
        }
        else if (verificationInfo.VerificationState == ADJPVerificationState.ADJPVerificationStateUnknown)
        {
            AdjustEvent adjustEvent = new AdjustEvent(AdjustEventKeys.purchase_unknown);
            Adjust.trackEvent(adjustEvent);
        }
        else
        {
            AdjustEvent adjustEvent = new AdjustEvent(AdjustEventKeys.purchase_notverified);
            Adjust.trackEvent(adjustEvent);
        }
    }

    void Start()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(instance);
    }
    public void CheckPointComplete(string token)
    {
        try
        {
            AdjustEvent adjustEvent = new AdjustEvent(token);
            Adjust.trackEvent(adjustEvent);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void TrackAdEvent(string token)
    {
        try
        {
            AdjustEvent adjustEvent = new AdjustEvent(token);
            Adjust.trackEvent(adjustEvent);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public string GetToken(int level)
    {
        switch (level)
        {
            case 1: return AdjustEventKeys.complete_level_1;
            case 2: return AdjustEventKeys.complete_level_2;
            case 3: return AdjustEventKeys.complete_level_3;
            case 4: return AdjustEventKeys.complete_level_4;
            case 5: return AdjustEventKeys.complete_level_5;
            default: return "";
        }
    }
    
    public void sendRevAdjust(string nameAD , AdValueEventArgs args)
    {
        AdValue adValue = args.AdValue;
        AdjustAdRevenue adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAdMob);
        adRevenue.setRevenue(adValue.Value / 1000000f, adValue.CurrencyCode);
        Debug.LogError("=> Adjust Value : " + (adValue.Value));
        Debug.LogError("=> Adjust 1000000 : " + (adValue.Value / 1000000f));
        Adjust.trackAdRevenue(adRevenue);
    }
}
