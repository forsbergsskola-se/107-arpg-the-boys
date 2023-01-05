using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HubShop : MonoBehaviour, IInteractable
{
    public ShopWeapon[] ShopWeapons;
    private ShopWeapon selectedWeapon;

    // Reference to the shop UI panel
    public GameObject shopUIPanel;

    // Reference to the player
    public GameObject player;
    private PlayerCombat _playerCombat;
    private PlayerMovement _playerMovement;
    private Rigidbody _playerRb;

    private void Awake()
    {
        // Load the player's progress from PlayerPrefs
        _playerCombat = player.GetComponent<PlayerCombat>();
        _playerMovement = player.GetComponent<PlayerMovement>();
        _playerRb = player.GetComponent<Rigidbody>();
        LoadProgress();
    }

    public void Interact()
    {
        // Open the shop UI when the player interacts with the NPC
        OpenShopUI();
    }

    public void Highlight()
    {
        throw new System.NotImplementedException();
    }

    private void OpenShopUI()
    {
        shopUIPanel.SetActive(true);
        UpdateShopUI();
        _playerMovement.canMove = false;
        _playerRb.isKinematic = true;
    }

    public void CloseShopUI()
    {
        UpdateShopUI();
        shopUIPanel.SetActive(false);
        _playerMovement.canMove = true;
        _playerRb.isKinematic = false;
    }

    public void TryBuyOrEquipItem(int shopWeaponArraySpot)
    {
        selectedWeapon = ShopWeapons[shopWeaponArraySpot];
        if(selectedWeapon.IsBought)
            EquipItem(selectedWeapon);
        else
            BuyItem(selectedWeapon);
        SaveProgress();
    }
    
    public void BuyItem(ShopWeapon selectedWeapon)
    {
        // Check if the player has enough money to buy the selected weapon
        if (true/*money >= selectedWeapon.Cost*/)
        {
            // Deduct the cost from the player's money
            //money -= selectedWeapon.Cost;

            // Set the weapon's "isBought" field to true
            selectedWeapon.IsBought = true;

            // Save the player's progress
            SaveProgress();
        }
        else
        {
            // Display a message to the player telling them that they don't have enough money
            Debug.Log("You don't have enough money to buy this weapon!");
        }
        
        UpdateShopUI();
    }

    public void EquipItem(ShopWeapon selectedWeapon)
    {
        // Check if the selected weapon has been bought
        if (selectedWeapon.IsBought)
        {
            // Equip the weapon by calling the "EquipWeapon" method on the player's inventory script
            if (_playerCombat.currentWeapon != null)
            {
                Destroy(_playerCombat.currentWeapon.gameObject);
                _playerCombat.currentWeapon = null;
            }

            var instance = Instantiate(selectedWeapon.Weapon);
            instance.Pickup(_playerCombat);

            // Save the player's progress
            SaveProgress();
        }
        else
        {
            // Display a message to the player telling them that they need to buy the weapon first
            Debug.Log("You need to buy this weapon first!");
        }
        
        UpdateShopUI();
    }

    private void UpdateShopUI()
    {
        // Iterate through the shop weapons and update their UI elements (e.g. "Buy" button to "Equip" button)
        foreach (ShopWeapon weapon in ShopWeapons)
        {
            // Get the UI element for the current weapon
            Button weaponButton = weapon.ShopButton;

            // Check if the weapon has been bought
            if (weapon.IsBought)
            {
                // Change the button's text to "Equip"
                weaponButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
            }
            else
            {
                // Change the button's text to "Buy"
                weaponButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
            }
        }
    }

    private void LoadProgress()
    {
        // Iterate through the shop weapons
        for (int i = 0; i < ShopWeapons.Length; i++)
        {
            // Load the "isBought" field of the current weapon from PlayerPrefs
            ShopWeapons[i].IsBought = PlayerPrefs.GetInt("WeaponBought" + i, 0) == 1;
        }

        // Load the index of the equipped weapon from PlayerPrefs
        int equippedWeaponIndex = PlayerPrefs.GetInt("EquippedWeaponIndex");
        if (equippedWeaponIndex == -1)
            Debug.LogError("equippedWeaponIndex is invalid. The index is -1");
        
        if (equippedWeaponIndex >= 0 && equippedWeaponIndex < ShopWeapons.Length)
        {
            // Equip the weapon
            selectedWeapon = ShopWeapons[equippedWeaponIndex];
            var instance = Instantiate(selectedWeapon.Weapon);
            instance.Pickup(_playerCombat);
            Debug.Log("Picked up loaded weapon");
        }
    }

    private void SaveProgress()
    {
        // Iterate through the shop weapons
        for (int i = 0; i < ShopWeapons.Length; i++)
        {
            // Save the "isBought" field of the current weapon to PlayerPrefs
            PlayerPrefs.SetInt("WeaponBought" + i, ShopWeapons[i].IsBought ? 1 : 0);
        }

        // Save the index of the equipped weapon in the ShopWeapons array to PlayerPrefs
        int equippedWeaponIndex =
            Array.FindIndex(ShopWeapons, it => it.Weapon.weaponID == _playerCombat.currentWeapon.weaponID);
        PlayerPrefs.SetInt("EquippedWeaponIndex", equippedWeaponIndex);
    }
    
    [Serializable]
    public class ShopWeapon
    {
        public BaseWeapon Weapon;
        public Button ShopButton;
        public int Cost;
        public bool IsBought;
    }
}