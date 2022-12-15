using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour, IInteractable, IPickupable
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

    public void Interact()
    {
        throw new System.NotImplementedException();
    }

    public void Highlight()
    {
        throw new System.NotImplementedException();
    }
    
    public void Pickup(PlayerCombat playerCombat)
    {
        Vector3.Lerp(transform.position, playerCombat.weaponHolder.position, 1);
        transform.position = playerCombat.weaponHolder.position;
        transform.parent = playerCombat.weaponHolder;
        playerCombat.currentWeapon = this;
    }

    public void DropWeapon(PlayerCombat playerCombat)
    {
        playerCombat.currentWeapon = null;
        transform.parent = null;
    }
}
