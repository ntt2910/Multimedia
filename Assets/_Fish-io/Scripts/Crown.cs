using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crown : MonoBehaviour
{
    [SerializeField] GameObject[] crowns;
    private void Start()
    {
        for (int i = 0; i < crowns.Length; i++)
        {
            crowns[i].gameObject.SetActive(false);
        }
    }
    public void SetActiveCrown(int index)
    {

        //{
        for (int i = 0; i < crowns.Length; i++)
        {
            crowns[i].gameObject.SetActive(false);
        }
        if (index >= crowns.Length) return;
        //}
        crowns[index].gameObject.SetActive(true);
    }
}
