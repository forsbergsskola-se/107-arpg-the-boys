using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RandomSound;

public class HubShop : MonoBehaviour, IInteractable
{
    [Header("Shop Weapons")]
    public ShopWeapon[] shopWeapons;
    
    [Header("References")]
    public Armory armory;
    private ShopWeapon _selectedWeapon;

    // Reference to the shop UI panel
    public GameObject shopUIPanel;

    // Reference to the player
    public GameObject player;
    private PlayerCombat _playerCombat;
    private PlayerMovement _playerMovement;
    private PlayerWeaponLoader _weaponLoader;
    private PlayerInventory _inventory;
    private Rigidbody _playerRb;
    

    [Header("Audio")] 
    public AudioSource audioSource;
    public AudioClip[] buySounds;
    public AudioClip[] equipSounds;
    public AudioClip[] toggleMenuSounds;

    private void Awake()
    {
        // Load the player's progress from PlayerPrefs
        _playerCombat = player.GetComponent<PlayerCombat>();
        _playerMovement = player.GetComponent<PlayerMovement>();
        _playerRb = player.GetComponent<Rigidbody>();
        _weaponLoader = player.GetComponent<PlayerWeaponLoader>();
        _inventory = player.GetComponent<PlayerInventory>();
    }

    private void Start()
    {
        //_weaponLoader.LoadProgress();
        for (var i = 0; i < armory.armoryWeapons.Length; i++)
        {
            shopWeapons[i].weapon = armory.armoryWeapons[i].weapon;
            shopWeapons[i].isBought = armory.armoryWeapons[i].isBought;
            shopWeapons[i].cost = armory.armoryWeapons[i].cost;
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
        audioSource.clip = GetRandomAudioClip(toggleMenuSounds);
        audioSource.Play();
        _playerMovement.canMove = false;
        _playerRb.isKinematic = true;
    }

    public void CloseShopUI()
    {
        UpdateShopUI();
        shopUIPanel.SetActive(false);
        audioSource.clip = GetRandomAudioClip(toggleMenuSounds);
        audioSource.Play();
        _playerMovement.canMove = true;
        _playerRb.isKinematic = false;
    }

    public void TryBuyOrEquipItem(int shopWeaponArraySpot)
    {
        _selectedWeapon = shopWeapons[shopWeaponArraySpot];
        if (!_selectedWeapon.isBought)
            BuyItem(_selectedWeapon);
        else
            EquipItem(_selectedWeapon);

        armory.armoryWeapons[shopWeaponArraySpot].isBought = _selectedWeapon.isBought;
        _weaponLoader.SaveProgress();
    }

    public void BuyItem(ShopWeapon selectedWeapon)
    {
        // Check if the player has enough money to buy the selected weapon
        if (_inventory.GetItemCount("Money") >= selectedWeapon.cost)
        {
            // Deduct the cost from the player's money
            _inventory.ShopSellMoney("Money", _inventory.GetItemCount("Money") - selectedWeapon.cost);
            Debug.Log($"Bought weapon, now you have {_inventory.GetItemCount("Money")} moneys.");

            // Set the weapon's "isBought" field to true
            selectedWeapon.isBought = true;
            
            // Play Buy-sound
            audioSource.clip = GetRandomAudioClip(buySounds);
            audioSource.Play();

            // Save the player's progress
            _weaponLoader.SaveProgress();
        }
        else
        {
            // Display a message to the player telling them that they don't have enough money
            Debug.Log($"You don't have enough money to buy this weapon! You need {selectedWeapon.cost} but you only have {_inventory.GetItemCount("Money")}");
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

            var instance = Instantiate(selectedWeapon.weapon, transform.position, Quaternion.identity);
            instance.Pickup(_playerCombat);
            
            // Play pickup-sound
            audioSource.clip = GetRandomAudioClip(equipSounds);
            audioSource.Play();

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
                weaponButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Buy ({weapon.cost})";
            }
        }
    }

    [Serializable]
    public class ShopWeapon
    {
        [NonSerialized] public BaseWeapon weapon;
        [NonSerialized] public int cost;
        [NonSerialized] public bool isBought;
        public Button shopButton;
    }
}