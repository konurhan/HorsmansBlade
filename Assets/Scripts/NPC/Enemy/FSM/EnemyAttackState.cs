using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    GameObject enemyObj;
    EnemyNPCMovement movement;
    EnemyNPCEquipmentSystem equipmentSystem;
    public EnemyAttackState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {
        enemyObj = enemy.gameObject;
        equipmentSystem = enemyObj.GetComponent<EnemyNPCEquipmentSystem>();
        movement = enemyObj.GetComponent<EnemyNPCMovement>();
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Entered to attack state");
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
        if(!movement.DoesHaveTarget())//target is destroyed
        {
            equipmentSystem.Sheat();
            fsm.ChangeState(enemy.idleState);
            return;
        }
        if (movement.TargetOutOfRange())
        {
            equipmentSystem.Sheat();
            fsm.ChangeState(enemy.idleState);
            return;
        }
        if(movement.GetDistanceTotarget() <= movement.detectionRadius && movement.GetDistanceTotarget() > movement.attackRadius + 0.1f)
        {
            //Debug.Log("target moved outside of attack range, start chasing. Distance: " + movement.GetDistanceTotarget());
            //Debug.Log("GetDistanceTotarget: enemyPos = " + enemyObj.transform.position + " || targetPos = " + movement.target.transform.position);
            fsm.ChangeState(enemy.chasingState);//draw will be triggered again in chasingState.EnterState()!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            return;
        }
        if(movement.GetDistanceTotarget() <= movement.attackRadius)
        {
            //attack
            Debug.Log("close enough, enemy will attack now!!");
            enemyObj.GetComponent<EnemyNPCAttack>().HandleAttackDefence();
            return;
        }
    }
}
