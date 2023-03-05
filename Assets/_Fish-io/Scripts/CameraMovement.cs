using DG.Tweening;
using LTABase.DesignPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float limit_x = 25f, limit_y = 12f;
    [SerializeField] Transform targetFollow;
    [SerializeField] ParticleSystem[] confetti;
    public float moveSpeed;

    public Vector3 offset, offsetOnPlay, eulerAngleOnPlay, offsetOnWin, offsetOnStart, eulerAngleOnStart;

    public bool freezeX, freezeY = false;
    float startOffset;
    bool win = false, playerDie = false;
    private void Start()
    {
        startOffset = offset.z;
        Observer.Instance.AddObserver(ObserverName.ON_PLAYER_LEVEL_UP, ChangeOffset);
        Observer.Instance.AddObserver(ObserverName.ON_PLAYER_DIE, OnPayerDie);
        Observer.Instance.AddObserver(ObserverName.ON_PLAYER_REVIVE, OnPlayerRevive);
        //targetFollow = FindObjectOfType<PlayerMovement>().transform;
        //offset = transform.position - targetFollow.position;
        //startOffset = offset.y;
        //offset = offsetOnStart;
        //transform.eulerAngles = eulerAngleOnStart;
    }
    private void OnDestroy()
    {

    }
    public void SetTarget(Transform t)
    {
        targetFollow = t;
    }
    public void RemoveTarget()
    {
        targetFollow = null;
    }
    private void LateUpdate()
    {
        if (targetFollow == null) return;
        //var rb = targetFollow.GetComponent<Rigidbody>();
        Vector3 v = Vector3.zero;
        if (targetFollow == null)
        {
            v = Vector3.Lerp(transform.position + (win ? offsetOnWin : offset), targetFollow.position + (win ? offsetOnWin : offset), moveSpeed * Time.deltaTime);
        }
        else
        {
            v = Vector3.Lerp(targetFollow.position + (win ? offsetOnWin : offset), targetFollow.position + (win ? offsetOnWin : offset), moveSpeed * Time.deltaTime);
        }

        if (freezeX)
        {
            v.x = 0;
        }
        if (freezeY) v.y = 0;

        if(!playerDie)
        {
            if (targetFollow.position.y >= limit_y || targetFollow.position.y <= -limit_y)
            {
                v.y = transform.position.y;
            }
            if (targetFollow.position.x >= limit_x || targetFollow.position.x <= -limit_x)
            {
                v.x = transform.position.x;
            }
        }    

        transform.position = v;
    }

    IEnumerator DelayAction(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }
    public void OnPlayerRevive(object data)
    {
        playerDie = false;
    }    
    public void ChangeOffset(object data)
    {
        Debug.Log("view");
        float value = offset.z;
        float newOffset = offset.z - 2.5f;
        limit_x -= 2.5f; limit_y -= 2.5f;
        DOTween.To(() => value, x => value = x, newOffset, 1f)
            .OnUpdate(() => { offset.z = value; })
            .OnComplete(() =>
            {
                //offset.y = newOffset;
            }).SetUpdate(true);
    }
    public void OnPayerDie(object data)
    {
        //playerDie = true;
        float value = offset.z;
        float newOffset = offset.z + 5f;
        //limit_x -= 2.5f; limit_y -= 2.5f;
        DOTween.To(() => value, x => value = x, newOffset, 1.25f)
            .OnUpdate(() => { offset.z = value; })
            .OnComplete(() =>
            {
                newOffset = offset.z - 5f;
                DOTween.To(() => value, x => value = x, newOffset, 1.25f)
            .OnUpdate(() => { offset.z = value; })
            .OnComplete(() =>
            {
                //offset.y = newOffset;
            }).SetUpdate(true);
                //offset.y = newOffset;
            }).SetUpdate(true);
    }
}
