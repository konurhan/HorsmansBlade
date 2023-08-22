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
        base.EnterState();
        equipmentSystem.Draw();
        //agent.updateRotation = true;
        Vector3 destination = movement.target.transform.position + movement.target.transform.forward * movement.destinationOffset.z + movement.target.transform.right * movement.destinationOffset.x;
        movement.agent.SetDestination(destination);
        Debug.Log(destination);
        if (agent.updateRotation) Debug.Log("agent can rotate");


        Debug.Log("Entered to chasing state");
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
            fsm.ChangeState(enemy.idleState);
            return;
        }
        if (movement.TargetOutOfRange())
        {
            Debug.Log("target is lost");
            fsm.ChangeState(enemy.idleState);
            return;
        }
        if (movement.GetDistanceToSurroundingDestination() <= agent.stoppingDistance)
        {
            agent.ResetPath();//doesn't update rotation when there are no paths present
            /*Quaternion lookRotation = Quaternion.LookRotation(movement.target.transform.position  - enemyObj.transform.position, Vector3.up);
            enemyObj.transform.rotation = lookRotation;*/
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
        //Debug.Log(destination);
    }
}
