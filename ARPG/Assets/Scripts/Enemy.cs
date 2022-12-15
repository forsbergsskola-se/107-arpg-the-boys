using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public class LightAttack
{
    public string lightAttackParameterNameOfTypeBool;
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
    public Vector3 heavyAttackSize;
    public float heavyAttackDamage;
    public float heavyAttackStartDelay;
    public float heavyAttackDurationDelay;
    public float heavyAttackDuration;
}

[System.Serializable]
public class Guard
{
    public string guardParameterNameOfTypeBool;
    public Vector3 guardSize;
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
        var inDistance = Vector3.Distance(transform.position, target.transform.position) < attackRange;

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
        float timer = 0;
        _currentAttackParameter = lightAttackInformation.lightAttackParameterNameOfTypeBool;
        yield return new WaitForSeconds(lightAttackInformation.lightAttackStartDelay);
        animator.SetBool(lightAttackInformation.lightAttackParameterNameOfTypeBool, true);
        yield return new WaitForSeconds(lightAttackInformation.lightAttackDurationDelay);
        while (timer < lightAttackInformation.lightAttackDuration)
        {
            Collider[] hits = Physics.OverlapBox(attackTransform.position,lightAttackInformation.lightAttackSize / 2, Quaternion.identity, hitLayer);
            for (var i = 0; i < hits.Length; i++)
            {
                if (hits[i].TryGetComponent(out IDamageable damageable))
                    damageable.TakeDamage(lightAttackInformation.lightAttackDamage);
            }

            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(lightAttackInformation.lightAttackDuration);
        animator.SetBool(lightAttackInformation.lightAttackParameterNameOfTypeBool, false);
        _isAttacking = false;
    }

    private IEnumerator CO_EnemyHeavyAttack()
    {
        _isAttacking = true;
        float timer = 0;
        _currentAttackParameter = heavyAttackInformation.heavyAttackParameterNameOfTypeBool;
        yield return new WaitForSeconds(heavyAttackInformation.heavyAttackStartDelay);
        animator.SetBool(heavyAttackInformation.heavyAttackParameterNameOfTypeBool, true);
        yield return new WaitForSeconds(heavyAttackInformation.heavyAttackDurationDelay);
        while (timer < heavyAttackInformation.heavyAttackDuration)
        {
            Collider[] hits = Physics.OverlapBox(attackTransform.position,heavyAttackInformation.heavyAttackSize / 2, Quaternion.identity, hitLayer);
            for (var i = 0; i < hits.Length; i++)
            {
                if (hits[i].TryGetComponent(out IDamageable damageable))
                    damageable.TakeDamage(heavyAttackInformation.heavyAttackDamage);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        
        animator.SetBool(heavyAttackInformation.heavyAttackParameterNameOfTypeBool, false);
        _isAttacking = false;
    }

    private IEnumerator CO_EnemyGuard()
    {
        _isAttacking = true;
        float timer = 0;
        _currentAttackParameter = guardInformation.guardParameterNameOfTypeBool;
        yield return new WaitForSeconds(guardInformation.guardStartDelay);
        animator.SetBool(guardInformation.guardParameterNameOfTypeBool, true);
        yield return new WaitForSeconds(guardInformation.guardDurationDelay);
        while (timer < guardInformation.guardDuration)
        {
            Collider[] hits = Physics.OverlapBox(attackTransform.position,guardInformation.guardSize / 2, Quaternion.identity, hitLayer);
            for (var i = 0; i < hits.Length; i++)
            {
                if (hits[i].TryGetComponent(out IDamageable damageable))
                    damageable.TakeDamage(heavyAttackInformation.heavyAttackDamage);
                //change for guard
            }
            DrawBoxCastBox(attackTransform.position,lightAttackInformation.lightAttackSize / 2, Quaternion.identity, Color.magenta);
            timer += Time.deltaTime;
            yield return null;
        }
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
        animator.SetBool(_currentAttackParameter, false);
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