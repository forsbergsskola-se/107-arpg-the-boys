using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponLoader : MonoBehaviour
{
    public Armory armory;
    private BaseWeapon[] _baseWeapons;
    private Armory.ArmoryWeapon _selectedWeapon;
    
    public GameObject player;
    private PlayerCombat _playerCombat;
    
    void Awake()
    {
        _playerCombat = player.GetComponent<PlayerCombat>();
        
        for (var i = 0; i < armory.armoryWeapons.Length; i++)
        {
            _baseWeapons[i] = armory.armoryWeapons[i].weapon.GetComponent<BaseWeapon>();
        }
    }

    public void LoadProgress()
    {
        // Iterate through the shop weapons
        for (int i = 0; i <  armory.armoryWeapons.Length; i++)
        {
            // Load the "isBought" field of the current weapon from PlayerPrefs
            armory.armoryWeapons[i].isBought = PlayerPrefs.GetInt("WeaponBought" + i, 0) == 1;
        }
    
        // Load the index of the equipped weapon from PlayerPrefs
        int equippedWeaponIndex = PlayerPrefs.GetInt("EquippedWeaponIndex");
        if (equippedWeaponIndex == -1)
            Debug.LogError("equippedWeaponIndex is invalid. The index is -1");
        
        if (equippedWeaponIndex >= 0 && equippedWeaponIndex < armory.armoryWeapons.Length)
        {
            // Equip the weapon
            _selectedWeapon = armory.armoryWeapons[equippedWeaponIndex];
            var instance = Instantiate(_selectedWeapon.weapon);
            instance.Pickup(_playerCombat);
            Debug.Log("Picked up loaded weapon");
        }
    }
    
    public void SaveProgress()
    {
        // Iterate through the shop weapons
        for (int i = 0; i < armory.armoryWeapons.Length; i++)
        {
            // Save the "isBought" field of the current weapon to PlayerPrefs
            PlayerPrefs.SetInt("WeaponBought" + i, armory.armoryWeapons[i].isBought ? 1 : 0);
        }
    
        // Save the index of the equipped weapon in the ShopWeapons array to PlayerPrefs
        int equippedWeaponIndex =
            Array.FindIndex(armory.armoryWeapons, it => it.weapon.weaponID == _playerCombat.currentWeapon.weaponID);
        PlayerPrefs.SetInt("EquippedWeaponIndex", equippedWeaponIndex);
    }
}
