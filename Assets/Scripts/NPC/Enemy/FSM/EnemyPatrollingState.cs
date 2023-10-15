using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrollingState : EnemyState
{
    GameObject enemyObj;
    GameObject target;
    EnemyNPCMovement movement;
    EnemyNPCEquipmentSystem equipmentSystem;
    NavMeshAgent agent;
    List<Transform> waypoints;

    int currentWaypoint;

    public EnemyPatrollingState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {
        enemyObj = enemy.gameObject;
        agent = enemy.GetComponent<NavMeshAgent>();
        movement = enemyObj.GetComponent<EnemyNPCMovement>();
        waypoints = movement.waypoints;
        equipmentSystem = enemyObj.GetComponent<EnemyNPCEquipmentSystem>();
    }

    public override void EnterState()
    {
        Debug.Log("Entered to patrolling state");
        base.EnterState();
        
        movement.animator.SetFloat("SpeedZ", 0);
        movement.animator.SetFloat("SpeedX", 0);
        
        agent.ResetPath();
        agent.isStopped = false;
        agent.SetDestination(waypoints[0].position);//agent should start pattrolling from the first node
        currentWaypoint = 0;
        
        if (equipmentSystem.IsHoldingMelee())
        {
            equipmentSystem.StartCoroutine(equipmentSystem.Sheat());//problem occurs when chasing state draw is called right after this
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (movement.DoesHaveTarget())
        {
            fsm.ChangeState(enemy.chasingState);
            return;
        }
        Patrol();
    }

    public override void Update()
    {
        base.Update();
        
    }

    private void Patrol()
    {
        //if waypoint is reached
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    currentWaypoint++;
                    if (currentWaypoint >= waypoints.Count) currentWaypoint = 0;
                    agent.SetDestination(waypoints[currentWaypoint].position);
                }
            }
        }
    }
}
