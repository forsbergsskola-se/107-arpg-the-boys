using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact()
    {
        
    }

    void Interact(PlayerStats playerStats, PlayerInventory playerInventory, TextPopUpScript textPopUpScript)
    {
        
    }
    void Highlight();
}
