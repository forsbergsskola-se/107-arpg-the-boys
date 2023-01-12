using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshBaker : MonoBehaviour
{
    public NavMeshSurface[] navMeshSurfaces;
    public LayerMask triggerSpawnLayers;

    void Start()
    {
        StartCoroutine(WaitForSpawn());
    }

    public IEnumerator WaitForSpawn()
    {
        yield return new WaitForSeconds(1);
        navMeshSurfaces = FindObjectsOfType<NavMeshSurface>();
        for (var i = 0; i < navMeshSurfaces.Length; i++)
        {
            if ((triggerSpawnLayers & (1 << navMeshSurfaces[i].gameObject.layer))!= 0)
                navMeshSurfaces[i].BuildNavMesh();
        }
    }
}
