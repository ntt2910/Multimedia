using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RewardElement : MonoBehaviour
{
    [SerializeField] private Image iconImg;
    [SerializeField] private Text valueTxt;
    [SerializeField] private GameObject rotateObj;

    private int value;

    public void Init(Sprite iconSpr, int value, bool isAnim = true)
    {
        this.value = value;
        iconImg.gameObject.SetActive(true);
        valueTxt.gameObject.SetActive(true);

        iconImg.sprite = iconSpr;
        if (value == 0)
        {
            valueTxt.text = "";
        }
        else
            valueTxt.text = value.ToString();
        
        if (isAnim)
        {
            this.transform.localScale = Vector3.zero;
            this.transform.DOKill();
            this.transform.DOScale(1, 0.3f).SetUpdate(true).SetEase(Ease.InBack).OnComplete(() => { if (rotateObj != null) rotateObj.gameObject.SetActive(true); });
        }
        else
        {
            this.transform.localScale = Vector3.one;
        }
    }

    public void MakeX2()
    {

        StartCoroutine(DoX2());
    }

    IEnumerator DoX2()
    {
        int curVal = value;
        int tarVal = 2 * value;

        int delta = ((tarVal - curVal) / 10);

        if (delta == 0)
        {
            delta = 1;
        }

        while (curVal < tarVal)
        {
            curVal += delta;
            if (curVal > tarVal) curVal = tarVal;

            DOTween.Sequence().Append(valueTxt.gameObject.GetComponent<RectTransform>().DOScale(1.1f, 0.01f)).Append(valueTxt.gameObject.GetComponent<RectTransform>().DOScale(1f, 0.01f));
            valueTxt.text = curVal.ToString();

            yield return new WaitForSeconds(0.02f);

        }

    }
}
