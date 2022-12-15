using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

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
        StartCoroutine(LerpToHand(3f, playerCombat));
        transform.parent = playerCombat.weaponHolder;
        playerCombat.currentWeapon = this;
    }

    public void DropWeapon(PlayerCombat playerCombat)
    {
        playerCombat.currentWeapon = null;
        transform.parent = null;
    }

    private IEnumerator LerpToHand(float time, PlayerCombat playerCombat)
    {
        Vector3 localDist = transform.position - playerCombat.transform.position;
        Quaternion localRot = Quaternion.Euler(playerCombat.transform.InverseTransformVector(transform.rotation.eulerAngles));
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float elapsed01 = Mathf.Clamp01(elapsedTime / time);
            //Quaternion.Lerp(playerCombat.transform.rotation * localRot, playerCombat.weaponHolder.rotation, elapsed01);
            Vector3.Lerp(playerCombat.transform.position + localDist, playerCombat.weaponHolder.position, elapsed01);
            yield return null;
        }
        transform.position = playerCombat.weaponHolder.position;
        transform.rotation = playerCombat.weaponHolder.rotation;
    }
}