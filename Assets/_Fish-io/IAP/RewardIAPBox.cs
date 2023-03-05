using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using HyperCasualTemplate;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RewardIAPBox : BaseBox
{
    public static GameObject instance;
    public UnityAction moreActionOff;
    public UnityAction actHide;
    [SerializeField] protected Transform contentPool;
    [SerializeField] protected ElementReceverIAP rewardPrefab;

    protected List<ElementReceverIAP> rewardsPool;
    public Button claimBtn;

    protected override void OnStart()
    {
        base.OnStart();
        backObj.timeAnimClose = 0.5f;
    }

    protected override void ActionDoOff()
    {
        base.ActionDoOff();
        moreActionOff?.Invoke();
        moreActionOff = null;

        mainPanel.localScale = Vector3.one;
        mainPanel.transform.DOScale(Vector3.zero, 0.5f).SetUpdate(true).SetEase(Ease.InBack);

        actHide?.Invoke();
    }

    public static RewardIAPBox Setup()
    {
        if (instance == null)
        {
            var obj = Resources.Load(PathPrefabs.RECEVER_IAP);

            if (obj != null)
            {
                // Create popup and attach it to UI
                instance = Instantiate(obj as GameObject);
            }
        }

        RewardIAPBox rw = null;

        if (instance != null)
        {
            instance.SetActive(true);
            rw = instance.GetComponent<RewardIAPBox>();
        }

        return rw;
    }

    

    
    public RewardIAPBox ShowByWatchVideo(int value)
    {
        for (int i = 0; i < contentPool.transform.childCount; i++)
        {
            contentPool.transform.GetChild(i).gameObject.SetActive(false);
        }

        //Show Reward
        ElementReceverIAP reward = GetElement();
        reward.Init(Enums.TypeItem.Coin, value);

        return this;
    }

    private ElementReceverIAP GetElement()
    {
        if (rewardsPool == null)
            rewardsPool = new List<ElementReceverIAP>();
        for (int i = 0; i < rewardsPool.Count; i++)
        {
            if (!rewardsPool[i].gameObject.activeSelf)
            {
                rewardsPool[i].gameObject.SetActive(true);
                return rewardsPool[i];
            }
        }

        ElementReceverIAP prefab = Instantiate(rewardPrefab.gameObject, contentPool).GetComponent<ElementReceverIAP>();
        rewardsPool.Add(prefab);
        return prefab;
    }
}