using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "DataLevels", menuName = "GameData/DataLevels")]
public class DataLevels : SerializedScriptableObject
{
    public List<DataLevel> lsLevel = new List<DataLevel>();

    public DataLevel GetLevel(int ID)
    {
        return lsLevel.Find(x => x.ID == ID);
    }
}


[System.Serializable]
public class DataLevel
{
    public int ID;
    public string NAME_LEVEL;
}