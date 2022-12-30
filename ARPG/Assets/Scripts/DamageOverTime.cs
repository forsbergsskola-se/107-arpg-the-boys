using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    public LayerMask hitLayer;
    public float dps;
    private void OnTriggerStay(Collider other)
    {
        if (hitLayer == (hitLayer | (1 << other.gameObject.layer)))
        {
            if(other.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(dps * Time.deltaTime);
        }
        
    }

    private Collider hits;

    private void Start()
    {
        
    }
}
