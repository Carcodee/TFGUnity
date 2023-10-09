using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;

public class EnemyController : NetworkBehaviour
{
    EnemyBase enemyBase;
    EnemyState state;

    NavMeshAgent navMeshAgent;
    Transform target;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        navMeshAgent = GetComponent<NavMeshAgent>();

    }

    void Update()
    {
        ChangeEnemyState(EnemyState.Chase);
    }

    private void FixedUpdate()
    {
        
    }

    public void ChangeEnemyState(EnemyState newState)
    {
        state = newState;

        switch (state)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Chase:
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Dead:
                break;
        }
    }


    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Dead
    }
}
