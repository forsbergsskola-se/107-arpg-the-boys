using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
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
    public float lightAttackSpeed = 1.5f;
    public Vector3 lightAttackColSize;
    public Vector3 lightAttackColOffset;
    
    [Header("Heavy Attack Stats")] 
    public float heavyAttackDamage = 20;
    public float heavyAttackSpeed = 0.5f;
    public Vector3 heavyAttackColSize;
    public Vector3 heavyAttackColOffset;

    [Header("Guard Stats")]
    public float guardTime;
    public float parryPunishRange = 3;
    public float parryTime;

    [Header("Appearance")] 
    public Vector3 modelPosOffset;
    public Vector3 modelRotOffset;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Interact() // Interact is never called on the weapon, pickup and drop are directly called on the correct weapons from the PlayerInteract script
    {
        throw new System.NotImplementedException();
    }

    public void Highlight()
    {
        throw new System.NotImplementedException();
    }
    
    public void Pickup(PlayerCombat playerCombat)
    {
        StartCoroutine(LerpToHand(0.25f, playerCombat));
        _rb.useGravity = false;
        _rb.detectCollisions = false;
        _rb.isKinematic = true;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        transform.parent = playerCombat.weaponHolder;
        playerCombat.currentWeapon = this;
    }

    public void DropWeapon(PlayerCombat playerCombat)
    {
        playerCombat.currentWeapon = null;
        _rb.useGravity = true;
        _rb.detectCollisions = true;
        _rb.isKinematic = false;
        transform.parent = null;
    }

    private IEnumerator LerpToHand(float time, PlayerCombat playerCombat)
    {
        Vector3 localDist = transform.position - playerCombat.transform.position;
        Quaternion localRot = transform.rotation;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float elapsed01 = Mathf.Clamp01(elapsedTime / time);
            transform.rotation = Quaternion.Lerp(localRot, GetHoldRot(playerCombat), elapsed01);
            transform.position = Vector3.Lerp(playerCombat.transform.TransformPoint(localDist), GetHoldPos(playerCombat), elapsed01);
            yield return null;
        }
        transform.position = GetHoldPos(playerCombat);
        transform.rotation = GetHoldRot(playerCombat);
    }

    Vector3 GetHoldPos(PlayerCombat playerCombat)
    {
        return playerCombat.weaponHolder.TransformPoint(modelPosOffset);
    }
    
    Quaternion GetHoldRot(PlayerCombat playerCombat)
    {
        return playerCombat.weaponHolder.rotation * Quaternion.Euler(modelRotOffset);
    }
}