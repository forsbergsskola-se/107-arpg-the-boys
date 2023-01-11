using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private GameObject _player;
    [NonSerialized]
    public NavMeshAgent navMeshAgent;
    private PlayerCombat _playerCombat;
    private Enemy _enemy;
    public float startWaitTime = 4;
    public float timeToRotate = 2;
    public float speedRun = 9;

    public float viewRadius = 15;
    public float viewAngle = 90;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float agroDistance;
    
    public Transform[] waypoints;
    private int _currentWaypointIndex;

    private Vector3 _playerLastPosition = Vector3.zero;
    private Vector3 _playerPosition;

    private float _waitTime;
    private float _timeToRotate;
    private bool _playerInRange;
    private bool _playerNear;
    private bool _isPatrol;
    private bool _caughtPlayer;
    
    void Start()
    {
        _playerPosition = Vector3.zero;
        _isPatrol = true;
        _caughtPlayer = false;
        _playerInRange = false;
        _waitTime = startWaitTime;
        _timeToRotate = timeToRotate;
        _currentWaypointIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();
        _playerCombat = FindObjectOfType<PlayerCombat>();
        _enemy = GetComponent<Enemy>();
        _player = _playerCombat.gameObject;
        
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedRun;
            navMeshAgent.SetDestination(_player.transform.position);
    }

    public void EnemyyMovement()
    {
        EnviromentView();
        if (!_isPatrol)
        {
            Chasing();
        }
        else
        {
            Patroling();
        }
    }
    private void Chasing()
    {  
        Move(speedRun);
        navMeshAgent.SetDestination(_player.transform.position);
    }

    private void Patroling()
    {
        Move(speedRun);
        navMeshAgent.SetDestination(_player.transform.position);
    }
    
    public void NextPoint()
    {
        _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);
    }

    private void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
        _enemy.animator.SetBool(_enemy.walkAnimationParameterName, false);
    }

    private void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
        _enemy.animator.SetBool(_enemy.walkAnimationParameterName, true);
    }

    void CaughtPlayer()
    {
        _caughtPlayer = true;
    }

    private void LookingPlayer(Vector3 playerLastPosition)
    {
        navMeshAgent.SetDestination(playerLastPosition);
        if (Vector3.Distance(transform.position, playerLastPosition) <= 0.3)
        {
            if (_waitTime <= 0)
            {
                _playerNear = false;
                Move(speedRun);
                navMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);
                _waitTime = startWaitTime;
                _timeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                _waitTime -= Time.deltaTime;
            }

        }

    }

    private void EnviromentView()
    {
        _playerPosition = _player.transform.position;
    }
}
