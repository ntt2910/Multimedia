using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using HyperCasualTemplate;
using UnityEngine;

[System.Serializable]
public struct SpriteItem
{

    public Enums.TypeItem itemName;
    public Sprite spItem;
}

[CreateAssetMenu(menuName = "ScriptableObject/RewardDatabase", fileName = "RewardDatabase.asset")]
public class RewardDatabase : SerializedScriptableObject
{
    public List<SpriteItem> spriteItems;

    public Sprite GetSpriteItem(Enums.TypeItem typeItem)
    {
        SpriteItem spItm = spriteItems.Find(spItem => spItem.itemName == typeItem);

        return spItm.spItem;
    }

    public void Claim(Enums.TypeItem typeItem, int amount, Enums.Reason reason, bool isUpdateUI = true)
    {
        switch (typeItem)
        {
            case Enums.TypeItem.Coin:
                // DataManager.AddCoin(amount, isUpdateUI);
                // GameController.instance.AnalyticsController.LogAddCoin(amount, reason);
                break;
            case Enums.TypeItem.Gem:
                // DataManager.AddGem(amount, isUpdateUI);
                // GameController.instance.AnalyticsController.LogAddGem(amount, reason);
                break;
            case Enums.TypeItem.Energy:
                // DataManager.AddEnergy(amount, isUpdateUI);
                // GameController.instance.AnalyticsController.LogAddEnergy(amount, reason);
                break;
            case Enums.TypeItem.RemoveADS:
                GameController.instance.RemoveAds();
                break;
            default:
                break;
        }
    }
    

    [Serializable]
    public class Reward
    {
        public Enums.TypeItem item;
        public int amount;
        public int weight;

        public Reward()
        {
        }
        public Reward(Enums.TypeItem item, int amount, int weight = 0)
        {
            this.item = item;
            this.amount = amount;
            this.weight = weight;
        }
    }
}

