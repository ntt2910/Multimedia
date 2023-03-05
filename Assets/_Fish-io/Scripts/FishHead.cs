using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHead : MonoBehaviour
{
    public void MoveToPoint(Vector3 pos, System.Action actionComplete, float time)
    {
        transform.DOMove(pos, time).OnComplete(()=>
        {
            gameObject.SetActive(false);
            actionComplete?.Invoke();
        });
    }
}
