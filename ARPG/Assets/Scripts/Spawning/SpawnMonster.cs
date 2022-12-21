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
        int thisEnemy = ThisEnemy();
        //Debug.Log(thisEnemy +" is the chosen enemy from the array");
        Instantiate(enemies[thisEnemy], transform.position, transform.rotation);
        spawnEffect.Play();
    }

    private int ThisEnemy()
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
        
        //Debug.Log(thisEnemy +" is the start lapp");
        //checks what lapp represents which enemy
        int _countedTimes = 0;
        for (int i = 0; i < enemiesSpawnRates.Length; i++)
        {
            for (int j = 0; j < enemiesSpawnRates[i]; j++)
            {
                if (thisEnemy == _countedTimes)
                {
                    //Debug.Log(i +" is the enemy we found the number at");
                    return i;
                }
                _countedTimes++;
            }
        }
        return 0;
    }
}
