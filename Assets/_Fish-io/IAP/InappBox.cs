using System;
using System.Collections;
using System.Collections.Generic;
using HyperCasualTemplate;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class InappBox : BaseBox
{
    private static InappBox instance;

    public static InappBox Setup(Enums.TypeItem typeItem = Enums.TypeItem.Energy)
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<InappBox>(PathPrefabs.SHOP_BOX));
        }
        instance.typeItemCurrent = typeItem;
        //instance.Show();
        return instance;
    }

    //[SerializeField] private IAPDatabase IAPData;
    [SerializeField] private List<PackItem> lsPackItem;
    [SerializeField] private Image imgBG;
    [SerializeField] private ToggleSwitchSprite[] toggles;
    [SerializeField] private Button watchVideoBtn;

    [SerializeField] private GameObject textCoolDownEnegry;

    private Enums.TypeItem typeItemCurrent = Enums.TypeItem.Coin;

    protected override void OnStart()
    {
        base.OnStart();

        InitData();
    }

    public override void Show()
    {
        base.Show();
        
        switch (typeItemCurrent)
        {
            case Enums.TypeItem.Energy:
                OnClickToggle(0);
                UpdateToogle(0);
                textCoolDownEnegry.gameObject.SetActive(true);
                break;
            case  Enums.TypeItem.Coin:
                OnClickToggle(1);
                UpdateToogle(1);
                textCoolDownEnegry.gameObject.SetActive(false);
                break;
            case Enums.TypeItem.Gem:
                OnClickToggle(2);
                UpdateToogle(2);
                textCoolDownEnegry.gameObject.SetActive(false);
                break;
            case Enums.TypeItem.RemoveADS:
                break;
        }
    }

    private void InitData()
    {
        for (int i = 0; i < lsPackItem.Count; i++)
        {
            lsPackItem[i].Init();
        }

        // watchVideoBtn.onClick.RemoveAllListeners();
        // watchVideoBtn.onClick.AddListener(OnClickWatchVideo);
    }

    public void OnClickToggle(int idTab)
    {
        for (int i = 0; i < lsPackItem.Count; i++)
        {
            if(idTab == i)
                lsPackItem[i].Show();
            else
                lsPackItem[i].Close();
        }

        imgBG.sprite = lsPackItem[idTab].BG;

        if(idTab == 0)
            textCoolDownEnegry.gameObject.SetActive(true);
        else
            textCoolDownEnegry.gameObject.SetActive(false);

        //MusicManager.Instance.PlaySfx(MusicManager.Instance.sfxClickButton);
    }

    private void UpdateToogle(int id)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if (i == id)
            {
                toggles[i].UpdateSprite();
            }
            else
            {
                toggles[i].UpdateSprite(false);
            }
        }
    }

    private void OnClickWatchVideo()
    {
        //GameController.instance.admobAds.ShowVideoReward(ActionReward, ActionNotLoad, ActionSkip, ActionWatchVideo.AddGem_InShop);
    }

    private void ActionReward()
    {
        Debug.Log("Claim watch ad");
        //DataManager.AddHint(1);
        //RewardIAPBox.Setup().ShowByWatchVideo(1);
    }

    private void ActionNotLoad()
    {
       // ConfirmBox.Setup().AddMessageYes("Fail", "Failed to load video", () => CloseCurrentBox());
    }

    private void ActionSkip()
    {
        Debug.Log("Skip video");
    }

}

[Serializable]
public class PackItem
{
    public Transform content;
    public Enums.TypeItem typeItem;
    public List<IAPItem> lsItems;
    public Sprite BG;

    public void Init()
    {
        for (int i = 0; i < lsItems.Count; i++)
        {
            //if (!lsItems[i].gameObject.activeInHierarchy)
            //    continue;

            lsItems[i].InitData(typeItem);
        }
    }

    public void Show()
    {
        content.gameObject.SetActive(true);
    }

    public void Close()
    {
        content.gameObject.SetActive(false);
    }
}