using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatAnimations : MonoBehaviour
{
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;

    public void EndAnimation()
    {
        playerCombat.animationEnded = true;
    }
    
    public void LightAttackEvent()
    {
        playerCombat.AttackBox(playerCombat.currentWeapon.lightAttackColSize, playerCombat.currentWeapon.lightAttackColOffset, playerCombat.currentWeapon.lightAttackDamage);
    }
    
    public void HeavyAttackEvent()
    {
        playerCombat.AttackBox(playerCombat.currentWeapon.heavyAttackColSize, playerCombat.currentWeapon.heavyAttackColOffset, playerCombat.currentWeapon.heavyAttackDamage);
    }

    public void PlayWhooshSound()
    {
        if (playerCombat.CurrentAttackState == IInterruptible.AttackState.LightAttack)
        {
            playerCombat.audioSource.pitch = .6f + playerCombat.currentWeapon.lightAttackSpeed * .5f;
        }
        if (playerCombat.CurrentAttackState == IInterruptible.AttackState.HeavyAttack)
        {
            playerCombat.audioSource.pitch = .5f + playerCombat.currentWeapon.heavyAttackSpeed * .5f;
        }
        playerCombat.audioSource.clip = playerCombat.GetRandomAudioClip(playerCombat.slashWhoosh);
        playerCombat.audioSource.Play();
    }

    public void OnInterruptExit()
    {
        playerMovement.canMove = true;
        playerMovement.playerAnimator.speed = 1f;
    }
    
    public void OnInterruptEnter()
    {
        playerMovement.playerAnimator.speed = 2f;
        playerMovement._rb.velocity = Vector3.zero;
        playerMovement.canMove = false;
    }
    
}
