using LTABase.DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopFishSlot : ShopSlot
{
    public FishType fishType;

    public override void OnPointerDown(PointerEventData eventData)
    {
        Observer.Instance.Notify<ShopFishSlot>(ObserverName.ON_CLICK_SHOP_FISH_SLOT, this);
    }
    public override void OnClickSelect()
    {
        base.OnClickSelect();
        DataManager.currentSkinFish = fishType;
        SetSelected();
        Observer.Instance.Notify<ShopFishSlot>(ObserverName.ON_CLICK_SELECT_FISH, this);
    }
    public override void OnClickUnLock()
    {
        base.OnClickUnLock();
        GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.UnlockElement);
        Debug.Log("Show Video Reward");
        Action actionSuccess = () =>
        {
            var fishUnlock = GameController.instance.fishDatabase.GetFishOObjectData(fishType);
            if (fishUnlock != null)
            {
                GameController.instance.unlockData.fishUnlocked.Add(fishUnlock);
                GameController.instance.unlockData.RemoveFishDataCanUnlock(fishType);
                DataManager.StringItemUnLock();
                SetSelect();
            }
        };
        AdsManager.instance.ShowRewardVideo(actionSuccess, "Shop");
        
    }
}
