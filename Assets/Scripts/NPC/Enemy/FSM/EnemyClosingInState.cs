using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyClosingInState : EnemyState
{
    GameObject enemyObj;
    EnemyNPCMovement movement;
    EnemyNPCEquipmentSystem equipmentSystem;
    NavMeshAgent agent;
    private Coroutine currentCoroutine;
    public EnemyClosingInState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {
        enemyObj = enemy.gameObject;
        agent = enemyObj.GetComponent<NavMeshAgent>();
        equipmentSystem = enemyObj.GetComponent<EnemyNPCEquipmentSystem>();
        movement = enemyObj.GetComponent<EnemyNPCMovement>();
        currentCoroutine = null;
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Entered to closing in state");
        //agent.ResetPath();
        movement.animator.SetFloat("SpeedZ", 0);
        movement.animator.SetFloat("SpeedX", 0);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
        float distanceToTarget = movement.GetDistanceTotarget();
        if (!movement.DoesHaveTarget())//if the target destroyed
        {
            Debug.Log("doesnt have a target anymore");
            if (currentCoroutine != null)
            {
                movement.StopCoroutine(currentCoroutine);
                movement.OnStopStrafing();
            }
            fsm.ChangeState(enemy.idleState);
            return;
        }
        if (movement.TargetOutOfRange())//if the target moved out of detection radius
        {
            Debug.Log("target is lost");
            if (currentCoroutine != null)
            {
                movement.StopCoroutine(currentCoroutine);
                movement.OnStopStrafing();
            }
            fsm.ChangeState(enemy.idleState);
            return;
        }
        if (distanceToTarget > movement.surroundingRadius + agent.stoppingDistance + 0.2f)
        {
            Debug.Log("Target is too far to close in, start chasing again.");
            if(currentCoroutine != null)
            {
                movement.StopCoroutine(currentCoroutine);
                movement.OnStopStrafing();
            }
            fsm.ChangeState(enemy.chasingState);
            return;
        }
        if (distanceToTarget < movement.attackRadius)
        {
            if (currentCoroutine != null)
            {
                movement.StopCoroutine(currentCoroutine);
                movement.OnStopStrafing();
            }
            fsm.ChangeState(enemy.attackState);
            return;
        }

        if (!movement.isClosingIn) currentCoroutine = movement.StartCoroutine(movement.StrafeTowardsTarget());
    }
}
