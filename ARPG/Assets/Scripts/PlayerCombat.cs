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
    public bool showParryHitBox;
    public GameObject parryDebugHitBox;
    [NonSerialized] public bool isAttacking;

    [Header("Audio")] public AudioSource audioSource;
    public AudioClip[] slashWhoosh, enemyHitSound, blockSound, parrySound, guardSound;

    private Coroutine _currentAttack;
    private Rigidbody _rb;
    private PlayerMovement _playerMovement;
    private PlayerStats _playerStats;

    [NonSerialized] public bool animationEnded;
    public float slashCost;

    void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
        _playerMovement = GetComponent<PlayerMovement>();
        parryDebugHitBox.SetActive(false);
    }

    void Update()
    {
        //Debug.Log(CurrentAttackState);
        if (!isAttacking && _playerMovement.canMove && !_playerMovement.isRolling && currentWeapon != null)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                _currentAttack = StartCoroutine(CO_Attack(currentWeapon.lightAttackSpeed, "Light-attack",
                    IInterruptible.AttackState.LightAttack));
            }

            if (Input.GetButtonDown("Fire2"))
            {
                _currentAttack = StartCoroutine(CO_Attack(currentWeapon.heavyAttackSpeed, "Heavy-attack",
                    IInterruptible.AttackState.HeavyAttack));
            }

            if (Input.GetButtonDown("Fire3"))
            {
                _currentAttack = StartCoroutine(CO_Guard(currentWeapon.guardTime, currentWeapon.parryTime));
            }

            if (Input.GetKeyDown(KeyCode.Q) && _playerStats.CurrentMana >= slashCost)
            {
                _currentAttack = StartCoroutine(CO_Attack(1, "Slash-attack", IInterruptible.AttackState.NoAttack));
                _playerStats.ChangeMana(slashCost);
            }
        }

        if (currentWeapon == null)
        {
            return;
        }

        if (showLightHitBox)
        {
            Vector3 worldOffset =
                attackCenter.TransformDirection(currentWeapon.lightAttackColOffset); // Offset in world space
            DrawBoxCastBox(attackCenter.position + worldOffset, currentWeapon.lightAttackColSize / 2,
                attackCenter.rotation, Color.cyan);
        }

        if (showHeavyHitBox)
        {
            Vector3 worldOffset =
                attackCenter.TransformDirection(currentWeapon.heavyAttackColOffset); // Offset in world space
            DrawBoxCastBox(attackCenter.position + worldOffset, currentWeapon.heavyAttackColSize / 2,
                attackCenter.rotation, Color.cyan);
        }

        if (showParryHitBox && currentWeapon != null)
        {
            parryDebugHitBox.SetActive(true);
            Vector3 hitBoxSize = new Vector3(currentWeapon.parryPunishRange * 2, currentWeapon.parryPunishRange * 2,
                currentWeapon.parryPunishRange * 2);
            parryDebugHitBox.transform.parent = null;
            parryDebugHitBox.transform.localScale = hitBoxSize;
            parryDebugHitBox.transform.parent = transform;
        }
        else
            parryDebugHitBox.SetActive(false);
    }

    private void FixedUpdate()
    {
        RotateToMouse();
    }

    private void RotateToMouse()
    {
        _playerMovement.body.rotation = Quaternion.Lerp(_playerMovement.body.rotation,
            Quaternion.LookRotation(_playerMovement.rotateDir), attackRotateSpeed * Time.deltaTime);
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
            _playerMovement.rotateDir = Vector3
                .ProjectOnPlane(
                    (Camera.main.transform.position + ray.direction.normalized * vectorDistance) - transform.position,
                    Vector3.up).normalized;
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
        audioSource.pitch = 1;
        audioSource.clip = GetRandomAudioClip(guardSound);
        audioSource.Play();

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
        audioSource.clip = GetRandomAudioClip(parrySound);
        audioSource.Play();
        Collider[] hits = Physics.OverlapSphere(transform.position, currentWeapon.parryPunishRange, hitLayer);
        for (var i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out Enemy enemy) && enemy.IsInterruptible)
            {
                enemy.Parried();
            }
        }

        _playerMovement.playerAnimator.SetTrigger("Parry");
        CancelAttack();
    }

    public void AttackBox(Vector3 attackColSize, Vector3 attackColOffset, float weaponDamage)
    {
        bool hasHitEnemy = false;
        Vector3 worldOffset = attackCenter.TransformDirection(attackColOffset); // Offset in world space
        Collider[] hits = Physics.OverlapBox(attackCenter.position + worldOffset, attackColSize / 2,
            Quaternion.identity, hitLayer);
        for (var i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out IDamageable damageable))
            {
                if (!hasHitEnemy)
                {
                    hasHitEnemy = true;
                    audioSource.pitch = 1;
                    audioSource.clip = GetRandomAudioClip(enemyHitSound);
                    _playerStats.AddDodge(1);
                    Debug.Log("An enemy was hit");
                }

                float damage = weaponDamage + _playerStats.AttackPower;

                if (hits[i].TryGetComponent(out IInterruptible interruptible))
                {
                    if (ShouldInterrupt(this, interruptible))
                    {
                        interruptible.Interrupt();
                    }

                    if (interruptible.CurrentAttackState == IInterruptible.AttackState.Guard)
                    {
                        damage = 0;
                        audioSource.clip = GetRandomAudioClip(blockSound);
                    }
                }

                audioSource.Play();
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
        if (enemy.IsInterruptible)
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
                    if (enemy.CurrentAttackState == IInterruptible.AttackState.LightAttack ||
                        enemy.CurrentAttackState == IInterruptible.AttackState.Guard)
                    {
                        return true;
                    }

                    break;
            }
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
    public bool IsInterruptible { get; }
}