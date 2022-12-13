using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonster : MonoBehaviour
{
    public ParticleSystem spawnEffect;
    
    void BeginSpawn()
    {
            spawnEffect.Play();
        
    }
}
