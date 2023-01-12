using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshBaker : MonoBehaviour
{
    private NavMeshSurface[] navMeshSurfaces;
    private NavMeshSurface[] _navMeshSurfaces;
    private NavMeshSurface[] _navMeshSurfaces1;

    void Start()
    {
        _navMeshSurfaces1 = FindObjectsOfType<NavMeshSurface>();
        StartCoroutine(WaitForSpawn());
    }

    public IEnumerator WaitForSpawn()
    {
        yield return new WaitForSeconds(1);
        _navMeshSurfaces = _navMeshSurfaces1;
        navMeshSurfaces = _navMeshSurfaces;
        for (var i = 0; i < navMeshSurfaces.Length; i++)
        {
            navMeshSurfaces[i].BuildNavMesh();
        }
    }
}
