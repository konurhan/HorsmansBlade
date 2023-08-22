using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleState : EnemyState
{
    GameObject enemyObj;
    GameObject target;
    EnemyNPCMovement movement;
    EnemyNPCEquipmentSystem equipmentSystem;
    NavMeshAgent agent;
    public EnemyIdleState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {
        enemyObj = enemy.gameObject;
        agent = enemy.GetComponent<NavMeshAgent>();
        movement = enemyObj.GetComponent<EnemyNPCMovement>();
        equipmentSystem = enemyObj.GetComponent<EnemyNPCEquipmentSystem>();
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Entered to idle state");
        agent.ResetPath();
        movement.animator.SetFloat("SpeedZ", 0);
        movement.animator.SetFloat("SpeedX", 0);
        equipmentSystem.Sheat();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if(movement.DoesHaveTarget())
        {
            fsm.ChangeState(enemy.chasingState);
            return;
        }
    }

    public override void Update()
    {
        base.Update();
    }
}
