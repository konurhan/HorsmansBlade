using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyState
{
    GameObject enemyObj;
    EnemyNPCMovement movement;
    EnemyNPCEquipmentSystem equipmentSystem;
    NavMeshAgent agent;
    public EnemyAttackState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {
        enemyObj = enemy.gameObject;
        agent = enemyObj.GetComponent<NavMeshAgent>();
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
        if(movement.GetDistanceTotarget() > movement.attackRadius + 0.2f)
        {
            Debug.Log("target moved outside of attack range, start closingIn.");
            //agent.ResetPath();//actually no need, agent wont have any path during the attack state
            fsm.ChangeState(enemy.closingInState);//draw will be triggered again in chasingState.EnterState()!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            return;
        }
        //Debug.Log("close enough, enemy will attack now!!");
        enemyObj.GetComponent<EnemyNPCAttack>().HandleDefence();
        enemyObj.GetComponent<EnemyNPCAttack>().HandleAttack();
    }
}
