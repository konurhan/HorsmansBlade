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
    [SerializeField] private Coroutine currentCoroutine;
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
        movement.animator.SetFloat("SpeedZ", 0);//don't make sudden cahnges to speed valuse in order to avoid feet jumps in anim
        movement.animator.SetFloat("SpeedX", 0);
        //Debug.Break();
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
            fsm.ChangeState(enemy.patrollingState);
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
            fsm.ChangeState(enemy.patrollingState);
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
        if (distanceToTarget <= movement.attackRadius)//feet jump must be happening in this transition
        {
            if (currentCoroutine != null)
            {
                movement.StopCoroutine(currentCoroutine);
                movement.OnStopStrafing();
            }
            fsm.ChangeState(enemy.attackState);
            return;
        }

        if (!movement.isClosingIn || currentCoroutine == null)
        {
            currentCoroutine = movement.StartCoroutine(movement.StrafeTowardsTarget());
            Debug.Log("New strafe towards target coroutine has started");
        }
    }
}
