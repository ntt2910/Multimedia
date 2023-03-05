using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MoveController
{
    [SerializeField] GameObject boomerang;
    [SerializeField] float speedRotate = 25f;
    PassiveBoomerang owner;
    private void Update()
    {
        //boomerang.transform.Rotate(0, speedRotate * Time.deltaTime * 10, 0);
        Move(transform.right);
        if (owner != null)
        {
            float distance = Vector3.Distance(transform.position, owner.transform.position);
            if (distance >= owner.range)
            {
                Destroy(gameObject);
            }
        }
        else Destroy(gameObject);
        
    }
    public void InitSpawn(Vector3 dir, PassiveBoomerang _owner)
    {
        GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.BoomerangCast);
        transform.right = dir;
        owner = _owner;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if(other.tag == "Fish" && other.transform != owner.transform)
        {
            other.transform.GetComponentInParent<ICanStun>()?.OnStun(1.25f);
            GameObject stunFx = CreateManager.Instance.CreateBoomerangStun(other.transform).gameObject;
            stunFx.transform.localPosition = new Vector3(0, 1, 0);
            //Destroy(stunFx, 1.25f);
        }    
    }
    private void OnDestroy()
    {
        if (boomerang == null) Destroy(boomerang);
    }
}
