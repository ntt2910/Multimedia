using LTABase.DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopWeaponSlot : ShopSlot
{
    public WeaponType weaponType;

    public override void OnPointerDown(PointerEventData eventData)
    {
        Observer.Instance.Notify<ShopWeaponSlot>(ObserverName.ON_CLICK_SHOP_WEAPON_SLOT, this);
    }
    public override void OnClickSelect()
    {
        base.OnClickSelect();
        DataManager.currentWeapon = weaponType;
        SetSelected();
        Observer.Instance.Notify<ShopWeaponSlot>(ObserverName.ON_CLICK_SELECT_WEAPON, this);
    }
    public override void OnClickUnLock()
    {
        base.OnClickUnLock();
        GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.UnlockElement);
        Debug.Log("Show Video Reward");

        Action actionSuccess = () =>
        {
            var weaponUnlock = GameController.instance.weaponDatabase.GetWeaponObjectData(weaponType);
            if (weaponUnlock != null)
            {
                GameController.instance.unlockData.weaponUnlocked.Add(weaponUnlock);
                GameController.instance.unlockData.RemoveWeaponDataCanUnlock(weaponType);
                DataManager.StringItemUnLock();
                SetSelect();
            }
        };
        AdsManager.instance.ShowRewardVideo(actionSuccess, "Shop");
        
    }
}
