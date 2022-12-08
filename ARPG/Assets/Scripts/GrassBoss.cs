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
    [SerializeField]
    private float _danceRadius;
    public GameObject bossPrefab;
    private GameObject _firePoint;
    public int groundScatterAmount;
    public float groundScatterDistance;
    public float groundScatterSpeed;


    private float _distanceBetween;


    void Start()
    {
        _firePoint = GameObject.FindGameObjectWithTag("firePoint");
        
    }

    // Update is called once per frame
    void Update()
    {
        var position = bossPrefab.transform.position;
        position = new Vector3(position.x, Mathf.Clamp(position.y, 0, 10),position.z);
        bossPrefab.transform.position = position;

        //move towards _firePoint
        //transform.position = Vector3.MoveTowards(transform.position,_firePoint.transform.position, 2 * Time.deltaTime);
        //transform.LookAt(_firePoint.transform);
        
        
        
    }
    
    public IEnumerator CO_GroundScatter()
    {
        _distanceBetween = groundScatterDistance * 2 / (groundScatterAmount - 1);
        int attackPattern = Random.Range(1, 4);
        if (attackPattern == 1)
        {
            for (int i = 0; i < groundScatterAmount; i++)
            {
                Instantiate(groundScatterPrefab, _firePoint.transform.position + _firePoint.transform.right * (-groundScatterDistance + i * _distanceBetween) + _firePoint.transform.forward * 2, _firePoint.transform.rotation);
                yield return new WaitForSeconds(groundScatterSpeed);
            } 
        }

        if (attackPattern == 2)
        {
            for (int i = 0; i < groundScatterAmount; i++)
            {
                Instantiate(groundScatterPrefab, _firePoint.transform.position + _firePoint.transform.right * (groundScatterDistance - i * _distanceBetween) + _firePoint.transform.forward * 2, _firePoint.transform.rotation);
                yield return new WaitForSeconds(groundScatterSpeed);
            } 
        }
        
        if (attackPattern == 3)
        {
            for (int i = 0; i < groundScatterAmount; i++)
            {
                Instantiate(groundScatterPrefab, _firePoint.transform.position + _firePoint.transform.forward * (-groundScatterDistance + i * _distanceBetween) + _firePoint.transform.right * -4 + _firePoint.transform.forward * 6, _firePoint.transform.rotation * Quaternion.Euler(0,90,0));
                yield return new WaitForSeconds(groundScatterSpeed);
            } 
        }
    }

    public IEnumerator CO_Fart()
    {
        yield return new WaitForSeconds(1.75f);
        Instantiate(fartPrefab, transform.position, Quaternion.identity);
    }

    public IEnumerator CO_Dance()
    {
        Instantiate(dancePrefab, new Vector3(Random.Range(-_danceRadius, _danceRadius), 0,Random.Range(-_danceRadius, _danceRadius)), Quaternion.identity);
        yield return null;
    }
}
