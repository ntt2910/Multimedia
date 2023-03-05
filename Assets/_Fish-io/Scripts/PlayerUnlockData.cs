using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerUnlockData
{
    public List<FishObjectData> fishUnlocked = new List<FishObjectData>();
    public List<WeaponObjectData> weaponUnlocked = new List<WeaponObjectData>();
    public List<FishObjectData> fishCanUnlocked = new List<FishObjectData>();
    public List<WeaponObjectData> weaponCanUnlocked = new List<WeaponObjectData>();
    public FishObjectData GetFishDataUnlock(FishType fishType)
    {
        for (int i = 0; i < fishUnlocked.Count; i++)
        {
            if (fishUnlocked[i].fishType == fishType) return fishUnlocked[i];
        }
        return null;
    }
    public WeaponObjectData GetWeaponDataUnlock(WeaponType weaponType)
    {
        for (int i = 0; i < weaponUnlocked.Count; i++)
        {
            if (weaponUnlocked[i].weaponType == weaponType) return weaponUnlocked[i];
        }
        return null;
    }
    public FishObjectData GetFishDataCanUnlock(FishType fishType)
    {
        for (int i = 0; i < fishCanUnlocked.Count; i++)
        {
            if (fishCanUnlocked[i].fishType == fishType) return fishCanUnlocked[i];
        }
        return null;
    }
    public WeaponObjectData GetWeaponDataCanUnlock(WeaponType weaponType)
    {
        for (int i = 0; i < weaponCanUnlocked.Count; i++)
        {
            if (weaponCanUnlocked[i].weaponType == weaponType) return weaponCanUnlocked[i];
        }
        return null;
    }

    public void RemoveFishDataCanUnlock(FishType fishType)
    {
        for (int i = 0; i < fishCanUnlocked.Count; i++)
        {
            if (fishCanUnlocked[i].fishType == fishType)
            {
                fishCanUnlocked.RemoveAt(i);
                break;
            }
        }
    }
    public void RemoveWeaponDataCanUnlock(WeaponType weaponType)
    {
        for (int i = 0; i < weaponCanUnlocked.Count; i++)
        {
            if (weaponCanUnlocked[i].weaponType == weaponType)
            {
                weaponCanUnlocked.RemoveAt(i);
                break;
            }
        }
    }
}
