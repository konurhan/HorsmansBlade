using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChasingState : EnemyState
{
    GameObject enemyObj;
    EnemyNPCMovement movement;
    EnemyNPCEquipmentSystem equipmentSystem;
    NavMeshAgent agent;
    public EnemyChasingState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {
        enemyObj = enemy.gameObject;
        agent = enemy.GetComponent<NavMeshAgent>();
        equipmentSystem = enemyObj.GetComponent<EnemyNPCEquipmentSystem>();
        movement = enemyObj.GetComponent<EnemyNPCMovement>();
    }

    public override void EnterState()
    {
        Debug.Log("enetered to chasing state");
        base.EnterState();
        equipmentSystem.Draw();
        Vector3 destination = movement.target.transform.position + movement.target.transform.forward * movement.destinationOffset.z + movement.target.transform.right * movement.destinationOffset.x;
        movement.agent.SetDestination(destination);
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
        if (!movement.DoesHaveTarget())
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
        if (movement.GetDistanceToSurroundingDestination() <= agent.stoppingDistance)
        {
            agent.ResetPath();
            fsm.ChangeState(enemy.closingInState);
            return;
        }
        else
        {
            //Debug.Log("did not arrive to surrounding distance");
            //Debug.Log("sur dist: " + movement.GetDistanceToSurroundingDestination());
        }

        Vector3 destination = movement.target.transform.position + movement.target.transform.forward * movement.destinationOffset.z + movement.target.transform.right * movement.destinationOffset.x;
        movement.agent.SetDestination(destination);
    }
}
