using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderSilk : MoveController
{
    [SerializeField] GameObject spiderSilk;
    [SerializeField] float speedRotate = 25f;
    PassiveEnsnare owner;
    private void Update()
    {
        //spiderSilk.transform.Rotate(0, speedRotate * Time.deltaTime * 10, 0);
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
    public void InitSpawn(Vector3 dir, PassiveEnsnare _owner)
    {
        GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.EnsnareCast);
        transform.right = dir;
        owner = _owner;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fish" && other.transform != owner.transform)
        {
            other.transform.GetComponentInParent<ICanStun>()?.OnStun(2f);
            GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.EnsnareTarget);
            CreateManager.Instance.CreateSpiderSilkImpact(other.transform.position);
            CreateManager.Instance.CreateSpiderSilkStun(other.transform);
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        if (spiderSilk == null) Destroy(spiderSilk);
    }
}
