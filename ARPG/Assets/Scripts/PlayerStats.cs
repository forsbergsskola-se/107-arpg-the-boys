using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    //hp variables
    public float baseMaxHealth = 100f;
    public float maxHealthModPerc = 1f;
    private float MaxHealth => baseMaxHealth * maxHealthModPerc;

    
    private float _currentHealth; //for clamp functionality
    public float CurrentHealth
        { get => _currentHealth; private set => _currentHealth = Math.Clamp(value, 0, MaxHealth); } 

    //mana variables
    public float baseMana = 100f;
    public float maxManaModPerc = 1f;
    private float MaxMana => baseMana * maxManaModPerc;

    private float _currentMana; // for clamp functionality
    public float CurrentMana
        { get => _currentMana; set => _currentMana = Math.Clamp(value, 0, MaxMana); }
    
    //player-bound attack variables
    public float basePower = 10f;
    public float powerModPerc = 1f;
    public float AttackPower => basePower * powerModPerc;

    public float critRate = 0.05f; //inherent percentile - doesn't need ModPerc
    public float critDamage = 2f; //inherent percentile - doesn't need ModPerc
    //todo: apply crit to attacks
    
    //move speed variables
    public float baseMoveSpeed;
    public float moveSpeedModPerc = 1f;
    public float MoveSpeed => baseMoveSpeed * moveSpeedModPerc; 

    //dodge variables
    public int dodgeCharges = 1;
    
    public float baseDodgeLength;
    public float dodgeLengthModPerc = 1f;
    public float DodgeLength => baseDodgeLength * dodgeLengthModPerc;
    
    public float baseDodgeSpeed;
    public float dodgeSpeedModPerc = 1f;
    public float DodgeSpeed => baseDodgeSpeed * dodgeSpeedModPerc;
    
    
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

}
