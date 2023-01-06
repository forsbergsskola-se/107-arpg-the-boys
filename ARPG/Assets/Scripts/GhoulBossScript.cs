using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static DrawBoxCast;

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
    public Vector3 attackRange;
    public Transform attackTransform;
    public Vector3 attackSize;
    public LayerMask hitLayer;
    public float damage;
    public bool showAttackRange;
    public bool showAttackSize;
    private Coroutine _currentChase;
    private float _timer;
    private bool _isAttacking;

    void Start()
    {
        
    }
    void Update()
    {
        print(_timer);
        print(_isInChaseMode);
        if (_isInChaseMode && !_isAttacking)
        {
            ChaseMode();
        }

        if (showAttackSize)
        {
            DrawBoxCastBox(attackTransform.position, attackSize / 2, transform.rotation, Color.red);
        }
        if (showAttackRange)
        {
            DrawBoxCastBox(attackTransform.position, attackRange / 2, transform.rotation, Color.yellow);
        }
    }
    
    public void ChaseMode()
    {
        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position);
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position,movespeed*Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed*Time.deltaTime);
        Collider[] hits = Physics.OverlapBox(attackTransform.position, attackRange / 2, transform.rotation, hitLayer);
        for (var i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out IDamageable damageable))
            {
                animator.SetTrigger("Attack");
                _isAttacking = true;
            }
        }
    }
    
    

    public IEnumerator CO_BombSpawn(GameObject indicatorInstance)
    {
        yield return new WaitForSeconds(1);
        Instantiate(bombPrefab, indicatorInstance.transform.position, Quaternion.identity);
    }

    public IEnumerator CO_ChaseMode()
    {
        _isAttacking = false;
        animator.SetTrigger("Chase");
        _isInChaseMode = true;
        while (_timer < chaseModeDuration)
        {
            _timer += Time.deltaTime;
            _isInChaseMode = true;
            yield return null;
        }
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
        HitBox();
        
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
                _timer = 0;
            }
            
        }
    }
    
    
    
}
