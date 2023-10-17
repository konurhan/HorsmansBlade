using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveOutState : EnemyState
{
    GameObject enemyObj;
    EnemyNPCMovement movement;
    EnemyNPCAttack attack;
    EnemyNPCEquipmentSystem equipmentSystem;
    NavMeshAgent agent;
    public EnemyMoveOutState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {
        enemyObj = enemy.gameObject;
        agent = enemyObj.GetComponent<NavMeshAgent>();
        equipmentSystem = enemyObj.GetComponent<EnemyNPCEquipmentSystem>();
        movement = enemyObj.GetComponent<EnemyNPCMovement>();
        attack = enemyObj.GetComponent<EnemyNPCAttack>();
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Entered to moveOut state");
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        movement.AssignClosestEmptyDestination();
        movement.CheckForStrafeDirectionChange();
    }

    public override void Update()
    {
        base.Update();

        if (!movement.DoesHaveTarget())//target is destroyed
        {
            Debug.Log("doesnt have a target anymore");
            fsm.ChangeState(enemy.patrollingState);
            return;
        }
        if (movement.TargetOutOfRange())
        {
            Debug.Log("target is lost");
            fsm.ChangeState(enemy.patrollingState);
            return;
        }
        if (movement.GetDistanceTotarget() >= movement.attackRadius)
        {
            //burada attack.strafeAroundRoutine'i de durdurmali mi????
            if (movement.strafingRoutine != null)
            {
                movement.StopCoroutine(movement.strafingRoutine);
                movement.OnStopStrafing();
            }
            fsm.ChangeState(enemy.attackState);
            return;
        }
        
        if (!movement.isMovingOut)
        {
            movement.strafingRoutine = movement.StartCoroutine(movement.KeepAttackingDistanceFromTarget());
            Debug.Log("New move out coroutine has started");
        }
    }
}
