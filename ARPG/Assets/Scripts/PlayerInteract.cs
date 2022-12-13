using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactAreaSize;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InteractCheck()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactAreaSize);

        var hit = hits
            .Where(hit => hit.GetComponent<Collider>().TryGetComponent(out IInteractable _))
            .OrderBy(hit => Vector3.Distance(hit.GetComponent<Collider>().transform.position, transform.position))
            .FirstOrDefault();
        
        hit.GetComponent<Collider>()?.GetComponent<IInteractable>().Interact();
        
        // int minDistance = float.MaxValue;'
        // Collider closest;
        // foreach
        //    if(has interactable && distance < minDistance)
        //       then minDistance = distance; closest = current;
        // 
    }
}
