using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthbar : MonoBehaviour
{
    public Slider slider;
    public Enemy enemy;

    public void Start()
    {
        SetMaxhealth(enemy.maxHealth);
    }

    public void FixedUpdate()
    {
        SetHealth(enemy.CurrentHealth);
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