using System;
using UnityEngine;

public class SlashScript : MonoBehaviour
{
    private PlayerStats _playerStats;
    private PlayerCombat _playerCombat;

    private void Start()
    {
        _playerStats = FindObjectOfType<PlayerStats>();
        _playerCombat = FindObjectOfType<PlayerCombat>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Enemy damageable))
        {
            float damage = _playerCombat.currentWeapon.magicAttackDamage * _playerStats.RangePower;
            damageable.TakeDamage(damage);
        }
    }
}
