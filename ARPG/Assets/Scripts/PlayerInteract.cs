using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactAreaSize;
    
    void Start()
    {
        
    }

    void Update()
    {
        

        if (Input.GetButtonDown("Interact"))
        {
            TryInteract(ClosestInteractable());
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
        if (interactable != null)
        {
            interactable.Interact();
        }
        else
        {
            Debug.Log("No Interactable Found.");
        }
    }
}
