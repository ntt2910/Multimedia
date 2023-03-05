using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] Slider progressBar;
    //private TweenerCore<float, float, FloatOptions> _tween;
    [SerializeField] private Text txtLoading, txtVersion;

    void Start()
    {
        UnityAction actionComplete =
            () =>
            {
                if(!DataManager.tutorial) SceneManager.LoadSceneAsync("Game");
                else SceneManager.LoadSceneAsync("Tutorial");
            };

        float value = 0;
        DOTween.To(() => value, x => value = x, 1, 2.5f)
            .OnUpdate(() => { fill.fillAmount = value; })
            .OnComplete(() =>
            {
                actionComplete.Invoke();
                fill.fillAmount = 1;
            }).SetUpdate(true);
    }
}
