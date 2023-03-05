using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using HyperCasualTemplate;
using UnityEngine;
using UnityEngine.UI;

public class ElementReceverIAP : MonoBehaviour
{

    [SerializeField] protected Image iconReward_Img;
    [SerializeField] protected Text valueReward_Txt;

    public void Init(Enums.TypeItem typeItem, int value)
    {
        valueReward_Txt.text = "";
        switch (typeItem)
       {
           case  Enums.TypeItem.RemoveADS:
                iconReward_Img.sprite = GameController.instance.rewardDatabase.GetSpriteItem(Enums.TypeItem.RemoveADS);
                valueReward_Txt.text = "No Ads";
                break;
           default:
                iconReward_Img.sprite = GameController.instance.rewardDatabase.GetSpriteItem(typeItem);
                valueReward_Txt.text = value.ToString();
                break;
       }
    }
}