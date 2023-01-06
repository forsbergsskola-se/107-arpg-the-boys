using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class CutoutObject : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private LayerMask wallMask;
    public Camera mainCamera;
    public Material[] shaderMaterials;
    private bool _hitSomething;
    public float offset;
    public float shaderMaxSize;
    private Renderer[] _lastRenderers;
    private Material[] _lastMaterials;
    private static readonly int CutoutPos = Shader.PropertyToID("_CutoutPos");
    private static readonly int CutoutSize = Shader.PropertyToID("_CutoutSize");
    private static readonly int FalloffSize = Shader.PropertyToID("_FalloffSize");
    private static readonly int MainTexture = Shader.PropertyToID("_MainTexture");
    private float shaderSize;
    public float durration;
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
        //RaycastHit[] hitObject = Physics.SphereCastAll(transform.position, rayCastSize, direction, direction.magnitude - offset, wallMask);
        RaycastHit[] hitObject = Physics.RaycastAll(transform.position, direction, direction.magnitude, wallMask);
        Wallscript[] wallObjects = new Wallscript[hitObject.Length];


        _hitSomething = false;
        for (int i = 0; i < hitObject.Length; i++)
        {

            wallObjects[i] = hitObject[i].transform.GetComponent<Wallscript>();
            _hitSomething = true;


            // hitObject[i].transform.GetComponent<Renderer>().materials = shaderMaterials;
            
            

            shaderSize += Time.deltaTime / durration;
            shaderSize = Mathf.Clamp(shaderSize,0, shaderMaxSize);
            
            for (int j = 0; j < shaderMaterials.Length; j++)
            {
                shaderMaterials[j].SetVector(CutoutPos, cutoutPos);
                shaderMaterials[j].SetFloat(CutoutSize, shaderSize);
                shaderMaterials[j].SetFloat(FalloffSize, 0.05f);
                shaderMaterials[j].SetTexture(MainTexture, wallObjects[i].defaultMat.mainTexture);
            }
            
            if (wallObjects[i] != null)
            {
                wallObjects[i].OnRayCastHit(shaderMaterials);
            }
        }

        if (!_hitSomething)
        {
            shaderSize -= Time.deltaTime / durration;
            shaderSize = Mathf.Clamp(shaderSize,0, shaderMaxSize);
            
            for (int j = 0; j < shaderMaterials.Length; j++)
            {
                shaderMaterials[j].SetVector(CutoutPos, cutoutPos);
                shaderMaterials[j].SetFloat(CutoutSize, shaderSize);
                shaderMaterials[j].SetFloat(FalloffSize, 0.05f);
            }
        }
    }
}