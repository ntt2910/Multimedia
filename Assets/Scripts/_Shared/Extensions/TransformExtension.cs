﻿using System;
using BW.Pools;
using BW.Services;
using UnityEngine;
using Object = UnityEngine.Object;

public static class TransformExtension
{
    private static readonly Vector3 CenterViewportPoint = new Vector3(0.5f, 0.5f, 0.0f);

    public static void DestroyChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            ServiceLocator.GetService<IPoolService>().Despawn(transform.GetChild(i).gameObject);
        }
    }

    public static void DestroyChildren(this Transform transform, string name, bool exact = true)
    {
        var allTrans = transform.GetComponentsInChildren<Transform>(true);

        for (int i = allTrans.Length - 1; i >= 1; --i)
        {
            Transform child = allTrans[i];

            if (exact)
            {
                if (string.Compare(child.name, name, StringComparison.Ordinal) == 0)
                {
                    Object.Destroy(allTrans[i].gameObject);
                }
            }
            else
            {
                if (child.name.Contains(name))
                {
                    Object.Destroy(allTrans[i].gameObject);
                }
            }
        }
    }

    public static Transform GetChildByName(this Transform transform, string name, bool canCreateIfNull = false)
    {
        var child = transform.Find(name);

        if (child == null && canCreateIfNull)
        {
            child = new GameObject(name).transform;
            child.SetParent(transform);
            child.localPosition = Vector3.zero;
        }

        return child;
    }

    public static void SetParentUI(this Transform transform, RectTransform parent)
    {
        //        transform.gameObject.SetActive(false);
        transform.SetParent(parent);
        transform.localScale = Vector3.one;
        //        var position = transform.localPosition;
        //        position.z = 0f;
        transform.localPosition = Vector3.zero;
        //        transform.gameObject.SetActive(true);
    }

    public static void SetParentUI(this Transform transform, Transform parent)
    {
        //        transform.gameObject.SetActive(false);
        transform.SetParent(parent);
        transform.localScale = Vector3.one;
        //        var position = transform.localPosition;
        //        position.z = 0f;
        transform.localPosition = Vector3.zero;
        //        transform.gameObject.SetActive(true);
    }

    /// <summary>
    /// Make 3D gameobject x axis look at target in 2D (with object has default rotation like in 3D).
    /// </summary>
    /// <param name="trans">Trans.</param>
    /// <param name="targetTrans">Target trans.</param>
    public static void LookAtAxisX2D(this Transform trans, Transform targetTrans)
    {
        LookAtAxisX2D(trans, targetTrans.position);
    }

    /// <summary>
    /// Make 3D gameobject x axis look at target in 2D (with object has default rotation like in 3D).
    /// </summary>
    /// <param name="trans">Trans.</param>
    /// <param name="targetPosition">Target position.</param>
    public static void LookAtAxisX2D(this Transform trans, Vector3 targetPosition)
    {
        // It's important to know rotating direction (clock-wise or counter clock-wise)
        // If target is above of gameobject (has y value higher) then rotate counter clock-wise and vice versa
        bool isAboveOfXAxis = targetPosition.y > trans.position.y;

        float angle = (isAboveOfXAxis ? 1 : -1) * Vector3.Angle(Vector3.right, targetPosition - trans.position);

//        trans.localRotation = Quaternion.identity;
        trans.localRotation = Quaternion.Euler(Vector3.forward * angle);
    }

    /// <summary>
    /// Make 3D gameobject y axis look at target in 2D (with object has default rotation like in 3D).
    /// </summary>
    /// <param name="trans">Trans.</param>
    /// <param name="targetTrans">Target trans.</param>
    public static void LookAtAxisY2D(this Transform trans, Transform targetTrans)
    {
        LookAtAxisY2D(trans, targetTrans.position);
    }

    /// <summary>
    /// Make 3D gameobject y axis look at target in 2D (with object has default rotation like in 3D).
    /// </summary>
    /// <param name="trans">Trans.</param>
    /// <param name="targetPosition">Target position.</param>
    public static void LookAtAxisY2D(this Transform trans, Vector3 targetPosition)
    {
        var position = trans.position;
        bool isLeftOfYAxis = targetPosition.x < position.y;

        float angle = (isLeftOfYAxis ? 1 : -1) * Vector3.Angle(Vector3.up, targetPosition - position);
//        trans.localRotation = Quaternion.identity;
        trans.localRotation = Quaternion.Euler(Vector3.forward * angle);
    }

    /// 
    /// This is a 2D version of Quaternion.LookAt; it returns a quaternion
    /// that makes the local +X axis point in the given forward direction.
    /// 
    /// forward direction
    /// Quaternion that rotates +X to align with forward
    public static void LookAt2D(this Transform transform, Vector2 forward)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }

    public static Bounds CombineBounds(this Transform transform)
    {
        var renderers = transform.GetComponentsInChildren<Renderer>();
        var combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; ++i)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        return combinedBounds;
    }
}