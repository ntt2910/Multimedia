using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EventDispatcher;
using DG.Tweening;
using HyperCasualTemplate;

public class IAPItem : MonoBehaviour
{
    public Enums.TypePackIAP typePack;
    //private IAPPack IAPpack;

    [SerializeField] private Text txtTittle;
    [SerializeField] private Text txtPriceInapp;
    [SerializeField] private Image imgIcon;
    [SerializeField] private Image imgBuy;
    [SerializeField] private Button btBuy;

    [SerializeField] private Text txtPriceNotInapp;
    [SerializeField] private Image iconCurrency;
    [SerializeField] private Image iconVideo;

    [SerializeField] private Sprite sprBuyNotInapp;
    [SerializeField] private Sprite sprBuyInapp;

    private bool isInited;

    private void Start()
    {
        btBuy.onClick.AddListener(() => OnClickBuy());
    }

    public void InitData(Enums.TypeItem _typeItem)
    {
        //if (IAPpack == null)
        //{
        //    var _pack = GameController.instance.iapController.inappDatabase.GetPackAll(typePack);
        //    if (_pack == null)
        //        return;
        
        //    IAPpack = _pack;
        //}
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        //if (IAPpack == null)
        //    return;
        //foreach (var item in IAPpack.itemsResult)
        //{
        //    txtTittle.text =  item.Value.ToString();
        //}

        //imgIcon.sprite = IAPpack.icon;
        //imgIcon.SetNativeSize();
        //btBuy.gameObject.transform.DOKill();
        //btBuy.gameObject.transform.localScale = 0.32f * Vector3.one;

        ////if (!isInited)
        //{
        //    if (IAPpack.typeBuy == TypeBuy.InApp)
        //    {
        //        txtPriceNotInapp.gameObject.SetActive(false);
        //        iconCurrency.gameObject.SetActive(false);
        //        iconVideo.gameObject.SetActive(false);

        //        txtPriceInapp.gameObject.SetActive(true);
        //        txtPriceInapp.text = GameController.instance.iapController.GetPrice(typePack);

        //        imgBuy.sprite = sprBuyInapp;
        //    }
        //    else if (IAPpack.typeBuy == TypeBuy.Gem)
        //    {
        //        txtPriceInapp.gameObject.SetActive(false);
        //        iconVideo.gameObject.SetActive(false);

        //        txtPriceNotInapp.gameObject.SetActive(true);
        //        iconCurrency.gameObject.SetActive(true);
        //        iconCurrency.sprite = GameController.instance.rewardDatabase.GetSpriteItem(Enums.TypeItem.Gem);
        //        txtPriceNotInapp.text = GameController.instance.iapController.GetPriceNotInapp(typePack).ToString();

        //        imgBuy.sprite = sprBuyNotInapp;
        //    }
        //    else if (IAPpack.typeBuy == TypeBuy.Coin)
        //    {
        //        txtPriceInapp.gameObject.SetActive(false);
        //        iconVideo.gameObject.SetActive(false);

        //        txtPriceNotInapp.gameObject.SetActive(true);
        //        iconCurrency.gameObject.SetActive(true);
        //        iconCurrency.sprite = GameController.instance.rewardDatabase.GetSpriteItem(Enums.TypeItem.Coin);
        //        txtPriceNotInapp.text = GameController.instance.iapController.GetPriceNotInapp(typePack).ToString();

        //        imgBuy.sprite = sprBuyNotInapp;
        //    }
        //    else if (IAPpack.typeBuy == TypeBuy.Video)
        //    {
        //        iconVideo.gameObject.SetActive(true);

        //        txtPriceInapp.gameObject.SetActive(false);
        //        txtPriceNotInapp.gameObject.SetActive(false);
        //        iconCurrency.gameObject.SetActive(false);

        //        imgBuy.sprite = sprBuyNotInapp;

        //        //btBuy.gameObject.transform.DOKill();
        //        btBuy.gameObject.transform.DOScale (0.36f * Vector3.one, 0.3f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        //    }

        //    isInited = true;
        //}
    }

    private void OnClickBuy()
    {
        //if (IAPpack == null)
        //    return;
        
        //if (IAPpack.typeBuy == TypeBuy.InApp)
        //    GameController.instance.iapController.BuyProduct(typePack);
        //else
        //    GameController.instance.iapController.BuyProductNotInApp(typePack);
        
        //MusicManager.Instance.PlaySfx(MusicManager.Instance.sfxClickButton);
    }
}
