using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicMusicPlayer : MonoBehaviour
{
    public float speed;
    private float _transitionState;
    private float TransitionState
    {
        set { _transitionState = Mathf.Clamp(value, 0, 100);}
        get => _transitionState;

    }
    public LayerMask enemyLayer;

    public AudioSource areaMusic;
    public AudioSource fightMusic;
    private SphereCollider _sphereCollider;
    private bool _containsEnemy;
    private void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    public void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _sphereCollider.radius, enemyLayer);
        _containsEnemy = (colliders.Length > 0);

        if (_containsEnemy)
        {
            Debug.Log("Touching Enemy");
            TransitionState += speed * Time.deltaTime;
        }
        else
        {
            Debug.Log("Not Touching Enemy");
            TransitionState -= speed * Time.deltaTime;
        }
        //Debug.Log("Transition state is " + TransitionState);
        areaMusic.volume = 1 - TransitionState / 100;
        fightMusic.volume = TransitionState / 100;
    }
}
