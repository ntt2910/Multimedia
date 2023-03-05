using UnityEngine;
public class PassiveSurge : PassiveBase
{
    [SerializeField] FishSkin fishSkin;
    private void Start()
    {
        passiveType = PassiveType.Surge;
        LoadData();
    }
    public override void OnPassiveActive()
    {
        base.OnPassiveActive();
        GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.LionFishSurgePassive);
        fishSkin.Fish.Surge(duration);
    }
}
