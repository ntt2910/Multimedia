using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Firebase.Extensions;
using System;
using System.Threading.Tasks;

public class RemoteConfigControl : MonoBehaviour
{
    public static RemoteConfigControl instance;
    public Action OnFetchDone;
    public bool isDataFetched = false;

    //public string StageOrder = "1,2,3,4,5,6,7,8,9,10";
    public int CausingtInterstitialOn = 30;
    public int AdTimer = 60;
    //public int Fuel_economy = 1;
    //public int UprgadeIngame = 0;
    //public int PowerGlobalConfig = 0;

    //*Win Screen Lv4-15*//
    public int WinScreen_1_3_Active_level_x = 1;
    public int WinScreen_1_3_to_Level = 5;
    public int WinScreen_1_3_Per = 1;
    public int WinScreen_1_3_Cooldown = 60;
    public int WinScreen_1_3_IsOn = 0;

    //*Win Screen Lv16-30*//
    public int WinScreen_4_12_Active_level_x = 6;
    public int WinScreen_4_12_to_Level = 20;
    public int WinScreen_4_12_Per = 1;
    public int WinScreen_4_12_Cooldown = 60;
    public int WinScreen_4_12_IsOn = 1;

    //*Win Screen Lv31*//
    public int WinScreen_13_20_Active_level_x = 21;
    public int WinScreen_13_20_to_Level = 30;
    public int WinScreen_13_20_Per = 1;
    public int WinScreen_13_20_Cooldown = 45;
    public int WinScreen_13_20_IsOn = 1;

    public int WinScreen_21_Active_level_x = 31;
    public int WinScreen_21_Per = 1;
    public int WinScreen_21_Cooldown = 35;
    public int WinScreen_21_IsOn = 1;

    public int Revive_Ads_Cooldown = 0;
    public int Revive_Ads_Is_On = 1;

    public int UnLockSkinInHomeIsOn = 1;
    public int UnLockSkinInHomeCooldown = 0;

    public int AddPieceIsOn = 1;
    public int AddPiece_Cooldown = 0;

    protected bool isFirebaseInitialized = false;
    //Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void InitializeFirebase()
    {
        LoadData();
        Dictionary<string, object> defaults =
          new Dictionary<string, object>
          {
              //{ "StageOrder", StageOrder },
              { "CausingtInterstitialOn", CausingtInterstitialOn },
              { "AdTimer", AdTimer },

              { "WinScreen_1_3_Active_level_x", WinScreen_1_3_Active_level_x },
              { "WinScreen_1_3_to_Level", WinScreen_1_3_to_Level },
              { "WinScreen_1_3_Per", WinScreen_1_3_Per },
              { "WinScreen_1_3_Cooldown", WinScreen_1_3_Cooldown },
              { "WinScreen_1_3_IsOn", WinScreen_1_3_IsOn },

              { "WinScreen_4_12_Active_level_x", WinScreen_4_12_Active_level_x },
              { "WinScreen_4_12_to_Level", WinScreen_4_12_to_Level },
              { "WinScreen_4_12_Per", WinScreen_4_12_Per },
              { "WinScreen_4_12_Cooldown", WinScreen_4_12_Cooldown },
              { "WinScreen_4_12_IsOn", WinScreen_4_12_IsOn },

              { "WinScreen_13_20_Active_level_x", WinScreen_13_20_Active_level_x},
              { "WinScreen_4_12_to_Level", WinScreen_13_20_to_Level},
              { "WinScreen_13_20_Per", WinScreen_13_20_Per },
              { "WinScreen_13_20_Cooldown", WinScreen_13_20_Cooldown },
              { "WinScreen_13_20_IsOn", WinScreen_13_20_IsOn },

              { "WinScreen_21_Active_level_x", WinScreen_21_Active_level_x},
              { "WinScreen_21_Per", WinScreen_21_Per },
              { "WinScreen_21_Cooldown", WinScreen_21_Cooldown },
              { "WinScreen_21_IsOn", WinScreen_21_IsOn },

              { "Revive_Ads_Cooldown", Revive_Ads_Cooldown },
              { "Revive_Ads_Is_On", Revive_Ads_Is_On },

              { "UnLockSkinInHomeIsOn", UnLockSkinInHomeIsOn },
              { "UnLockSkinInHomeCooldown", UnLockSkinInHomeCooldown },

              { "AddPieceIsOn", AddPieceIsOn },
              { "AddPiece_Cooldown", AddPiece_Cooldown },
          };

        //Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
        
        Debug.Log("RemoteConfig configured and ready!");

        isFirebaseInitialized = true;
        FetchDataAsync();
    }

