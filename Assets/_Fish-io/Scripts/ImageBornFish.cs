using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageBornFish : MonoBehaviour
{
    [SerializeField] Weapon[] weapons;
    public Fish owner;
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    public void SetActiveWeapon(WeaponType weaponType)
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i].weaponType == weaponType)
            {
                weapons[i].gameObject.SetActive(true);
            }
            else
            {
                weapons[i].gameObject.SetActive(false);
            }
        }
    }
    public void Init(Fish _owner, WeaponType weaponType, float duration)
    {
        weapons = GetComponentsInChildren<Weapon>();
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].ChangeTag("ImageBorn");
        }
        owner = _owner;
        SetActiveWeapon(weaponType);
        //transform.localScale = _owner.transform.localScale;
       
        //transform.parent = _owner.imageBornParent.transform;
        transform.localPosition = Vector3.zero;

        transform.localEulerAngles = new Vector3(0, 90, 0);
        Destroy(gameObject, duration);
    }
}
