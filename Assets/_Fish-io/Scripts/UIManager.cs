using HyperCasualTemplate;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using System.Collections.Generic;
using DG.Tweening;
public class UIManager : SingletonMonoBehavier<UIManager>
{
    [SerializeField] TransitionPopup transitionPopup;
    [SerializeField] Canvas mainCanvas;
    [SerializeField] Camera cameraUI;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] GameObject winUI, loseUI, homeUI;
    [SerializeField] private MatchingPanel matchingPanel;
    [SerializeField] LeaderboardItem[] leaderboardItems;
    [SerializeField] Image circle;
    [SerializeField] private Button noTksBtn;
    [SerializeField] private Button noAdsBtn;
    [SerializeField] private ParticleSystem winCup;
    [SerializeField]
    Button
        btnShowSetting,
        btnRevive,
        btnPause,
        btnShowShop
        ;
    public bool isWin = false, endGame = false;
    public float time;
    public int playerHighestRank = -1;
    public List<GameObject> comboKill;
    void Start()
    {
        LTABase.DesignPattern.Observer.Instance.AddObserver(ObserverName.ON_PLAYER_DIE, OnPlayerDie);
        btnShowSetting.onClick.AddListener(ShowSetting);
        btnPause.onClick.AddListener(ShowPause);
        btnRevive.onClick.AddListener(OnClickRevive);
        btnShowShop.onClick.AddListener(ShowShop);
        noTksBtn.onClick.AddListener(OnRestart);
        noAdsBtn.onClick.AddListener(OnClickNoAdsBtn);
        noAdsBtn.gameObject.SetActive(DataManager.RemoveAds != 1);
        winCup.gameObject.SetActive(false);
        if(GameController.isRetry)
        {
            OnStartPlay();
            cameraUI.enabled = false;
            mainCanvas.enabled = true;
            GameController.isRetry = false;
        }    
    }
    private void OnDestroy()
    {
        LTABase.DesignPattern.Observer.Instance.RemoveObserver(ObserverName.ON_PLAYER_DIE, OnPlayerDie);
    }
    IDisposable disposableRevive;


    private void OnClickNoAdsBtn()
    {
        PaymentHelper.Purchase(StringHelper.GameIAPID.ID_NO_ADS, OnNoAdsDone);
        
    }
    
    private void OnNoAdsDone(bool obj)
    {
        if (obj)
        {
            GameController.instance.RemoveAds();
            noAdsBtn.gameObject.SetActive(false);
        }
        var box = IAPBox.Setup();
        box.Show();
    }
    
    public void OnPlayerDie(object data)
    {
        isWin = false;
        AnalyticsManager.instance.LogLevelFailEvent(DataManager.LevelPlayer.ToString());
        Observable.Timer(TimeSpan.FromSeconds(2.5f), Scheduler.MainThreadIgnoreTimeScale).Subscribe(_ =>
        {
            loseUI.SetActive(true);
            float currentDeltaTime = Time.deltaTime;
            circle.fillAmount = 1f;

            btnRevive.gameObject.SetActive(true);
            //Time.timeScale = 0;
            float delayTime = 2.5f;
            if (RemoteConfigControl.instance.Revive_Ads_Is_On >= 1 && DataManager.LastTimeShowAdsRevive.AddSeconds(RemoteConfigControl.instance.Revive_Ads_Cooldown) < DateTime.Now)
            {

                disposableRevive = Observable.Interval(TimeSpan.FromSeconds(currentDeltaTime), Scheduler.MainThreadIgnoreTimeScale).Subscribe(_ =>
                {
                    if (delayTime >= 0)
                    {
                        delayTime -= currentDeltaTime;
                        circle.fillAmount -= (1f / 2.5f) * currentDeltaTime;
                        if (delayTime <= 1.25f)
                        {
                            noTksBtn.gameObject.SetActive(true);
                        }
                        if (delayTime < 0f)
                        {
                            if (CheckShowInterAds())
                            {
                                Debug.Log("Show inter ads");
                                AdsManager.instance.ShowAdUnit(() =>
                                {
                                    btnRevive.gameObject.SetActive(false);
                                    endGame = true;
                                    var box = EndGameBox.Setup();
                                    box.Show();
                                    GameController.instance.gameState = GameState.NORMAL;
                                    disposableRevive.Dispose();
                                    for (int i = 0; i < SpawnLevelController.Instance.allFish.Count; i++)
                                    {
                                        SpawnLevelController.Instance.allFish[i].gameObject.SetActive(false);
                                    }
                                }, "WAIT_REVIVE_POPUP");
                            }
                            else
                            {
                                btnRevive.gameObject.SetActive(false);
                                endGame = true;
                                var box = EndGameBox.Setup();
                                box.Show();
                                GameController.instance.gameState = GameState.NORMAL;
                                disposableRevive.Dispose();
                                for (int i = 0; i < SpawnLevelController.Instance.allFish.Count; i++)
                                {
                                    SpawnLevelController.Instance.allFish[i].gameObject.SetActive(false);
                                }
                            }
                            //btnLosesNext.gameObject.SetActive(true);
                        }
                    }
                }).AddTo(this);
            }
            else
            {
                loseUI.SetActive(false);
                var box = EndGameBox.Setup();
                box.Show();
                GameController.instance.gameState = GameState.NORMAL;
                for (int i = 0; i < SpawnLevelController.Instance.allFish.Count; i++)
                {
                    SpawnLevelController.Instance.allFish[i].gameObject.SetActive(false);
                }
            }
        }).AddTo(this);
    }

    public void OnRestart()
    {
        if (CheckShowInterAds())
        {
            Debug.Log("Show inter ads");
            AdsManager.instance.ShowAdUnit(() =>
            {
                endGame = true;
                btnRevive.gameObject.SetActive(false);
                noTksBtn.gameObject.SetActive(false);
                var box = EndGameBox.Setup();
                box.Show();
                GameController.instance.gameState = GameState.NORMAL;
                disposableRevive.Dispose();
                for (int i = 0; i < SpawnLevelController.Instance.allFish.Count; i++)
                {
                    SpawnLevelController.Instance.allFish[i].gameObject.SetActive(false);
                }
            }, "click_no_thank");
        }
        else
        {
            endGame = true;
            btnRevive.gameObject.SetActive(false);
            noTksBtn.gameObject.SetActive(false);
            var box = EndGameBox.Setup();
            box.Show();
            GameController.instance.gameState = GameState.NORMAL;
            disposableRevive.Dispose();
            for (int i = 0; i < SpawnLevelController.Instance.allFish.Count; i++)
            {
                SpawnLevelController.Instance.allFish[i].gameObject.SetActive(false);
            }
        }
    }
    public void OnStartPlay()
    {
        time = 75;
        // Observer.Instance.Notify(ObserverName.ON_START_PLAY);
        // GameController.instance.gameState = GameState.PLAYING;
        matchingPanel.Init();
        homeUI.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (GameController.instance.gameState != GameState.PLAYING || endGame) return;

        if (time > 0f)
        {
            time -= Time.deltaTime;
            float m = (int)time / 60;
            float s = (int)time % 60;
            if (s < 10) timeText.text = $"Time : 0{m} : 0{s}";
            else timeText.text = $"Time : 0{m} : {s}";
            if (time <= 0 && GameController.instance.gameState == GameState.PLAYING && !endGame)
            {
                endGame = true;
                GameController.instance.gameState = GameState.NORMAL;
                if (GameController.instance.isWin == false)
                {
                    winCup.gameObject.SetActive(true);
                    winCup.Play();
                    DOVirtual.DelayedCall(3.25f, delegate
                    {
                        GameController.instance.isWin = true;
                        LTABase.DesignPattern.Observer.Instance.Notify(ObserverName.ON_END_GAME);
                        var box = EndGameBox.Setup();
                        box.Show();
                        //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.VictorySound);
                        DataManager.LevelPlayer++;
                        for (int i = 0; i < SpawnLevelController.Instance.allFish.Count; i++)
                        {
                            if (SpawnLevelController.Instance.allFish[i] == null) continue;
                            SpawnLevelController.Instance.allFish[i].gameObject.SetActive(false);
                        }
                        if (disposableRevive != null) disposableRevive.Dispose();
                        if (CheckShowInterAds())
                        {
                            Debug.Log("Show inter ads");
                            AdsManager.instance.ShowAdUnit(() =>
                            {
                            
                            }, "Win");
                        }
                    });
                }
            }
        }
    }
    private void LateUpdate()
    {
        if (GameController.instance.gameState != GameState.PLAYING) return;
        for (int i = 0; i < leaderboardItems.Length; i++)
        {
            if (i >= SpawnLevelController.Instance.allFish.Count) break;
            var fish = SpawnLevelController.Instance.allFish[i];
            if (fish == null) continue;
            leaderboardItems[i].SetData(fish.FishName, fish.point.ToString());
            if (fish.point > 0) fish.crowns.SetActiveCrown(i);
            var player = fish.transform.GetComponent<PlayerController>();
            if (player != null)
            {
                if (playerHighestRank == -1) playerHighestRank = i + 1;
                else
                {
                    if (playerHighestRank > i) playerHighestRank = i + 1;
                }
            }
        }
    }
    ///////////////////////////////////////
    void ShowSetting()
    {
        SettingBox.Setup();
    }
    void ShowPause()
    {
        PauseBox.Setup();
    }
    void ShowShop()
    {
        var box = ShopBox.Setup();
        box.Show();
    }
    void OnClickRevive()
    {
        if (disposableRevive != null) disposableRevive.Dispose();
        loseUI.gameObject.SetActive(false);
        Debug.Log("Show video reward");
        Action actionSuccess = () =>
        {
            LTABase.DesignPattern.Observer.Instance.Notify(ObserverName.ON_PLAYER_REVIVE);
        };

        if (RemoteConfigControl.instance.Revive_Ads_Is_On >= 1 && DataManager.LastTimeShowAdsRevive.AddSeconds(RemoteConfigControl.instance.Revive_Ads_Cooldown) < DateTime.Now)
        {
            AdsManager.instance.ShowRewardVideo(actionSuccess, "Player_Revive");
            DataManager.LastTimeShowAdsRevive = DateTime.Now;
        }
    }
    public bool CheckShowInterAds()
    {
        if (DataManager.TimePlay >= RemoteConfigControl.instance.WinScreen_1_3_Active_level_x
            && DataManager.TimePlay <= RemoteConfigControl.instance.WinScreen_1_3_to_Level)
        {
            if (DataManager.LastTimeShowInterAds.AddSeconds(RemoteConfigControl.instance.WinScreen_1_3_Cooldown) < DateTime.Now)
            {
                return RemoteConfigControl.instance.WinScreen_1_3_IsOn >= 1;
            }
        }
        if (DataManager.TimePlay >= RemoteConfigControl.instance.WinScreen_4_12_Active_level_x
            && DataManager.TimePlay <= RemoteConfigControl.instance.WinScreen_4_12_to_Level)
        {
            if (DataManager.LastTimeShowInterAds.AddSeconds(RemoteConfigControl.instance.WinScreen_4_12_Cooldown) < DateTime.Now)
            {
                return RemoteConfigControl.instance.WinScreen_4_12_IsOn >= 1;
            }
        }
        if (DataManager.TimePlay >= RemoteConfigControl.instance.WinScreen_13_20_Active_level_x
            && DataManager.TimePlay <= RemoteConfigControl.instance.WinScreen_13_20_to_Level)
        {
            if (DataManager.LastTimeShowInterAds.AddSeconds(RemoteConfigControl.instance.WinScreen_13_20_Cooldown) < DateTime.Now)
            {
                return RemoteConfigControl.instance.WinScreen_13_20_IsOn >= 1;
            }
        }
        if (DataManager.TimePlay >= RemoteConfigControl.instance.WinScreen_21_Active_level_x)
        {
            if (DataManager.LastTimeShowInterAds.AddSeconds(RemoteConfigControl.instance.WinScreen_21_Cooldown) < DateTime.Now)
            {
                return RemoteConfigControl.instance.WinScreen_21_IsOn >= 1;
            }
        }
        return false;
    }
}
