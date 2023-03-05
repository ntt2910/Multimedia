using LTABase.DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockSkinSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fishName;
    [SerializeField] Image skinIcon;
    [SerializeField] Button btnUnlock;

    FishType fishType;
    private void Start()
    {
        btnUnlock.onClick.AddListener(OnClickUnlock);
        if (RemoteConfigControl.instance.UnLockSkinInHomeIsOn >= 1 && DataManager.LastTimeShowAdsAddPiece.AddSeconds(RemoteConfigControl.instance.UnLockSkinInHomeCooldown) < DateTime.Now)
        {
            LoadData();
        }
        else
        {
            gameObject.gameObject.SetActive(false);
        }
    }
    public void LoadData()
    {
        //Debug.Log(GameController.instance.unlockData.fishCanUnlocked.Count);
        if (GameController.instance.unlockData.fishCanUnlocked.Count <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
        var fishUnlockData = GameController.instance.unlockData.fishCanUnlocked[0];

        if (fishUnlockData != null)
        {
            fishName.text = fishUnlockData.fishType.ToString();
            skinIcon.sprite = fishUnlockData.icon;
            fishType = fishUnlockData.fishType;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void OnClickUnlock()
    {
        Debug.Log("Show Video Reward");
        Action actionSuccess = () =>
        {
            var fishUnlock = GameController.instance.fishDatabase.GetFishOObjectData(fishType);
            if (fishUnlock != null)
            {
                DataManager.LastTimeShowAdsUnLockSkinOnHome = DateTime.Now;
                GameController.instance.unlockData.fishUnlocked.Add(fishUnlock);
                GameController.instance.unlockData.RemoveFishDataCanUnlock(fishType);
                DataManager.StringItemUnLock();
                if (RemoteConfigControl.instance.UnLockSkinInHomeIsOn >= 1 && DataManager.LastTimeShowAdsUnLockSkinOnHome.AddSeconds(RemoteConfigControl.instance.UnLockSkinInHomeCooldown) < DateTime.Now)
                {
                    LoadData();
                }
                else
                {
                    gameObject.gameObject.SetActive(false);
                }    
                    
                DataManager.currentSkinFish = fishType;
                Observer.Instance.Notify(ObserverName.ON_CLICK_UNLOCK_FISH);
            }
        };
        if (RemoteConfigControl.instance.UnLockSkinInHomeIsOn >= 1 && DataManager.LastTimeShowAdsUnLockSkinOnHome.AddSeconds(RemoteConfigControl.instance.UnLockSkinInHomeCooldown) < DateTime.Now)
        {
            AdsManager.instance.ShowRewardVideo(actionSuccess, "Main menu Screen");
        }    
    }
}
