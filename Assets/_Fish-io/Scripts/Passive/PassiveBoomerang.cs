using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveBoomerang : PassiveBase
{
    public float range = 6f;
    private void Start()
    {
        passiveType = PassiveType.Boomerang;
        LoadData();
    }
    public override void OnPassiveActive()
    {
        base.OnPassiveActive();
        Boomerang boomerang = CreateManager.Instance.CreateBoomerang(transform.position);
        boomerang.InitSpawn(transform.right, this);
    }
}
