using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bumptest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerStats playerstats = collision.gameObject.GetComponent<PlayerStats>();
        playerstats.TakeDamage(5);
        Debug.Log($"colliding with {collision} and is pulling {playerstats} from it to deal damage");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
