using System.Collections;
using System.Collections.Generic;
using BW.Coroutine;
using HyperCasualTemplate;
using TMPro;
using UnityEngine;

public class MatchingPanel : MonoBehaviour
{
    [SerializeField] private MatchingItem[] matchingItems;
    [SerializeField] TextMeshProUGUI matching;
    private CoroutineHandle loadTextCor;
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void Init()
    {
        gameObject.SetActive(true);
        SpawnLevelController.Instance.GetFishPlayerData();
        var lsRandomInt = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            lsRandomInt.Add(i);
        }

        var lsFishPlayerIndex = Helper.DisruptiveListObject(lsRandomInt);
        StartCoroutine(ShowFishPlayerData(lsFishPlayerIndex));
        this.loadTextCor = Timing.RunCoroutine(LoadingText());
    }

    IEnumerator ShowFishPlayerData(List<int> lsFishPlayerIndex)
    {
        var lsFishPlayerData = SpawnLevelController.Instance.lsFishPlayerData;
        if (lsFishPlayerData == null || lsFishPlayerData.Count <= 0)
        {
            Debug.LogError("SpawnLevelController.Instance.lsFishPlayerData null");
            yield return null;
        }
        for (int i = 0; i < lsFishPlayerIndex.Count; i++)
        {
            yield return new WaitForSeconds(0.25f);
            var data = lsFishPlayerData[i];
            matchingItems[lsFishPlayerIndex[i]].Init(data);
        }
        
        yield return new WaitForSeconds(0.5f);
        LTABase.DesignPattern.Observer.Instance.Notify(ObserverName.ON_START_PLAY);
        GameController.instance.gameState = GameState.PLAYING;
        DataManager.TimePlay++;
        gameObject.SetActive(false);
    }

    // [Button]
    // public void GetItem()
    // {
    //     List<MatchingItem> temp = new List<MatchingItem>();
    //     for (int i = 0; i < transform.childCount; i++)
    //     {
    //         temp.Add(transform.GetChild(i).GetComponent<MatchingItem>());
    //     }
    //
    //     matchingItems = temp.ToArray();
    // }
    IEnumerator<float> LoadingText()
    {
        const string loading1 = "Matching .";
        const string loading2 = "Matching ..";
        const string loading3 = "Matching ...";

        while (true)
        {
            matching.text = loading1;
            yield return Timing.WaitForSeconds(0.3f);

            matching.text = loading2;
            yield return Timing.WaitForSeconds(0.3f);

            matching.text = loading3;
            yield return Timing.WaitForSeconds(0.3f);
        }
    }
}
