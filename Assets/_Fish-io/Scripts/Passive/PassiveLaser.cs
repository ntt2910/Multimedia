using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveLaser : PassiveBase
{
    [SerializeField] FishSkin fish;
    [SerializeField] Transform shootPoint;
    private void Start()
    {
        passiveType = PassiveType.Laser;
        LoadData();
    }
    public override void OnPassiveActive()
    {
        base.OnPassiveActive();
        CreateManager.Instance.CreateLaser(transform.position).OnSpawn(shootPoint.position , fish.Fish.Dir.right * 100f, duration, gameObject);

    }
}
