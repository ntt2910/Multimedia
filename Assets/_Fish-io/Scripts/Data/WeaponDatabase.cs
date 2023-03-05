using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "ScriptableObject/WeaponDatabase")]
public class WeaponDatabase : ScriptableObject
{
   [TableList]  public List<WeaponObjectData> weaponObjectDatas = new List<WeaponObjectData>();

    public WeaponObjectData GetWeaponObjectData(WeaponType weaponType)
    {
        for (int i = 0; i < weaponObjectDatas.Count; i++)
        {
            if (weaponObjectDatas[i].weaponType == weaponType)
            {
                return weaponObjectDatas[i];
            }
        }
        return null;
    }
    
    public WeaponObjectData GetWeaponObjectData(int id)
    {
        for (int i = 0; i < weaponObjectDatas.Count; i++)
        {
            if (weaponObjectDatas[i].id == id)
            {
                return weaponObjectDatas[i];
            }
        }
        return null;
    }
}
[System.Serializable]
public class WeaponObjectData
{
    public int id;
    public WeaponType weaponType;
    public GameObject weaponPrefabs;
    public Sprite icon;
    public string des;
}
public enum WeaponType
{
    FireSword,
    GreenSaber,
    Horn,
    Katana,
    Laser,
    LegendKatana,
    Lightning,
    Poisedon,
    Sword,
    Umbrella
}