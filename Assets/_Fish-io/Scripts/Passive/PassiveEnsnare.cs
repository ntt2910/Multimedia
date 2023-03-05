using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveEnsnare : PassiveBase
{
    public float range = 6f;
    [SerializeField] FishSkin fishSkin;
    bool allowShot = true;
    private void Start()
    {
        passiveType = PassiveType.Ensnare;
        LoadData();
        //fish.SetBlur(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(fishSkin != null)
        {
            if(fishSkin.Fish != null)
            {
                if (fishSkin.Fish.IsDash && allowShot)
                {
                    Shoot();
                    StartCoroutine(ShootCooldown());
                }
            }
            
        }
        
    }
    void Shoot()
    {
        CreateManager.Instance.CreateSpiderSilk(transform.position).InitSpawn(fishSkin.Fish.Dir.right, this);
    }
    IEnumerator ShootCooldown()
    {
        allowShot = false;
        yield return new WaitForSeconds(2f);
        allowShot = true;
    }
    private void OnEnable()
    {
        allowShot = true;
    }
}
