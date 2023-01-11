using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshBaker : MonoBehaviour
{
    private NavMeshSurface[] navMeshSurfaces;
    private NavMeshSurface[] _navMeshSurfaces;

    void Start()
    {
        _navMeshSurfaces = FindObjectsOfType<NavMeshSurface>();
        StartCoroutine(WaitForSpawn());
    }

    public IEnumerator WaitForSpawn()
    {
        for (int j = 0; j < 6; j++)
        {
            yield return new WaitForSeconds(2);
            navMeshSurfaces = _navMeshSurfaces;
            for (var i = 0; i < navMeshSurfaces.Length; i++)
            {
                navMeshSurfaces[i].BuildNavMesh();
            }
        }
    }
}
