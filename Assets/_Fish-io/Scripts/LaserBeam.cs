using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    GameObject owner;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] ParticleSystem startFx, endFx;
    public void OnSpawn(Vector3 start, Vector3 end, float time, GameObject _owner)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        startFx.transform.position = start;
        endFx.transform.position = end;
        owner = _owner;
        GameController.instance.musicManager.PlayOneShot(GameController.instance.soundData.MachineFishLaserPassive);
        Destroy(gameObject, 1f);
        //transform.localScale = Vector3.zero;
        //transform.DOScale(Vector3.one * 2, time / 2).OnComplete(() =>
        //{
        //    transform.DOScale(Vector3.one, time / 2).OnComplete(() =>
        //    {
        //        gameObject.SetActive(false);
        //    });
        //});
    }
    private void Update()
    {

        // Physics.Raycast(start, end - start, out hit, Vector2.Distance(start, end));
        if (owner)
        {
            Vector2 end = lineRenderer.GetPosition(1);
            Vector2 start = lineRenderer.GetPosition(0);
            RaycastHit[] hits = Physics.RaycastAll(start, end - start, Vector2.Distance(start, end));
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {

                    if (hits[i].transform != owner.transform)
                    {
                        var skinFish = hits[i].transform.GetComponentInParent<FishSkin>();
                        if (skinFish != null)
                        {
                            skinFish.OnKillOtherFish();
                            hits[i].transform.GetComponentInParent<Fish>()?.TakeDamage(1);
                            //skinFish.fish.UpgradeFish();
                            //CreateManager.Instance.CreateMeat(skinFish.fish.levelSize, transform.position);
                            //if (skinFish.passive) skinFish.passive.OnUpdateKill(1);
                            //skinFish.UpgradeSword();
                        }
                    }

                }
            }
        }

    }
}
