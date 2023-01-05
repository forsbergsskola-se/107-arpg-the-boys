using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupable
{
    void Pickup()
    {
        
    }

    void Pickup(PlayerCombat playerCombat)
    {
        
    }
    
    void Pickup(PlayerStats playerStats, PlayerInventory playerInventory)
    {
        
    }
}
