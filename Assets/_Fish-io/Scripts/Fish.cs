using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveDir
{
    public static Vector3 dir1_1 = new Vector3(0, 90, 90);
    public static Vector3 dir1_2 = new Vector3(0, 90, -90);
    public static Vector3 dir1_3 = new Vector3(0, 0, 0);
    public static Vector3 dir1_4 = new Vector3(0, -180, 0);
    public static Vector3 dir2_1 = new Vector3(45, 90, 90);
    public static Vector3 dir2_2 = new Vector3(-45, 90, 90);
    public static Vector3 dir2_3 = new Vector3(45, 45, 245);
    public static Vector3 dir2_4 = new Vector3(-30, 30, 300);
}
public abstract class Fish : MoveController, ICanStun
{
    public Transform imageBornParent;
    public Transform shieldPos;
    public Crown crowns;
    [SerializeField] ParticleSystem dashBuble;
    [SerializeField] protected Transform dirTran;
    [SerializeField] protected Animator animator;
    [SerializeField] protected TextMeshPro nameText;
    [SerializeField] SpriteRenderer nationalityImage;
    [SerializeField] protected FishSkin fishSkin;
    [SerializeField] protected List<GameObject> exps = new List<GameObject>();
    public static float limit_x = 40, limit_y = 19.85f;
    protected Rigidbody rb;
    public float power = 100f, maxPower = 100, exp, expMax = 4;
    public float increaseSpeedCooldown = 1f;
    public int hp = 1, levelSize = 1, id, point = 0, killAmount = 0;
    public float dashSpeedConfig = 1;
    protected bool isCreaseSpeed = false,
        isStun = false,
        allowTakeDamage = true,
        isSurge = false,
        isReincarnation = false,
        isRecive = false,
        isBlur = false,
        isImmortal = false,
        isDead = false;
    public bool AllowTakeDamage
    {
        get { return allowTakeDamage; }
        set { allowTakeDamage = value; }
    }
    public bool IsImortal
    {
        get { return isImmortal; }
        set { isImmortal = value; }
    }
    protected string fishName, nationality;
    public string FishName => fishName;
    public string Nationality => name;
    public Transform Dir => dirTran;
    public bool IsBlur => isBlur;
    public bool IsDash => isCreaseSpeed;
    public bool IsDead => isDead;
    public FishSkin Skin => fishSkin;
    public FishType Type => fishSkin.fishType;

    protected Vector3 initScale;
    public Action actionCompleteDash;
    private void Awake()
    {
        initScale = transform.localScale;
    }
    protected virtual void Start()
    {
        expMax = GameController.instance.fishDatabase.GetFishDataLevel(levelSize).conditionLevel;
    }

    public virtual void Init()
    {

    }
    public abstract void Attack();

    public abstract void Rotate(Vector3 dir);

    public abstract void Die();

