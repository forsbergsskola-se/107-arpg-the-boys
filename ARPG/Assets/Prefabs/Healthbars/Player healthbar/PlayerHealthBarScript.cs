using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarScript : MonoBehaviour
{
    public Image slider;
    public PlayerStats player;

    public void FixedUpdate()
    {
        SetHealth(player.CurrentHealth);
        print(player.CurrentHealth);
    }

    public void SetHealth(float health)
    {
        slider.fillAmount = health / player.MaxHealth;
    }
    
}
