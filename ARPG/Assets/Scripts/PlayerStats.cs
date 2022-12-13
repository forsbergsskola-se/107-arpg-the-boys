using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [Header("HP Variables")]
    public float baseMaxHealth = 100f;
    public float maxHealthModPerk = 1f;
    public float MaxHealth => baseMaxHealth * maxHealthModPerk;
    
    private float _currentHealth; //for clamp functionality
    public float CurrentHealth
        { get => _currentHealth; private set => _currentHealth = Math.Clamp(value, 0, MaxHealth); } 

    [Header("Mana Variables")]
    public float baseMaxMana = 100f;
    public float maxManaModPerk = 1f;
    public float MaxMana => baseMaxMana * maxManaModPerk;
    
    private float _currentMana; // for clamp functionality
    public float CurrentMana
        { get => _currentMana; private set => _currentMana = Math.Clamp(value, 0, MaxMana); }
    
    [Header("Player-Bound Attack Variables")]
    public float basePower = 10f;
    public float powerModPerk = 1f;
    public float AttackPower => basePower * powerModPerk;

    public float critRate = 0.05f; //inherent percentile - doesn't need ModPerc
    public float critDamage = 2f; //inherent percentile - doesn't need ModPerc
    public float CritHit => AttackPower * critDamage;
    
    [Header("Movement Speed Variables")]
    public float baseWalkMoveSpeed = 5f;
    public float baseRunMoveSpeed = 11f;
    public float moveSpeedModPerk = 1f;
    public float WalkMoveSpeed => baseWalkMoveSpeed * moveSpeedModPerk; 
    public float RunMoveSpeed => baseRunMoveSpeed * moveSpeedModPerk; 

    
    
    [NonSerialized]
    public int dodgesCharges;
    [Header("Dodge Variables")]
    public int maxDodgeCharges;

    public float baseDodgeSpeed = 20f;
    public float dodgeSpeedModPerk = 1f;
    public float DodgeSpeed => baseDodgeSpeed * dodgeSpeedModPerk;
    
    [Header("Passive Damage Mitigation Variables")]
    public float damageTakenPercentage = 1; //inherent percentile - doesn't need ModPerk
    public float evasionChance; //inherent percentile - doesn't need ModPerk
    
    //'element' variables
    [Header("Element Variables")]
    [Header("Poison Variables")]
    public float basePoisonDamage = 1f;
    public float poisonDamageModPerk = 1f;
    public float poisonLength = 5f;
    public float PoisonDamage => basePoisonDamage * poisonDamageModPerk;
    
    [Header("Fire Variables")]
    public float baseFireDamage = 1f;
    public float fireDamageModPerk = 1f;
    public float fireLength = 5f;
    public float FireDamage => baseFireDamage * fireDamageModPerk;
    
    [Header("Ice Variables")]
    public float iceSlowdownPercentage = 0.2f; //inherent percentile - doesn't need ModPerk
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
    
    public void AddDash() 
    {
        Math.Clamp(dodgesCharges + 1, 0, maxDodgeCharges);
    }

    void Start()
    {
        _currentHealth = MaxHealth;
        _currentMana = MaxMana;
    }
    
}
