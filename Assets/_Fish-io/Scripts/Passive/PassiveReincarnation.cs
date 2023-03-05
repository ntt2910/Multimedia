using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PassiveReincarnation : PassiveBase
{
    [SerializeField] FishSkin fishSkin;
    private void Start()
    {
        passiveType = PassiveType.Reincarnation;
        LoadData();
        if(fishSkin.Fish != null) fishSkin.Fish.SetReincarnation(true);
    }
    
}
