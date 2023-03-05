using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections.Generic;

//  ----------------------------------------------
//  Author:     CuongCT <caothecuong91@gmail.com> 
//  Copyright (c) 2016 OneSoft JSC
// ----------------------------------------------
public class MusicManager : SingletonMonoBehavier<MusicManager>
{
    public enum SourceAudio { Music, Effect, Sound };
    [SerializeField]
    private AudioMixer mixer;
    //public Musicdatabase musicdatabase;
    public List<AudioSource> effectSource;
    public AudioSource musicSource;
    public AudioSource soundSource;
    public float lowPitchRange = 0.95f;              //The lowest a sound effect will be randomly pitched.
    public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.
    public float delayTime = 0.5f;
    public const string MASTER_KEY = "MASTER_KEY";
    public const string MUSIC_KEY = "MUSIC_KEY";
    public const string SOUND_KEY = "SOUND_KEY";
    public const float MIN_VALUE = -80f;
    public const float MAX_VALUE = 0f;

    [SerializeField] private AudioClip BGMusic;
    [SerializeField] private AudioClip InGameMusic;
    //public AudioClip enemyHit, enemyDie, buildingHit, weaponAim, cointGet, planeCall, planeEngine;

    private AudioClip _currentMusic;
    // private static MusicManager _instance;
    //
    // public static MusicManager Instance
    // {
    //     get
    //     {
    //         if (_instance == null)
    //         {
    //             _instance = FindObjectOfType<MusicManager>();
    //             if (_instance != null)
    //                 DontDestroyOnLoad(_instance.gameObject);
    //         }
    //         return _instance;
    //     }
    // }
    public static bool musicTurn
    {
        get
        {
            return PlayerPrefs.GetInt($"musicTurn", 0) == 0;
        }
        set
        {
            PlayerPrefs.SetInt($"musicTurn", value ? 0 : 1);
            PlayerPrefs.Save();
        }
    }
    public static bool vibrate
    {
        get
        {
            return PlayerPrefs.GetInt($"Vibrate", 0) == 0;
        }
        set
        {
            PlayerPrefs.SetInt($"Vibrate", value ? 0 : 1);
            PlayerPrefs.Save();
        }
    }
    public static bool soundTurn
    {
        get
        {
            return PlayerPrefs.GetInt($"soundTurn", 0) == 0;
        }
        set
        {
            PlayerPrefs.SetInt($"soundTurn", value ? 0 : 1);
            PlayerPrefs.Save();
        }
    }
    public float MasterVolume
    {
        get
        {
            return PlayerPrefs.GetFloat(MASTER_KEY, 0f);
        }
        set
        {
            SetMasterVolume(value);
        }
    }
    public float MusicVolume
    {
        get
        {
            return PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        }
        set
        {
            SetMusicVolume(value);
        }
    }
    public float SoundVolume
    {
        get
        {
            return PlayerPrefs.GetFloat(SOUND_KEY, 1f);
        }
        set
        {
            SetSoundVolume(value);
        }
    }
    public void TurnSoundVolune(bool on)
    {
        soundTurn = on;
        foreach (var item in effectSource)
        {
            item.mute = !on;
        }
    }
    public void TurnMusicVolune(bool on)
    {
        musicTurn = on;
        musicSource.mute = !on;
        if (on) PlayBGMusic();
    }
    //public bool Vibrate
    //{
    //    get => PlayerPrefs.GetInt("Vibrate", 1) == 1;
    //    set => PlayerPrefs.SetInt("Vibrate", value ? 1 : 0);
    //}
    private void Awake()
    {
        // if (_instance == null)
        // {
        //     //If I am the first instance, make me the Singleton
        //     _instance = this;
        //     DontDestroyOnLoad(this);
        // }
        // else
        // {
        //     //If a Singleton already exists and you find
        //     //another reference in scene, destroy it!
        //     if (this != _instance)
        //     {
        //         Destroy(gameObject);
        //     }
        // }
    }
    private void Start()
    {
        if (musicTurn) PlayBGMusic();
    }
    AudioSource GetSourceCanUse()
    {
        foreach (var item in effectSource)
        {
            if (!item.isPlaying) return item;
        }
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = soundSource.volume;
        effectSource.Add(audioSource);
        return audioSource;
    }
    //Used to play single sound clips.
    public void PlaySingle(AudioClip clip, SourceAudio source = SourceAudio.Sound)
    {
        if (clip == null)
            return;
        switch (source)
        {
            case SourceAudio.Music:
                if (!musicTurn) return;

                musicSource.clip = clip;
                musicSource.Play();
                break;
            case SourceAudio.Effect:
                if (!soundTurn) return;

                AudioSource audioSource = GetSourceCanUse();
                audioSource.clip = clip;
                audioSource.Play();
                break;
            case SourceAudio.Sound:

                if (!soundTurn) return;
                AudioSource sound = GetSourceCanUse();
                sound.clip = clip;
                sound.Play();
                break;
        }

    }

    public void PauseBGMusic()
    {
        musicSource.Pause();
    }

    public void ResumeBGMusic()
    {
        musicSource.UnPause();
    }

    //Used to play single sound clips.
    public void PlayOneShot(AudioClip clip, SourceAudio source = SourceAudio.Effect)
    {
        if (clip == null)
            return;
        switch (source)
        {
            case SourceAudio.Music:
                if (!musicTurn) return;
                musicSource.PlayOneShot(clip);
                break;
            case SourceAudio.Effect:
                if (!soundTurn) return;
                GetSourceCanUse().PlayOneShot(clip);
                break;
            case SourceAudio.Sound:
                if (!soundTurn) return;
                soundSource.PlayOneShot(clip);
                break;
        }
    }

    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    public void RandomizeSfx(params AudioClip[] clips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        // effectSource.pitch = randomPitch;

        // //Set the clip to the clip at our randomly chosen index.
        // effectSource.clip = clips[randomIndex];

        // //Play the clip.
        // effectSource.Play();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || _currentMusic == clip)
            return;
        SetMasterVolume(MasterVolume);
        SetMusicVolume(MusicVolume);
        SetSoundVolume(SoundVolume);
        _currentMusic = clip;
        StopMusic();
        musicSource.clip = clip;
        musicSource.PlayDelayed(delayTime);
    }
    public void RandomizeMusic(params AudioClip[] clips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);
        var clip = clips[randomIndex];
        if (clip == null || _currentMusic == clip)
            return;
        _currentMusic = clip;
        StopMusic();
        //Set the clip to the clip at our randomly chosen index.
        musicSource.clip = clips[randomIndex];

        //Play the clip.
        musicSource.PlayDelayed(delayTime);
    }

    public void PauseMusic()
    {
        //Play the clip.
        musicSource.Pause();
    }
    public void UnPauseMusic()
    {
        //Play the clip.
        musicSource.UnPause();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    public bool IsMusicPlaying()
    {
        return musicSource.isPlaying;
    }


    private void SetMasterVolume(float volume)
    {
        //mixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat(MASTER_KEY, volume);
    }
    private void SetMusicVolume(float volume)
    {
        //mixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat(MUSIC_KEY, volume);
    }
    private void SetSoundVolume(float volume)
    {
        //mixer.SetFloat("SoundVolume", volume);
        PlayerPrefs.SetFloat(SOUND_KEY, volume);
    }

    #region === Play Sound ===
    public void PlayBGMusic()
    {
        PlaySingle(BGMusic, SourceAudio.Music);
    }

    public void PlayCombatMusic()
    {
        PlaySingle(InGameMusic, SourceAudio.Music);
    }
    #endregion
}
