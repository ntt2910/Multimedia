using LTABase.DesignPattern;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopBox : BaseBox
{
    private static GameObject instance;
    public static ShopBox Setup()
    {
        if (instance == null)
        {
            // Create popup and attach it to UI

            instance = Instantiate(Resources.Load(PathPrefabs.SHOP_BOX) as GameObject);
        }
        instance.SetActive(false);

        return instance.GetComponent<ShopBox>();
    }
    [SerializeField] TextMeshProUGUI itemName, itemDescription;
    [SerializeField] ShopSlot shopSlotFish, shopSlotWeapon;
    [SerializeField] Transform parentSlotFish, parentSlotWeapon, previewItem, previewItem2, previewWeapon;

    List<ShopSlot> shopFishSlots = new List<ShopSlot>();
    List<ShopSlot> shopWeaponSlots = new List<ShopSlot>();

    [SerializeField] Canvas canvas;
    [SerializeField] Camera cameraUI;

    [SerializeField] Button btnShowShopFish, btnShowShopSword;

    [SerializeField] GameObject shopFish, shopSword;

    [SerializeField] Color colorTabOn, colorTabOff;

    ShopSlot targetSlot;
    GameObject itemView;
    private bool _isNewFish;

    protected override void Start()
    {
        base.Start();
        btnShowShopFish.onClick.AddListener(ShowShopFish);
        btnShowShopSword.onClick.AddListener(ShowShopSword);
        ShowShopFish();
        
        Observer.Instance.AddObserver(ObserverName.ON_CLICK_SHOP_FISH_SLOT, OnClickSlotFishShop);
        Observer.Instance.AddObserver(ObserverName.ON_CLICK_SHOP_WEAPON_SLOT, OnClickSlotSwordShop);
        Observer.Instance.AddObserver(ObserverName.ON_CLICK_SELECT_FISH, OnClickSelectFish);
        Observer.Instance.AddObserver(ObserverName.ON_CLICK_SELECT_WEAPON, OnClickSelectWeapon);
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverName.ON_CLICK_SHOP_FISH_SLOT, OnClickSlotFishShop);
        Observer.Instance.RemoveObserver(ObserverName.ON_CLICK_SHOP_WEAPON_SLOT, OnClickSlotSwordShop);
        Observer.Instance.RemoveObserver(ObserverName.ON_CLICK_SELECT_FISH, OnClickSelectFish);
        Observer.Instance.RemoveObserver(ObserverName.ON_CLICK_SELECT_WEAPON, OnClickSelectWeapon);
    }
    public override void Show()
    {
        base.Show();
        cameraUI.gameObject.SetActive(true);
        //Camera.main.enabled = (false);
        foreach (var slot in shopFishSlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        foreach (var slot in shopWeaponSlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }

        shopFishSlots.Clear();
        shopWeaponSlots.Clear();

        for (int i = 0; i < GameController.instance.fishDatabase.fishObjectDatas.Count; i++)
        {
            if (GameController.instance.fishDatabase.fishObjectDatas[i].prefab == null) continue;
            var slot = Instantiate(shopSlotFish, parentSlotFish);
            var data = GameController.instance.fishDatabase.fishObjectDatas[i];
            slot.LoadData(data.fishType.ToString(), data.icon);
            slot.GetComponent<ShopFishSlot>().fishType = GameController.instance.fishDatabase.fishObjectDatas[i].fishType;
            shopFishSlots.Add(slot);
            var fishUnlock = GameController.instance.unlockData.GetFishDataUnlock(GameController.instance.fishDatabase.fishObjectDatas[i].fishType);
            if (fishUnlock != null)
            {
                slot.SetSelect();
            }
            else
            {
                slot.SetLock();
            }

            if (data.fishType == DataManager.currentSkinFish) slot.SetSelected();

            var canUnlock = GameController.instance.unlockData.GetFishDataCanUnlock(GameController.instance.fishDatabase.fishObjectDatas[i].fishType);
            if(canUnlock != null)
            {
                slot.SetUnLock();
            }
        }

        for (int i = 0; i < GameController.instance.weaponDatabase.weaponObjectDatas.Count; i++)
        {
            if (GameController.instance.weaponDatabase.weaponObjectDatas[i].weaponPrefabs == null) continue;
            var slot = Instantiate(shopSlotWeapon, parentSlotWeapon);
            var data = GameController.instance.weaponDatabase.weaponObjectDatas[i];
            slot.LoadData(data.weaponType.ToString(), data.icon);
            slot.GetComponent<ShopWeaponSlot>().weaponType = GameController.instance.weaponDatabase.weaponObjectDatas[i].weaponType;
            shopWeaponSlots.Add(slot);
            var weaponUnlock = GameController.instance.unlockData.GetWeaponDataUnlock(GameController.instance.weaponDatabase.weaponObjectDatas[i].weaponType);
            if (weaponUnlock != null)
            {
                slot.SetSelect();
            }
            else
            {
                slot.SetLock();
            }
            if (data.weaponType == DataManager.currentWeapon) slot.SetSelected();

            var canUnlock = GameController.instance.unlockData.GetWeaponDataCanUnlock(GameController.instance.weaponDatabase.weaponObjectDatas[i].weaponType);
            if (canUnlock != null)
            {
                slot.SetUnLock();
            }
        }

        for (int i = 0; i < FindObjectsOfType<Canvas>().Length; i++)
        {
            FindObjectsOfType<Canvas>()[i].enabled = false;
        }
        canvas.enabled = true;
        canvas.worldCamera = cameraUI;
        cameraUI.transform.parent = null;
    }
    public override void Hide()
    {
        //base.Hide();
        //cameraUI.gameObject.SetActive(false);
        //for (int i = 0; i < FindObjectsOfType<Canvas>().Length; i++)
        //{
        //    FindObjectsOfType<Canvas>()[i].enabled = true;
        //}
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void ShowShopFish()
    {
        shopFish.gameObject.SetActive(true);
        shopSword.gameObject.SetActive(false);
        btnShowShopFish.GetComponentInChildren<TextMeshProUGUI>().color = colorTabOn;
        btnShowShopSword.GetComponentInChildren<TextMeshProUGUI>().color = colorTabOff;

        if (itemView != null) Destroy(itemView);
        var fishData = GameController.instance.fishDatabase.GetFishOObjectData(DataManager.currentSkinFish);
        if (fishData.isNewFish)
        {
            _isNewFish = true;
        }
        else
        {
            _isNewFish = false;
        }
        itemName.text = fishData.fishType.ToString();
        if (string.IsNullOrEmpty(fishData.des)) itemDescription.gameObject.SetActive(false);
        else
        {
            itemDescription.gameObject.SetActive(true);
            itemDescription.text = fishData.des;
        }
        if (fishData != null)
        {
            if (_isNewFish)
            {
                itemView = Instantiate(fishData.prefab, previewItem2);
            }
            else if (_isNewFish == false)
            {
                itemView = Instantiate(fishData.prefab, previewItem);
            }
            
            Transform[] chids = itemView.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < chids.Length; i++)
            {
                chids[i].gameObject.layer = LayerMask.NameToLayer("UI");
            }
            itemView.transform.localPosition = Vector3.zero;
            itemView.GetComponent<FishSkin>().enabled = false;
            itemView.GetComponent<FishSkin>().sword.gameObject.SetActive(false);
            Destroy(itemView.GetComponent<Collider>());
        }
    }
    public void ShowShopSword()
    {
        shopFish.gameObject.SetActive(false);
        shopSword.gameObject.SetActive(true);
        btnShowShopSword.GetComponentInChildren<TextMeshProUGUI>().color = colorTabOn;
        btnShowShopFish.GetComponentInChildren<TextMeshProUGUI>().color = colorTabOff;

        if (itemView != null) Destroy(itemView);
        var weapon = GameController.instance.weaponDatabase.GetWeaponObjectData(DataManager.currentWeapon);
        itemName.text = weapon.weaponType.ToString();
        if (string.IsNullOrEmpty(weapon.des)) itemDescription.gameObject.SetActive(false);
        else
        {
            itemDescription.gameObject.SetActive(true);
            itemDescription.text = weapon.des;
        }
        if (weapon != null)
        {
            itemView = Instantiate(weapon.weaponPrefabs, previewWeapon);
            Transform[] chids = itemView.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < chids.Length; i++)
            {
                chids[i].gameObject.layer = LayerMask.NameToLayer("UI");
            }
            itemView.transform.localPosition = Vector3.zero;
            itemView.GetComponent<Weapon>().enabled = false;
            Destroy(itemView.GetComponent<Collider>());
        }
    }

    void OnClickSlotFishShop(object data)
    {
        if (targetSlot != null) targetSlot.SetActiveBorder(false);

        targetSlot = (ShopSlot)data;
        targetSlot.SetActiveBorder(true);
        if (itemView != null) Destroy(itemView);
        var fishData = GameController.instance.fishDatabase.GetFishOObjectData(((ShopFishSlot)data).fishType);
        if (fishData.isNewFish)
        {
            _isNewFish = true;
        }
        else
        {
            _isNewFish = false;
        }
        itemName.text = fishData.fishType.ToString();
        if (string.IsNullOrEmpty(fishData.des)) itemDescription.gameObject.SetActive(false);
        else
        {
            itemDescription.gameObject.SetActive(true);
            itemDescription.text = fishData.des;
        }
        if (fishData != null)
        {
            if (_isNewFish)
            {
                itemView = Instantiate(fishData.prefab, previewItem2);
            }
            else if (_isNewFish == false)
            {
                itemView = Instantiate(fishData.prefab, previewItem);
            }
            
            Transform[] chids = itemView.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < chids.Length; i++)
            {
                chids[i].gameObject.layer = LayerMask.NameToLayer("UI");
            }
            itemView.transform.localPosition = Vector3.zero;
            itemView.GetComponent<FishSkin>().sword.gameObject.SetActive(false);
            Destroy(itemView.GetComponent<FishSkin>());
            Destroy(itemView.GetComponent<PassiveBase>());
            Destroy(itemView.GetComponent<Collider>());
        }

    }
    void OnClickSlotSwordShop(object data)
    {
        if (targetSlot != null) targetSlot.SetActiveBorder(false);

        targetSlot = (ShopSlot)data;
        targetSlot.SetActiveBorder(true);
        if (itemView != null) Destroy(itemView);
        var weapon = GameController.instance.weaponDatabase.GetWeaponObjectData(((ShopWeaponSlot)data).weaponType);
        itemName.text = weapon.weaponType.ToString();
        if (string.IsNullOrEmpty(weapon.des)) itemDescription.gameObject.SetActive(false);
        else
        {
            itemDescription.gameObject.SetActive(true);
            itemDescription.text = weapon.des;
        }
        if (weapon != null)
        {
            itemView = Instantiate(weapon.weaponPrefabs, previewWeapon);
            Transform[] chids = itemView.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < chids.Length; i++)
            {
                chids[i].gameObject.layer = LayerMask.NameToLayer("UI");
            }
            itemView.transform.localPosition = Vector3.zero;
            itemView.GetComponent<Weapon>().enabled = false;
            Destroy(itemView.GetComponent<Collider>());
        }
    }
    void OnClickSelectFish(object data)
    {
        OnClickSlotFishShop(data);
        var fishSlot = (ShopFishSlot)data;
        foreach (var slot in shopFishSlots)
        {
            var fishUnlock = GameController.instance.unlockData.GetFishDataUnlock(((ShopFishSlot)slot).fishType);
            if (slot != fishSlot && fishUnlock != null)
            {
                slot.SetSelect();
            }
        }
    }
    void OnClickSelectWeapon(object data)
    {
        OnClickSlotSwordShop(data);
        var weaponSlot = (ShopWeaponSlot)data;
        foreach (var slot in shopWeaponSlots)
        {
            var weaponUnlock = GameController.instance.unlockData.GetWeaponDataUnlock(((ShopWeaponSlot)slot).weaponType);
            if (slot != weaponSlot && weaponUnlock != null)
            {
                slot.SetSelect();
            }
        }
    }
}
