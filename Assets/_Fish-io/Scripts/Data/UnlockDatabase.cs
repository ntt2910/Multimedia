using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UnlockDatabase", menuName = "ScriptableObject/UnlockDatabase")]
public class UnlockDatabase : ScriptableObject
{
   [TableList] public List<UnlockDataObject> unlockDataObjects = new List<UnlockDataObject>();

    public UnlockDataObject GetUnlockDataObject(int id)
    {
        for(int i = 0; i < unlockDataObjects.Count; i ++)
        {
            if (unlockDataObjects[i].id == id) return unlockDataObjects[i];
        }
        return null;
    }    
}
[System.Serializable]
public class UnlockDataObject
{
    public int id;
    public bool isFish;
    [HideIf("isFish")] public WeaponType weaponType;
    [ShowIf("isFish")] public FishType fishType;
    public int unlockCondision;
    public int fishPieceReward;
    public string description;
}
