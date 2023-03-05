using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroll : MonoBehaviour
{
    [SerializeField] private float speed;

    private Vector2 _mainTextureOffset;
    private bool _isScroll = false;
    private void Start()
    {
        _isScroll = false;
        _mainTextureOffset = GetComponent<Image>().material.mainTextureOffset;
    }

    private void Update()
    {
        if (!_isScroll) return;
        var offset = new Vector2(Time.time * speed, 0);
        _mainTextureOffset = offset;
    }

    public void PlayScroll(bool isScroll)
    {
        _isScroll = isScroll;
    }
}
