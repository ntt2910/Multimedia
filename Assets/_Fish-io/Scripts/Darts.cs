using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darts : MonoBehaviour
{
    //[ser]
    [SerializeField] float speedRotate = 25f;
    FishSkin owner;
    private void Start()
    {
        Destroy(gameObject, Random.Range(3f, 5f));
    }
    private void Update()
    {
        transform.Rotate(0, 0, speedRotate * Time.deltaTime * 10);
    }
    public void InitSpawn(Vector3 dir, FishSkin _owner)
    {
        GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.ShurikenTossCast);
        transform.right = dir;
        owner = _owner;
    }
    private void OnTriggerEnter(Collider other)
    {
       
        if(owner)
        {
            if (other.tag == "Fish" && other.transform != owner.transform)
            {
                Debug.Log("aaa");
                other.GetComponentInParent<Fish>()?.TakeDamage(1);
                owner.OnKillOtherFish();
            }
        }
        
    }
}
