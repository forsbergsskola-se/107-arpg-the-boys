using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonster : MonoBehaviour
{
    public ParticleSystem spawnEffect;
    public LayerMask triggerSpawnLayers;
    public bool readyToSpawn = true;
    public GameObject enemy;
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
        Instantiate(enemy, transform.position, transform.rotation);
        spawnEffect.Play();
    }
}
