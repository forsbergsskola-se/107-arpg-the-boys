using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    [Header("General Weapon Stats")] 
    public float critRate = 0.1f;
    public float critDamageBonus = 1.5f;

    [Header("Light Attack Stats")] 
    public float lightAttackDamage = 10;
    public float lightAttackCooldown = 0.5f;
    public Vector3 lightAttackColSize;
    
    [Header("Heavy Attack Stats")] 
    public float heavyAttackDamage = 20;
    public float heavyAttackCooldown = 1;
    public Vector3 heavyAttackColSize;

    [Header("Guard Stats")]
    public float guardTime;
    public float guardPunish;
    public float parryTime;

}
