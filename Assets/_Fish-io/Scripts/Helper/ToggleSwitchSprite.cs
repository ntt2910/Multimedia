using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSwitchSprite : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite onSprite, offSprite;
    private bool isOn;

    public bool IsOn {
        get { return isOn; }
        set
        {
            isOn = value;
            if (_toggle == null)
                _toggle = GetComponent<Toggle>();
            _toggle.isOn = value;
        }
    }

    public IObservable<bool> Value {
        get
        {
            if (_toggle == null)
                _toggle = GetComponent<Toggle>();
            return _toggle.OnValueChangedAsObservable();
        }
    }

    private Toggle _toggle;

    private void Start() {
        if (_toggle == null)
            _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(UpdateValue);
        //UpdateValue(_toggle.isOn);
    }

    private void UpdateValue(bool value = true) {
        if (targetImage == null)
            return;
        isOn = value;
        targetImage.sprite = isOn ? onSprite : offSprite;
    }

    public void UpdateSprite(bool _isOn = true)
    {
        if (targetImage == null)
            return;
        IsOn = _isOn;
        targetImage.sprite = isOn ? onSprite : offSprite;
    }
}
