using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using static DrawBoxCast;

[Serializable]
public class soundEffects
{
    public AudioClip walkSound;
    public AudioClip lightAttackSound;
    public AudioClip heavyAttackSound;
    public AudioClip attackHitSound;
    public AudioClip guardSound;
}

[System.Serializable]
public class LightAttack
{
    public bool showHitBox;
    public string lightAttackParameterNameOfTypeBool;
    public Vector3 lightAttackSize;
    public float lightAttackDamage;
    public float lightAttackStartDelay;
}

[System.Serializable]
public class HeavyAttack
{
    public bool showHitBox;
    public string heavyAttackParameterNameOfTypeBool;
    public Vector3 heavyAttackSize;
    public float heavyAttackDamage;
    public float heavyAttackStartDelay;
}

[System.Serializable]
public class Guard
{
    public string guardParameterNameOfTypeBool;
    public GameObject guardChild;
    public float guardStartDelay;
}

public class Enemy : MonoBehaviour, IInterruptible, IDamageable
{
    public AudioSource audioSource;
    public soundEffects soundEffects;
    private GameObject target;
    public float moveSpeed;
    public Vector3 attackRange;
    public bool showHitbox;
    public float maxHealth;
    public EnemyMovement enemyMovement;
    public bool hasAiMovement;

    public Animator animator;
    public string walkAnimationParameterName;

    public string interruptedAnimationParameter;
    public bool interruptible;

    public LightAttack lightAttackInformation;
    public bool hasLightAttacks;

    public HeavyAttack heavyAttackInformation;
    public bool hasHeavyAttacks;

    public Guard guardInformation;
    public bool hasGuard;

    private bool[] _abilities;
    private bool _isAttacking;
    [NonSerialized] public bool endAttack = true;

    private Coroutine _startedAttack;

    private string _currentAttackParameter;
    public Transform attackTransform;
    public LayerMask hitLayer;
    public float rotationSpeed;
    private bool _isInRange;
    private bool _isInterrupted;
    private PlayerCombat _playerCombat;


    // Start is called before the first frame update
    void Start()
    {
        _abilities = new[] { hasGuard, hasHeavyAttacks, hasLightAttacks };
        CurrentHealth = maxHealth;
        _playerCombat = FindObjectOfType<PlayerCombat>();
        target = _playerCombat.gameObject;
    }


    // Update is called once per frame
    void Update()
    {
        Collider[] hits = Physics.OverlapBox(attackTransform.position, attackRange / 2,
            attackTransform.rotation, hitLayer);

        _isInRange = hits.Length > 0;


        if (_isInRange && !_isAttacking && !_isInterrupted)
        {
            _startedAttack = StartCoroutine(SelectedAttack());
        }
        else if (!_isAttacking && !_isInterrupted)
        {
            EnemyMovement();
        }
        else
            animator.SetBool(walkAnimationParameterName, false);

        if (showHitbox)
            DrawBoxCastBox(attackTransform.position, attackRange / 2, attackTransform.rotation, Color.green);
        if (lightAttackInformation.showHitBox)
            DrawBoxCastBox(attackTransform.position, lightAttackInformation.lightAttackSize / 2,
                attackTransform.rotation, Color.cyan);
        if (heavyAttackInformation.showHitBox)
            DrawBoxCastBox(attackTransform.position, heavyAttackInformation.heavyAttackSize / 2,
                attackTransform.rotation, Color.red);


        //dont call every frame later
    }

