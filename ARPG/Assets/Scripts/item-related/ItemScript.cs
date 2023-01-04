using System;
using Unity.Mathematics;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public ItemScriptableObject itemSo;
    [Header("if this is disabled, item power will curve towards zero for this item type.")]
    public bool isLinearScaling;
    
    private void OnCollisionEnter(Collision collision) //can be replaced with other pick-up logic if required
    {
        if (collision.gameObject.GetComponent<PlayerStats>())
        {
            //get necessary components
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            PlayerInventory playerInventory = collision.gameObject.GetComponent<PlayerInventory>();
            heldCount = playerInventory.GetItemCount(itemSo.name);//get # of items held from player inventory
            ItemPickedUp(playerStats); // runs pick-up method
            playerInventory.UpdateItemCount(itemSo.name); // updates # held
            Debug.Log($"{gameObject} collided with {collision.gameObject} and was destroyed.");
            Destroy(gameObject); //kills object
        }
    }
    /* todo: method for selling items.
     method should be calling 'ItemPickedUp' so that value decreases instead of increases.
     increase/decrease should employ scaling curve and update items held count as appropriate.
     see trello backlog entry 'sell item method'.
    */

    float Scale(int numItems, float power) //scaling math stuff
    {
        if (numItems == 0)
        {
            return power;
        }
        return power / (1.0f + MathF.Sqrt(numItems));
    }

    public int heldCount;
    private void ItemPickedUp(PlayerStats plStat) //could probably clean this up, but... it works. so i won't.
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
            //recovery
            plStat.hpRecovModPerc += itemSo.hpRecovModPercChange;
            plStat.mpRecovModPerc += itemSo.mpRecovModPercChange;
            //atk
            plStat.basePower += itemSo.basePowerChange;
            plStat.powerModPerc += itemSo.powerModPercChange;
            //crit
            plStat.critRate += itemSo.critRateChange;
            plStat.critDamage += itemSo.critDamageChange;
            //heavy/light attacks
            plStat.lightAtkModPerc += itemSo.lightAtkModPercChange;
            plStat.lightAtkSpeedPerc += itemSo.lightAtkSpeedPercChange;
            plStat.heavyAtkModPerc += itemSo.heavyAtkModPercChange;
            plStat.heavyAtkSpeedPerc += itemSo.heavyAtkSpeedPercChange;
            //block
            plStat.guardTimeModPerc += itemSo.guardTimeModPercChange;
            plStat.guardPunishModPerc += itemSo.guardPunishModPercChange;
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
            plStat.baseMaxHealth += Scale(heldCount,itemSo.baseMaxHealthChange);
            plStat.maxHealthModPerc += Scale(heldCount,itemSo.baseMaxHpModPercChange);
            plStat.TakeDamage(Scale(heldCount,itemSo.currentHealthChange));
            //mp
            plStat.baseMaxMana += Scale(heldCount,itemSo.baseMaxManaChange);
            plStat.maxManaModPerc += Scale(heldCount,itemSo.maxMpModPercChange);
            plStat.ChangeMana(Scale(heldCount,itemSo.currentManaChange));
            //recovery
            plStat.hpRecovModPerc +=Scale(heldCount, itemSo.hpRecovModPercChange);
            plStat.mpRecovModPerc +=Scale(heldCount, itemSo.mpRecovModPercChange);
            //atk
            plStat.basePower += Scale(heldCount,itemSo.basePowerChange);
            plStat.powerModPerc += Scale(heldCount,itemSo.powerModPercChange);
            //crit
            plStat.critRate += Scale(heldCount,itemSo.critRateChange);
            plStat.critDamage += Scale(heldCount,itemSo.critDamageChange);
            //heavy/light attacks
            plStat.lightAtkModPerc +=Scale(heldCount,itemSo.lightAtkModPercChange);
            plStat.lightAtkSpeedPerc +=Scale(heldCount,itemSo.lightAtkSpeedPercChange);
            plStat.heavyAtkModPerc +=Scale(heldCount,itemSo.heavyAtkModPercChange);
            plStat.heavyAtkSpeedPerc +=Scale(heldCount,itemSo.heavyAtkSpeedPercChange);
            //block
            plStat.guardTimeModPerc +=Scale(heldCount, itemSo.guardTimeModPercChange);
            plStat.guardPunishModPerc +=Scale(heldCount, itemSo.guardPunishModPercChange);
            //ranged
            plStat.baseRangedRange += Scale(heldCount,itemSo.baseRangedRangeChange);
            plStat.rangedRangeModPerc += Scale(heldCount,itemSo.rangedRangeModPercChange);
            plStat.baseRangePower += Scale(heldCount,itemSo.baseRangePowerChange);
            plStat.rangePowerModPerc += Scale(heldCount,itemSo.rangePowerModPercChange);
            //move speed
            plStat.baseWalkMoveSpeed += Scale(heldCount,itemSo.baseWalkMoveSpeedChange);
            plStat.baseRunMoveSpeed += Scale(heldCount,itemSo.baseRunMoveSpeedChange);
            plStat.moveSpeedModPerc += Scale(heldCount,itemSo.moveSpeedModPercChange);
            //dodge
            plStat.AddDodge(itemSo.dodgeChargesChange); //dodgeCharges cannot be scaled - is int.
            plStat.maxDodgeCharges += itemSo.maxDodgeChargesChange; //dodgeCharges cannot be scaled - is int.
            plStat.baseDodgeSpeed += Scale(heldCount,itemSo.baseDodgeSpeedChange);
            plStat.dodgeSpeedModPerc += Scale(heldCount,itemSo.dodgeSpeedModPercChange);
            //mitigation
            plStat.dmgTakePerc += Scale(heldCount,itemSo.dmgTakePerc);
            plStat.evasionChance += Scale(heldCount,itemSo.evasionChanceChange);
            //elements
            //poison
            plStat.basePoisonDamage += Scale(heldCount,itemSo.basePoisonDamageChange);
            plStat.poisonDmgModPerc += Scale(heldCount,itemSo.poisonDmgModPercChange);
            plStat.poisonLength += Scale(heldCount,itemSo.poisonLengthChange);
            //fire
            plStat.baseFireDamage += Scale(heldCount,itemSo.baseFireDamageChange);
            plStat.fireDmgModPerc += Scale(heldCount,itemSo.fireDmgModPercChange);
            plStat.fireLength += Scale(heldCount,itemSo.fireLengthChange);
            //ice
            plStat.iceSlowPerc += Scale(heldCount,itemSo.iceSlowPercChange);
            plStat.iceLength += Scale(heldCount,itemSo.iceLengthChange);
        }
        Debug.Log("items held value in item script: " + heldCount);
    }

    public float rotationSpeed;
    private void Update()
    {
        transform.Rotate(transform.up, rotationSpeed);
    }
}
