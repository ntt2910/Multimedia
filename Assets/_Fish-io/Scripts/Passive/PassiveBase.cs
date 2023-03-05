using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveBase : MonoBehaviour
{
    public PassiveType passiveType;
    protected int eatActive = 0, killActive = 0;
    protected float duration = 0;
    int currentEat = 0, currentKill = 0;
    public void LoadData()
    {
        PassiveObjectData passiveObjectData = GameController.instance.passiveDatabase.GetPassiveObjectData(passiveType);
        eatActive = passiveObjectData.eatActive;
        killActive = passiveObjectData.killActive;
        duration = passiveObjectData.duration;
    }  
    public void OnUpdateEat(int amount)
    {
        if (eatActive == 0) return;
        currentEat += amount;
        if (currentEat >= eatActive)
        {
            OnPassiveActive();
            currentEat = 0;
        }
    }    
    public void OnUpdateKill(int amount)
    {
        if (killActive == 0) return;

        currentKill += amount;
        if (currentKill >= killActive)
        {
            OnPassiveActive();
            currentKill = 0;
        }
    }    
    public virtual void OnPassiveActive()
    {
        Debug.Log("Passive Active");
    }    
}
