using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChasingState : EnemyState
{
    GameObject enemyObj;
    EnemyNPCMovement movement;
    EnemyNPCEquipmentSystem equipmentSystem;
    NavMeshAgent agent;

    //SurroundingDest dest;
    public EnemyChasingState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {
        enemyObj = enemy.gameObject;
        agent = enemy.GetComponent<NavMeshAgent>();
        equipmentSystem = enemyObj.GetComponent<EnemyNPCEquipmentSystem>();
        movement = enemyObj.GetComponent<EnemyNPCMovement>();
        //dest = movement.surroundingDestination;
    }

    public override void EnterState()
    {
        Debug.Log("enetered to chasing state");
        base.EnterState();

        equipmentSystem.StartCoroutine(equipmentSystem.Draw());
        
        movement.AssignClosestEmptyDestination();

        agent.isStopped = false;
        Vector3 destination = movement.target.transform.position + movement.target.transform.forward * movement.destinationOffset.z + movement.target.transform.right * movement.destinationOffset.x;
        movement.agent.SetDestination(destination);
    }

    public override void ExitState()
    {
        base.ExitState();
        movement.surroundingDestination.occupantNPC = null;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        movement.AssignClosestEmptyDestination();
        Vector3 destination = movement.target.transform.position + movement.target.transform.forward * movement.destinationOffset.z + movement.target.transform.right * movement.destinationOffset.x;
        movement.agent.SetDestination(destination);
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
        if (movement.GetDistanceTotarget() <= movement.attackRadius)
        {
            agent.ResetPath();
            agent.isStopped = true;
            fsm.ChangeState(enemy.attackState);
            return;
        }
        //if closer to player then current surrounding radius, directly go in to closing in state: happens when enmy approaches from behind
        if (movement.GetDistanceTotarget() < movement.surroundingDestination.relativePosToPlayer.transform.localPosition.magnitude)
        {
            agent.ResetPath();
            agent.isStopped = true;
            fsm.ChangeState(enemy.closingInState);
            return;
        }
        //if closer to player then current surrounding destination, directly go in to closing in state
        if (movement.GetDistanceTotarget() < (enemyObj.transform.position - movement.surroundingDestination.relativePosToPlayer.transform.position).magnitude)
        {
            agent.ResetPath();
            agent.isStopped = true;
            fsm.ChangeState(enemy.closingInState);
            return;
        }
        //if reached to destination
        if (movement.GetDistanceToSurroundingDestination() <= agent.stoppingDistance)
        {
            agent.ResetPath();
            agent.isStopped = true;
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
