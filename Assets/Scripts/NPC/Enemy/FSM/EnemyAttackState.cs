using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyState
{
    GameObject enemyObj;
    EnemyNPCMovement movement;
    EnemyNPCAttack attack;
    EnemyNPCEquipmentSystem equipmentSystem;
    NavMeshAgent agent;
    public EnemyAttackState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {
        enemyObj = enemy.gameObject;
        agent = enemyObj.GetComponent<NavMeshAgent>();
        equipmentSystem = enemyObj.GetComponent<EnemyNPCEquipmentSystem>();
        movement = enemyObj.GetComponent<EnemyNPCMovement>();
        attack = enemyObj.GetComponent<EnemyNPCAttack>();
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
        movement.AssignClosestEmptyDestination();
        movement.CheckForStrafeDirectionChange();
    }

    public override void Update()
    {
        base.Update();

        if (attack.attacking || attack.blocking) return;//if an attack action or coroutine is underway

        if(!movement.DoesHaveTarget())//target is destroyed
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
        if(movement.GetDistanceTotarget() > movement.attackRadius + 0.5f)//this will only be called after strafe around is finished and speed has come down to 0, shouldn't cause feet to jump
        {
            Debug.Log("target moved outside of attack range, start closingIn.");
            fsm.ChangeState(enemy.closingInState);
            //Debug.Break();
            return;
        }
        //Debug.Log("close enough, enemy will attack now!!");
        enemyObj.GetComponent<EnemyNPCAttack>().HandleDefence();
        enemyObj.GetComponent<EnemyNPCAttack>().HandleAttack();
    }
}
