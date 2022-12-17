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
    public bool passiveStageActive;
    private float _distanceBetween;
    public bool firePointIsOnBoss;


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
        //transform.position = Vector3.MoveTowards(transform.position,_firePoint.transform.position, 2 * Time.deltaTime);
        //transform.LookAt(_firePoint.transform);

    }

    public IEnumerator CO_PassiveStage()
    {
        passiveStageActive = true;
        yield return new WaitForSeconds(passiveStageDuration);
        passiveStageActive = false;
    }
    public IEnumerator CO_GroundScatter()
    {
        _distanceBetween = groundScatterDistance * 2 / (groundScatterAmount - 1);
        int attackPattern = Random.Range(1, 3);
        if (attackPattern == 1)
        {
            for (int i = 0; i < groundScatterAmount; i++)
            {
                GameObject groundScatterInstance = Instantiate(groundScatterPrefab, _firePoint.transform.position + _firePoint.transform.right * (-groundScatterDistance + i * _distanceBetween) + _firePoint.transform.forward * 2, _firePoint.transform.rotation);
                groundScatterInstance.transform.localScale = abilityScale;
                yield return new WaitForSeconds(groundScatterSpeed);
            } 
        }

        if (attackPattern == 2)
        {
            for (int i = 0; i < groundScatterAmount; i++)
            {
                GameObject groundScatterInstance = Instantiate(groundScatterPrefab, _firePoint.transform.position + _firePoint.transform.right * (groundScatterDistance - i * _distanceBetween) + _firePoint.transform.forward * 2, _firePoint.transform.rotation);
                groundScatterInstance.transform.localScale = abilityScale;
                yield return new WaitForSeconds(groundScatterSpeed);
            } 
        }
        
        if (attackPattern == 3)
        {
            for (int i = 0; i < groundScatterAmount; i++)
            {
                GameObject groundScatterInstance = Instantiate(groundScatterPrefab, _firePoint.transform.position + _firePoint.transform.forward * (-groundScatterDistance + i * _distanceBetween) + _firePoint.transform.right * -13 + _firePoint.transform.forward * 13, _firePoint.transform.rotation * Quaternion.Euler(0,90,0));
                groundScatterInstance.transform.localScale = abilityScale;
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
        
        var position = _firePoint.transform.position;
        Vector3 desiredArea = _firePoint.transform.rotation * new Vector3(Random.Range(position.x - danceRadius, position.x + danceRadius), 0, Random.Range(position.z, position.z + danceRadius * 2));
        GameObject danceInstance = Instantiate(dancePrefab, desiredArea, Quaternion.identity);
        danceInstance.transform.localScale = abilityScale;
        yield return null;
    }
}