using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatAnimations : MonoBehaviour
{
    public PlayerCombat playerCombat;

    public void EndAnimation()
    {
        playerCombat.animationEnded = true;
    }
    
    public void LightAttackEvent()
    {
        playerCombat.AttackBox(playerCombat.currentWeapon.lightAttackColSize, playerCombat.currentWeapon.lightAttackDamage, true);
    }
    
    public void HeavyAttackEvent()
    {
        playerCombat.AttackBox(playerCombat.currentWeapon.heavyAttackColSize, playerCombat.currentWeapon.heavyAttackDamage, true);
    }

    public void PlayWhooshSound()
    {
        playerCombat.audioSource.clip = playerCombat.GetRandomAudioClip(playerCombat.slashWhoosh);
        playerCombat.audioSource.Play();
    }
}
