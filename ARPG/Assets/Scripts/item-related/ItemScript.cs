using System;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public ItemScriptableObject itemSo;
    [Header("if this is disabled, item power will curve towards zero for this item type.")]
    public bool isLinearScaling;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerStats>())
        {
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            ItemPickedUp(playerStats);
            Debug.Log($"{gameObject} collided with {collision.gameObject} and was destroyed.");
            Destroy(gameObject);
        }
    }
    float Scale(int numItems, float power)
    {
        if (numItems == 0)
        {
            return power;
        }
        return power / (1.0f + MathF.Sqrt(numItems));
    }

    private int _itemCountPlaceHold = 0; //todo: replace all instances of _itemCountPlaceHold with actual inventory count of item
    private void ItemPickedUp(PlayerStats plStat)
    {
        if(isLinearScaling)
        {
            //hp
            plStat.baseMaxHealth +=  itemSo.baseMaxHealthChange;
            plStat.maxHealthModPerc += itemSo.baseMaxHpModPercChange;
            plStat.TakeDamage(itemSo.currentHealthChange);
            //mp
            plStat.baseMaxMana += itemSo.baseMaxManaChange;
            plStat.maxManaModPerc += itemSo.maxMpModPercChange;
            plStat.ChangeMana(itemSo.currentManaChange);
            //atk
            plStat.basePower += itemSo.basePowerChange;
            plStat.powerModPerc += itemSo.powerModPercChange;
            //crit
            plStat.critRate += itemSo.critRateChange;
            plStat.critDamage += itemSo.critDamageChange;
            //ranged
            plStat.baseRangedRange += itemSo.baseRangedRangeChange;
            plStat.rangedRangeModPerc += itemSo.rangedRangeModPercChange;
            plStat.baseRangePower += itemSo.baseRangePowerChange;
            plStat.rangePowerModPerc += itemSo.rangePowerModPercChange;
            //move speed
            plStat.baseWalkMoveSpeed += itemSo.baseWalkMoveSpeedChange;
            plStat.baseRunMoveSpeed += itemSo.baseRunMoveSpeedChange;
            plStat.moveSpeedModPerc += itemSo.moveSpeedModPercChange;
            //dodge
            plStat.AddDodge(itemSo.dodgeChargesChange);
            plStat.maxDodgeCharges += itemSo.maxDodgeChargesChange;
            plStat.baseDodgeSpeed += itemSo.baseDodgeSpeedChange;
            plStat.dodgeSpeedModPerc += itemSo.dodgeSpeedModPercChange;
            plStat.baseDodgeRange += itemSo.baseDodgeRangeChange;
            plStat.dodgeRangeModPerc += itemSo.dodgeRangeModPercChange;
            //mitigation
            plStat.dmgTakePerc += itemSo.dmgTakePerc;
            plStat.evasionChance += itemSo.evasionChanceChange;
            //elements
            //poison
            plStat.basePoisonDamage += itemSo.basePoisonDamageChange;
            plStat.poisonDmgModPerc += itemSo.poisonDmgModPercChange;
            plStat.poisonLength += itemSo.poisonLengthChange;
            //fire
            plStat.baseFireDamage += itemSo.baseFireDamageChange;
            plStat.fireDmgModPerc += itemSo.fireDmgModPercChange;
            plStat.fireLength += itemSo.fireLengthChange;
            //ice
            plStat.iceSlowPerc += itemSo.iceSlowPercChange;
            plStat.iceLength += itemSo.iceLengthChange;
        }
        else
        {
            //hp
            plStat.baseMaxHealth += Scale(_itemCountPlaceHold,itemSo.baseMaxHealthChange);
            plStat.maxHealthModPerc += Scale(_itemCountPlaceHold,itemSo.baseMaxHpModPercChange);
            plStat.TakeDamage(Scale(_itemCountPlaceHold,itemSo.currentHealthChange));
            //mp
            plStat.baseMaxMana += Scale(_itemCountPlaceHold,itemSo.baseMaxManaChange);
            plStat.maxManaModPerc += Scale(_itemCountPlaceHold,itemSo.maxMpModPercChange);
            plStat.ChangeMana(Scale(_itemCountPlaceHold,itemSo.currentManaChange));
            //atk
            plStat.basePower += Scale(_itemCountPlaceHold,itemSo.basePowerChange);
            plStat.powerModPerc += Scale(_itemCountPlaceHold,itemSo.powerModPercChange);
            //crit
            plStat.critRate += Scale(_itemCountPlaceHold,itemSo.critRateChange);
            plStat.critDamage += Scale(_itemCountPlaceHold,itemSo.critDamageChange);
            //ranged
            plStat.baseRangedRange += Scale(_itemCountPlaceHold,itemSo.baseRangedRangeChange);
            plStat.rangedRangeModPerc += Scale(_itemCountPlaceHold,itemSo.rangedRangeModPercChange);
            plStat.baseRangePower += Scale(_itemCountPlaceHold,itemSo.baseRangePowerChange);
            plStat.rangePowerModPerc += Scale(_itemCountPlaceHold,itemSo.rangePowerModPercChange);
            //move speed
            plStat.baseWalkMoveSpeed += Scale(_itemCountPlaceHold,itemSo.baseWalkMoveSpeedChange);
            plStat.baseRunMoveSpeed += Scale(_itemCountPlaceHold,itemSo.baseRunMoveSpeedChange);
            plStat.moveSpeedModPerc += Scale(_itemCountPlaceHold,itemSo.moveSpeedModPercChange);
            //dodge
            plStat.AddDodge(itemSo.dodgeChargesChange); //dodgeCharges cannot be scaled - is int.
            plStat.maxDodgeCharges += itemSo.maxDodgeChargesChange; //dodgeCharges cannot be scaled - is int.
            plStat.baseDodgeSpeed += Scale(_itemCountPlaceHold,itemSo.baseDodgeSpeedChange);
            plStat.dodgeSpeedModPerc += Scale(_itemCountPlaceHold,itemSo.dodgeSpeedModPercChange);
            //mitigation
            plStat.dmgTakePerc += Scale(_itemCountPlaceHold,itemSo.dmgTakePerc);
            plStat.evasionChance += Scale(_itemCountPlaceHold,itemSo.evasionChanceChange);
            //elements
            //poison
            plStat.basePoisonDamage += Scale(_itemCountPlaceHold,itemSo.basePoisonDamageChange);
            plStat.poisonDmgModPerc += Scale(_itemCountPlaceHold,itemSo.poisonDmgModPercChange);
            plStat.poisonLength += Scale(_itemCountPlaceHold,itemSo.poisonLengthChange);
            //fire
            plStat.baseFireDamage += Scale(_itemCountPlaceHold,itemSo.baseFireDamageChange);
            plStat.fireDmgModPerc += Scale(_itemCountPlaceHold,itemSo.fireDmgModPercChange);
            plStat.fireLength += Scale(_itemCountPlaceHold,itemSo.fireLengthChange);
            //ice
            plStat.iceSlowPerc += Scale(_itemCountPlaceHold,itemSo.iceSlowPercChange);
            plStat.iceLength += Scale(_itemCountPlaceHold,itemSo.iceLengthChange);
        }
    }

}
