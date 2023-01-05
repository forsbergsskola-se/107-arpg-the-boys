using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallscript : MonoBehaviour
{
    private Material _defaultMat;
    private Renderer _renderer;
    private Coroutine _currentRayCast;
    
    public void OnRayCastHit(Material[] shaderMat)
    {
        if (_currentRayCast != null)
        {
            StopCoroutine(_currentRayCast);
        }
        _renderer.materials = shaderMat;
        _currentRayCast = StartCoroutine(CO_OnRayCastExit());
    }

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _defaultMat = _renderer.material;

    }

    private IEnumerator CO_OnRayCastExit()
    {
        yield return new WaitForSeconds(0.3f);
        _renderer.material = _defaultMat;
        print("big");
    }
    void Update()
    {
        
    }
}