    public void FetchDataAsync()
    {
        Debug.Log("Fetching data...");
        // System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
        //     TimeSpan.Zero);
        // fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
            Debug.Log("Fetch canceled.");
        }
        else if (fetchTask.IsFaulted)
        {
            Debug.Log("Fetch encountered an error.");
        }
        else if (fetchTask.IsCompleted)
        {
            Debug.Log("Fetch completed successfully!");
        }

        // var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        // switch (info.LastFetchStatus)
        // {
        //     case Firebase.RemoteConfig.LastFetchStatus.Success:
        //         Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
        //         Debug.Log($"Remote data loaded and ready (last fetch time {info.FetchTime}).");
        //         RefrectProperties();
        //         break;
        //     case Firebase.RemoteConfig.LastFetchStatus.Failure:
        //         switch (info.LastFetchFailureReason)
        //         {
        //             case Firebase.RemoteConfig.FetchFailureReason.Error:
        //                 Debug.Log("Fetch failed for unknown reason");
        //                 break;
        //             case Firebase.RemoteConfig.FetchFailureReason.Throttled:
        //                 Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
        //                 break;
        //         }
        //         break;
        //     case Firebase.RemoteConfig.LastFetchStatus.Pending:
        //         Debug.Log("Latest Fetch call still pending.");
        //         break;
        // }

