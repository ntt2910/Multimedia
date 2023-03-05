using LTABase.DesignPattern;
//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Random = UnityEngine.Random;

public class FishAI : Fish
{
    public DataInRange dataInRange;
    public DataOutRange dataOutRange;
    bool allowRotate = true, inRange = false;
    float rotateCooldown = 2f, rangeAtk = 10f;
    Vector3 moveDir;
    Transform target;
    public int currentAction = 0;
    protected override void Start()
    {
        base.Start();
        LoadDataFishAI();
        StartCoroutine(DelayAction());

        int random = Random.Range(0, GameController.instance.weaponDatabase.weaponObjectDatas.Count);
        WeaponType weaponType = ((WeaponType[])WeaponType.GetValues(typeof(WeaponType)))[random];
        var _weapon = GameController.instance.weaponDatabase.GetWeaponObjectData(weaponType);
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
        }
    }
    public void LoadDataFishAI()
    {
        var fishAIData = GameController.instance.fishAIDatabase.GetFishAIDataWithFishType(fishSkin.fishType);
        if (fishAIData != null)
        {
            dataInRange = fishAIData.fishAIData.inRange;
            dataOutRange = fishAIData.fishAIData.outRange;
        }
        var fishData = GameController.instance.fishDatabase.GetFishOObjectData(fishSkin.fishType);
        speed = fishData.moveSpeed;
    }
    protected override void Update()
    {
        if (power < maxPower) power += Time.deltaTime * 2;
        if (!isImmortal)
        {
            if (isStun || isRecive) return;
        }
        Vector3 newpos = this.transform.position + dirTran.right;
        if (newpos.x <= -limit_x || newpos.x > limit_x || newpos.y > limit_y || newpos.y < -limit_y)
        {
            moveDir = new Vector3(Random.Range(-1, 2) == 1 ? 1 : -1, Random.Range(-1, 2) == 1 ? 1 : -1, 0);
            //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, ();

        }
        else
        {
            if (allowRotate)
            {
                StartCoroutine(CooldownRotate());
            }
            if (target == null) Move(dirTran.right);
            else
            {
                var pos = (target.position - transform.position).normalized;
                Move(pos);
            }
        }
        if (target == null) Rotate(moveDir);
        else Rotate(target.position - transform.position);

        Collider[] colliders = Physics.OverlapSphere(transform.position, rangeAtk / 2);
        bool check = false;
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag("Fish") && colliders[i].transform != transform)
                {
                    check = true;
                    break;
                }
            }
        }
        inRange = check;
    }
    public override void TakeDamage(int damage)
    {
        if (!allowTakeDamage) return;
        Die();
    }
    public override void Attack()
    {
        int randomAttack = Random.Range(1, 100);
        //speed = dataOutRange.moveSpeed;

        int randomAction = Random.Range(1, 100);

        if (randomAction <= dataInRange.percentAttackUser)
        {
            //Debug.Log("chase user");
            var player = FindObjectOfType<PlayerController>();
            if (player != null)
                target = player.transform;

        }
        if (randomAction < dataInRange.percentAttackUser && randomAction < (dataInRange.percentAttackUser + dataInRange.percentAttackOtherEnemy))
        {
            //Debug.Log("chase other enemy");

            target = SpawnLevelController.Instance.GetRandomFish().transform;
            while (target == transform)
            {
                target = SpawnLevelController.Instance.GetRandomFish().transform;
            }
        }
        int eatValue = dataInRange.percentAttackUser + dataInRange.percentAttackOtherEnemy + dataInRange.percentEat;
        if (randomAction > (dataInRange.percentAttackUser + dataInRange.percentAttackOtherEnemy) && randomAction <= eatValue)
        {
            //Debug.Log("eat");
            Collider[] colliders = Physics.OverlapSphere(transform.position, rangeAtk / 2);

            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].CompareTag("Meat"))
                    {
                        target = colliders[i].transform;
                        break;
                    }
                }
            }
        }
    }

    public void MoveAI()
    {
        if (inRange)
        {
            int randomEvade = Random.Range(1, 100);
            Collider[] colliders = Physics.OverlapSphere(transform.position, rangeAtk / 2);
            if (randomEvade <= dataInRange.percentEvade)
            {

                if (colliders.Length > 0)
                {
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (colliders[i].CompareTag("Weapon"))
                        {
                            moveDir = transform.position - colliders[i].transform.position;
                            break;
                        }
                    }
                }
            }
            if (randomEvade > dataInRange.percentEvade && randomEvade < (dataInRange.percentEvade + dataInRange.percentCounter))
            {
                if (colliders.Length > 0)
                {
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (colliders[i].CompareTag("Fish"))
                        {
                            moveDir = colliders[i].transform.position - transform.position;
                        }
                    }
                }
            }
        }
    }
    public void ChaseAI()
    {
        int randomChase = Random.Range(1, 100);
        //speed = dataOutRange.moveSpeed;
        if (randomChase > dataOutRange.chase)
        {
            int randomAction = Random.Range(1, 100);

            if (randomAction <= dataOutRange.percentChaseUser)
            {
                Debug.Log("chase user");

                target = FindObjectOfType<PlayerController>().transform;

            }
            if (randomAction < dataOutRange.percentChaseUser && randomAction < (dataOutRange.percentChaseUser + dataOutRange.percentChaseOtherEnemy))
            {
                Debug.Log("chase other enemy");

                target = SpawnLevelController.Instance.GetRandomFish().transform;
                while(target == transform)
                {
                    target = SpawnLevelController.Instance.GetRandomFish().transform;
                }    
            }
        }
    }
    public void DashAI()
    {
        if (inRange)
        {
            float randomDash = Random.Range(1, 100);
            if (randomDash < dataInRange.percentHoldDash)
            {
                Debug.Log("Hold Dash");
                increaseSpeedCooldown = 3f;
            }
            else
            {
                increaseSpeedCooldown = 1f;
            }
            IncreaseSpeed();
        }
        else
        {
            float randomDash = Random.Range(1, 100);
            if (randomDash < dataOutRange.percentHoldDash)
            {
                //Debug.Log("Hold Dash");
                increaseSpeedCooldown = 3f;
            }
            else
            {
                increaseSpeedCooldown = 1f;
            }
            IncreaseSpeed();
        }
    }
    public override void Die()
    {
        if (isReincarnation)
        {
            crowns.gameObject.SetActive(false);
            CreateManager.Instance.CreateRelifeFX(transform.position);
            fishSkin.gameObject.SetActive(false);
            isReincarnation = false;
            isRecive = true;
            StopAllCoroutines();
            DelayAction(3f, () =>
            {
                isRecive = false;
                Immortal(2f);
                crowns.gameObject.SetActive(true);
                fishSkin.gameObject.SetActive(true);
            });
        }
        else
        {
            isDead = true;
            crowns.gameObject.SetActive(false);
            fishSkin.sword.SetActiveCollider(false);
            Destroy(gameObject);
            Observer.Instance.Notify<Fish>(ObserverName.ON_FISH_DIE, this);
        }
    }
    public override void Rotate(Vector3 dir)
    {
        if (dir.magnitude != 0) dirTran.right = dir;
        var vectorLook = new Vector3(-dir.x, -dir.y, 0);
        if (vectorLook != Vector3.zero)
        {
            fishSkin.transform.parent.rotation = Quaternion.Lerp(fishSkin.transform.parent.rotation, Quaternion.LookRotation(vectorLook), Time.deltaTime * 10);
        }
    }
    IEnumerator CooldownRotate()
    {
        allowRotate = false;
        moveDir = new Vector3(Random.Range(-1, 2) == 1 ? 1 : -1, Random.Range(-1, 2) == 1 ? 1 : -1, 0);

        yield return new WaitForSeconds(rotateCooldown);
        allowRotate = true;
    }
    bool actionComplete = false;
    float _time = 0;
    IEnumerator DelayAction()
    {
        if (!actionComplete)
        {
            int randomAI = Random.Range(1, 100);
            if (inRange)
            {
                if (randomAI <= dataInRange.attack)
                {
                    currentAction = 1;
                    _time = dataInRange.time;
                }
                else if (randomAI > dataInRange.attack && randomAI <= (dataInRange.attack + dataInRange.dashTime))
                {
                    currentAction = 2;
                    _time = dataInRange.dashTime;
                }
                else
                {
                    currentAction = 3;
                    _time = dataInRange.timeMove;
                }
            }
            else
            {
                if (randomAI <= dataOutRange.percentAttack)
                {
                    currentAction = 1;
                    _time = dataOutRange.timeAttack;
                }
                else if (randomAI > dataOutRange.percentAttack && randomAI <= (dataOutRange.percentAttack + dataInRange.percentDash))
                {
                    currentAction = 2;
                    _time = dataOutRange.timeDash;
                }
                else
                {
                    currentAction = 3;
                    _time = dataOutRange.timeMove;
                }
            }
            actionComplete = true;
        }
        if (currentAction == 1)
        {
            Attack();
        }
        else if (currentAction == 2)
        {
            DashAI();
        }
        else
        {
            MoveAI();
        }

        yield return new WaitForSeconds(_time);
        //yield return null;
        actionComplete = false;
        //currentAction = Random.Range(0, 4);
        StartCoroutine(DelayAction());
    }
}
