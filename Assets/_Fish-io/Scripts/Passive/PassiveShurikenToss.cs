using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveShurikenToss : PassiveBase
{
    [SerializeField] FishSkin fishSkin;
    private void Start()
    {
        fishSkin.Fish.actionCompleteDash += OnCreateShuriken;
        passiveType = PassiveType.IonShell;
        LoadData();
    }
    public override void OnPassiveActive()
    {
        base.OnPassiveActive();
    }
    public void OnCreateShuriken()
    {
        CreateManager.Instance.CreateDarts(transform.position).InitSpawn(transform.right, fishSkin);
    }
}
