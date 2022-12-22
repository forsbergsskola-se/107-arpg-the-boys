using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarScript : MonoBehaviour
{
    public Slider slider;
    public Enemy enemy;
    private Camera _camera;

    public void Start()
    {
        SetMaxhealth(enemy.maxHealth);
        _camera = Camera.main;
    }

    public void FixedUpdate()
    {
        SetHealth(enemy.CurrentHealth);
        transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
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
