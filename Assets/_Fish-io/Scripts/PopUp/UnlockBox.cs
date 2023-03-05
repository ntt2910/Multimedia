using System;
using System.Collections;
using System.Collections.Generic;
using LTABase.DesignPattern;
using UnityEngine;
using UnityEngine.UI;

public class UnlockBox : BaseBox
{
    private static GameObject instance;
    [SerializeField] private Image item;
    [SerializeField] private Text name;
    [SerializeField] private Text des;
    [SerializeField] private Button unlockBtn;
    [SerializeField] private Button noTksBtn;
    private bool _isShow;
    protected override void Awake()
    {
        unlockBtn.onClick.AddListener(OnClickUnlockBtn);
        noTksBtn.onClick.AddListener(OnClickNoTksBtn);
    }

    public static UnlockBox Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load(PathPrefabs.UNLOCK_BOX) as GameObject);
        }
        instance.SetActive(true);

        return instance.GetComponent<UnlockBox>();
    }
    
    public override void Show()
    {
        if (_isShow == false)
        {
            base.Show();
            _isShow = true;
            var dataUnlock = GameController.instance.unlockDatabase.GetUnlockDataObject(DataManager.unlockIndex);
            var fish = GameController.instance.fishDatabase.GetFishOObjectData(dataUnlock.fishType);
            var weapon = GameController.instance.weaponDatabase.GetWeaponObjectData(dataUnlock.weaponType);
            if (dataUnlock.isFish)
            {
                item.sprite = fish.icon;
                name.text = fish.fishType.ToString();
                des.text = fish.des;
            }
            else
            {
                item.sprite = weapon.icon;
                name.text = weapon.weaponType.ToString();
            }
        }
        
    }

    private void SecondTimeShow()
    {
        if (_isShow == false)
        {
            base.Show();
            _isShow = true;
            var dataUnlock = GameController.instance.unlockDatabase.GetUnlockDataObject(DataManager.unlockIndex -1);
            var fish = GameController.instance.fishDatabase.GetFishOObjectData(dataUnlock.fishType);
            var weapon = GameController.instance.weaponDatabase.GetWeaponObjectData(dataUnlock.weaponType);
            if (dataUnlock.isFish)
            {
                item.sprite = fish.icon;
                name.text = fish.fishType.ToString();
                des.text = fish.des;
            }
            else
            {
                item.sprite = weapon.icon;
                name.text = weapon.weaponType.ToString();
            }
        }
    }

    private void OnClickNoTksBtn()
    {
        var dataUnlock = GameController.instance.unlockDatabase.GetUnlockDataObject(DataManager.unlockIndex - DataManager.CountUnlockBoxShow);
        if (dataUnlock.isFish)
        {
            GameController.instance.unlockData.fishCanUnlocked.Add(GameController.instance.fishDatabase.GetFishOObjectData(dataUnlock.fishType));
            DataManager.StringItemUnLock();
        }
        else
        {
            GameController.instance.unlockData.weaponCanUnlocked.Add(GameController.instance.weaponDatabase.GetWeaponObjectData(dataUnlock.weaponType));
            DataManager.StringItemUnLock();
        }
        _isShow = false;
        DataManager.CountUnlockBoxShow--;
        if (DataManager.CountUnlockBoxShow >= 1)
        {
            SecondTimeShow();
        }
        else
        {
            DataManager.CountUnlockBoxShow = 0;
            base.Hide();
        }
    }

    FishType fishType;
    WeaponType weaponType;

    private void OnClickUnlockBtn()
    {
        Debug.Log("Show Video Reward");
        var dataUnlock = GameController.instance.unlockDatabase.GetUnlockDataObject(DataManager.unlockIndex - DataManager.CountUnlockBoxShow);
        Debug.Log(DataManager.unlockIndex - DataManager.CountUnlockBoxShow);
        Action actionSuccess = () =>
        {
            var fishUnlock = GameController.instance.fishDatabase.GetFishOObjectData(dataUnlock.fishType);
            var weaponUnlock = GameController.instance.weaponDatabase.GetWeaponObjectData(dataUnlock.weaponType);
            if (dataUnlock.isFish)
            {
                if (fishUnlock != null)
                {
                    GameController.instance.unlockData.fishUnlocked.Add(fishUnlock);
                    Debug.Log(fishUnlock.fishType.ToString());
                    GameController.instance.unlockData.RemoveFishDataCanUnlock(dataUnlock.fishType);
                    DataManager.StringItemUnLock();
                    DataManager.currentSkinFish = dataUnlock.fishType;
                }
            }
            else
            {
                if (weaponUnlock != null)
                {
                    GameController.instance.unlockData.weaponUnlocked.Add(weaponUnlock);
                    GameController.instance.unlockData.RemoveWeaponDataCanUnlock(dataUnlock.weaponType);
                    DataManager.StringItemUnLock();
                    DataManager.currentWeapon = dataUnlock.weaponType;
                }
            }
        };
        AdsManager.instance.ShowRewardVideo(actionSuccess, "Unlock Box");
        _isShow = false;
        DataManager.CountUnlockBoxShow--;
        if (DataManager.CountUnlockBoxShow >= 1)
        {
            SecondTimeShow();
        }
        else
        {
            DataManager.CountUnlockBoxShow = 0;
            gameObject.SetActive(false);
        }
    }
}
