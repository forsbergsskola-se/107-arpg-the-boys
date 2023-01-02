using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhoulBossScript : MonoBehaviour
{
    public GameObject player;
    public GameObject indicatorPrefab;
    public GameObject bombPrefab;
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public void BombAttack()
    {
        GameObject indicatorInstance = Instantiate(indicatorPrefab, player.transform.position, Quaternion.identity);
        StartCoroutine(CO_BombSpawn(indicatorInstance));
    }

    public IEnumerator CO_BombSpawn(GameObject indicatorInstance)
    {
        yield return new WaitForSeconds(1);
        Instantiate(bombPrefab, indicatorInstance.transform.position, Quaternion.identity);
    }
    
    
}
