using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelDatabase", menuName = "ScriptableObject/LevelDatabase")]
public class LevelDatabase : ScriptableObject
{
    public List<LevelDataObject> levelDataObjects = new List<LevelDataObject>();

    public LevelDataObject GetLevelDataObject(int levelID)
    {
        for (int i = 0; i < levelDataObjects.Count; i++)
        {
            if (levelDataObjects[i].level_ID == levelID) return levelDataObjects[i];
        }
        return levelDataObjects[levelDataObjects.Count - 1];
    }
}
[System.Serializable]
public class LevelDataObject
{
    public int level_ID;
    public List<LevelObjet> levelObjets = new List<LevelObjet>();

    public LevelObjet GetLevelObjet(int level)
    {
        for(int i = 0; i < levelObjets.Count; i++)
        {
            if (levelObjets[i].level == level) return levelObjets[i];
        }
        return null;
    }
}
[System.Serializable]
public class LevelObjet
{
    public int level;
    public int startUnit;
    public float spawnDelay;
    [TableList] public List<FishSpawnData> fishSpawnDatas = new List<FishSpawnData>();

    public FishSpawnData GetFishSpawnData(int id)
    {
        for (int i = 0; i < fishSpawnDatas.Count; i++)
        {
            if (fishSpawnDatas[i].id == id) return fishSpawnDatas[i];
        }
        return null;
    }
}
[System.Serializable]
public class FishSpawnData
{
    public int id;
    public float spawnPercent;
}
