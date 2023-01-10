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
    public float moveSpeed;
    public float rotationSpeed;
    public Animator animator;
    public Vector3 attackRange;
    public Transform attackTransform;
    public Vector3 attackSize;
    public LayerMask hitLayer;
    public float damage;
    public bool showAttackRange;
    public bool showAttackSize;
    public int bombTurns;
    private Coroutine _currentChase;
    private float _timer;
    private bool _isAttacking;
    private bool _isInChaseMode;
    private int _bombCounter;

    void Update()
    {
        //Locks rotation and position
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        Quaternion rotation = transform.localRotation;
        rotation.z = 0;
        rotation.x = 0;
        transform.localRotation = rotation;

        if (_isInChaseMode && !_isAttacking)
        {
            ChaseMode();
        }
        
        print(_bombCounter);
        print(_timer);

        if (showAttackSize)
        {
            DrawBoxCastBox(attackTransform.position, attackSize / 2, transform.rotation, Color.red);
        }

        if (showAttackRange)
        {
            DrawBoxCastBox(attackTransform.position, attackRange / 2, transform.rotation, Color.yellow);
        }
    }

    private void ChaseMode()
    {
        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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

    private void BombMode()
    {
        animator.SetTrigger("Bomb");
    }


    private IEnumerator CO_BombSpawn(GameObject indicatorInstance)
    {
        yield return new WaitForSeconds(1);
        Instantiate(bombPrefab, indicatorInstance.transform.position, Quaternion.identity);
    }

    private IEnumerator CO_ChaseMode()
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
        BombMode();
        _isInChaseMode = false;
        _timer = 0;
    }

    #region Animation Methods

    public void BombAnimation()
    {
        if (_bombCounter <= bombTurns)
        {
            _bombCounter++;
            GameObject indicatorInstance = Instantiate(indicatorPrefab, player.transform.position, Quaternion.identity);
            StartCoroutine(CO_BombSpawn(indicatorInstance));
        }
    }

    public void PunchAnimation()
    {
        HitBox();
    }

    public void Chase()
    {
        if (_currentChase != null)
            StopCoroutine(_currentChase);
        _currentChase = StartCoroutine(CO_ChaseMode());
    }

    public void BombEnd()
    {
        if (_bombCounter >= bombTurns)
        {
            _currentChase = StartCoroutine(CO_ChaseMode());
            _bombCounter = 0;
        }
        else
            animator.SetTrigger("Bomb");
        
        
    }

    #endregion

    private void HitBox()
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