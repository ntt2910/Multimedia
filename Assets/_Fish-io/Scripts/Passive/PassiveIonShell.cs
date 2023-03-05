using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PassiveIonShell : PassiveBase
{
    [SerializeField] FishSkin fishSkin;
    ParticleSystem immortalEff;
    private void Start()
    {
        passiveType = PassiveType.IonShell;
        LoadData();
    }
    public override void OnPassiveActive()
    {
        base.OnPassiveActive();
        if (fishSkin != null)
        {
            fishSkin.Fish.Immortal(duration);
            if (immortalEff == null) immortalEff = CreateManager.Instance.CreateImmortalEff(fishSkin.shieldPos);

            immortalEff.gameObject.SetActive(true);
            immortalEff.Play();
            GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.ProtectorFishPassive);
            Observable.Timer(TimeSpan.FromSeconds(2f), Scheduler.MainThreadIgnoreTimeScale).Subscribe(_ =>
            {
                immortalEff.gameObject.SetActive(false);
            }).AddTo(this);
        }
    }
    //public void Update()
    //{
    //    if(fishSkin != null)
    //    {
    //        if (fishSkin.Fish != null)
    //        {
    //            if (!fishSkin.Fish.AllowTakeDamage)
    //            {
    //                if (immortalEff)
    //                {
    //                    immortalEff.transform.position = fishSkin.transform.position;
    //                }
    //            }
    //            else
    //            {
    //                if (immortalEff)
    //                {
    //                    immortalEff.gameObject.SetActive(false);
    //                }
    //            }
    //        }
    //    }    
        
    //}
    private void OnDestroy()
    {
        if (immortalEff != null)
        {
            Destroy(immortalEff);
        }
    }
}
