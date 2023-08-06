using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyChasingState : EnemyState
{
    GameObject enemyObj;
    EnemyNPCMovement movement;
    EnemyNPCEquipmentSystem equipmentSystem;
    public EnemyChasingState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {
        enemyObj = enemy.gameObject;
        equipmentSystem = enemyObj.GetComponent<EnemyNPCEquipmentSystem>();
        movement = enemyObj.GetComponent<EnemyNPCMovement>();
    }

    public override void EnterState()
    {
        base.EnterState();
        equipmentSystem.Draw();
        movement.agent.SetDestination(movement.target.transform.position);
        //movement.agent.destination = movement.target.transform.position;
        Debug.Log("setting destination to: "+movement.target.transform.position);
        //movement.StartSpeedUp();
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
            //movement.StartSlowDown();
            fsm.ChangeState(enemy.idleState);
            return;
        }
        if (movement.TargetOutOfRange())
        {
            Debug.Log("target is lost");
            //movement.StartSlowDown();
            fsm.ChangeState(enemy.idleState);
            return;
        }
        if (movement.GetDistanceTotarget() <= movement.attackRadius && movement.GetDistanceTotarget() != -1)
        {
            //Debug.Log("target moved inside of attack range, start attacking. Distance: " + movement.GetDistanceTotarget());
            //Debug.Log("GetDistanceTotarget: enemyPos = " + enemyObj.transform.position + " || targetPos = " + movement.target.transform.position);
            //Debug.Log("target is close, npc will start attacking");
            //movement.StartSlowDown();
            fsm.ChangeState(enemy.attackState);
            return;
        }
        movement.agent.SetDestination(movement.target.transform.position);
        //movement.agent.destination = movement.target.transform.position;
    }
}
