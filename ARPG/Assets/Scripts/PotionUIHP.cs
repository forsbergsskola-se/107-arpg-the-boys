using TMPro;
using System;
using UnityEngine;

public class PotionUIHP : MonoBehaviour
{
    public TMP_Text healthPotText;
    private PlayerStats _playerStats;

    private void Start()
    {
        _playerStats = FindObjectOfType<PlayerStats>();
    }

    private void Update()
    {
        healthPotText.text = Convert.ToString(_playerStats.CurrentHealthPotions) + "/" +  Convert.ToString(_playerStats.maxHealthPotions);
    }
}
