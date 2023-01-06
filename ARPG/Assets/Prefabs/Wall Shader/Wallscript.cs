using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallscript : MonoBehaviour
{
    public Material defaultMat;
    private Renderer _renderer;
    private Coroutine _currentRayCast;
    public void OnRayCastHit(Material[] shaderMat)
    {

        if (_currentRayCast != null)
        {
            StopCoroutine(_currentRayCast);
        }
        
        _currentRayCast = StartCoroutine(CO_OnRayCastExit());
        
        
        _renderer.materials = shaderMat;
    }

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        defaultMat = _renderer.material;
        
    }

    private IEnumerator CO_OnRayCastExit()
    {
        yield return new WaitForSeconds(0.3f);
        _renderer.material = defaultMat;
    }
    void Update()
    {
        
    }
}
