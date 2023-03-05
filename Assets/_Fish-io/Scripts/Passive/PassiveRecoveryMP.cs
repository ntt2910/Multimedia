using UnityEngine;
public class PassiveRecoveryMP : PassiveBase
{
    [SerializeField] FishSkin fish;
    private void Start()
    {
        passiveType = PassiveType.RecoveryMP;
        LoadData();
    }
    public override void OnPassiveActive()
    {
        base.OnPassiveActive();
        fish.RecoveryMana(GameController.instance.fishDatabase.recoveryManaMeatConfig *2);
        GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.WhalePassive);
    }
}
