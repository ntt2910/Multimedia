using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardItem : MonoBehaviour
{
    [SerializeField] private Image iconReward_Img;
    [SerializeField] private Text valueReward_Txt;

    public void Init(RewardDatabase.Reward reward)
    {
        valueReward_Txt.text = $"{reward.amount}";
        iconReward_Img.sprite = GameController.instance.rewardDatabase.GetSpriteItem(reward.item);
    }
}
