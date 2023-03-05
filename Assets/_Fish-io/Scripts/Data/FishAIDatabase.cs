using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "FishAIDatabase", menuName = "ScriptableObject/FishAIDatabase")]
public class FishAIDatabase : ScriptableObject
{
    [TableList] public List<FishAIDataObject> fishAIDataObjects = new List<FishAIDataObject>();

    public FishAIDataObject GetFishAIDataWithID(int id)
    {
        for(int i = 0; i < fishAIDataObjects.Count; i++)
        {
            if (fishAIDataObjects[i].id == id) return fishAIDataObjects[i];
        }
        return null;
    }
    public FishAIDataObject GetFishAIDataWithFishType(FishType fishType)
    {
        for (int i = 0; i < fishAIDataObjects.Count; i++)
        {
            if (fishAIDataObjects[i].fishType == fishType) return fishAIDataObjects[i];
        }
        return null;
    }
}
[System.Serializable]
public class FishAIDataObject
{
    public int id;
    public FishType fishType;
    public FishAIData fishAIData;
}
[System.Serializable]
public class FishAIData
{
    public DataInRange inRange;
    public DataOutRange outRange;
}
[System.Serializable]
public class DataInRange
{
    [Header("ATTACK")]
    public int attack;
    public int percentAttackUser;
    public int percentAttackOtherEnemy;
    public int percentEat;
    public float time;
    [Header("MOVE")]
    public int moveSpeed;
    public int percentEvade;
    public int percentCounter;
    public float timeMove;
    [Header("DASH")]
    public int percentDash;
    public int percentHoldDash;
    public float dashTime;
}
[System.Serializable]
public class DataOutRange
{
    [Header("CHASE")]
    public int chase;
    public int percentChaseUser;
    public int percentChaseOtherEnemy;
    public float timeChase;
    [Header("MOVE")]
    public int moveSpeed;
    public float timeMove;
    [Header("DASH")]
    public int dashSpeed;
    public int percentHoldDash;
    public float timeDash;
    [Header("ATTACK")]
    public int percentAttack;
    public float timeAttack;
}
