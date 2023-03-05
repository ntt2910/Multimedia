using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveBlur : PassiveBase
{
    [SerializeField] FishSkin fishSkin;
    private void Start()
    {
        passiveType = PassiveType.Blur;
        LoadData();
        fishSkin.Fish.SetBlur(true);
    }
}
