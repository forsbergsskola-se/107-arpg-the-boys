using TMPro;
using System;
using UnityEngine;

public class PotionUIMP : MonoBehaviour
{
    public TMP_Text manaPotText;
    private PlayerStats _playerStats;

    private void Start()
    {
        _playerStats = FindObjectOfType<PlayerStats>();
    }

    private void Update()
    {
        manaPotText.text = Convert.ToString(_playerStats.CurrentManaPotions) + "/" +  Convert.ToString(_playerStats.maxManaPotions);
    }
}
