using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    //hp variables
    public float baseMaxHealth = 100f;
    public float maxHealthModPerc = 1f;
    public float MaxHealth => baseMaxHealth * maxHealthModPerc;
    
    private float _currentHealth; //for clamp functionality
    public float CurrentHealth
        { get => _currentHealth; private set => _currentHealth = Math.Clamp(value, 0, MaxHealth); } 

    //mana variables
    
    public float baseMaxMana = 100f;
    public float maxManaModPerc = 1f;
    public float MaxMana => baseMaxMana * maxManaModPerc;
    
    private float _currentMana; // for clamp functionality
    public float CurrentMana
        { get => _currentMana; private set => _currentMana = Math.Clamp(value, 0, MaxMana); }
    
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
    public float iceSlowdownPercentage = 0.2f; //inherent percentile - doesn't need ModPerc
    public float iceLength = 5f;
    
    
    //methods
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage*damageTakenPercentage;
    }

    public void ChangeMana(float change)
    {
        CurrentMana -= change;
    }

    void Start()
    {
        _currentHealth = MaxHealth;
        _currentMana = MaxMana;
    }
    
}
