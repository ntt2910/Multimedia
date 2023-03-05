using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class FishSkin : MonoBehaviour, ICanTakeDamage
{
    public FishType fishType;
    public Transform shieldPos;
    [SerializeField] Fish fish;
    public Weapon sword;
    public PassiveBase passive;
    public int recoveryManaConfig = 1;
    public int comboCount = 0;
    bool isCombo = false;
    public IDisposable coroutine;
    public Fish Fish => fish;

    private void Start()
    {
        Fish.shieldPos = shieldPos;
    }

    public void TakeDamage(int damage)
    {
        fish.TakeDamage(damage);
    }
    public void UpgradeSword(int level)
    {
        sword.LevelUp(level);
    }

    private void ShowComboKill(GameObject obj)
    {
        obj.SetActive(true);
        obj.transform.DOScale(0, 0);
        obj.transform.DOScale(1.4f, 0.3f).OnComplete(() => obj.transform.DOScale(1, 0.15f));
        DOVirtual.DelayedCall(3f, delegate
        {
            if (obj.activeSelf)
            {
                obj.SetActive(false);
            }
        });
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        if (other.CompareTag("Weapon") && other.transform != sword.transform.parent)
        {
            var skinFish = other.GetComponentInParent<FishSkin>();
            if (skinFish != null)
            {

                if (skinFish.fish.IsBlur)
                {
                    RaycastHit[] hits;
                    hits = Physics.RaycastAll(transform.position, fish.Dir.right, 100f);


                    for (int i = 0; i < hits.Length; i++)
                    {
                        var fish = hits[i].transform.GetComponentInParent<FishSkin>();
                        if (fish != null)
                        {
                            if (fish.transform == skinFish.transform)
                                return;
                            //skinFish.fish.UpgradeFish();
                            //CreateManager.Instance.CreateMeat(skinFish.fish.levelSize, transform.position);
                            //if (skinFish.passive) skinFish.passive.OnUpdateKill(1);
                            //skinFish.UpgradeSword();

                        }
                    }
                    TakeDamage(1);

                    if (!fish.IsDead) return;
                    skinFish.comboCount++;
                    if (skinFish.fish.GetComponent<PlayerController>())
                    {
                        switch (skinFish.comboCount)
                        {
                            case 0:

                                break;
                            case 1:

                                break;
                            case 2:
                                //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.DoubleKill);
                                ShowComboKill(UIManager.Instance.comboKill[0]);
                                break;
                            case 3:
                                //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.TrippleKill);
                                UIManager.Instance.comboKill[0].gameObject.SetActive(false);
                                ShowComboKill(UIManager.Instance.comboKill[1]);
                                break;
                            case 4:
                                //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.UltraKill);
                                UIManager.Instance.comboKill[1].gameObject.SetActive(false);
                                ShowComboKill(UIManager.Instance.comboKill[2]);
                                break;
                            case 5:
                                //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.Rampage);
                                UIManager.Instance.comboKill[2].gameObject.SetActive(false);
                                ShowComboKill(UIManager.Instance.comboKill[3]);
                                break;
                        }

                        if (skinFish.comboCount >= 6)
                        {
                            UIManager.Instance.comboKill[3].gameObject.SetActive(false);
                            ShowComboKill(UIManager.Instance.comboKill[4]);
                            //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.Rampage);
                        }
                    }
                    CreateManager.Instance.CreateHitFx(Fish.transform.position);
                    skinFish.OnKillOtherFish();
                    CreateManager.Instance.CreateMeat(fish.levelSize, transform.position);
                    skinFish.fish.ChangeKillAmount(1);
                    
                    int newPoint = GameController.instance.fishDatabase.pointKillConfig;
                    skinFish.fish.ChangePoint(newPoint);
                    if (skinFish.coroutine != null) skinFish.coroutine.Dispose();
                    skinFish.coroutine = skinFish.ComboDelay();
                    if (skinFish.fish.GetComponent<PlayerController>() != null) SlowmotionManager.Instance.DoSlowmotion();
                }
                else
                {
                    TakeDamage(1);
                    if (!fish.IsDead) return;
                    skinFish.comboCount++;
                    if (skinFish.fish.GetComponent<PlayerController>())
                    {
                        switch (skinFish.comboCount)
                        {
                            case 0:

                                break;
                            case 1:

                                break;
                            case 2:
                                //GameController.instance.musicManager.PlaySingle(GameController.instance.soundData.DoubleKill);
                                ShowComboKill(UIManager.Instance.comboKill[0]);
                                break;
                            case 3:
                                //GameController.instance.musicManager.PlaySingle(GameController.instance.soundData.TrippleKill);
                                UIManager.Instance.comboKill[0].gameObject.SetActive(false);
                                ShowComboKill(UIManager.Instance.comboKill[1]);
                                break;
                            case 4:
                                //GameController.instance.musicManager.PlaySingle(GameController.instance.soundData.UltraKill);
                                UIManager.Instance.comboKill[1].gameObject.SetActive(false);
                                ShowComboKill(UIManager.Instance.comboKill[2]);
                                break;
                            case 5:
                                //GameController.instance.musicManager.PlaySingle(GameController.instance.soundData.Rampage);
                                UIManager.Instance.comboKill[2].gameObject.SetActive(false);
                                ShowComboKill(UIManager.Instance.comboKill[3]);
                                break;
                            
                        }

                        if (skinFish.comboCount >= 6)
                        {
                            UIManager.Instance.comboKill[3].gameObject.SetActive(false);
                            ShowComboKill(UIManager.Instance.comboKill[4]);
                            //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.Rampage);
                        }
                    }
                    CreateManager.Instance.CreateHitFx(Fish.transform.position);
                    skinFish.OnKillOtherFish();
                    CreateManager.Instance.CreateMeat(fish.levelSize, transform.position);
                    skinFish.fish.ChangeKillAmount(1);
                    
                    int newPoint = GameController.instance.fishDatabase.pointKillConfig;
                    skinFish.fish.ChangePoint(newPoint);
                    if (skinFish.coroutine != null) skinFish.coroutine.Dispose();
                    skinFish.coroutine = skinFish.ComboDelay();
                    if (skinFish.fish.GetComponent<PlayerController>() != null) SlowmotionManager.Instance.DoSlowmotion();
                }

                //skinFish.fish.UpgradeFish();
                //CreateManager.Instance.CreateMeat(skinFish.fish.levelSize, transform.position);
                //if (skinFish.passive) skinFish.passive.OnUpdateKill(1);
                //skinFish.UpgradeSword();
            }
        }
        if (other.CompareTag("ImageBorn"))
        {
            var skinFish = other.GetComponentInParent<ImageBornFish>();

            if (skinFish != null)
            {
                if (skinFish.owner.transform == fish.transform) return;
                if (fish.IsBlur)
                {
                    RaycastHit[] hits;
                    hits = Physics.RaycastAll(transform.position, fish.Dir.right, 100f);


                    for (int i = 0; i < hits.Length; i++)
                    {
                        var fish = hits[i].transform.GetComponentInParent<FishSkin>();
                        if (fish != null)
                        {
                            if (fish.transform == skinFish.transform)
                                return;
                            //skinFish.fish.UpgradeFish();
                            //CreateManager.Instance.CreateMeat(skinFish.fish.levelSize, transform.position);
                            //if (skinFish.passive) skinFish.passive.OnUpdateKill(1);
                            //skinFish.UpgradeSword();

                        }
                    }
                    TakeDamage(1);
                    if (!fish.IsDead) return;
                    skinFish.owner.Skin.comboCount++;
                    if (skinFish.owner.GetComponent<PlayerController>())
                    {
                        switch (skinFish.owner.Skin.comboCount)
                        {
                            case 0:

                                break;
                            case 1:

                                break;
                            case 2:
                                //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.DoubleKill);
                                ShowComboKill(UIManager.Instance.comboKill[0]);
                                break;
                            case 3:
                                //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.TrippleKill);
                                UIManager.Instance.comboKill[0].gameObject.SetActive(false);
                                ShowComboKill(UIManager.Instance.comboKill[1]);
                                break;
                            case 4:
                                //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.UltraKill);
                                UIManager.Instance.comboKill[1].gameObject.SetActive(false);
                                ShowComboKill(UIManager.Instance.comboKill[2]);
                                break;
                            case 5:
                                //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.Rampage);
                                UIManager.Instance.comboKill[2].gameObject.SetActive(false);
                                ShowComboKill(UIManager.Instance.comboKill[3]);
                                break;
                            
                        }

                        if (skinFish.owner.Skin.comboCount >= 6)
                        {
                            UIManager.Instance.comboKill[3].gameObject.SetActive(false);
                            ShowComboKill(UIManager.Instance.comboKill[4]);
                            //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.Rampage);
                        }
                    }
                    CreateManager.Instance.CreateHitFx(Fish.transform.position);
                    skinFish.owner.Skin.OnKillOtherFish();
                    CreateManager.Instance.CreateMeat(fish.levelSize, transform.position);
                    
                    int newPoint = GameController.instance.fishDatabase.pointKillConfig;
                    skinFish.owner.ChangePoint(newPoint);
                    skinFish.owner.ChangeKillAmount(1);
                    if (skinFish.owner.Skin.coroutine != null) skinFish.owner.Skin.coroutine.Dispose();
                    skinFish.owner.Skin.coroutine = skinFish.owner.Skin.ComboDelay();
                }
                else
                {
                    TakeDamage(1);
                    if (!fish.IsDead) return;
                    skinFish.owner.Skin.comboCount++;
                    if (skinFish.owner.GetComponent<PlayerController>())
                    {
                        switch (skinFish.owner.Skin.comboCount)
                        {
                            case 0:

                                break;
                            case 1:

                                break;
                            case 2:
                                //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.DoubleKill);
                                ShowComboKill(UIManager.Instance.comboKill[0]);
                                break;
                            case 3:
                                //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.TrippleKill);
                                UIManager.Instance.comboKill[0].gameObject.SetActive(false);
                                ShowComboKill(UIManager.Instance.comboKill[1]);
                                break;
                            case 4:
                                //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.UltraKill);
                                UIManager.Instance.comboKill[1].gameObject.SetActive(false);
                                ShowComboKill(UIManager.Instance.comboKill[2]);
                                break;
                            case 5:
                                //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.Rampage);
                                UIManager.Instance.comboKill[2].gameObject.SetActive(false);
                                ShowComboKill(UIManager.Instance.comboKill[3]);
                                break;
                            
                        }

                        if (skinFish.owner.Skin.comboCount >= 6)
                        {
                            UIManager.Instance.comboKill[3].gameObject.SetActive(false);
                            ShowComboKill(UIManager.Instance.comboKill[4]);
                            //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.Rampage);
                        }
                    }
                    CreateManager.Instance.CreateHitFx(Fish.transform.position);
                    skinFish.owner.Skin.OnKillOtherFish();
                    CreateManager.Instance.CreateMeat(fish.levelSize, transform.position);
                    
                    int newPoint = GameController.instance.fishDatabase.pointKillConfig;
                    skinFish.owner.ChangePoint(newPoint);
                    skinFish.owner.ChangeKillAmount(1);
                    if (skinFish.owner.Skin.coroutine != null) skinFish.owner.Skin.coroutine.Dispose();
                    skinFish.owner.Skin.coroutine = skinFish.owner.Skin.ComboDelay();
                }

                //skinFish.fish.UpgradeFish();
                //CreateManager.Instance.CreateMeat(skinFish.fish.levelSize, transform.position);
                //if (skinFish.passive) skinFish.passive.OnUpdateKill(1);
                //skinFish.UpgradeSword();
            }
        }
        if (other.CompareTag("Meat"))
        {
            if(fish.GetComponent<PlayerController>())  GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.EatSound);
            RecoveryMana(GameController.instance.fishDatabase.recoveryManaMeatConfig);
            fish.ChangePoint(GameController.instance.fishDatabase.pointMeatConfig);
            Destroy(other.gameObject);
            if (passive) passive.OnUpdateEat(1);
            LTABase.DesignPattern.Observer.Instance.Notify(ObserverName.ON_FISH_CHANGE_POINT);
        }
        if (other.CompareTag("GreenPearl"))
        {
            RecoveryMana(GameController.instance.fishDatabase.recoveryManaMeatConfig);
            Destroy(other.gameObject);
            LTABase.DesignPattern.Observer.Instance.Notify(ObserverName.ON_PLAYER_PICK_UP_GREEN_PEARL);
        }
    }
    public void OnKillOtherFish()
    {
        fish.UpgradeFish();
        
        if (passive) passive.OnUpdateKill(1);
        //skinFish.UpgradeSword();
    }
    public void RecoveryMana(int recoveryMana)
    {
        CreateManager.Instance.CreateRecoveryMana(transform.parent);
        if (fish.power + recoveryMana > fish.maxPower)
        {
            fish.power = fish.maxPower;
        }
        else
        {
            fish.power += recoveryMana * recoveryManaConfig;
        }
    }
    public void SetRecoveryManaConfig(int _value)
    {
        recoveryManaConfig = _value;
    }
    public IDisposable ComboDelay()
    {
        isCombo = true;
        
        var delay = Observable.Timer(TimeSpan.FromSeconds(5), Scheduler.MainThreadIgnoreTimeScale).Subscribe(_ =>
        {
            isCombo = false;
            comboCount = 0;

        }).AddTo(this);
        return delay;
    }
    public void SetFish(Fish _fish)
    {
        fish = _fish;
    }
}
