using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TransitionPopup : MonoBehaviour
{
    [SerializeField]
    private Image image;
    bool firtShow = true;

    private float duration = 0.25f;

    public TransitionPopup SetDuration(float value)
    {
        duration = value;
        return this;
    }

    public void Show(Action onComplete)
    {
        var color = image.color;
        color.a = 0;
        image.color = color;
        image.DOFade(1, duration).OnComplete(() =>  onComplete?.Invoke());
        
        gameObject.SetActive(true);
    }

    public void Hide(Action onComplete)
    {
        var color = image.color;
        color.a = 1;
        image.color = color;
        image.DOFade(0, duration).OnComplete(() =>
        {
            onComplete?.Invoke();
            if (firtShow)
            {
                firtShow = false;
                image.enabled = false;
            }
        });
    }

    public void StartTransition(float duration,Action onCenter,Action onComplete)
    {
        SetDuration(duration);
        Show(delegate
        {
            onCenter?.Invoke();
            Hide(onComplete);
        });
    }
}
