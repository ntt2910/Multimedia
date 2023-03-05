using System.Collections;
using System.Collections.Generic;
using HyperCasualTemplate;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoSingleton<GameController>
{
    public GameState gameState;
    public MusicManager musicManager;
    public static bool isRetry = false;
    public bool isWin;

    ///public IapController iapController;
    [Header("DATA")]
    public RewardDatabase rewardDatabase;
    public PassiveDatabase passiveDatabase;
    public FishDatabase fishDatabase;
    public LevelDatabase levelDatabase;
    public WeaponDatabase weaponDatabase;
    public FishAIDatabase fishAIDatabase;
    public UnlockDatabase unlockDatabase;
    public SoundData soundData;

    [Header("DATA COUNTRY")]
    public List<string> lsCountry;
    public List<string> lsNameTemplate;

    [Header("DATA PLAYER")]
    public PlayerUnlockData unlockData;

    private AsyncOperation async;
    
    private void Awake()
    {
        unlockData = DataManager.GetUnlockData();
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        //iapController.Init();
        gameState = GameState.NORMAL;
        GetCountryCode();
        GetPlayerName();
        isWin = false;
        
#if UNITY_EDITOR || DEVELOPMENT
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        
    }

    private IEnumerator IELoadScene(string sceneName)
    {
        //yield return new WaitForSeconds(0.25f);

        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        yield return new WaitUntil(() => async.progress == 0.9f);
        
        async.allowSceneActivation = true;

        // int time = RemoteConfigControl.instance.time_load_startapp;
        //
        // if (time < 0)
        // {
        //     time = 6;
        // }
        //
        // yield return new WaitForSeconds(time);
        // AppOpenAdManager.Instance.ShowAdIfAvailable(() =>
        // {
        //     Debug.LogError("===== ShowAdIfAvailable");
        //     async.allowSceneActivation = true;
        //     AdsManager.instance.canShowAOA = true;
        // });
    }

    public void RemoveAds()
    {
        DataManager.RemoveAds = 1;
    }

    private void GetCountryCode()
    {
        lsCountry = new List<string>();
        var textFile = Resources.Load<TextAsset>("country");

        var  lineSeperater = '\n';
        var  fieldSeperator = ',';
        string[] records = textFile.text.Split (lineSeperater);
        for (int i = 0; i < records.Length; i++)
        {
            var record = records[i];
            string[] fields = record.Split(fieldSeperator);
            foreach(string field in fields)
            {
                //Data in 1 row
                var countryCode = field.Replace('\r', ' ');
                lsCountry.Add(countryCode.Trim().ToLower());
            }
        }
        
        Debug.Log("=== Done GetCountryCode();");
    }

    private void GetPlayerName()
    {
        lsNameTemplate = new List<string>();
        var textFile = Resources.Load<TextAsset>("PlayerName");
        
        var  lineSeperater = '\n';
        var  fieldSeperator = ',';
        string[] records = textFile.text.Split (lineSeperater);
        for (int i = 0; i < records.Length; i++)
        {
            var record = records[i];
            string[] fields = record.Split(fieldSeperator);
            foreach(string field in fields)
            {
                //Data in 1 row
                lsNameTemplate.Add(field);
            }
        }
        
        Debug.Log("=== Done GetPlayerName();");
    }
    
    
    // [Button]
    // public void GetFlags()
    // {
    //    StartCoroutine(ABCD());
    // }

    IEnumerator ABCD()
    {
        var textFile = Resources.Load<TextAsset>("country");
        Debug.Log(textFile.text);
        
        var datas = new List<string>();
        var  lineSeperater = '\n';
        var  fieldSeperator = ',';
        string[] records = textFile.text.Split (lineSeperater);
        for (int i = 0; i < records.Length; i++)
        {
            var record = records[i];
            string[] fields = record.Split(fieldSeperator);
            foreach(string field in fields)
            {
                //Data in 1 row
                datas.Add(field.Replace('\r',' '));
            }
        }

        // var random = Random.Range(0, datas.Count - 1);
        // var url = $"https://ipdata.co/flags/ba.png";
        // Debug.Log(url);
        // ObservableWWW.GetWWW(url).CatchIgnore().Subscribe(www =>
        // {
        //     Debug.Log(url);
        //     avatar.texture = www.texture;
        // }).AddTo(this);
        
         for (int i = 0; i < datas.Count; i++)
         {
             yield return new WaitForSeconds(0.25f);
             var name = datas[i].Trim().ToLower();
             var texture = Resources.Load<Texture2D>($"flags/{name}");
             if (texture == null)
             {
                 Debug.LogError(name);
                 continue;
             }
             //todo
             //avatar.texture = texture;
             yield return new WaitForEndOfFrame();
         }
        Debug.Log("Doneeeeee");

        // for (int i = 0; i < datas.Count; i++)
        // {
        //     yield return new WaitForSeconds(0.5f);
        //     
        //     var code = datas[i].ToLower();
        //     var url = $"https://ipdata.co/flags/{code}.png";
        //     Debug.Log(url);
        //     ObservableWWW.GetWWW(url).CatchIgnore().Subscribe(www =>
        //     {
        //         Debug.Log(url);
        //         avatar.texture = www.texture;
        //     });
        //
        // }

    }
}
