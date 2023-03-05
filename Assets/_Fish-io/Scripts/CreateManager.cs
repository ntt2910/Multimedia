using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateManager : SingletonMonoBehavier<CreateManager>
{
    public static float limit_x = 19, limit_y = 9f;
    [SerializeField] FishAI fishAI;
    [SerializeField] GameObject exp, spiderStun, greenPearl;
    [SerializeField] List<GameObject> effectFishDie = new List<GameObject>();
    [SerializeField] Boomerang boomerang;
    [SerializeField] ParticleSystem 
        immortalEff, 
        boomerangStun, 
        spiderImpact, 
        manaEff, 
        relifeEff,
        hitFx;
    [SerializeField] LaserBeam laser;
    [SerializeField] Darts darts;
    [SerializeField] SpiderSilk spiderSilk;
    public FishAI CreateFishAI(Vector3 pos)
    {
        return Instantiate(fishAI, pos, Quaternion.identity);
    }
    public LaserBeam CreateLaser(Vector3 spawnPos)
    {
        return Instantiate(laser, spawnPos, Quaternion.identity);
    }
    public SpiderSilk CreateSpiderSilk(Vector3 spawnPos)
    {
        return Instantiate(spiderSilk, spawnPos, Quaternion.identity);
    }
    public Darts CreateDarts(Vector3 spawnPos)
    {
        return Instantiate(darts, spawnPos, darts.transform.rotation);
    }
    public GameObject CreateImageBorn(FishType fishType, Transform spawnPos)
    {
        var gob = GameController.instance.fishDatabase.GetFishOObjectData(fishType).imageBorn;
        return Instantiate(gob, spawnPos);
    }
    public GameObject CreateFishEXP(Transform parent)
    {
        GameObject gob = Instantiate(exp, parent);
        //gob.transform.rotation = quaternion;
        gob.transform.localPosition = Vector3.zero;
        return gob;
    }
    public Boomerang CreateBoomerang(Vector3 pos)
    {
        return Instantiate(boomerang, pos, Quaternion.identity);
    }   
    public void CreateMeat(int levelFish, Vector3 pos)
    {
        var vectorLook = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        if(levelFish == 2)
        {
            for (int i = 0; i < 3; i++)
            {
                if (vectorLook != Vector3.zero)
                {
                    Quaternion ro = Quaternion.LookRotation(vectorLook);
                    GameObject gob = Instantiate(effectFishDie[0].gameObject, new Vector3(pos.x, pos.y, 0), ro);
                    gob.GetComponent<Collider>().enabled = false;
                    gob.transform.DOMove(new Vector3(pos.x, pos.y, 0) + (Vector3)Random.insideUnitCircle * 2, 1f).OnComplete(() =>
                    {
                        gob.GetComponent<Collider>().enabled = true;
                    });

                    Destroy(gob, 5);
                }
            }
        }
        else if(levelFish == 3)
        {
            for (int i = 0; i < 9; i++)
            {
                if (vectorLook != Vector3.zero)
                {
                    Quaternion ro = Quaternion.LookRotation(vectorLook);
                    GameObject gob = Instantiate(effectFishDie[0].gameObject, new Vector3(pos.x, pos.y, 0), ro);
                    gob.GetComponent<Collider>().enabled = false;
                    gob.transform.DOMove(new Vector3(pos.x, pos.y, 0) + (Vector3)Random.insideUnitCircle * 2, 1f).OnComplete(() =>
                    {
                        gob.GetComponent<Collider>().enabled = true;
                    });
                }
                //Destroy(gob, 5);
            }
        }
        for(int i = 0; i < effectFishDie.Count; i ++)
        {
            if (vectorLook != Vector3.zero)
            {
                Quaternion ro = Quaternion.LookRotation(vectorLook);
                GameObject gob = Instantiate(effectFishDie[i].gameObject, new Vector3(pos.x, pos.y, 0), ro);
                gob.GetComponent<Collider>().enabled = false;
                gob.transform.DOMove(new Vector3(pos.x, pos.y, 0) + (Vector3)Random.insideUnitCircle * 2, 1f).OnComplete(() =>
                {
                    gob.GetComponent<Collider>().enabled = true;
                });
            }
            //Destroy(gob, 5);
        }
    }
    public ParticleSystem CreateImmortalEff(Transform parent)
    {
        return Instantiate(immortalEff, parent);
    }
    public ParticleSystem CreateBoomerangStun(Transform parent)
    {
        return Instantiate(boomerangStun, parent);
    }
    public GameObject CreateSpiderSilkStun(Transform parent)
    {
        GameObject gob = Instantiate(spiderStun, parent);
        Destroy(gob, 2f);
        return gob;
    }
    public ParticleSystem CreateSpiderSilkImpact(Vector3 pos)
    {
        ParticleSystem gob = Instantiate(spiderImpact, pos, Quaternion.identity);
        //Destroy(gob, 2f);
        return gob;
    }
    public ParticleSystem CreateRecoveryMana(Transform parent)
    {
        ParticleSystem gob = Instantiate(manaEff, parent);
        //Destroy(gob, 2f);
        return gob;
    }
    public ParticleSystem CreateHitFx(Vector3 spawnPos)
    {
        ParticleSystem gob = Instantiate(hitFx, spawnPos, Quaternion.identity);
        //Destroy(gob, 2f);
        return gob;
    }
    public ParticleSystem CreateRelifeFX(Vector3 pos)
    {
        ParticleSystem gob = Instantiate(relifeEff, pos, Quaternion.identity);
        //Destroy(gob, 2f);
        return gob;
    }
    public GameObject CreateGreenPearl(Vector3 pos)
    {
        GameObject gob = Instantiate(greenPearl, pos, Quaternion.identity);
        //Destroy(gob, 2f);
        return gob;
    }
}
