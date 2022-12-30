using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarScript : MonoBehaviour
{
    public Slider slider;
    public PlayerStats player;

    public void Start()
    {
        SetMaxhealth(player.MaxHealth);
    }

    public void FixedUpdate()
    {
        SetHealth(player.CurrentHealth);
    }

    public void SetMaxhealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }
    
}
