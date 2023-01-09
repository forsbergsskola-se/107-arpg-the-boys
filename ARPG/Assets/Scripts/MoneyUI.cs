
using TMPro;
using System;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    public TMP_Text moneyText;
    private PlayerInventory _playerInventory;

    private void Start()
    {
        _playerInventory = FindObjectOfType<PlayerInventory>();
    }

    private void Update()
    {
        moneyText.text = Convert.ToString(_playerInventory.GetItemCount("Money"));
    }
}
