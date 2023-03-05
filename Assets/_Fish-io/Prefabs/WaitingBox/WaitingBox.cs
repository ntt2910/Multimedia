using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WaitingBox : BaseBox
{
    private static GameObject instance;
    public UnityAction moreActionOff;
    private UnityAction actionCloseButton;
    public UnityAction actionHide;
    //[SerializeField] Text msgText;
    private IDisposable _waitDispose;
    
    
    public static WaitingBox Setup()
    {
        if (instance == null)
        {
            // Create popup and attach it to UI
            instance = Instantiate(Resources.Load(PathPrefabs.WAITING_BOX) as GameObject);
        }

        instance.SetActive(true);
        return instance.GetComponent<WaitingBox>();
    }
    
    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void ActionDoOff()
    {
        base.ActionDoOff();
        if (moreActionOff != null)
            moreActionOff();
    }

    private void Start()
    {
       // msgText.text = Localization.Get("LOADING");
    }

    private void OnDestroy()
    {
        if (_waitDispose != null)
            _waitDispose.Dispose();
    }

    public void ShowWaiting()
    {
        Debug.LogError("===========================Show Waiting==========================");
        gameObject.SetActive(true);
        GetComponent<RectTransform>().rect.Set(0, 0, 0, 0);
        //msgText.DOKill();
        
        TimeOut(20);
    }


    public void HideWaiting()
    {
        Debug.LogError("===========================Hide Waiting==========================");
        gameObject.SetActive(false);
        if (_waitDispose != null)
            _waitDispose.Dispose();
    }

    public void ShowWaiting(float time)
    {
        // Show va Hide, ko lam gi ca
        ShowWaiting();
        TimeOut(time);
    }

    public void ShowWaiting(float time, Action action)
    {
        // Show va Hide, ko lam gi ca
        ShowWaiting();
        Debug.Log("stop all waitingbox");
        TimeOut(time);
    }

    private void TimeOut(float time)
    {
        if (_waitDispose != null)
            _waitDispose.Dispose();
        _waitDispose = Observable.Timer(TimeSpan.FromSeconds(time), Scheduler.MainThreadIgnoreTimeScale)
            .Subscribe(_ =>
            {
                Debug.Log("TimeOut");
                HideWaiting();
            });
    }
}
