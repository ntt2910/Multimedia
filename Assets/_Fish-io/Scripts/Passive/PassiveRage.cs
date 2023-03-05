using UnityEngine;

public class PassiveRage : PassiveBase
{ 
    [SerializeField] private FishSkin fishSkin;
    private void Start()
    {
        passiveType = PassiveType.Rage;
        LoadData();
        fishSkin.Fish.dashSpeedConfig *= 1.5f;
    }
}
