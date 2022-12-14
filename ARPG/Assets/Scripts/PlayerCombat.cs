using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public BaseWeapon currentWeapon;
    public LayerMask hitLayer;
    public Transform attackCenter;
    private bool _canAttack = true;
    private PlayerMovement _playerMovement;
    private PlayerStats _playerStats;
    
    void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (_canAttack)
        {
            if (Input.GetButton("Fire1"))
            {
                LightAttack();
            }
            if (Input.GetButtonDown("Fire2"))
            {
                HeavyAttack();
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
        _playerMovement.body.rotation = Quaternion.Lerp(_playerMovement.body.rotation, Quaternion.LookRotation(_playerMovement.rotateDir), _playerMovement.rotationSpeed * Time.deltaTime);
    }
    
    private void SetRotatePoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            _playerMovement.rotateDir = Vector3.ProjectOnPlane(transform.position + hit.point, Vector3.up);
    }
    
    private void LightAttack()
    {
        Debug.Log("Light attack!");
        
        // TODO: Cooldown
        StartCoroutine(CO_AttackCooldown(currentWeapon.lightAttackCooldown));
        
        // TODO: Does animation
        
        // TODO: Plays sound
        
        // TODO: Rotate towards mouse
        SetRotatePoint();
        
        // TODO: Freeze player movement
        
        // TODO: Boxcast with take damage
        AttackBox(currentWeapon.lightAttackColSize, currentWeapon.lightAttackDamage);

        
    }
    
    private void HeavyAttack()
    {
        Debug.Log("Heavy attack!");
        
        // TODO: Does animation
        
        // TODO: Plays sound
        
        // TODO: Boxcast with take damage
        AttackBox(currentWeapon.heavyAttackColSize, currentWeapon.heavyAttackDamage);
        
        // TODO: Cooldown
    }
    
    private void Guard()
    {
        Debug.Log("Guarded!");
    }
    
    private void Parry()
    {
        Debug.Log("Parried!");
    }

    private void AttackBox(Vector3 attackColSize, float weaponDamage)
    {
        Collider[] hits = Physics.OverlapBox(attackCenter.position, attackColSize / 2, Quaternion.identity, hitLayer);
        DrawBoxCastBox(attackCenter.position, attackColSize / 2, attackCenter.rotation, Color.cyan);
        for (var i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out IDamageable damageable))
            {
                float damage = weaponDamage + _playerStats.AttackPower;
                damageable.TakeDamage(damage);
            }
        }
    }

    private IEnumerator CO_AttackCooldown(float cooldown)
    {
        _canAttack = false;
        _playerMovement.canMove = false;
        yield return new WaitForSeconds(cooldown);
        _canAttack = true;
        _playerMovement.canMove = true;
    }
    
    
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
