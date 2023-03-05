using LTABase.DesignPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveCheater : PassiveBase
{
    [SerializeField] Fish fish;
    // Start is called before the first frame update
    void Start()
    {
        passiveType = PassiveType.Cheater;
        LoadData();
        fish.UpgradeFish();
        fish.UpgradeFish();
        Observer.Instance.AddObserver(ObserverName.ON_START_PLAY, OnStartPlay);
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverName.ON_START_PLAY, OnStartPlay);
    }
    void OnStartPlay(object data)
    {
        
    }
}
