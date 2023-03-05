using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] Image fade;
    [SerializeField] GameObject[] chatTutorials;
    int index = 0;
    [SerializeField] Joystick joystick;
    [SerializeField] ButtonDash buttonDash;
    // Update is called once per frame
    float timePressed1 = 0.25f, timePressed2 = 0.1f;
    float count = 0;
    bool loadGamePlay = false;
    void Update()
    {
        index = Mathf.Clamp(index, 0, chatTutorials.Length - 1);
        switch (index)
        {
            case 0:
                if (!chatTutorials[index].gameObject.activeInHierarchy) chatTutorials[index].gameObject.SetActive(true);
                float axis = new Vector2(joystick.Horizontal, joystick.Vertical).magnitude;
                if(axis != 0)
                {
                    count += Time.deltaTime;
                    if (count >= 1.5f)
                    {
                        chatTutorials[index].gameObject.SetActive(false);
                        index++;
                        count = 0;
                    }
                }    
                if(Input.GetMouseButtonUp(0))
                {
                    if(count < timePressed1)
                    {
                        count = 0;
                    }    
                    else
                    {
                        chatTutorials[index].gameObject.SetActive(false);
                        index++;
                        count = 0;
                    }    
                }    
                break;
            case 1:
                Observable.Timer(TimeSpan.FromSeconds(1.5f), Scheduler.MainThreadIgnoreTimeScale).Subscribe(_ =>
                {
                    if (!buttonDash.gameObject.activeInHierarchy) buttonDash.gameObject.SetActive(true);
                    if (!chatTutorials[index].gameObject.activeInHierarchy) chatTutorials[index].gameObject.SetActive(true);

                    if (buttonDash.buttonPressed)
                    {
                        count += Time.deltaTime;
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (count < timePressed2)
                        {
                            count = 0;
                        }
                        else
                        {
                            chatTutorials[index].gameObject.SetActive(false);
                            index++;
                            count = 0;
                        }
                    }
                }).AddTo(this);
                break;
            case 2:
                Observable.Timer(TimeSpan.FromSeconds(1.5f), Scheduler.MainThreadIgnoreTimeScale).Subscribe(_ =>
                {
                    chatTutorials[index].gameObject.SetActive(true);
                    DataManager.tutorial = false;
                    fade.DOFade(1, 1f);
                    Observable.Timer(TimeSpan.FromSeconds(3f), Scheduler.MainThreadIgnoreTimeScale).Subscribe(_ =>
                    {
                        if (!loadGamePlay)
                        {
                            SceneManager.LoadScene("Game");
                            loadGamePlay = true;
                        }
                    }).AddTo(this);
                }).AddTo(this);
                break;
            default:
                break;
        }
    }
}
