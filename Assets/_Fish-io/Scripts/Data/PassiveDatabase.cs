using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveDatabase", menuName = "ScriptableObject/PassiveDatabase")]
public class PassiveDatabase : ScriptableObject
{
    [TableList] public List<PassiveObjectData> passiveObjectDatas = new List<PassiveObjectData>();

    public PassiveObjectData GetPassiveObjectData(PassiveType passiveType)
    {
        for (int i = 0; i < passiveObjectDatas.Count; i++)
        {
            if (passiveObjectDatas[i].passiveType == passiveType)
            {
                return passiveObjectDatas[i];
            }
        }
        return null;
    }    
}
[System.Serializable]
public class PassiveObjectData
{
    public PassiveType passiveType;
    public int killActive;
    public int eatActive;
    public float duration;
}
public enum PassiveType
{
    None,
    Laser,
    RecoveryMP,
    Surge,
    IonShell,
    ShurikenToss,
    Reincarnation,
    Blur,
    Ensnare,
    Boomerang,
    Rage,
    DangerDash,
    ImageBorn,
    ClownFish,
    Symbiote,
    Cheater,
    SuperDash,
    Hide
}