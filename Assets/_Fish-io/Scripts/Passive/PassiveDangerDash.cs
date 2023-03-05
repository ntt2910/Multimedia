using UnityEngine;
public class PassiveDangerDash : PassiveBase
{
    [SerializeField] FishSkin fishSkin;
    private void Start()
    {
        passiveType = PassiveType.DangerDash;
        LoadData();
    }
    
    void Update()
    {
        if(fishSkin.Fish.IsDash)
        {
            fishSkin.Fish.IsImortal = true;
        }
        else
        {
            fishSkin.Fish.IsImortal = false;
        }
    }
}
