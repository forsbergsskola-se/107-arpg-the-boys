using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GhoulBossScript : MonoBehaviour
{
    public GameObject player;
    public GameObject indicatorPrefab;
    public GameObject bombPrefab;
    public float chaseModeDuration;
    private bool _isInChaseMode;
    public float movespeed;
    public float rotationSpeed;
    public Animator animator;
    public float attackRange;
    public Transform attackTransform;
    public Vector3 attackSize;
    public LayerMask hitLayer;
    public float damage;

    void Start()
    {
        
    }
    void Update()
    {
        if (_isInChaseMode)
        {
            ChaseMode();
        }
    }
    
    public void ChaseMode()
    {
        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position);
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position,movespeed*Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed*Time.deltaTime);
        if (Vector3.Distance(transform.position,player.transform.position)<attackRange)
        {
            animator.SetTrigger("Attack");
        }
    }
    
    

    public IEnumerator CO_BombSpawn(GameObject indicatorInstance)
    {
        yield return new WaitForSeconds(1);
        Instantiate(bombPrefab, indicatorInstance.transform.position, Quaternion.identity);
    }

    public IEnumerator CO_ChaseMode()
    {
        animator.SetTrigger("Chase");
        _isInChaseMode = true;
        yield return new WaitForSeconds(chaseModeDuration);
        _isInChaseMode = false;
    }

    #region Animation Methods
    public void BombAnimation()
    {
        GameObject indicatorInstance = Instantiate(indicatorPrefab, player.transform.position, Quaternion.identity);
        StartCoroutine(CO_BombSpawn(indicatorInstance));
    }

    public void PunchAnimation()
    {
        
    }
    

    #endregion

    public void HitBox()
    {
        Collider[] hits = Physics.OverlapBox(attackTransform.position, attackSize / 2, transform.rotation, hitLayer);
        for (var i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
        }
    }
    
    
}
