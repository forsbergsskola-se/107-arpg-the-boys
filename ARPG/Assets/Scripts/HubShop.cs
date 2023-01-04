using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HubShop : MonoBehaviour, IInteractable
{
    public ShopWeapon[] ShopWeapons;

    // Reference to the shop UI panel
    public GameObject shopUIPanel;

    // Reference to the player's inventory script
    public PlayerCombat playerCombat;

    private void Start()
    {
        // Load the player's progress from PlayerPrefs
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
    }

    private void CloseShopUI()
    {
        shopUIPanel.SetActive(false);
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
            // Instantiate the selected weapon
            Instantiate(selectedWeapon.Weapon.gameObject, playerCombat.transform.position, Quaternion.identity);
            
            // Equip the weapon by calling the "EquipWeapon" method on the player's inventory script
            if (playerCombat.currentWeapon != null)
            {
                Destroy(playerCombat.currentWeapon);
                playerCombat.currentWeapon = null;
            }
            selectedWeapon.Weapon.Pickup(playerCombat);

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
                weaponButton.GetComponentInChildren<Text>().text = "Equip";
            }
            else
            {
                // Change the button's text to "Buy"
                weaponButton.GetComponentInChildren<Text>().text = "Buy";
            }
        }
    }

    private void LoadProgress()
    {
        // Iterate through the shop weapons
        for (int i = 0; i < ShopWeapons.Length; i++)
        {
            // Check if the player has bought the current weapon
            if (PlayerPrefs.GetInt("WeaponBought" + i, 0) == 1)
            {
                // Set the weapon's "isBought" field to true
                ShopWeapons[i].IsBought = true;
            }
            else
            {
                // Set the weapon's "isBought" field to false
                ShopWeapons[i].IsBought = false;
            }
        }

        // Load the player's equipped weapon
        string equippedWeapon = PlayerPrefs.GetString("EquippedWeapon", "");

        // Check if the player has an equipped weapon
        if (equippedWeapon != "")
        {
            // Iterate through the shop weapons
            foreach (ShopWeapon weapon in ShopWeapons)
            {
                // Check if the current weapon is the equipped weapon
                if (weapon.Weapon.name == equippedWeapon)
                {
                    // Equip the weapon by calling the "EquipWeapon" method on the player's inventory script
                    EquipItem(weapon);
                    break;
                }
            }
        }
    }

    private void SaveProgress()
    {
        // Iterate through the shop weapons
        for (int i = 0; i < ShopWeapons.Length; i++)
        {
            // Save the "isBought" field for the current weapon
            PlayerPrefs.SetInt("WeaponBought" + i, ShopWeapons[i].IsBought ? 1 : 0);
        }

        // Save the player's equipped weapon
        string equippedWeapon = "";
        if (playerCombat.currentWeapon != null)
        {
            equippedWeapon = playerCombat.currentWeapon.name;
        }
        PlayerPrefs.SetString("EquippedWeapon", equippedWeapon);
    }
    
    public struct ShopWeapon
    {
        public BaseWeapon Weapon;
        public Button ShopButton;
        public int Cost;
        public bool IsBought;

        public ShopWeapon(BaseWeapon weapon, Button shopButton)
        {
            Weapon = weapon;
            ShopButton = shopButton;
            Cost = 0;
            IsBought = false;
        }
    }
}