using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GrassBoss : MonoBehaviour
{
    public GameObject groundScatterPrefab;
    public GameObject fartPrefab;
    public GameObject dancePrefab;
    public Vector3 abilityScale;
    public Enemy enemyScript;
    
    public float danceRadius;
    private GameObject _firePoint;
    public int groundScatterAmount;
    public float groundScatterDistance;
    public float groundScatterSpeed;
    public float passiveStageDuration;

    [NonSerialized]
    public float scatterRounds;
    [NonSerialized]
    public bool passiveStageActive;
    [NonSerialized]
    public bool switchFromPassive;
    private float _distanceBetween;
    public bool firePointIsOnBoss;

    private bool _moveToAttackSpot;
    private GameObject _groundScatterInstance;


    void Start()
    {
        _firePoint = GameObject.FindGameObjectWithTag("firePoint");
        if (firePointIsOnBoss)
        {
            _firePoint.transform.position = transform.position;
            _firePoint.transform.rotation = transform.rotation;
        }

    }

    // Update is called once per frame
    void Update()
    {
        var position = transform.position;
        position = new Vector3(position.x, Mathf.Clamp(position.y, 0, 10),position.z);
        transform.position = position;

        //move towards _firePoint
        if (_moveToAttackSpot)
            MoveToAttackSpot();
    }

    private void MoveToAttackSpot()
    {
        Quaternion targetRotation = Quaternion.LookRotation(_firePoint.transform.position - transform.position);
        transform.position = Vector3.MoveTowards(transform.position,_firePoint.transform.position, enemyScript.moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, enemyScript.rotationSpeed * Time.deltaTime);
    }

    public IEnumerator CO_PassiveStage()
    {
        passiveStageActive = true;
        enemyScript.enabled = true;
        yield return new WaitForSeconds(passiveStageDuration);
        yield return new WaitUntil(() => enemyScript.endAttack);
        enemyScript.enabled = false;
        enemyScript.animator.SetBool(enemyScript.walkAnimationParameterName, true);
        _moveToAttackSpot = true;
        yield return new WaitUntil(() => transform.position == _firePoint.transform.position);
        enemyScript.animator.SetBool(enemyScript.walkAnimationParameterName, false);
        switchFromPassive = true;
        _moveToAttackSpot = false;
        yield return StartCoroutine(CO_RotateToTarget(_firePoint.transform));
        passiveStageActive = false;
    }

    private IEnumerator CO_RotateToTarget(Transform target)
    {
        while (transform.rotation != target.transform.rotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation,
                enemyScript.rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
    public IEnumerator CO_GroundScatter()
    {
        _distanceBetween = groundScatterDistance * 2 / (groundScatterAmount - 1);
        int attackPattern = Random.Range(1, 3);
        if (attackPattern == 1)
        {
            for (int i = 0; i < groundScatterAmount; i++)
            {
                _groundScatterInstance = Instantiate(groundScatterPrefab, _firePoint.transform.position + _firePoint.transform.right * (-groundScatterDistance + i * _distanceBetween) + _firePoint.transform.forward * 2, _firePoint.transform.rotation);
                _groundScatterInstance.transform.localScale = abilityScale;
                yield return new WaitForSeconds(groundScatterSpeed); 
            } 
        }

        if (attackPattern == 2)
        {
            for (int i = 0; i < groundScatterAmount; i++)
            {
                _groundScatterInstance = Instantiate(groundScatterPrefab, _firePoint.transform.position + _firePoint.transform.right * (groundScatterDistance - i * _distanceBetween) + _firePoint.transform.forward * 2, _firePoint.transform.rotation);
                _groundScatterInstance.transform.localScale = abilityScale;
                yield return new WaitForSeconds(groundScatterSpeed);
            } 
        }
        
        if (attackPattern == 3)
        {
            for (int i = 0; i < groundScatterAmount; i++)
            {
                _groundScatterInstance = Instantiate(groundScatterPrefab, _firePoint.transform.position + _firePoint.transform.forward * (-groundScatterDistance + i * _distanceBetween) + _firePoint.transform.right * -13 + _firePoint.transform.forward * 13, _firePoint.transform.rotation * Quaternion.Euler(0,90,0));
                _groundScatterInstance.transform.localScale = abilityScale;
                yield return new WaitForSeconds(groundScatterSpeed);
            } 
        }
    }

    public IEnumerator CO_Fart()
    {
        yield return new WaitForSeconds(1.75f);
        GameObject fartInstance = Instantiate(fartPrefab, transform.position, _firePoint.transform.rotation);
        fartInstance.transform.localScale = abilityScale * 1.8f;
    }

    public IEnumerator CO_Dance()
    {
        
        var position = _firePoint.transform;
        var zPos = _firePoint.transform.forward;
        var xPos = _firePoint.transform.right;
        Vector3 desiredArea =  new Vector3(Random.Range(xPos.x - danceRadius, xPos.x + danceRadius), 0, Random.Range(zPos.z, zPos.z + danceRadius * 2));
        GameObject danceInstance = Instantiate(dancePrefab, desiredArea, Quaternion.identity);
        danceInstance.transform.localScale = abilityScale;
        yield return null;
    }
}
