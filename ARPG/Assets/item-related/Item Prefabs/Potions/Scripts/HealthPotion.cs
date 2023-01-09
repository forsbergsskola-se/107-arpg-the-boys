using UnityEngine;

public class HealthPotion : MonoBehaviour, IInteractable, IPickupable
{
    public int charges;
    public void Interact(PlayerStats playerStats, PlayerInventory playerInventory)
    { Pickup(playerStats, playerInventory); }

    void Pickup(PlayerStats playerStats, PlayerInventory playerInventory)
    {
        if (playerStats.CurrentHealthPotions < playerStats.maxHealthPotions)
        {
            playerStats.AddHPotion(charges); 
            Destroy(gameObject);
        }
        
    }

    public float rotationSpeed;
    private void Update() 
    { transform.Rotate(transform.up, rotationSpeed); }
    
    public void Highlight() { throw new System.NotImplementedException(); }
}
