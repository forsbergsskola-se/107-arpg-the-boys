using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public PlayerCombat playerCombat;
    public float interactAreaSize;
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
        Collider[] hits = Physics.OverlapSphere(transform.position, interactAreaSize);

        var hit = hits
            .Where(hit => hit.GetComponent<Collider>().TryGetComponent(out IInteractable _))
            .OrderBy(hit => Vector3.Distance(hit.GetComponent<Collider>().transform.position, transform.position))
            .FirstOrDefault();

        if (hit != null) 
            return hit.GetComponent<Collider>()?.GetComponent<IInteractable>();
        return null;
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
