using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "FishDatabase", menuName = "ScriptableObject/FishDatabase")]
public class FishDatabase : ScriptableObject
{
    public int recoveryManaMeatConfig, 
        recoveryManaGreenPearConfig, 
        pointMeatConfig,
        pointKillConfig;
    [TableList] public List<FishObjectData> fishObjectDatas = new List<FishObjectData>();
    [TableList] public List<FishLevelData> fishConfigDataLevel = new List<FishLevelData>();

    public FishObjectData GetFishOObjectData(FishType fishType)
    {
        for(int i = 0; i < fishObjectDatas.Count; i ++)
        {
            if(fishObjectDatas[i].fishType == fishType)
            {
                return fishObjectDatas[i];
            }    
        }
        return null;
    }
    public FishObjectData GetFishOObjectData(int id)
    {
        for (int i = 0; i < fishObjectDatas.Count; i++)
        {
            if (fishObjectDatas[i].id == id)
            {
                return fishObjectDatas[i];
            }
        }
        return null;
    }
    public FishLevelData GetFishDataLevel(int level)
    {
        for (int i = 0; i < fishConfigDataLevel.Count; i++)
        {
            if (fishConfigDataLevel[i].level == level)
            {
                return fishConfigDataLevel[i];
            }
        }
        return null;
    }
}
[System.Serializable]
public class FishObjectData
{
    public int id;
    public bool isNewFish;
    public FishType fishType;
    public string fishName;
    public GameObject prefab;
    public GameObject prefabAI;
    public GameObject headFish;
    public GameObject imageBorn;
    public Sprite icon;
    public float moveSpeed;
    public PassiveType passiveType;
    public string des;
}
[System.Serializable]
public class FishLevelData
{
    public int level;
    public int conditionLevel;
}
public enum FishType
{
    Rainbow,
    Spidey,
    ScaryCat,
    Nemo,
    Huggy,
    Quinn,
    Devil,
    Lala,
    Zombie,
    Ninja,
    Clown,
    Protector,
    Mummy,
    Shiba,
    Phantom,
    Bat,
    Irony,
    Cyber,
    IT,
    GreenHuggy,
    VioletHuggy,
    BlondeHuggy,
    JavaSaw,
    Orange,
    Pumpkin,
    Tinkie,
    Dipsi,
    Poo,
    Rex,
    SunFire
}

