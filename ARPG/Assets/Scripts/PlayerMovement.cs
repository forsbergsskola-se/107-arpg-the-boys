using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public GameObject cam;
    public Transform body;
    [NonSerialized]
    public Vector3 move;
    
    
    private CapsuleCollider _col;
    private Rigidbody _rb;
    private PlayerStats _playerStats;
    private Animator _playerAnimator;
    private bool _grounded;
    private Vector3 _groundNormal = Vector3.up;
    private float _regularSpeed;
    private Vector3 _endVel;
    private float _invulnerabilityTimer = 0.0f;
    private bool _isRolling = false;
    private float _desiredSpeed;
    private bool _dash;
    private bool _running;

    [Header("Movement Variables")]
    public float acceleration = 2;
    //public float rollSpeed = 5.0f;
    public float rotationSpeed = 10;
    public float rollRotationSpeed = 20;

    [Header("Move Limitations")]
    public float friction = 0.6f;
    public float maxStrafeSpeed = 30;
    public float maxGroundAngle = 35f;
    public float maxVelocity = 50;
    public LayerMask walkableLayers;
    
    [Header("Other movement variables")]
    public float gravityScale = 9.82f;
    public float rollDuration = 0.4f;
    public bool canMove = true;

    void Start()
    {
        _col = GetComponent<CapsuleCollider>();
        _rb = GetComponent<Rigidbody>();
        _playerStats = GetComponent<PlayerStats>();
        _playerAnimator = body.GetComponent<Animator>();
        _playerStats.DodgeCharges = _playerStats.maxDodgeCharges;
    }

    void Update()
    {
        move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (Input.GetButtonDown("Dash"))
            StartCoroutine(CO_DashActivate());  //Using a coroutine so I can use WaitForFixedUpdate
        if (Input.GetButton("Run"))
            _running = true;
        else
            _running = false;
        
        MovementAnimation();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        _endVel = transform.InverseTransformVector(_rb.velocity);

        if (canMove)
        {
            if (_grounded)
            {
                if (!_isRolling)
                {
                    if (_running)
                    {
                        _endVel = Accelerate(_endVel, _playerStats.RunMoveSpeed, acceleration, _groundNormal);
                        _endVel = Friction(_endVel, _playerStats.RunMoveSpeed, friction, _groundNormal);
                    }
                    else
                    {
                        _endVel = Accelerate(_endVel, _playerStats.WalkMoveSpeed, acceleration, _groundNormal);
                        _endVel = Friction(_endVel, _playerStats.WalkMoveSpeed, friction, _groundNormal);
                    }
                    
                }
            }
            
            if (_isRolling)
            {
                RollTimer();
            }
            else if (_dash && _playerStats.DodgeCharges > 0)
            {
                StartRoll();
            }
        }

        if (!_grounded)
        {
            Gravity();
        }

        RotatePlayer();
        _rb.velocity = transform.TransformVector(_endVel);
    }

    private void MovementAnimation()
    {
        _playerAnimator.SetFloat("Movement Speed", _rb.velocity.magnitude);
    }
    
    void RollTimer()
    {
        _invulnerabilityTimer += Time.deltaTime;
        body.rotation = Quaternion.Lerp(body.rotation, Quaternion.LookRotation(rotateDir), rollRotationSpeed * Time.deltaTime);

        if (_invulnerabilityTimer >= rollDuration)
        {
            _isRolling = false;
            _invulnerabilityTimer = 0.0f;
        }
    }
    
    void StartRoll()
    {
        // Set the player's velocity to the roll speed in the direction the player is currently facing
        _endVel = rotateDir * _playerStats.DodgeSpeed;

        // Uses up one dash
        _playerStats.DodgeCharges = Math.Clamp(_playerStats.DodgeCharges - 1, 0, _playerStats.maxDodgeCharges);
        
        // Play the roll animation
        _playerAnimator.SetTrigger("Roll");

        // Set the invulnerability flag and reset the invulnerability timer
        _isRolling = true;
        _invulnerabilityTimer = 0.0f;
    }

    private IEnumerator CO_DashActivate()
    {
        _dash = true;
        yield return new WaitForFixedUpdate();
        _dash = false;
    }
    
    private Vector3 rotateDir;
    private void RotatePlayer()
    {
        if (MoveFromCamera().sqrMagnitude >= 0.1f && !_isRolling)
        {
            rotateDir = MoveFromCamera();
            body.rotation = Quaternion.Lerp(body.rotation, Quaternion.LookRotation(rotateDir), rotationSpeed * Time.deltaTime);
        }
    }
    
    private Vector3 Accelerate(Vector3 vel, float wishSpeed, float accel, Vector3 normal)
    {
        Vector3 moveRight = Vector3.Cross(MoveFromCamera(), Vector3.up);
        Vector3 planeMove = Vector3.Cross(normal, moveRight).normalized;

        float currentSpeed = Vector3.Dot(vel, planeMove);  //tf does this mean
        float addSpeed = Mathf.Max(wishSpeed - currentSpeed, 0);
        float accelSpeed = Mathf.Min(accel * wishSpeed, addSpeed);
        Vector3 clampedSpeed = Vector3.ClampMagnitude(new Vector3(vel.x, 0, vel.z) + planeMove * accelSpeed, maxStrafeSpeed);
        return clampedSpeed + Vector3.up * vel.y;
        
    }

    private Vector3 MoveFromCamera()
    {
        return (Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * move).normalized;
    }
    
    private Vector3 Friction(Vector3 vel, float wishSpeed, float friction, Vector3 normal)
    {
        Vector3 moveRight = Vector3.Cross(MoveFromCamera(), Vector3.up);
        Vector3 planeMove = Vector3.Cross(normal, moveRight).normalized;
        Vector3 overDesired = vel - planeMove * wishSpeed;

        float frictionCheck = Vector3.Dot(vel, overDesired);
        if (frictionCheck <= 0)
            return vel;

        Vector3 frictionAdd = -overDesired * Mathf.Clamp01(friction);
        return vel + frictionAdd;
    }
    
    private void Gravity()
    {
        _endVel += Vector3.down * (gravityScale * Time.fixedDeltaTime);
    }
    
    private void OnCollisionExit(Collision col)
    {
        if (!_grounded) return;
        _grounded = false;
    }

    private void OnCollisionStay(Collision col)
    {
        for (int i = 0; i < col.contactCount; i++)
        {
            
            if (IsWithinAngle(col.GetContact(i).normal, maxGroundAngle))
            {
                _grounded = true;
                _groundNormal = col.GetContact(i).normal;
            }
        }
    }

    private bool IsWithinAngle(Vector3 normal, float angle)
    {
        return Vector3.Angle(normal, Vector3.up) <= angle;
    }
}
