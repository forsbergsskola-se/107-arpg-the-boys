using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    //todo: privatize fields - public only for testing in editor!
    
    //hp variables
    private float BaseHealth => 100f;
    public float maxHealthModPerc = 1f;
    public float maxHealth; //todo: make function of base*ModPerc
    public float CurrentHealth { get; private set; }
    
    //mana variables
    private float BaseMana => 100f;
    public float maxManaModPerc = 1f;
    public float maxMana; //todo: make function of base*ModPerc
    public float currentMana;
    
    //player-bound attack variables
    private float BasePower => 10f;
    public float powerModPerc = 1f;
    public float attackPower; //todo: make function of base*ModPerc

    public float critRate = 0.05f; //inherent percentile - doesn't need ModPerc
    public float critDamage = 2f; //inherent percentile - doesn't need ModPerc
    //todo: apply crit to attacks
    
    //move speed variables
    public float baseMoveSpeed;
    public float moveSpeedModPerc = 1f;
    public float moveSpeed; //todo: make function of base*ModPerc

    //dodge variables
    public int dodgeCharges = 1;
    
    public float baseDodgeLength;
    public float dodgeLengthModPerc = 1f;
    public float dodgeLength; //todo: make function of base*ModPerc
    
    public float baseDodgeSpeed;
    public float dodgeSpeedModPerc = 1f;
    public float dodgeSpeed; //todo: make function of base*ModPerc
    
    
    //passive damage mitigation variables
    public float damageReductionPercentage; //inherent percentile - doesn't need ModPerc
    public float evasionChance; //inherent percentile - doesn't need ModPerc
    
    //effect over time variables
    //no idea on how to actually treat these - percentiles? base stat nums? 
    public float eotLength; 
    public float eotDamage;
    
    

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage*damageReductionPercentage;
    }

    public void ManaUsePlaceholder(float manaUsed)
    {
        currentMana -= manaUsed;
    }
    
    
    
}
