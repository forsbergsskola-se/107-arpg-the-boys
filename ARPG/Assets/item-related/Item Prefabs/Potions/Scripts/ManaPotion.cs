using UnityEngine;

public class ManaPotion : MonoBehaviour, IInteractable, IPickupable
{
    public int charges;
    public void Interact(PlayerStats playerStats, PlayerInventory playerInventory)
    { Pickup(playerStats, playerInventory); }

    void Pickup(PlayerStats playerStats, PlayerInventory playerInventory)
    {
        if (playerStats.CurrentManaPotions < playerStats.maxManaPotions)
        {
            playerStats.AddMPotion(charges);
            Destroy(gameObject);
        }
    }

    public float rotationSpeed;
    private void Update() 
    { transform.Rotate(transform.up, rotationSpeed); }
    
    public void Highlight() { throw new System.NotImplementedException(); }
}
