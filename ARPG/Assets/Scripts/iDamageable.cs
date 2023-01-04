using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
   float CurrentHealth { get; }

   void TakeDamage(float damage);
}
