using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponType weaponType;
    public List<GameObject> weaponGraphics = new List<GameObject>();

    public List<Transform> headPointLv1 = new List<Transform>();
    public List<Transform> headPointLv2 = new List<Transform>();
    public List<Transform> headPointLv3 = new List<Transform>();

    int sword1HeadCount = 0, sword2HeadCount = 0, sword3HeadCount = 0;
    public void LevelUp(int level)
    {
        for(int i = 0; i < weaponGraphics.Count; i++)
        {
            weaponGraphics[i].gameObject.SetActive(false);
        }
        weaponGraphics[level - 1].gameObject.SetActive(true);
    }   
    public BoxCollider GetCurrentGraphic()
    {
        for (int i = 0; i < weaponGraphics.Count; i++)
        {
            if (weaponGraphics[i].activeInHierarchy) return weaponGraphics[i].GetComponent<BoxCollider>();
        }
        return null;
    }
    public Transform GetHeadPoint(int level)
    {
        Transform point = null;
        switch(level)
        {
            case 1:
                point = headPointLv1[sword1HeadCount];
                sword1HeadCount++;
                break;
            case 2:
                point = headPointLv2[sword2HeadCount];
                sword2HeadCount++;
                break;
            case 3:
                point = headPointLv3[sword3HeadCount];
                sword3HeadCount++;
                break;

        }
        return point;
    }   
    public void SetActiveCollider(bool active)
    {
        for (int i = 0; i < weaponGraphics.Count; i++)
        {
            weaponGraphics[i].GetComponent<Collider>().enabled = active;
        }
    }
    public void ChangeTag(string tag)
    {
        for (int i = 0; i < weaponGraphics.Count; i++)
        {
            weaponGraphics[i].gameObject.tag = tag;
        }
    }
}
