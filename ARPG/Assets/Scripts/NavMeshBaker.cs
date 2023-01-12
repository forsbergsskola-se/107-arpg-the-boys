using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshBaker : MonoBehaviour
{
    private NavMeshSurface[] _navMeshSurfaces;

    void Start()
    {
        StartCoroutine(WaitForSpawn());
    }

    public IEnumerator WaitForSpawn()
    {
        yield return new WaitForSeconds(2);
        _navMeshSurfaces = FindObjectsOfType<NavMeshSurface>();
        for (var i = 0; i < _navMeshSurfaces.Length; i++)
        {
            _navMeshSurfaces[i].BuildNavMesh();
        }
    }
}
