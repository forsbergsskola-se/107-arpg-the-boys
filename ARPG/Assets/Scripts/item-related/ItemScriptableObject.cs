using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "ScriptableObjects/Item")]
public class ItemScriptableObject : ScriptableObject
{
    //"ModPerc" implies the value is a modifying percentage, where 1f = 100%, 1.5f = 150%, etc.
    

    [Header("HP Variables")] 
    public float baseMaxHealthChange;
    public float baseMaxHealthModPercChange;
    //stat 'MaxHealth' variable ignored - derived value.
    public float currentHealthChange; //recovery items should modify this.

    [Header("Mana Variables")]
    public float baseMaxManaChange;
    public float maxManaModPercChange;
    //stat 'MaxMana' variable ignored - derived value.
    public float currentManaChange; //recovery items should modify this.

    [Header("Player-Bound Attack Variables")]
    public float basePowerChange;
    public float powerModPercChange;
    //stat 'AttackPower' ignored - derived value.

    public float critRateChange;
    public float critDamageChange; //damage = critDamage * AttackPower, so keep above 1f for actual crits.
    //stat 'CritHit' ignored - derived value.

    [Header("Movement Speed Variables")]
    public float baseWalkMoveSpeedChange;
    public float baseRunMoveSpeedChange;
    public float moveSpeedModPercChange;
    //stat 'WalkMoveSpeed' ignored - derived value.
    //stat 'RunMoveSpeed' ignored - derived value.

    [Header("Dodge Variables")]
    public int dodgeChargesChange; 
    public int maxDodgeChargesChange;
    public float baseDodgeSpeedChange;
    public float dodgeSpeedModPercChange;
    //stat 'DodgeSpeed' ignored - derived value.

    [Header("Passive Damage Mitigation Variables")]
    public float damageTakenPercentageChange; // 1f=100% damage taken, 0.5f=50% damage taken, etc.
    public float evasionChanceChange; // 1f = 100% attacks dodged, 0.5f=50% attacks dodged.
    
    [Header("Element Variables")]
    [Header("Poison Variables")]
    public float basePoisonDamageChange;
    public float poisonDamageModPercChange;
    public float poisonLengthChange;
    //stat 'PoisonDamage' ignored - derived value.
    
    [Header("Fire Variables")]
    public float baseFireDamageChange;
    public float fireDamageModPercChange;
    public float fireLengthChange;
    //stat 'FireDamage' ignored - derived value.
    
    [Header("Ice Variables")]
    public float iceSlowdownPercentageChange; 
    public float iceLengthChange;
    
    //optional variables for any extra value changes one might want to add not included in above list.
    //really - just there as a 'just in case it's needed' thing.
    [Header("Optional variables")]
    public float optValue;
    public float optValueTwo;
}
