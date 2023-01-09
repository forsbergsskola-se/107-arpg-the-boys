using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CutoutObject : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private LayerMask wallMask;
    public Camera mainCamera;
    public Material[] shaderMaterials;
    private bool _hitSomething;
    private bool kuk = true;
    public float offset;
    private Renderer[] _lastRenderers;
    private Material[] _lastMaterials;
    private static readonly int CutoutPos = Shader.PropertyToID("_CutoutPos");
    private static readonly int CutoutSize = Shader.PropertyToID("_CutoutSize");
    private static readonly int FalloffSize = Shader.PropertyToID("_FalloffSize");
    public float rayCastSize;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        ShaderThing();
    }


    private void ShaderThing()
    {
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= (Screen.width / Screen.height) - 1;

        Vector3 direction = targetObject.position - transform.position;
        RaycastHit[] hitObject = Physics.SphereCastAll(transform.position, rayCastSize, direction, direction.magnitude - offset, wallMask);
        _hitSomething = false;
        for (int i = 0; i < hitObject.Length; i++)
        {
            // if (kuk)
            // {
            //     _lastRenderers = new Renderer[hitObject.Length];
            //     _lastMaterials = new Material[hitObject.Length];
            //     
            //     for (var i1 = 0; i1 < hitObject.Length; i1++)
            //     {
            //
            //         _lastRenderers[i1] = hitObject[i1].transform.GetComponent<Renderer>();
            //         _lastMaterials[i1] = hitObject[i1].transform.GetComponent<Renderer>().material;
            //
            //     }
            //     kuk = false;
            // }

            _hitSomething = true;


            // hitObject[i].transform.GetComponent<Renderer>().materials = shaderMaterials;
            if (hitObject[i].transform.GetComponent<wallscript>() != null)
            {
                hitObject[i].transform.GetComponent<wallscript>().OnRayCastHit(shaderMaterials);
            }

            for (int j = 0; j < shaderMaterials.Length; j++)
            {
                shaderMaterials[j].SetVector(CutoutPos, cutoutPos);
                shaderMaterials[j].SetFloat(CutoutSize, 0.13f);
                shaderMaterials[j].SetFloat(FalloffSize, 0.05f);
            }
        }

        // if (!_hitSomething)
        // {
        //     for (var i = 0; i < _lastRenderers.Length; i++)
        //     {
        //         _lastRenderers[i].material = _lastMaterials[i];
        //     }
        //
        //     kuk = true;
        // }
    }
}