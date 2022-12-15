using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemExampleScript : MonoBehaviour
{
    
    public ItemScriptableObject itemScriptableObject;
    
    private void OnCollisionEnter(Collision collision)
    {
        PlayerStats playerstats = collision.gameObject.GetComponent<PlayerStats>();
        playerstats.baseMaxHealth += itemScriptableObject.optValue;
    }

}
