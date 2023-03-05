using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseBox : BaseBox
{
    private static GameObject instance;
    [SerializeField] SliceOnOff sliceSound, sliceVibrate, sliceMusic;
    bool init = false;
    protected override void Awake()
    {
        base.Awake();
        if (!init)
        {
            sliceVibrate.onClickOff += TurnHapticsOff;
            sliceVibrate.onClickOn += TurnHapticsOn;

            sliceSound.onClickOff += TurnSoundsOff;
            sliceSound.onClickOn += TurnSoundsOn;

            sliceMusic.onClickOff += TurnMusicOff;
            sliceMusic.onClickOn += TurnMusicOn;

            Debug.Log(MusicManager.vibrate);
            if (!MusicManager.soundTurn) sliceSound.SetOn(MusicManager.soundTurn);
            sliceVibrate.SetOn(MusicManager.vibrate);
            if (!MusicManager.musicTurn) sliceMusic.SetOn(MusicManager.musicTurn);

            init = true;
        }
    }
    protected override void Start()
    {
        base.Start();

    }
    public override void Show()
    {
        base.Show();
        sliceSound.SetOn(MusicManager.soundTurn);
        sliceMusic.SetOn(MusicManager.musicTurn);
    }
    protected override void OnStart()
    {
        base.OnStart();
        //var c = FindObjectsOfType<Canvas>();
        //for (int i = 0; i < c.Length; i++)
        //{
        //    if (c[i].transform != transform) c[i].GetComponent<Canvas>().enabled = false;
        //}


    }

    public static PauseBox Setup()
    {
        if (instance == null)
        {
            // Create popup and attach it to UI

            instance = Instantiate(Resources.Load(PathPrefabs.PAUSE_BOX) as GameObject);
        }
        instance.SetActive(true);

        return instance.GetComponent<PauseBox>();
    }
    public virtual void TurnSoundsOn()
    {
        MusicManager.Instance.TurnSoundVolune(true);
        //MusicManager.Instance.TurnMusicVolune(true);
        //AudioListener.volume = 1f;
        //SettingManager.instance.Sound = true;
        //MMVibrationManager.Haptic(HapticTypes.Success, false, true, this);
    }

    public virtual void TurnSoundsOff()
    {
        MusicManager.Instance.TurnSoundVolune(false);
        //MusicManager.Instance.TurnMusicVolune(false);
        //AudioListener.volume = 0f;
        //SettingManager.instance.Sound = false;
        //MMVibrationManager.Haptic(HapticTypes.Warning, false, true, this);
    }

    public virtual void TurnMusicOn()
    {
        //MusicManager.Instance.TurnSoundVolune(true);
        MusicManager.Instance.TurnMusicVolune(true);
        //AudioListener.volume = 1f;
        //SettingManager.instance.Sound = true;
        //MMVibrationManager.Haptic(HapticTypes.Success, false, true, this);
    }

    public virtual void TurnMusicOff()
    {
        //MusicManager.Instance.TurnSoundVolune(false);
        MusicManager.Instance.TurnMusicVolune(false);
        //AudioListener.volume = 0f;
        //SettingManager.instance.Sound = false;
        //MMVibrationManager.Haptic(HapticTypes.Warning, false, true, this);
    }
    public void TurnHapticsOff()
    {
        MMVibrationManager.SetHapticsActive(false);
        MusicManager.vibrate = false;
    }
    public void TurnHapticsOn()
    {
        MMVibrationManager.SetHapticsActive(true);
        MusicManager.vibrate = true;
    }
    public override void Hide()
    {
        base.Hide();
        //Observer.Instance.Notify(ObserverName.ON_CLOSE_SETTING);
    }
    public void OnClickHome()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }    
}