    public void EnemyMovement()
    {
        if (hasAiMovement)
            enemyMovement.EnemyyMovement();
        else
        {
            //Boss Movement prolly
            animator.SetBool(walkAnimationParameterName, true);
            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            var targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    //selection of attacks
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

    #region Attacks

    //Attacks
    private IEnumerator CO_EnemyLightAttack()
    {
        endAttack = false;
        _isAttacking = true;
        CurrentAttackState = IInterruptible.AttackState.LightAttack;
        _currentAttackParameter = lightAttackInformation.lightAttackParameterNameOfTypeBool;
        yield return new WaitForSeconds(lightAttackInformation.lightAttackStartDelay);
        animator.SetBool(lightAttackInformation.lightAttackParameterNameOfTypeBool, true);
        yield return new WaitUntil(() => endAttack);
        animator.SetBool(lightAttackInformation.lightAttackParameterNameOfTypeBool, false);
        CurrentAttackState = IInterruptible.AttackState.NoAttack;
        _isAttacking = false;
    }

    private IEnumerator CO_EnemyHeavyAttack()
    {
        endAttack = false;
        _isAttacking = true;
        CurrentAttackState = IInterruptible.AttackState.HeavyAttack;
        _currentAttackParameter = heavyAttackInformation.heavyAttackParameterNameOfTypeBool;
        yield return new WaitForSeconds(heavyAttackInformation.heavyAttackStartDelay);
        animator.SetBool(heavyAttackInformation.heavyAttackParameterNameOfTypeBool, true);
        yield return new WaitUntil(() => endAttack);
        animator.SetBool(heavyAttackInformation.heavyAttackParameterNameOfTypeBool, false);
        CurrentAttackState = IInterruptible.AttackState.NoAttack;
        _isAttacking = false;
    }

    private IEnumerator CO_EnemyGuard()
    {
        endAttack = false;
        _isAttacking = true;
        CurrentAttackState = IInterruptible.AttackState.Guard;
        _currentAttackParameter = guardInformation.guardParameterNameOfTypeBool;
        yield return new WaitForSeconds(guardInformation.guardStartDelay);
        animator.SetBool(guardInformation.guardParameterNameOfTypeBool, true);
        yield return new WaitUntil(() => endAttack);
        if (guardInformation.guardChild != null)
            guardInformation.guardChild.SetActive(false);
        animator.SetBool(guardInformation.guardParameterNameOfTypeBool, false);
        CurrentAttackState = IInterruptible.AttackState.NoAttack;
        _isAttacking = false;
    }

    //Used for animation events
    public void InterruptedAnimationEnd()
    {
        _isInterrupted = false;
        animator.SetBool(interruptedAnimationParameter, false);
        animator.speed = 1;
    }

    public void LightAttackAnimationAttack()
    {
        HitBox(lightAttackInformation.lightAttackSize, lightAttackInformation.lightAttackDamage);
    }

    public void LightAttackAnimationEnd()
    {
        endAttack = true;
    }

    public void HeavyAttackAnimation()
    {
        HitBox(heavyAttackInformation.heavyAttackSize, heavyAttackInformation.heavyAttackDamage);
    }

    public void HeavyAttackAnimationEnd()
    {
        endAttack = true;
    }

    public void GuardAnimation()
    {
        guardInformation.guardChild.SetActive(true);
    }

    public void GuardAnimationEnd()
    {
        endAttack = true;
    }

    public void WalkAnimationSound()
    {
        audioSource.clip = soundEffects.walkSound;
        audioSource.Play();
    }

    public void LightAttackAnimationSoundStart()
    {
        audioSource.clip = soundEffects.lightAttackSound;
        audioSource.Play();
    }

    public void HeavyAttackAnimationSoundStart()
    {
        audioSource.clip = soundEffects.heavyAttackSound;
        audioSource.Play();
    }

    public void GuardAnimationSoundStart()
    {
        audioSource.clip = soundEffects.guardSound;
        audioSource.Play();
    }

    //boxcast for hitbox of attack
    public void HitBox(Vector3 size, float damage)
    {
        Collider[] hits = Physics.OverlapBox(attackTransform.position, size / 2,
            attackTransform.rotation, hitLayer);
        for (var i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out IDamageable damageable))
            {
                audioSource.clip = soundEffects.attackHitSound;
                audioSource.Play();
                if (hits[i].TryGetComponent(out IInterruptible interruptible))
                    if (interruptible.CurrentAttackState != IInterruptible.AttackState.Guard &&
                        interruptible.CurrentAttackState != IInterruptible.AttackState.Parry)
                        interruptible.Interrupt();
                    else
                    {
                        if (interruptible.CurrentAttackState == IInterruptible.AttackState.Guard)
                            damage *= 0.05f;
                        if (interruptible.CurrentAttackState == IInterruptible.AttackState.Parry)
                        {
                            //_playerCombat.Parry();
                            damage = 0;
                        }
                    }

                damageable.TakeDamage(damage);
            }
        }
    }

    #endregion

    private void Death()
    {
        if (hasAiMovement)
            enemyMovement.navMeshAgent.enabled = false;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            script.enabled = false;
        }

        animator.SetTrigger("Dead");
        Collider[] hitBox = GetComponentsInChildren<Collider>();
        for (var i = 0; i < hitBox.Length; i++)
        {
            hitBox[i].enabled = false;
        }

        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = false;
        DropLogic dropTable = GetComponent<DropLogic>();
        dropTable.DropItem();
    }


    private void CancelAttack()
    {
        if (_startedAttack != null)
            StopCoroutine(_startedAttack);
        if (guardInformation.guardChild != null)
            guardInformation.guardChild.SetActive(false);
        if (_currentAttackParameter != null)
            animator.SetBool(_currentAttackParameter, false);
        _isAttacking = false;
        CurrentAttackState = IInterruptible.AttackState.NoAttack;
    }

    #region Interfaces

    //Interfaces

    public float CurrentHealth { get; private set; }

    public IInterruptible.AttackState CurrentAttackState { get; set; }

    public bool IsInterruptible => interruptible;

    [ContextMenu("Interrupt")]
    public void Interrupt()
    {
        CancelAttack();
        _isInterrupted = true;
        animator.SetBool(interruptedAnimationParameter, true);
    }

    public void Parried()
    {
        animator.speed = 0.5f;
        Interrupt();
    }


    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
            Death();
    }

    #endregion
}