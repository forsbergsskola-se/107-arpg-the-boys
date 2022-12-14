using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public class LightAttack
{
    public string lightAttackParameterNameOfTypeBool;
    public GameObject lightAttackChild;
    public Vector3 lightAttackSize;
    public float lightAttackDamage;
    public float lightAttackStartDelay;
    public float lightAttackDurationDelay;
    public float lightAttackDuration;
}
[System.Serializable]
public class HeavyAttack
{
    public string heavyAttackParameterNameOfTypeBool;
    public GameObject heavyAttackChild;
    public float heavyAttackStartDelay;
    public float heavyAttackDurationDelay;
    public float heavyAttackDuration;
}

[System.Serializable]
public class Guard
{
    public string guardParameterNameOfTypeBool;
    public GameObject guardChild;
    public float guardStartDelay;
    public float guardDurationDelay;
    public float guardDuration;
}

public interface Iinteruptable
{
    void Interrupted();
}

public class Enemy : MonoBehaviour, Iinteruptable
{
    public GameObject target;
    public float moveSpeed;
    public float attackRange;
    
    public Animator animator;
    
    public LightAttack lightAttackInformation;
    public bool hasLightAttacks;
    
    public HeavyAttack heavyAttackInformation;
    public bool hasHeavyAttacks;

    public Guard guardInformation;
    public bool hasGuard;
    
    private bool[] _abilities;
    private bool _isAttacking;
    private Coroutine _startedAttack;

    private GameObject _currentAttackChild;
    private string _currentAttackParameter;
    public Transform attackTransform;
    public LayerMask hitLayer;


    // Start is called before the first frame update
    void Start()
    {
        _abilities = new[] {hasGuard, hasHeavyAttacks, hasLightAttacks};
    }

    // Update is called once per frame
    void Update()
    {
        bool inDistance = Vector3.Distance(transform.position, target.transform.position) < attackRange;

        if (inDistance && !_isAttacking)
        {
            _startedAttack = StartCoroutine(SelectedAttack());
        }
        else if(!_isAttacking)
        {
            EnemyMovement();
        }
    }

    private void EnemyMovement()
    {
        //paste movement code for the enemy here so he can be interrupted :)
        transform.position =  Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);;
    }

    IEnumerator SelectedAttack()
    {
        var enabledAbilities = 0;
        var checkRolledNumbers = 0;
        var selectedAbility = new bool[3];
        foreach (var t in _abilities)
        {
            if (t)
            {
                enabledAbilities++;
            }
        }

        var rolledNumber = Random.Range(0, enabledAbilities);
        for (var i = 0; i < _abilities.Length; i++)
        {
            if (_abilities[i])
            {
                if (rolledNumber == checkRolledNumbers)
                {
                    selectedAbility[i] = _abilities[i];
                }
                else
                {
                    checkRolledNumbers++;
                }
            }
        }

        if (selectedAbility[0])
            return CO_EnemyGuard();
        if (selectedAbility[1])
            return CO_EnemyHeavyAttack();
        if (selectedAbility[2])
            return CO_EnemyLightAttack();
        return null;
    }

    private IEnumerator CO_EnemyLightAttack()
    {
        _isAttacking = true;
        _currentAttackChild = lightAttackInformation.lightAttackChild;
        _currentAttackParameter = lightAttackInformation.lightAttackParameterNameOfTypeBool;
        yield return new WaitForSeconds(lightAttackInformation.lightAttackStartDelay);
        animator.SetBool(lightAttackInformation.lightAttackParameterNameOfTypeBool, true);
        yield return new WaitForSeconds(lightAttackInformation.lightAttackDurationDelay);
        lightAttackInformation.lightAttackChild.SetActive(true);
        Collider[] hits = Physics.OverlapBox(attackTransform.position,lightAttackInformation.lightAttackSize / 2, Quaternion.identity, hitLayer);
        for (var i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(lightAttackInformation.lightAttackDamage);
        }
        yield return new WaitForSeconds(lightAttackInformation.lightAttackDuration);
        lightAttackInformation.lightAttackChild.SetActive(false);
        animator.SetBool(lightAttackInformation.lightAttackParameterNameOfTypeBool, false);
        _isAttacking = false;
    }

    private IEnumerator CO_EnemyHeavyAttack()
    {
        _isAttacking = true;
        _currentAttackChild = heavyAttackInformation.heavyAttackChild;
        _currentAttackParameter = heavyAttackInformation.heavyAttackParameterNameOfTypeBool;
        yield return new WaitForSeconds(heavyAttackInformation.heavyAttackStartDelay);
        animator.SetBool(heavyAttackInformation.heavyAttackParameterNameOfTypeBool, true);
        yield return new WaitForSeconds(heavyAttackInformation.heavyAttackDurationDelay);
        heavyAttackInformation.heavyAttackChild.SetActive(true);
        yield return new WaitForSeconds(heavyAttackInformation.heavyAttackDuration);
        heavyAttackInformation.heavyAttackChild.SetActive(false);
        animator.SetBool(heavyAttackInformation.heavyAttackParameterNameOfTypeBool, false);
        _isAttacking = false;
    }

    private IEnumerator CO_EnemyGuard()
    {
        _isAttacking = true;
        _currentAttackChild = guardInformation.guardChild;
        _currentAttackParameter = guardInformation.guardParameterNameOfTypeBool;
        yield return new WaitForSeconds(guardInformation.guardStartDelay);
        animator.SetBool(guardInformation.guardParameterNameOfTypeBool, true);
        yield return new WaitForSeconds(guardInformation.guardDurationDelay);
        guardInformation.guardChild.SetActive(true);
        yield return new WaitForSeconds(guardInformation.guardDuration);
        guardInformation.guardChild.SetActive(false);
        animator.SetBool(guardInformation.guardParameterNameOfTypeBool, false);
        _isAttacking = false;
    }

    [ContextMenu("Interrupt")]
    public void Interrupted()
    {
        CancelAttack();
    }

    private void CancelAttack()
    {
        StopCoroutine(_startedAttack);
        _isAttacking = false;
        _currentAttackChild.SetActive(false);
        animator.SetBool(_currentAttackParameter, false);
    }
}