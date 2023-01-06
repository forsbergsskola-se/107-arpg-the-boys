using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public GameObject player;
    private PlayerCombat _playerCombat;
    private PlayerInventory _playerInventory;
    private PlayerStats _playerStats;
    public float interactAreaSize;
    public LayerMask interactableLayerMask;
    private IInteractable _closestInteractable;

    void Start()
    {
        _closestInteractable = null;
        _playerCombat = player.GetComponent<PlayerCombat>();
        _playerStats = player.GetComponent<PlayerStats>();
        _playerInventory = player.GetComponent<PlayerInventory>();
    }

    void Update()
    {
        _closestInteractable = ClosestInteractable();
        if (_closestInteractable != null)
        {
            //_closestInteractable.Highlight();
            if (_closestInteractable is ItemScript)
            {
                _closestInteractable.Interact(_playerStats, _playerInventory);
            }
        }

        if (Input.GetButtonDown("Interact"))
        {
            TryInteract(_closestInteractable);
        }
    }

    private IInteractable ClosestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactAreaSize, interactableLayerMask);

        // Find the closest interactable
        IInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;
        foreach (var hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable == null)
                continue;

            float distance = Vector3.Distance(hit.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestInteractable = interactable;
                closestDistance = distance;
            }
        }

        return closestInteractable;
    }

    private void TryInteract(IInteractable interactable)
    {
        if (interactable is IPickupable pickupable and BaseWeapon)  // What happens when you interact with a weapon
        {
            if (_playerCombat.currentWeapon != null)
                _playerCombat.currentWeapon.DropWeapon(_playerCombat);  // It never calls Interact() on the weapon, it just calls DropWeapon() and Pickup()
            pickupable.Pickup(_playerCombat);
        }
        else if (interactable != null)
        {
            interactable.Interact();
        }
        else
        {
            Debug.Log("No Interactable Found.");
        }
    }
}
