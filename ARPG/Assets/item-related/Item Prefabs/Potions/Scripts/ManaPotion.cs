using System.Collections;
using UnityEngine;

public class ManaPotion : MonoBehaviour, IInteractable, IPickupable
{
    public int charges;
    public void Interact(PlayerStats playerStats, PlayerInventory playerInventory)
    {  
        if(_pickUpEnabled)
        { Pickup(playerStats, playerInventory); } 
    }

    void Pickup(PlayerStats playerStats, PlayerInventory playerInventory)
    {
        if (playerStats.CurrentManaPotions < playerStats.maxManaPotions)
        {
            playerStats.AddMPotion(charges);
            Destroy(gameObject);
        }
    }
    private bool _pickUpEnabled;
    private void Start()
    {
        _camera = Camera.main;
        _pickUpEnabled = false;
        StartCoroutine(PickupDelay());
    }
    private IEnumerator PickupDelay()
    {
        yield return new WaitForSecondsRealtime(2);
        _pickUpEnabled = true;
    }

    private Camera _camera;
    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
    }
    
    public void Highlight() { throw new System.NotImplementedException(); }
}
