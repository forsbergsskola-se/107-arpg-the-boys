using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponLoader : MonoBehaviour
{
    public Armory armory;
    private BaseWeapon[] _baseWeapons;
    private Armory.ArmoryWeapon _selectedWeapon;
    
    private PlayerCombat _playerCombat;
    
    void Awake()
    {
        _playerCombat = gameObject.GetComponent<PlayerCombat>();
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
        int equippedWeaponIndex = PlayerPrefs.GetInt("EquippedWeaponIndex", -1);
        if (equippedWeaponIndex == -1)
            Debug.Log("There is no saved weapon, equipping nothing.");
        
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
        int equippedWeaponIndex = -1;
        if (_playerCombat.currentWeapon != null)
        {
            equippedWeaponIndex = Array.FindIndex(armory.armoryWeapons, it => it.weapon.weaponID == _playerCombat.currentWeapon.weaponID);
        }
        PlayerPrefs.SetInt("EquippedWeaponIndex", equippedWeaponIndex);
    }
}
