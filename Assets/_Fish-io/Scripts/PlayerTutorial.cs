using LTABase.DesignPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTutorial : Fish
{
    [SerializeField] Slider powerSlide;
    [SerializeField] ButtonDash buttonDash;
    [SerializeField] GameObject graphicParent, preview, previewParent;

    public static float limit_left, limit_right, limit_top, limit_bot;
    public bool isPlay = false;

    protected override void Start()
    {
        //base.Start();
        nameText.text = DataManager.playerName;
        fishName = DataManager.playerName;
        Observer.Instance.AddObserver(ObserverName.ON_START_PLAY, OnStartPlay);
        Observer.Instance.AddObserver(ObserverName.ON_CHANGE_NAME, OnChangeName);
        Observer.Instance.AddObserver(ObserverName.ON_PLAYER_REVIVE, Revive);
        Observer.Instance.AddObserver(ObserverName.ON_CLICK_UNLOCK_FISH, ChangeSkin);
        //LoadData();
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverName.ON_START_PLAY, OnStartPlay);
        Observer.Instance.RemoveObserver(ObserverName.ON_CHANGE_NAME, OnChangeName);
        Observer.Instance.RemoveObserver(ObserverName.ON_PLAYER_REVIVE, Revive);
        Observer.Instance.RemoveObserver(ObserverName.ON_CLICK_UNLOCK_FISH, ChangeSkin);
    }
    public void LoadData()
    {
        var _fishSkin = GameController.instance.fishDatabase.GetFishOObjectData(DataManager.currentSkinFish);
        if (_fishSkin != null)
        {
            if (fishSkin != null) fishSkin.gameObject.SetActive(false);
            fishSkin = Instantiate(_fishSkin.prefab, graphicParent.transform).GetComponent<FishSkin>();
            fishSkin.SetFish(this);
            animator = fishSkin.GetComponent<Animator>();
        }
        if (preview != null) Destroy(preview.gameObject);
        preview = Instantiate(_fishSkin.prefab, previewParent.transform);
        var previewFish = preview.GetComponent<FishSkin>();
        //preview.GetComponent<FishSkin>().enabled = false;
        Destroy(preview.GetComponent<Rigidbody>());
        Destroy(preview.GetComponent<Collider>());


        var _weapon = GameController.instance.weaponDatabase.GetWeaponObjectData(DataManager.currentWeapon);
        if (_weapon != null)
        {
            if (fishSkin.sword != null)
            {

                Weapon sword = Instantiate(_weapon.weaponPrefabs, fishSkin.sword.transform.parent).GetComponent<Weapon>();
                sword.transform.localPosition = Vector3.zero;
                if (fishSkin.sword != null)
                {
                    Destroy(fishSkin.sword.gameObject);
                }
                fishSkin.sword = sword;

            }

            if (previewFish.sword != null)
            {
                Weapon sword = Instantiate(_weapon.weaponPrefabs, previewFish.sword.transform.parent).GetComponent<Weapon>();
                Destroy(previewFish.sword.gameObject);
                previewFish.sword = sword;
            }
        }
        Transform[] chids = preview.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < chids.Length; i++)
        {
            chids[i].gameObject.layer = LayerMask.NameToLayer("FishPreview");
        }

        var fishData = GameController.instance.fishDatabase.GetFishOObjectData(fishSkin.fishType);
        speed = fishData.moveSpeed;
    }
    public override void UpgradeFish()
    {
        exp++;
        if (levelSize >= 3 && exp >= expMax) return;
        transform.localScale = initScale * 1.05f;
        GameObject gob = CreateManager.Instance.CreateFishEXP(fishSkin.sword.GetHeadPoint(levelSize));
        exps.Add(gob);
        //SortExp();
        if (exp >= expMax)
        {

            exp = 0;
            //expMax += 2;
            levelSize++;

            for (int i = 0; i < exps.Count; i++)
            {
                exps[i].gameObject.SetActive(false);
            }
            exps.Clear();
            fishSkin.UpgradeSword(levelSize);
            transform.localScale = initScale * 1.5f;
            initScale = transform.localScale;
            expMax = GameController.instance.fishDatabase.GetFishDataLevel(levelSize).conditionLevel;
            Observer.Instance.Notify(ObserverName.ON_PLAYER_LEVEL_UP);
        }
    }
    protected override void Update()
    {
        //if (!isPlay) return;
        Vector3 axis = new Vector3(InputController.Instance.Hor, InputController.Instance.Ver, 0);
        //Debug.Log(axis);
        Rotate(axis);
        base.Update();

        //powerSlide.value = (float)(power / maxPower);
        //if (Input.GetKeyDown(KeyCode.Space)) IncreaseSpeed();
        if (buttonDash.buttonPressed) IncreaseSpeed();

    }
    //public override void IncreaseSpeed()
    //{
    //    if (isSurge)
    //    {
    //        if (!isCreaseSpeed)
    //        {
    //            speed *= 3f;
    //            isCreaseSpeed = true;
    //            StartCoroutine(IncreaseSpeedCooldown());
    //            //animator.SetTrigger("Dash");
    //        }
    //    }
    //    else
    //    {
    //        if (!isCreaseSpeed && power >= 20)
    //        {
    //            power -= 20;
    //            speed *= 3f;
    //            isCreaseSpeed = true;
    //            StartCoroutine(IncreaseSpeedCooldown());
    //            //animator.SetTrigger("Dash");
    //        }
    //    }
    //}
    //protected override IEnumerator IncreaseSpeedCooldown()
    //{
    //    yield return new WaitForSeconds(increaseSpeedCooldown);
    //    isCreaseSpeed = false;
    //    speed /= 3f;
    //}
    public override void Attack()
    {

    }

    public override void Die()
    {
        isPlay = false;

        if (isReincarnation)
        {
            crowns.gameObject.SetActive(false);
            CreateManager.Instance.CreateRelifeFX(transform.position);
            fishSkin.gameObject.SetActive(false);
            isReincarnation = false;
            isRecive = true;
            DelayAction(3f, () =>
            {
                isRecive = false;
                isPlay = true;
                Immortal(2f);
                crowns.gameObject.SetActive(true);
                fishSkin.gameObject.SetActive(true);
            });
        }
        else
        {
            if (!isDead)
            {
                fishSkin.sword.SetActiveCollider(false);
                fishSkin.gameObject.SetActive(false);
                Observer.Instance.Notify(ObserverName.ON_PLAYER_DIE);
                isDead = true;
                crowns.gameObject.SetActive(false);
            }

        }
    }
    Vector3 moveDir;
    public override void Rotate(Vector3 dir)
    {
        if (dir.magnitude != 0) dirTran.right = new Vector3(dir.x, dir.y, 0);
        if (dir.magnitude != 0)
        {
            fishSkin.transform.parent.rotation = Quaternion.Slerp(fishSkin.transform.parent.rotation, Quaternion.LookRotation(new Vector3(-dir.x, -dir.y, 0)), Time.deltaTime * 10);
            imageBornParent.rotation = Quaternion.Slerp(imageBornParent.rotation, Quaternion.LookRotation(new Vector3(-dir.x, -dir.y, 0)), Time.deltaTime * 10);
        }
    }
    public override void TakeDamage(int damage)
    {
        if (!allowTakeDamage || IsImortal) return;

        SlowmotionManager.Instance.DoSlowmotion();
        Die();
    }

    //*Observer*//
    void OnStartPlay(object data)
    {
        nameText.gameObject.SetActive(true);
        fishSkin.gameObject.SetActive(true);
        isPlay = true;
        DelayAction(2f, () => { allowTakeDamage = true; });
    }
    void OnChangeName(object data)
    {
        fishName = DataManager.playerName;
        nameText.text = DataManager.playerName;
    }
    void Revive(object data)
    {
        Immortal(2f);
        isPlay = true;
        isDead = false;
        fishSkin.sword.SetActiveCollider(true);
        fishSkin.gameObject.SetActive(true);
        crowns.gameObject.SetActive(true);
    }
    void ChangeSkin(object data)
    {
        LoadData();
    }
}