        OnFetchDone?.Invoke();
    }

    private void RefrectProperties()
    {
        //StageOrder = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("StageOrder").StringValue;
        //CausingtInterstitialOn = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("CausingtInterstitialOn").DoubleValue;
        //AdTimer = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("AdTimer").DoubleValue;
        //Fuel_economy = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("Fuel_economy").DoubleValue;
        //UprgadeIngame = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("UprgadeIngame").DoubleValue;
        //PowerGlobalConfig = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("PowerGlobalConfig").DoubleValue;
        // //Nhớ remote config là string thì gọi StringValue
        // GameManager.Instance.LoadRemoteConfigSuccess = true;
        SaveData();
        //var lsStage = StageOrder.Trim().Split(',');
        //for (int i = 0; i < GameController.instance.stageDatabase.stageDatas.Count; i++)
        //{
        //    var stageData = GameController.instance.stageDatabase.stageDatas[i];
        //    stageData.id = int.Parse(lsStage[i]);
        //}
        isDataFetched = true;
    }

    // save cache data
    private void SaveData()
    {
        PlayerPrefs.SetInt("CausingtInterstitialOn", CausingtInterstitialOn);

        PlayerPrefs.SetInt("WinScreen_1_Active_level_x", WinScreen_1_3_Active_level_x);
        PlayerPrefs.SetInt("WinScreen_1_to_Level", WinScreen_1_3_to_Level);
        PlayerPrefs.SetInt("WinScreen_1_Per", WinScreen_1_3_Per);
        PlayerPrefs.SetInt("WinScreen_1_Cooldown", WinScreen_1_3_Cooldown);
        PlayerPrefs.SetInt("WinScreen_1_IsOn", WinScreen_1_3_IsOn);

        PlayerPrefs.SetInt("WinScreen_2_Active_level_x", WinScreen_4_12_Active_level_x);
        PlayerPrefs.SetInt("WinScreen_2_to_Level", WinScreen_4_12_to_Level);
        PlayerPrefs.SetInt("WinScreen_2_Per", WinScreen_4_12_Per);
        PlayerPrefs.SetInt("WinScreen_2_Cooldown", WinScreen_4_12_Cooldown);
        PlayerPrefs.SetInt("WinScreen_2_IsOn", WinScreen_4_12_IsOn);

        PlayerPrefs.SetInt("WinScreen_13_20_Active_level_x", WinScreen_13_20_Active_level_x);
        PlayerPrefs.SetInt("WinScreen_13_20_to_Level", WinScreen_13_20_to_Level);
        PlayerPrefs.SetInt("WinScreen_13_20_Per", WinScreen_13_20_Per);
        PlayerPrefs.SetInt("WinScreen_13_20_Cooldown", WinScreen_13_20_Cooldown);
        PlayerPrefs.SetInt("WinScreen_13_20_IsOn", WinScreen_13_20_IsOn);

        PlayerPrefs.SetInt("WinScreen_21_Active_level_x", WinScreen_21_Active_level_x);
        PlayerPrefs.SetInt("WinScreen_21_Per", WinScreen_21_Per);
        PlayerPrefs.SetInt("WinScreen_21_Cooldown", WinScreen_21_Cooldown);
        PlayerPrefs.SetInt("WinScreen_21_IsOn", WinScreen_21_IsOn);

        PlayerPrefs.SetInt("Revive_Ads_Cooldown", Revive_Ads_Cooldown);
        PlayerPrefs.SetInt("Revive_Ads_Is_On", Revive_Ads_Is_On);

        PlayerPrefs.SetInt("UnLockSkinInHomeIsOn", UnLockSkinInHomeIsOn);
        PlayerPrefs.SetInt("UnLockSkinInHomeCooldown", UnLockSkinInHomeCooldown);

        PlayerPrefs.SetInt("AddPieceIsOn", AddPieceIsOn);
        PlayerPrefs.SetInt("AddPiece_Cooldown", AddPiece_Cooldown);
    }

    //load cache data
    private void LoadData()
    {
        //StageOrder = PlayerPrefs.GetString("StageOrder", "1,2,3,4,5,6,7,8,9,10");
        CausingtInterstitialOn = PlayerPrefs.GetInt("CausingtInterstitialOn", 60);
        AdTimer = PlayerPrefs.GetInt("AdTimer", 60);

        WinScreen_1_3_Active_level_x = PlayerPrefs.GetInt("WinScreen_1_3_Active_level_x", 1);
        WinScreen_1_3_to_Level = PlayerPrefs.GetInt("WinScreen_1_3_to_Level", 5);
        WinScreen_1_3_Per = PlayerPrefs.GetInt("WinScreen_1_3_Per", 1);
        WinScreen_1_3_Cooldown = PlayerPrefs.GetInt("WinScreen_1_3_Cooldown", 60);
        WinScreen_1_3_IsOn = PlayerPrefs.GetInt("WinScreen_1_3_IsOn", 0);

        //*Win Screen Lv16-30*//
        WinScreen_4_12_Active_level_x = PlayerPrefs.GetInt("WinScreen_4_12_Active_level_x", 6);
        WinScreen_4_12_to_Level = PlayerPrefs.GetInt("WinScreen_4_12_to_Level", 20);
        WinScreen_4_12_Per = PlayerPrefs.GetInt("WinScreen_4_12_Per", 1);
        WinScreen_4_12_Cooldown = PlayerPrefs.GetInt("WinScreen_4_12_Cooldown", 60);
        WinScreen_4_12_IsOn = PlayerPrefs.GetInt("WinScreen_4_12_IsOn", 1);

        //*Win Screen Lv31*//
        WinScreen_13_20_Active_level_x = PlayerPrefs.GetInt("WinScreen_13_20_Active_level_x", 21);
        WinScreen_13_20_to_Level = PlayerPrefs.GetInt("WinScreen_13_20_to_Level", 30);
        WinScreen_13_20_Per = PlayerPrefs.GetInt("WinScreen_13_20_Per", 1);
        WinScreen_13_20_Cooldown = PlayerPrefs.GetInt("WinScreen_13_20_Cooldown", 45);
        WinScreen_13_20_IsOn = PlayerPrefs.GetInt("WinScreen_13_20_IsOn", 1);

        WinScreen_21_Active_level_x = PlayerPrefs.GetInt("WinScreen_21_Active_level_x", 31);
        WinScreen_21_Per = PlayerPrefs.GetInt("WinScreen_21_Per", 1);
        WinScreen_21_Cooldown = PlayerPrefs.GetInt("WinScreen_21_Cooldown", 35);
        WinScreen_21_IsOn = PlayerPrefs.GetInt("WinScreen_21_IsOn", 1);

        Revive_Ads_Cooldown = PlayerPrefs.GetInt("Revive_Ads_Cooldown", 0);
        Revive_Ads_Is_On = PlayerPrefs.GetInt("Revive_Ads_Is_On", 1);

        UnLockSkinInHomeIsOn = PlayerPrefs.GetInt("UnLockSkinInHomeIsOn", 1);
        UnLockSkinInHomeCooldown = PlayerPrefs.GetInt("UnLockSkinInHomeCooldown", 0);

        AddPieceIsOn = PlayerPrefs.GetInt("AddPieceIsOn", 1);
        AddPiece_Cooldown = PlayerPrefs.GetInt("AddPiece_Cooldown", 0);
    }

}
