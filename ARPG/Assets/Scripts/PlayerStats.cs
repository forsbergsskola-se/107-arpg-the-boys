using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    //hp variables
    public float baseMaxHealth = 100f;
    public float maxHealthModPerc = 1f;
    public float MaxHealth => baseMaxHealth * maxHealthModPerc;

    
    public float currentHealth; //for clamp functionality
    public float CurrentHealth
        { get => currentHealth; private set => currentHealth = Math.Clamp(value, 0, MaxHealth); } 

    //mana variables
    public float baseMana = 100f;
    public float maxManaModPerc = 1f;
    public float MaxMana => baseMana * maxManaModPerc;

    public float currentMana; // for clamp functionality
    public float CurrentMana
        { get => currentMana; set => currentMana = Math.Clamp(value, 0, MaxMana); }
    
    //player-bound attack variables
    public float basePower = 10f;
    public float powerModPerc = 1f;
    public float AttackPower => basePower * powerModPerc;

    public float critRate = 0.05f; //inherent percentile - doesn't need ModPerc
    public float critDamage = 2f; //inherent percentile - doesn't need ModPerc
    public float CritHit => AttackPower * critDamage;

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
    public float damageTakenPercentage = 1; //inherent percentile - doesn't need ModPerc
    public float evasionChance; //inherent percentile - doesn't need ModPerc
    
    //'element' variables
    
    //poison
    public float basePoisonDamage = 1f;
    public float poisonDamageModPerc = 1f;
    public float poisonLength = 5f;
    public float PoisonDamage => basePoisonDamage * poisonDamageModPerc;
    
    //fire
    public float baseFireDamage = 1f;
    public float fireDamageModPerc = 1f;
    public float fireLength = 5f;
    public float FireDamage => baseFireDamage * fireDamageModPerc;
    
    //ice
    public float baseIceDamage = 1f;
    public float iceDamageModPerc = 1f;
    public float iceLength = 5f;
    public float IceDamage => baseIceDamage * iceDamageModPerc;
    
    
    
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage*damageTakenPercentage;
    }

    void Start()
    {
        currentHealth = MaxHealth;
        currentMana = MaxMana;
    }
    
}
