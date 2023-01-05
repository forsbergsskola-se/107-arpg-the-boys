using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HubShop : MonoBehaviour, IInteractable
{
    public Armory armory;
    public ShopWeapon[] shopWeapons;
    private ShopWeapon _selectedWeapon;

    // Reference to the shop UI panel
    public GameObject shopUIPanel;

    // Reference to the player
    public GameObject player;
    private PlayerCombat _playerCombat;
    private PlayerMovement _playerMovement;
    private PlayerWeaponLoader _weaponLoader;
    private Rigidbody _playerRb;

    private void Awake()
    {
        // Load the player's progress from PlayerPrefs
        _playerCombat = player.GetComponent<PlayerCombat>();
        _playerMovement = player.GetComponent<PlayerMovement>();
        _playerRb = player.GetComponent<Rigidbody>();
        _weaponLoader.LoadProgress();
        for (var i = 0; i < armory.armoryWeapons.Length; i++)
        {
            shopWeapons[i].weapon = armory.armoryWeapons[i].weapon;
            shopWeapons[i].isBought = armory.armoryWeapons[i].isBought;
            
        }
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
        _selectedWeapon = shopWeapons[shopWeaponArraySpot];
        if(_selectedWeapon.isBought)
            EquipItem(_selectedWeapon);
        else
            BuyItem(_selectedWeapon);
        _weaponLoader.SaveProgress();
    }
    
    public void BuyItem(ShopWeapon selectedWeapon)
    {
        // Check if the player has enough money to buy the selected weapon
        if (true/*money >= selectedWeapon.Cost*/)
        {
            // Deduct the cost from the player's money
            //money -= selectedWeapon.Cost;

            // Set the weapon's "isBought" field to true
            selectedWeapon.isBought = true;

            // Save the player's progress
            _weaponLoader.SaveProgress();
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
        if (selectedWeapon.isBought)
        {
            // Equip the weapon by calling the "EquipWeapon" method on the player's inventory script
            if (_playerCombat.currentWeapon != null)
            {
                Destroy(_playerCombat.currentWeapon.gameObject);
                _playerCombat.currentWeapon = null;
            }

            var instance = Instantiate(selectedWeapon.weapon);
            instance.Pickup(_playerCombat);

            // Save the player's progress
            _weaponLoader.SaveProgress();
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
        foreach (ShopWeapon weapon in shopWeapons)
        {
            // Get the UI element for the current weapon
            Button weaponButton = weapon.shopButton;

            // Check if the weapon has been bought
            if (weapon.isBought)
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

    [Serializable]
    public class ShopWeapon
    {
        public BaseWeapon weapon;
        public Button shopButton;
        public int cost;
        public bool isBought;
    }
}