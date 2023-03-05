using UnityEngine;
public class PassiveImageBorn : PassiveBase
{
    [SerializeField] FishSkin fishSkin;
    ImageBornFish bornFish;
    private void Start()
    {
        passiveType = PassiveType.ImageBorn;
        LoadData();
       
    }
    
    public override void OnPassiveActive()
    {
        if (bornFish == null)
        {
            base.OnPassiveActive();
            bornFish = CreateManager.Instance.CreateImageBorn(fishSkin.Fish.Type, fishSkin.Fish.imageBornParent).GetComponent<ImageBornFish>();
            bornFish.Init(fishSkin.Fish, fishSkin.Fish.Skin.sword.weaponType, duration);
            
            //bornFish.transform.rotation = Quaternion.identity;
        }    
    }
}
