using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public PlayerCombat playerCombat;
    public float interactAreaSize;
    public LayerMask interactableLayerMask;
    private IInteractable _closestInteractable;

    void Start()
    {
        _closestInteractable = null;
    }

    void Update()
    {
        _closestInteractable = ClosestInteractable();
        if (_closestInteractable != null)
        {
            //_closestInteractable.Highlight();
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
        if (interactable is IPickupable pickupable and BaseWeapon)
        {
            if (playerCombat.currentWeapon != null)
                playerCombat.currentWeapon.DropWeapon(playerCombat);
            pickupable.Pickup(playerCombat);
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
