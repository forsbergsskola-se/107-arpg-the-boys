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
        navMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);
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
        _playerNear = false;
        _playerLastPosition = Vector3.zero;
        if (!_caughtPlayer)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(_playerPosition);
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!_caughtPlayer && Vector3.Distance(transform.position, _player.transform.position) >= agroDistance)
            {
                _isPatrol = true;
                _playerNear = false;
                Move(speedRun);
                _timeToRotate = timeToRotate;
                _waitTime = startWaitTime;
                navMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);
            }
            else //if (Vector3.Distance(transform.position, player.transform.position) >= 2.5f)
            {
                Stop();
                _waitTime -= Time.deltaTime;
            }

        }
    }

    private void Patroling()
    {
        if (_playerNear)
        {
            if (_timeToRotate <= 0)
            {
                Move(speedRun);
                LookingPlayer(_playerLastPosition);
            }
            else
            {
                Stop();
                _timeToRotate -= Time.deltaTime;

            }
        }
        else
        {
            _playerNear = false;
            _playerPosition = Vector3.zero;
            navMeshAgent.SetDestination(waypoints[_currentWaypointIndex].position);
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (_waitTime <= 0)
                {
                    NextPoint();
                    Move(speedRun);
                    _waitTime = startWaitTime;
                }
                else
                {
                    
                    Stop();
                    _waitTime -= Time.deltaTime;
                }
            }
        }
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
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    _playerInRange = true;
                    _isPatrol = false;
                }
                else
                {
                    _playerInRange = false;
                }
            }
            if (Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                _playerInRange = false;
            }
        }

        if (_playerInRange)
        {
            _playerPosition = _player.transform.position;
        }
    }
}
