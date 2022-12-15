using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SpawnMonster : MonoBehaviour
{
    public ParticleSystem spawnEffect;
    public LayerMask triggerSpawnLayers;
    public bool readyToSpawn = true;
    public GameObject[] enemies;
    [FormerlySerializedAs("enemiesSpawnRate")] public int[] enemiesSpawnRates;
    private void OnTriggerEnter(Collider other)
    {
        if ((triggerSpawnLayers & (1 << other.gameObject.layer)) != 0 && readyToSpawn)
        {
            readyToSpawn = false;
            BeginSpawn();   
        }
    }

    void BeginSpawn()
    {
        // all enemies have a number of lapps they put into a skål. this counts all the lapps.
        int fullRateAmount = 0;
        for (int i = 0; i < enemiesSpawnRates.Length; i++)
        {
            for (int j = 0; j < enemiesSpawnRates[i]; j++)
            {
                fullRateAmount++;
            }
        }
        // picks a random lapp
        int thisEnemy = Random.Range(0, fullRateAmount);
        
        //checks what lapp represents which enemy
        for (int i = 0; i < enemiesSpawnRates.Length; i++)
        {
            for (int j = 0; j < enemiesSpawnRates[i]; j++)
            {
                if (j == thisEnemy)
                {
                    thisEnemy = i;
                    break;
                }
            }
        }
        
        Instantiate(enemies[thisEnemy], transform.position, transform.rotation);
        spawnEffect.Play();
    }
}
