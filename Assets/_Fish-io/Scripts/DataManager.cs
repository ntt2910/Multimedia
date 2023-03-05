using System;
using UnityEngine;

public static class DataManager
{
    public static PlayerUnlockData GetUnlockData()
    {
        PlayerUnlockData unlockData = new PlayerUnlockData();
        var cached = PlayerPrefs.GetString(StringHelper.PLAYER_UNLOCK_DATA);
        if (string.IsNullOrEmpty(cached))
        {
            // for (int i = 0; i < 31; i++)
            // {
            //     unlockData.fishUnlocked.Add(GameController.instance.fishDatabase.GetFishOObjectData(i));
            // }
            unlockData.fishUnlocked.Add(GameController.instance.fishDatabase.GetFishOObjectData(FishType.Huggy));
            // for (int j = 0; j < 11; j++)
            // {
            //     unlockData.weaponUnlocked.Add(GameController.instance.weaponDatabase.GetWeaponObjectData(j));
            // }
            unlockData.weaponUnlocked.Add(GameController.instance.weaponDatabase.GetWeaponObjectData(WeaponType.LegendKatana));
            return unlockData;
        }
        try
        {
            unlockData = JsonUtility.FromJson<PlayerUnlockData>(cached);
        }
        catch
        {
            Debug.Log("<color=red>JSON DATA_MISSIONS not correct</color>");
            PlayerPrefs.DeleteKey(StringHelper.PLAYER_UNLOCK_DATA);
        }
        return unlockData;
    }
    public static void StringItemUnLock()
    {
        var str = JsonUtility.ToJson(GameController.instance.unlockData);
        PlayerPrefs.SetString(StringHelper.PLAYER_UNLOCK_DATA, str);
        PlayerPrefs.Save();
    }
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }
    public static string Dataversion
    {
        get
        {
            return PlayerPrefs.GetString(StringHelper.DATAVERSION, String.Empty);
        }

        set
        {
            PlayerPrefs.SetString(StringHelper.DATAVERSION, value);
            PlayerPrefs.Save();
        }
    }
    public static int LevelPlayer
    {
        get
        {
            return PlayerPrefs.GetInt($"{StringHelper.LEVEL_PLAYER}", 1);
        }
        set
        {
            PlayerPrefs.SetInt($"{StringHelper.LEVEL_PLAYER}", value);
            PlayerPrefs.Save();
            
        }
    }
    
    public static int TimePlay
    {
        get
        {
            return PlayerPrefs.GetInt($"{StringHelper.TIME_PLAY}", 0);
        }
        set
        {
            PlayerPrefs.SetInt($"{StringHelper.TIME_PLAY}", value);
            PlayerPrefs.Save();
        }
    }
    
    public static int CountUnlockBoxShow
    {
        get
        {
            return PlayerPrefs.GetInt($"{StringHelper.COUNT_UNLOCK_BOX_SHOW}", 0);
        }
        set
        {
            PlayerPrefs.SetInt($"{StringHelper.COUNT_UNLOCK_BOX_SHOW}", value);
            PlayerPrefs.Save();
        }
    }
    
    public static string playerName
    {
        get
        {
            return PlayerPrefs.GetString($"{StringHelper.PLAYER_NAME}", "Player");
        }
        set
        {
            PlayerPrefs.SetString($"{StringHelper.PLAYER_NAME}", value);
            PlayerPrefs.Save();

        }
    }
    public static WeaponType currentWeapon
    {
        get
        {
            string str = PlayerPrefs.GetString($"{StringHelper.CURRENT_WEAPON}", "");

            foreach (var weaponType in (WeaponType[])Enum.GetValues(typeof(WeaponType)))
            {
                if (str == weaponType.ToString()) return weaponType;
            }
            return WeaponType.LegendKatana;
        }
        set
        {
            PlayerPrefs.SetString($"{StringHelper.CURRENT_WEAPON}", value.ToString());
            PlayerPrefs.Save();
        }
    }
    public static FishType currentSkinFish
    {
        get
        {
            string str = PlayerPrefs.GetString($"{StringHelper.CURRENT_SKIN}", "");

            foreach (var fish in (FishType[])Enum.GetValues(typeof(FishType)))
            {
                if (str == fish.ToString()) return fish;
            }
            return FishType.Huggy;
        }
        set
        {
            PlayerPrefs.SetString($"{StringHelper.CURRENT_SKIN}", value.ToString());
            PlayerPrefs.Save();

        }
    }
    public static DateTime LastTimeCheckInDailyLogin
    {
        get
        {
            if (PlayerPrefs.HasKey(StringHelper.LAST_TIME_DAILY_LOGIN))
            {
                var temp = Convert.ToInt64(PlayerPrefs.GetString(StringHelper.LAST_TIME_DAILY_LOGIN));
                return DateTime.FromBinary(temp);
            }
            else
            {
                var newDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                PlayerPrefs.SetString(StringHelper.LAST_TIME_DAILY_LOGIN, newDateTime.ToBinary().ToString());
                PlayerPrefs.Save();
                return newDateTime;
            }
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.LAST_TIME_DAILY_LOGIN, value.ToBinary().ToString());
            PlayerPrefs.Save();
        }
    }
    public static DateTime LastTimeShowInterAds
    {
        get
        {
            if (PlayerPrefs.HasKey(StringHelper.LAST_TIME_SHOW_INTER_ADS))
            {
                var temp = Convert.ToInt64(PlayerPrefs.GetString(StringHelper.LAST_TIME_SHOW_INTER_ADS));
                return DateTime.FromBinary(temp);
            }
            else
            {
                var newDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                PlayerPrefs.SetString(StringHelper.LAST_TIME_SHOW_INTER_ADS, newDateTime.ToBinary().ToString());
                PlayerPrefs.Save();
                return newDateTime;
            }
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.LAST_TIME_SHOW_INTER_ADS, value.ToBinary().ToString());
            PlayerPrefs.Save();
        }
    }
    public static DateTime LastTimeShowAds
    {
        get
        {
            if (PlayerPrefs.HasKey(StringHelper.LAST_TIME_SHOW_ADS))
            {
                var temp = Convert.ToInt64(PlayerPrefs.GetString(StringHelper.LAST_TIME_SHOW_ADS));
                return DateTime.FromBinary(temp);
            }
            else
            {
                var newDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                PlayerPrefs.SetString(StringHelper.LAST_TIME_SHOW_ADS, newDateTime.ToBinary().ToString());
                PlayerPrefs.Save();
                return newDateTime;
            }
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.LAST_TIME_SHOW_ADS, value.ToBinary().ToString());
            PlayerPrefs.Save();
        }
    }
    public static int CurrentRewardDailyLogin
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.CURRENT_ID_DAILY_LOGIN, -1);
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.CURRENT_ID_DAILY_LOGIN, value);
            PlayerPrefs.Save();
            
        }
    }

    public static int Gold
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.GOLD, 0);
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.GOLD, Mathf.Clamp(value, 0, int.MaxValue));
            PlayerPrefs.Save();
        }
    }

    public static bool tutorial
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.TUTORIAL, 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.TUTORIAL, value? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static int unlockIndex
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.INDEX_UNLOCK, 1);
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.INDEX_UNLOCK, Mathf.Clamp(value, 0, int.MaxValue));
            PlayerPrefs.Save();
        }
    }

    public static int unlockProgressAmount
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.UNLOCK_PROGRESS_AMOUNT, 0);
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.UNLOCK_PROGRESS_AMOUNT, Mathf.Clamp(value, 0, int.MaxValue));
            PlayerPrefs.Save();
        }
    }
    public static int currentPieceReward
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.CURRENT_PIECE_REWARD, 1);
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.CURRENT_PIECE_REWARD, Mathf.Clamp(value, 0, int.MaxValue));
            PlayerPrefs.Save();
        }

    }
    public static DateTime LastTimeShowAdsAddPiece
    {
        get
        {
            if (PlayerPrefs.HasKey(StringHelper.LAST_TIME_SHOW_ADS_ADD_PIECE))
            {
                var temp = Convert.ToInt64(PlayerPrefs.GetString(StringHelper.LAST_TIME_SHOW_ADS_ADD_PIECE));
                return DateTime.FromBinary(temp);
            }
            else
            {
                var newDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                PlayerPrefs.SetString(StringHelper.LAST_TIME_SHOW_ADS_ADD_PIECE, newDateTime.ToBinary().ToString());
                PlayerPrefs.Save();
                return newDateTime;
            }
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.LAST_TIME_SHOW_ADS_ADD_PIECE, value.ToBinary().ToString());
            PlayerPrefs.Save();
        }
    }
    public static DateTime LastTimeShowAdsUnLockSkinOnHome
    {
        get
        {
            if (PlayerPrefs.HasKey(StringHelper.LAST_TIME_SHOW_ADS_UNLOCK_SKIN_ON_HOME))
            {
                var temp = Convert.ToInt64(PlayerPrefs.GetString(StringHelper.LAST_TIME_SHOW_ADS_UNLOCK_SKIN_ON_HOME));
                return DateTime.FromBinary(temp);
            }
            else
            {
                var newDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                PlayerPrefs.SetString(StringHelper.LAST_TIME_SHOW_ADS_UNLOCK_SKIN_ON_HOME, newDateTime.ToBinary().ToString());
                PlayerPrefs.Save();
                return newDateTime;
            }
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.LAST_TIME_SHOW_ADS_UNLOCK_SKIN_ON_HOME, value.ToBinary().ToString());
            PlayerPrefs.Save();
        }
    }
    public static DateTime LastTimeShowAdsRevive
    {
        get
        {
            if (PlayerPrefs.HasKey(StringHelper.LAST_TIME_SHOW_ADS_REVIVE))
            {
                var temp = Convert.ToInt64(PlayerPrefs.GetString(StringHelper.LAST_TIME_SHOW_ADS_REVIVE));
                return DateTime.FromBinary(temp);
            }
            else
            {
                var newDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                PlayerPrefs.SetString(StringHelper.LAST_TIME_SHOW_ADS_REVIVE, newDateTime.ToBinary().ToString());
                PlayerPrefs.Save();
                return newDateTime;
            }
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.LAST_TIME_SHOW_ADS_REVIVE, value.ToBinary().ToString());
            PlayerPrefs.Save();
        }
    }
    #region === Remove Ads ===
    public static int RemoveAds
    {
        get => PlayerPrefs.GetInt(StringHelper.REMOVE_ADS, 0);
        set
        {
            PlayerPrefs.SetInt(StringHelper.REMOVE_ADS, value);
            PlayerPrefs.Save();
        }
    }

    public static bool IsCanShowAds()
    {
        return PlayerPrefs.GetInt(StringHelper.REMOVE_ADS, 0) == 0;
    }
    #endregion
}