    public virtual void TakeDamage(int damage)
    {
        if (!allowTakeDamage || isImmortal) return;
    }
    public virtual void UpgradeFish()
    {
        exp++;
        if (levelSize >= 3 && exp >= expMax) return;
        transform.localScale = initScale * 1.05f;
        GameObject gob = CreateManager.Instance.CreateFishEXP(fishSkin.sword.GetHeadPoint(levelSize));
        exps.Add(gob);
        //SortExp();
        if (exp >= expMax)
        {
            //GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.UpgradeSizeSound);
            exp = 0;
            //expMax += 2;
            levelSize++;

            for (int i = 0; i < exps.Count; i++)
            {
                if (exps[i] != null)
                    exps[i].gameObject.SetActive(false);
            }
            exps.Clear();
            fishSkin.UpgradeSword(levelSize);
            transform.localScale = initScale * 1.5f;
            initScale = transform.localScale;
            expMax = GameController.instance.fishDatabase.GetFishDataLevel(levelSize).conditionLevel;
        }

    }
    public virtual void IncreaseSpeed()
    {
        if (isSurge)
        {
            if (!isCreaseSpeed)
            {
                speed *= (2.7f * dashSpeedConfig);
                isCreaseSpeed = true;
                StartCoroutine(IncreaseSpeedCooldown());
                //animator.SetTrigger("Dash");
            }
        }
        else
        {
            if (!isCreaseSpeed && power >= 20)
            {
                power -= 20;
                speed *= (2.7f * dashSpeedConfig);
                isCreaseSpeed = true;
                StartCoroutine(IncreaseSpeedCooldown());
                //animator.SetTrigger("Dash");
            }
        }
    }
    protected virtual void Update()
    {
        //if(power < maxPower) power += Time.deltaTime * 2;
        if (!isImmortal)
        {
            if (isStun || isRecive) return;
        }

        Vector3 newpos = this.transform.position + dirTran.right;
        Vector3 dir = dirTran.right;
        if (newpos.x <= -limit_x + 1f || newpos.x > limit_x - 1f)
        {
            dir.x = 0;
        }
        if (newpos.y > limit_y || newpos.y < -limit_y + 1)
        {
            dir.y = 0;
        }
        dir.z = 0;
        //newpos.z = 0;
        Move(dir);
        //|| newpos.y > limit_y || newpos.y < -limit_y
    }
    protected virtual IEnumerator IncreaseSpeedCooldown()
    {
        if (dashBuble != null) dashBuble.Play();
        actionCompleteDash?.Invoke();
        GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.SlashSound);
        yield return new WaitForSeconds(increaseSpeedCooldown);
        speed /= (2.7f * dashSpeedConfig);
        isCreaseSpeed = false;
        if (dashBuble != null) dashBuble.Stop();
    }
    IEnumerator TimeStun(float timeStun)
    {
        isStun = true;
        yield return new WaitForSeconds(timeStun);
        isStun = false;
    }
    IEnumerator TimeImmortal(float time)
    {
        allowTakeDamage = false;
        yield return new WaitForSeconds(time);
        allowTakeDamage = true;
    }
    IEnumerator TimeSurge(float time)
    {
        isSurge = true;
        yield return new WaitForSeconds(time);
        isSurge = false;
    }
    IEnumerator TimeRecive(float time)
    {
        isRecive = true;
        yield return new WaitForSeconds(time);
        isRecive = false;
    }
    IEnumerator IEDelayAction(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }
    public void SortExp()
    {
        if (exps.Count > 0)
        {
            Vector3 size = exps[0].GetComponent<Renderer>().bounds.size;
            exps[0].transform.localPosition = new Vector3(fishSkin.sword.transform.localPosition.x + size.x * 3, 0, 0);
            for (int i = 1; i < exps.Count; i++)
            {
                exps[i].transform.localPosition = new Vector3(exps[i - 1].transform.localPosition.x + size.x * 3, 0, 0);
            }
        }
    }

    public void OnStun(float timeStub)
    {
        StartCoroutine(TimeStun(timeStub));
    }
    public void Immortal(float time)
    {
        StartCoroutine(TimeImmortal(time));
    }
    public void Surge(float time)
    {
        StartCoroutine(TimeSurge(time));
    }
    public void SetReincarnation(bool _isReincarnation)
    {
        isReincarnation = _isReincarnation;
    }
    public void SetBlur(bool _isBlur)
    {
        isBlur = _isBlur;
    }
    public void DelayAction(float time, System.Action action)
    {
        StartCoroutine(IEDelayAction(time, action));
    }
    public void ChangePoint(int _point)
    {
        point += _point;
    }
    public virtual void ChangeKillAmount(int _kill)
    {
        killAmount += _kill;
    }
    public void LoadProfile(FishPlayerData fishPlayerData)
    {
        fishName = fishPlayerData.name;
        nameText.text = fishPlayerData.name;
        nationality = fishPlayerData.countryCode;
        var texture = Resources.Load<Sprite>($"flags/{fishPlayerData.countryCode}");
        nationalityImage.sprite = texture;
    }
    private void LateUpdate()
    {
        if (Camera.main != null)
            nameText.transform.forward = Camera.main.transform.forward;
    }
    public void StartGame()
    {
        ParticleSystem immortalEff = CreateManager.Instance.CreateImmortalEff(shieldPos.transform);
        
        immortalEff.gameObject.SetActive(true);
        immortalEff.Play();
        Immortal(3f);
        Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ =>
        {
            Destroy(immortalEff.gameObject);
        }).AddTo(this);
    }    
}
