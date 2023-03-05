using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LTABase.DesignPattern;

public class ShopSlot : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] protected Button btnSelect, btnUnlock;
    [SerializeField] protected TextMeshProUGUI itemName;
    [SerializeField] protected Image iconBg, icon, border;
    [SerializeField] protected Sprite bgSelected, bgLock, bgNormal, bgUnlock;
    [SerializeField] protected GameObject selectText, selectedText, lockText, unlockText, lockGraphic;

    protected virtual void Start()
    {
        btnSelect.onClick.AddListener(OnClickSelect);
        btnUnlock.onClick.AddListener(OnClickUnLock);
    }
    public void LoadData(string _itemName, Sprite spr)
    {
        icon.sprite = spr;
        itemName.text = _itemName;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        
    }
    public void SetActiveBorder(bool active)
    {
        border.enabled = active;
    }    
    public void SetLock()
    {
        //btn.interactable = false;

        iconBg.sprite = bgLock;
        lockGraphic.gameObject.SetActive(true);
        selectText.gameObject.SetActive(false);
        selectedText.gameObject.SetActive(false);
        lockText.gameObject.SetActive(true);
        unlockText.gameObject.SetActive(false);
    }    
    public void SetSelect()
    {
        //btn.interactable = true;

        iconBg.sprite = bgNormal;
        lockGraphic.gameObject.SetActive(false);
        selectText.gameObject.SetActive(true);
        selectedText.gameObject.SetActive(false);
        lockText.gameObject.SetActive(false);
        unlockText.gameObject.SetActive(false);
    }
    public void SetSelected()
    {
        //btn.interactable = true;

        iconBg.sprite = bgSelected;

        lockGraphic.gameObject.SetActive(false);
        selectText.gameObject.SetActive(false);
        selectedText.gameObject.SetActive(true);
        lockText.gameObject.SetActive(false);
        unlockText.gameObject.SetActive(false);
    }
    public void SetUnLock()
    {
        //btn.interactable = true;

        iconBg.sprite = bgUnlock;

        lockGraphic.gameObject.SetActive(false);
        selectText.gameObject.SetActive(false);
        selectedText.gameObject.SetActive(false);
        lockText.gameObject.SetActive(false);
        unlockText.gameObject.SetActive(true);
    }

    public virtual void OnClickSelect()
    {
        Debug.Log("Select Item");
    }
    public virtual void OnClickUnLock()
    {
        Debug.Log("Unlock Item");
    }
}
