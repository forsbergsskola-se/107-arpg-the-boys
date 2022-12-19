using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class PlayerCombat : MonoBehaviour, IInterruptible
{
    
    public BaseWeapon currentWeapon;
    public Transform weaponHolder;
    public LayerMask hitLayer;
    public Transform attackCenter;
    public float attackRotateSpeed = 20;
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
        if (!isAttacking && _playerMovement.canMove && !_playerMovement.isRolling && currentWeapon != null)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                _currentAttack = StartCoroutine(Attack(currentWeapon.lightAttackSpeed, "Light-attack", IInterruptible.AttackState.LightAttack));
            }
            if (Input.GetButtonDown("Fire2"))
            {
                _currentAttack = StartCoroutine(Attack(currentWeapon.heavyAttackSpeed, "Heavy-attack", IInterruptible.AttackState.HeavyAttack));
            }
            if (Input.GetButtonDown("Fire3"))
            {
                Guard();
            }
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

    private IEnumerator Attack(float attackSpeed, string animTriggerName, IInterruptible.AttackState attackState)
    {
        CurrentAttackState = attackState;
        Debug.Log(animTriggerName);
        
        _playerMovement.playerAnimator.speed = attackSpeed;

        animationEnded = false;
        isAttacking = true;
        _playerMovement._rb.velocity = Vector3.zero;
        
        SetRotatePoint();
        
        // TODO: Plays sound
        
        
        // Plays animation and waits until animation has ended before finishing up the attack
        _playerMovement.playerAnimator.SetTrigger(animTriggerName);

        yield return new WaitUntil(() => animationEnded);
        
        // End attack
        isAttacking = false;
        _playerMovement.playerAnimator.speed = 1;
        CurrentAttackState = IInterruptible.AttackState.NoAttack;
    }

    private void Guard()
    {
        Debug.Log("Guarded!");
    }
    
    private void Parry()
    {
        Debug.Log("Parried!");
        Collider[] hits = Physics.OverlapSphere(transform.position, currentWeapon.parryPunishRange, hitLayer);
        for (var i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out Enemy enemy))
            {
                // TODO: Call enemy parried function here
            }
        }
    }

    public void AttackBox(Vector3 attackColSize, float weaponDamage, bool showBox)
    {
        bool hitEnemy = false;
        Collider[] hits = Physics.OverlapBox(attackCenter.position, attackColSize / 2, Quaternion.identity, hitLayer);
        if (showBox)
            DrawBoxCastBox(attackCenter.position, attackColSize / 2, attackCenter.rotation, Color.cyan);
        for (var i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out IDamageable damageable))
            {
                if (!hitEnemy)
                {
                    hitEnemy = true;
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
        _playerMovement.playerAnimator.speed = 2.5f;
        _playerMovement.playerAnimator.SetTrigger("Interrupted");
        CancelAttack();
    }

    public void CancelAttack()
    {
        if (_currentAttack != null)
        {
            StopCoroutine(_currentAttack);
            isAttacking = false;
            _playerMovement.playerAnimator.speed = 1;
            CurrentAttackState = IInterruptible.AttackState.NoAttack;
        }
    }

    public IInterruptible.AttackState CurrentAttackState { get; set; }
    

    // TA BORT ALL DET HÄR SEN!!! SNÄLLA SNÄLLA SNÄLLA
    //Draws just the box at where it is currently hitting.
    public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color)
    {
        origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
        DrawBox(origin, halfExtents, orientation, color);
    }
    
    
     public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
     {
         Box bottomBox = new Box(origin, halfExtents, orientation);
         DrawBox(bottomBox, color);
     }
     
     public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
     {
         DrawBox(new Box(origin, halfExtents, orientation), color);
     }
     public static void DrawBox(Box box, Color color)
     {
         Debug.DrawLine(box.frontTopLeft,     box.frontTopRight,    color);
         Debug.DrawLine(box.frontTopRight,     box.frontBottomRight, color);
         Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
         Debug.DrawLine(box.frontBottomLeft,     box.frontTopLeft, color);
                                                  
         Debug.DrawLine(box.backTopLeft,         box.backTopRight, color);
         Debug.DrawLine(box.backTopRight,     box.backBottomRight, color);
         Debug.DrawLine(box.backBottomRight,     box.backBottomLeft, color);
         Debug.DrawLine(box.backBottomLeft,     box.backTopLeft, color);
                                                  
         Debug.DrawLine(box.frontTopLeft,     box.backTopLeft, color);
         Debug.DrawLine(box.frontTopRight,     box.backTopRight, color);
         Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
         Debug.DrawLine(box.frontBottomLeft,     box.backBottomLeft, color);
     }
     
     
     
     
     public struct Box
     {
         public Vector3 localFrontTopLeft     {get; private set;}
         public Vector3 localFrontTopRight    {get; private set;}
         public Vector3 localFrontBottomLeft  {get; private set;}
         public Vector3 localFrontBottomRight {get; private set;}
         public Vector3 localBackTopLeft      {get {return -localFrontBottomRight;}}
         public Vector3 localBackTopRight     {get {return -localFrontBottomLeft;}}
         public Vector3 localBackBottomLeft   {get {return -localFrontTopRight;}}
         public Vector3 localBackBottomRight  {get {return -localFrontTopLeft;}}
 
         public Vector3 frontTopLeft     {get {return localFrontTopLeft + origin;}}
         public Vector3 frontTopRight    {get {return localFrontTopRight + origin;}}
         public Vector3 frontBottomLeft  {get {return localFrontBottomLeft + origin;}}
         public Vector3 frontBottomRight {get {return localFrontBottomRight + origin;}}
         public Vector3 backTopLeft      {get {return localBackTopLeft + origin;}}
         public Vector3 backTopRight     {get {return localBackTopRight + origin;}}
         public Vector3 backBottomLeft   {get {return localBackBottomLeft + origin;}}
         public Vector3 backBottomRight  {get {return localBackBottomRight + origin;}}
 
         public Vector3 origin {get; private set;}
 
         public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
         {
             Rotate(orientation);
         }
         public Box(Vector3 origin, Vector3 halfExtents)
         {
             this.localFrontTopLeft     = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
             this.localFrontTopRight    = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
             this.localFrontBottomLeft  = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
             this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
 
             this.origin = origin;
         }
 
 
         public void Rotate(Quaternion orientation)
         {
             localFrontTopLeft     = RotatePointAroundPivot(localFrontTopLeft    , Vector3.zero, orientation);
             localFrontTopRight    = RotatePointAroundPivot(localFrontTopRight   , Vector3.zero, orientation);
             localFrontBottomLeft  = RotatePointAroundPivot(localFrontBottomLeft , Vector3.zero, orientation);
             localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
         }
     }
 
      //This should work for all cast types
     static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
     {
         return origin + (direction.normalized * hitInfoDistance);
     }
     
     static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
     {
         Vector3 direction = point - pivot;
         return pivot + rotation * direction;
     }
     
}