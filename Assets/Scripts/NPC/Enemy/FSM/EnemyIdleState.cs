using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    GameObject enemyObj;
    GameObject target;
    EnemyNPCMovement movement;
    public EnemyIdleState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {
        enemyObj = enemy.gameObject;
        movement = enemyObj.GetComponent<EnemyNPCMovement>();
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Entered to idle state");
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
        }
    }

    public override void Update()
    {
        base.Update();
    }
}
