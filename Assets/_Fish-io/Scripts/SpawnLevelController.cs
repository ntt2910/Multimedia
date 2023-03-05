using LTABase.DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnLevelController : SingletonMonoBehavier<SpawnLevelController>
{
    public static float limit_x = 19, limit_y = 9f;
    public float delaySpawn = 3f;
    FishSpawnData fishSpawnData;
    List<Fish> fishSpawn = new List<Fish>();
    public List<FishPlayerData> lsFishPlayerData;
    public List<Fish> allFish = new List<Fish>();
    public List<GameObject> greenPearls = new List<GameObject>();
    private bool _isIAPInited = false;

    // Start is called before the first frame update
    void Start()
    {
        var levelDataObject = GameController.instance.levelDatabase.GetLevelDataObject(DataManager.LevelPlayer);
        //Debug.Log(levelDataObject);
        delaySpawn = levelDataObject.GetLevelObjet(DataManager.LevelPlayer).spawnDelay;
        
        List<string> iapIds = new List<string>();
        iapIds.Add(StringHelper.GameIAPID.ID_NO_ADS);
        PaymentHelper.Instance.InitProducts(iapIds, new List<string>(), OnInitIAPDone);

        LTABase.DesignPattern.Observer.Instance.AddObserver(ObserverName.ON_START_PLAY, OnStartPlay);
        LTABase.DesignPattern.Observer.Instance.AddObserver(ObserverName.ON_FISH_DIE, OnFishDie);
        LTABase.DesignPattern.Observer.Instance.AddObserver(ObserverName.ON_FISH_CHANGE_POINT, SortFish);
        LTABase.DesignPattern.Observer.Instance.AddObserver(ObserverName.ON_PLAYER_PICK_UP_GREEN_PEARL, PickUpGreenPearl);
    }
    private void OnDestroy()
    {
        LTABase.DesignPattern.Observer.Instance.RemoveObserver(ObserverName.ON_START_PLAY, OnStartPlay);
        LTABase.DesignPattern.Observer.Instance.RemoveObserver(ObserverName.ON_FISH_DIE, OnFishDie);
        LTABase.DesignPattern.Observer.Instance.RemoveObserver(ObserverName.ON_FISH_CHANGE_POINT, SortFish);

        LTABase.DesignPattern.Observer.Instance.RemoveObserver(ObserverName.ON_PLAYER_PICK_UP_GREEN_PEARL, PickUpGreenPearl);
    }
    
    private void OnInitIAPDone(bool obj)
    {
        if (obj)
        {
            _isIAPInited = true;
        }
    }
    public Fish GetRandomFish()
    {
        return fishSpawn[Random.Range(0, fishSpawn.Count)];
    }
    void OnFishDie(object data)
    {
        Fish fish = (Fish)data;
        fishSpawn.Remove(fish);
        allFish.Remove(fish);
        StartCoroutine(DelayAction(() =>
        {
            var levelDataObject = GameController.instance.levelDatabase.GetLevelDataObject(DataManager.LevelPlayer);
            var spawnFishData = levelDataObject.GetLevelObjet(DataManager.LevelPlayer);

            bool inBound = false;
            int index = 0;
            Vector3 spawnPos = Vector3.zero;
            int random = Random.Range(0, 100);
            while (!inBound)
            {

                float percentAmount = 0;
                for (int j = 0; j < spawnFishData.fishSpawnDatas.Count; j++)
                {
                    percentAmount += spawnFishData.fishSpawnDatas[j].spawnPercent;
                    if (percentAmount > random) break;
                    else
                    {
                        //percentAmount = spawnFishData.fishSpawnDatas[j].spawnPercent;
                        index = j;
                    }
                }
                spawnPos = new Vector3(Random.Range(-limit_x, limit_x), Random.Range(-limit_y, limit_y), 0);

                Vector3 viewPos = Camera.main.WorldToViewportPoint(spawnPos);
                if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
                {
                    inBound = false;
                }
                else
                {
                    inBound = true;
                }

                Collider[] colliders = Physics.OverlapSphere(spawnPos, 5f);
                if (colliders.Length > 0)
                {
                    for (int j = 0; j < colliders.Length; j++)
                    {
                        if (colliders[j].tag == "Fish")
                        {
                            inBound = false;
                        }
                    }
                }
            }
            //Debug.Log(index);
            //CreateManager.Instance.CreateFishAI(spawnPos);
            GameObject fish = Instantiate(GameController.instance.fishDatabase.GetFishOObjectData(spawnFishData.fishSpawnDatas[index].id).prefabAI, spawnPos, Quaternion.identity);
            fishSpawn.Add(fish.GetComponent<Fish>());
            allFish.Add(fish.GetComponent<Fish>());

            var fishAI = fish.GetComponent<Fish>();
            if (fishAI != null)
            {
                fishAI.LoadProfile(GetOneFishPlayerData());
            }

            int randomSize = Random.Range(0, 100);
            if (UIManager.Instance.time < 75 && UIManager.Instance.time >= 45)
            {
                if (randomSize > 30)
                {
                    for (int i = 0; i < GameController.instance.fishDatabase.GetFishDataLevel(2).conditionLevel; i++)
                    {
                        fishAI.UpgradeFish();
                    }
                }
            }
            else if (UIManager.Instance.time < 45 && UIManager.Instance.time >= 15)
            {
                if (randomSize > 20 && randomSize <= 80)
                {
                    for (int i = 0; i < GameController.instance.fishDatabase.GetFishDataLevel(2).conditionLevel; i++)
                    {
                        fishAI.UpgradeFish();
                    }
                }
                else if (randomSize > 80)
                {
                    for (int i = 0; i < GameController.instance.fishDatabase.GetFishDataLevel(3).conditionLevel; i++)
                    {
                        fishAI.UpgradeFish();
                    }
                }
            }
            else if (UIManager.Instance.time < 15)
            {
                if (randomSize > 10 && randomSize <= 50)
                {
                    for (int i = 0; i < GameController.instance.fishDatabase.GetFishDataLevel(2).conditionLevel; i++)
                    {
                        fishAI.UpgradeFish();
                    }
                }
                else if (randomSize > 50)
                {
                    for (int i = 0; i < GameController.instance.fishDatabase.GetFishDataLevel(3).conditionLevel; i++)
                    {
                        fishAI.UpgradeFish();
                    }
                }
            }
        }));
    }
    void OnStartPlay(object data)
    {
        Observable.Timer(TimeSpan.FromSeconds(0)).Subscribe(_ =>
        {
            allFish.Add(FindObjectOfType<PlayerController>());
            var levelDataObject = GameController.instance.levelDatabase.GetLevelDataObject(DataManager.LevelPlayer);
            var spawnFishData = levelDataObject.GetLevelObjet(DataManager.LevelPlayer);
            for (int i = 0; i < 10; i++)
            {
                bool inBound = false;
                int index = 0;
                Vector3 spawnPos = Vector3.zero;
                int random = Random.Range(0, 100);
                while (!inBound)
                {

                    float percentAmount = 0;
                    for (int j = 0; j < spawnFishData.fishSpawnDatas.Count; j++)
                    {
                        percentAmount += spawnFishData.fishSpawnDatas[j].spawnPercent;
                        if (percentAmount > random)
                        {
                            index = j - 1;
                            index = Mathf.Clamp(index, 0, spawnFishData.fishSpawnDatas.Count);
                            //Debug.Log(index);
                            //Debug.Log(random);
                            break;
                        }
                        else
                        {
                            //percentAmount = spawnFishData.fishSpawnDatas[j].spawnPercent;

                        }
                    }
                    spawnPos = new Vector3(Random.Range(-limit_x, limit_x), Random.Range(-limit_y, limit_y), 0);

                    Vector3 viewPos = Camera.main.WorldToViewportPoint(spawnPos);
                    if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
                    {
                        inBound = false;
                    }
                    else
                    {
                        inBound = true;
                    }

                    Collider[] colliders = Physics.OverlapSphere(spawnPos, 5f);
                    if (colliders.Length > 0)
                    {
                        for (int j = 0; j < colliders.Length; j++)
                        {
                            if (colliders[j].tag == "Fish")
                            {
                                inBound = false;
                            }
                        }
                    }
                }
                //Debug.Log(index);
                //CreateManager.Instance.CreateFishAI(spawnPos);
                GameObject fish = Instantiate(GameController.instance.fishDatabase.GetFishOObjectData(spawnFishData.fishSpawnDatas[index].id).prefabAI, spawnPos, Quaternion.identity);
                fish.GetComponent<Fish>().StartGame();
                fishSpawn.Add(fish.GetComponent<Fish>());
                allFish.Add(fish.GetComponent<Fish>());

                //for (int i = 0; i < 10; i++)
                //{
                var fishAI = fish.GetComponent<Fish>();
                if (fishAI != null)
                {
                    fishAI.LoadProfile(lsFishPlayerData[i]);
                }
                //}
            }
            for (int i = 0; i < 10; i++)
            {
                bool inBound = false;
                Vector3 spawnPos = Vector3.zero;
                while (!inBound)
                {

                    spawnPos = new Vector3(Random.Range(-limit_x, limit_x), Random.Range(-limit_y, limit_y), 0);

                    Vector3 viewPos = Camera.main.WorldToViewportPoint(spawnPos);
                    if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
                    {
                        inBound = false;
                    }
                    else
                    {
                        inBound = true;
                    }

                    Collider[] colliders = Physics.OverlapSphere(spawnPos, 5f);
                    if (colliders.Length > 0)
                    {
                        for (int j = 0; j < colliders.Length; j++)
                        {
                            if (colliders[j].tag == "Fish")
                            {
                                inBound = false;
                            }
                        }
                    }
                }
                //Debug.Log(index);
                //CreateManager.Instance.CreateFishAI(spawnPos);
                GameObject greenPear = CreateManager.Instance.CreateGreenPearl(spawnPos);

                greenPearls.Add(greenPear);
            }
        }).AddTo(this);
    }
    IEnumerator DelayAction(System.Action action)
    {
        yield return new WaitForSeconds(delaySpawn);
        action?.Invoke();
    }

    public void GetFishPlayerData()
    {
        lsFishPlayerData = new List<FishPlayerData>();
        var lsName = GetRandomName();
        var lsCountry = GetRandomCountry();
        for (int i = 0; i < 10; i++)
        {
            var fishPlayerData = new FishPlayerData();
            fishPlayerData.name = lsName[Mathf.Clamp(i, 0, lsName.Count - 1)];
            fishPlayerData.countryCode = lsCountry[Mathf.Clamp(i, 0, lsCountry.Count - 1)];
            lsFishPlayerData.Add(fishPlayerData);
        }
    }
    public FishPlayerData GetOneFishPlayerData()
    {
        var lsName = GetRandomName();
        var lsCountry = GetRandomCountry();
        var fishPlayerData = new FishPlayerData();

        int random_name = Random.Range(0, lsName.Count);
        int random_countryCode = Random.Range(0, lsCountry.Count);

        fishPlayerData.name = lsName[Mathf.Clamp(random_name, 0, lsName.Count - 1)];
        fishPlayerData.countryCode = lsCountry[Mathf.Clamp(random_countryCode, 0, lsCountry.Count - 1)];

        return fishPlayerData;
    }
    private List<string> GetRandomName()
    {
        var lsName = new List<string>();
        var lsNameTemplate = GameController.instance.lsNameTemplate;
        for (int i = 0; i < 11; i++)
        {
            var random = Random.Range(0, lsNameTemplate.Count - 1);
            lsName.Add(lsNameTemplate[Mathf.Clamp(random, 0, lsNameTemplate.Count - 1)]);
        }
        return lsName;
    }

    private List<string> GetRandomCountry()
    {
        var lsName = new List<string>();
        var lsNameTemplate = GameController.instance.lsCountry;
        for (int i = 0; i < 11; i++)
        {
            var random = Random.Range(0, lsNameTemplate.Count - 1);
            lsName.Add(lsNameTemplate[Mathf.Clamp(random, 0, lsNameTemplate.Count - 1)]);
        }
        return lsName;
    }

    void SortFish(object data)
    {
        for (int i = 1; (i <= allFish.Count - 1); i++)
        {
            for (int j = 0; j < (allFish.Count - 1); j++)
            {
                if (allFish[j + 1].point > allFish[j].point)
                {
                    var temp = allFish[j];
                    allFish[j] = allFish[j + 1];
                    allFish[j + 1] = temp;
                }
            }
        }
    }
    void PickUpGreenPearl(object data)
    {
        bool inBound = false;
        Vector3 spawnPos = Vector3.zero;
        Observable.Timer(TimeSpan.FromSeconds(Random.Range(2, 5)), Scheduler.MainThreadIgnoreTimeScale).Subscribe(_ =>
        {
            while (!inBound)
            {

                spawnPos = new Vector3(Random.Range(-limit_x, limit_x), Random.Range(-limit_y, limit_y), 0);

                Vector3 viewPos = Camera.main.WorldToViewportPoint(spawnPos);
                if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
                {
                    inBound = false;
                }
                else
                {
                    inBound = true;
                }

                Collider[] colliders = Physics.OverlapSphere(spawnPos, 5f);
                if (colliders.Length > 0)
                {
                    for (int j = 0; j < colliders.Length; j++)
                    {
                        if (colliders[j].tag == "Fish")
                        {
                            inBound = false;
                        }
                    }
                }
            }
        //Debug.Log(index);
        //CreateManager.Instance.CreateFishAI(spawnPos);
        GameObject greenPear = CreateManager.Instance.CreateGreenPearl(spawnPos);
            greenPearls.Add(greenPear);
        });
    }
}
