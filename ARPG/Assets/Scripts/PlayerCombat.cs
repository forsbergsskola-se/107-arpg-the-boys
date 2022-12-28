using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using static DrawBoxCast;

public class PlayerCombat : MonoBehaviour, IInterruptible
{
    
    public BaseWeapon currentWeapon;
    public Transform weaponHolder;
    public LayerMask hitLayer;
    public Transform attackCenter;
    public float attackRotateSpeed = 20;
    public bool showLightHitBox;
    public bool showHeavyHitBox;
    [NonSerialized]
    public bool isAttacking;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] slashWhoosh, enemyHitSound;

    private Coroutine _currentAttack;
    private Rigidbody _rb;
    private PlayerMovement _playerMovement;
    private PlayerStats _playerStats;

    [NonSerialized] 
    public bool animationEnded;
    
    void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        Debug.Log(CurrentAttackState);
        if (!isAttacking && _playerMovement.canMove && !_playerMovement.isRolling && currentWeapon != null)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                _currentAttack = StartCoroutine(CO_Attack(currentWeapon.lightAttackSpeed, "Light-attack", IInterruptible.AttackState.LightAttack));
            }
            if (Input.GetButtonDown("Fire2"))
            {
                _currentAttack = StartCoroutine(CO_Attack(currentWeapon.heavyAttackSpeed, "Heavy-attack", IInterruptible.AttackState.HeavyAttack));
            }
            if (Input.GetButtonDown("Fire3"))
            {
                _currentAttack = StartCoroutine(CO_Guard(currentWeapon.guardTime, currentWeapon.parryTime));
            }
        }

        if (currentWeapon == null)
        {
            return;
        }
        if (showLightHitBox)
        {
            Vector3 worldOffset = attackCenter.TransformDirection(currentWeapon.lightAttackColOffset); // Offset in world space
            DrawBoxCastBox(attackCenter.position + worldOffset, currentWeapon.lightAttackColSize / 2, attackCenter.rotation, Color.cyan);
        }

        if (showHeavyHitBox)
        {
            Vector3 worldOffset = attackCenter.TransformDirection(currentWeapon.heavyAttackColOffset); // Offset in world space
            DrawBoxCastBox(attackCenter.position + worldOffset, currentWeapon.heavyAttackColSize / 2, attackCenter.rotation, Color.cyan);
        }
        
    }

    private void FixedUpdate()
    {
        RotateToMouse();
    }

    private void RotateToMouse()
    {
        _playerMovement.body.rotation = Quaternion.Lerp(_playerMovement.body.rotation, Quaternion.LookRotation(_playerMovement.rotateDir), attackRotateSpeed * Time.deltaTime);
    }
    
    private void SetRotatePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
            _playerMovement.rotateDir = Vector3.ProjectOnPlane((hit.point - transform.position), Vector3.up).normalized;
        else
        {
            float playerYDiff = Camera.main.transform.position.y - transform.position.y;
            float angle = Vector3.Angle(Vector3.down, ray.direction);
            float vectorDistance = Mathf.Abs(playerYDiff / Mathf.Cos(angle));
            _playerMovement.rotateDir = Vector3.ProjectOnPlane((Camera.main.transform.position + ray.direction.normalized * vectorDistance) - transform.position, Vector3.up).normalized;
        }
    }

    private IEnumerator CO_Attack(float attackSpeed, string animTriggerName, IInterruptible.AttackState attackState)
    {
        CurrentAttackState = attackState;
        Debug.Log(animTriggerName);
        
        // TODO: Attack speed modifier.
        
        _playerMovement.playerAnimator.speed = attackSpeed;

        animationEnded = false;
        isAttacking = true;
        _playerMovement._rb.velocity = Vector3.zero;
        
        SetRotatePoint();

        // Plays animation and waits until animation has ended before finishing up the attack
        _playerMovement.playerAnimator.SetTrigger(animTriggerName);

        yield return new WaitUntil(() => animationEnded);
        
        // End attack
        isAttacking = false;
        _playerMovement.playerAnimator.speed = 1;
        CurrentAttackState = IInterruptible.AttackState.NoAttack;
    }

    private IEnumerator CO_Guard(float guardTime, float parryTime)
    {
        Debug.Log("Guarded!");

        
        
        _playerMovement.playerAnimator.speed = 1f / guardTime;
        animationEnded = false;
        isAttacking = true;
        _playerMovement._rb.velocity = Vector3.zero;

        _playerMovement.playerAnimator.SetTrigger("Guard");

        float elapsedTime = 0f;
        while (elapsedTime < guardTime)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime < parryTime)
            {
                CurrentAttackState = IInterruptible.AttackState.Parry;
            }
            else
            {
                CurrentAttackState = IInterruptible.AttackState.Guard;
            }

            yield return null;
        }

        // Wait for the animation to end
        yield return new WaitUntil(() => animationEnded);

        // End guard
        isAttacking = false;
        _playerMovement.playerAnimator.speed = 1;
        CurrentAttackState = IInterruptible.AttackState.NoAttack;
    }
    
    public void Parry()
    {
        Debug.Log("Parried!");
        Collider[] hits = Physics.OverlapSphere(transform.position, currentWeapon.parryPunishRange, hitLayer);
        for (var i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out Enemy enemy))
            {
                enemy.Parried();
            }
        }
    }

    public void AttackBox(Vector3 attackColSize, Vector3 attackColOffset, float weaponDamage)
    {
        bool hasHitEnemy = false;
        Vector3 worldOffset = attackCenter.TransformDirection(attackColOffset); // Offset in world space
        Collider[] hits = Physics.OverlapBox(attackCenter.position + worldOffset, attackColSize / 2, Quaternion.identity, hitLayer);        // if (showBox)
        //     DrawBoxCastBox(attackCenter.position, attackColSize / 2, attackCenter.rotation, Color.cyan);
        for (var i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out IDamageable damageable))
            {
                if (!hasHitEnemy)
                {
                    hasHitEnemy = true;
                    audioSource.pitch = 1;
                    audioSource.clip = GetRandomAudioClip(enemyHitSound);
                    audioSource.Play();
                    _playerStats.AddDash();
                    Debug.Log("An enemy was hit");
                }
                if (hits[i].TryGetComponent(out IInterruptible interruptible) && ShouldInterrupt(this, interruptible))
                    interruptible.Interrupt();
                float damage = weaponDamage + _playerStats.AttackPower;
                damageable.TakeDamage(damage);
            }
        }
    }

    public AudioClip GetRandomAudioClip(AudioClip[] audioClips)
    {
        // Select a random index from the array
        int randomIndex = Random.Range(0, audioClips.Length);

        // Return the audio clip at the random index
        return audioClips[randomIndex];
    }

    private bool ShouldInterrupt(IInterruptible player, IInterruptible enemy)
    {
        switch (player.CurrentAttackState)
        {
            case IInterruptible.AttackState.LightAttack:
                if (enemy.CurrentAttackState == IInterruptible.AttackState.LightAttack)
                {
                    return true;
                }
                break;
            case IInterruptible.AttackState.HeavyAttack:
                if (enemy.CurrentAttackState == IInterruptible.AttackState.LightAttack || enemy.CurrentAttackState == IInterruptible.AttackState.Guard)
                {
                    return true;
                }
                break;
        }

        return false;
    }

    public void Interrupt()
    {
        _playerMovement.playerAnimator.SetTrigger("Interrupted");
        CancelAttack();
    }

    public bool CancelAttack()
    {
        if (_currentAttack != null)
        {
            StopCoroutine(_currentAttack);
            isAttacking = false;
            _playerMovement.playerAnimator.speed = 1;
            CurrentAttackState = IInterruptible.AttackState.NoAttack;
            return true;
        }

        return false;
    }

    public IInterruptible.AttackState CurrentAttackState { get; set; }
}