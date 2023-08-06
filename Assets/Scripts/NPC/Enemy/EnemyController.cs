using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Body Parts")]
    public GameObject head;
    public GameObject torso;
    public GameObject armLeft;
    public GameObject armRight;
    public GameObject legLeft;
    public GameObject legRight;

    #region State Machine
    public EnemyStateMachine stateMachine;
    public EnemyIdleState idleState;
    public EnemyChasingState chasingState;
    public EnemyAttackState attackState;
    public EnemyFleeState fleeState;
    #endregion


    private void Awake()
    {
        stateMachine = new EnemyStateMachine();
        idleState = new EnemyIdleState(this, stateMachine);
        chasingState = new EnemyChasingState(this, stateMachine);
        attackState = new EnemyAttackState(this, stateMachine);
        fleeState = new EnemyFleeState(this, stateMachine);

        SetbodyPartReferences();
    }

    void Start()
    {
        stateMachine.Initialize(idleState, gameObject);
    }

    void Update()
    {
        stateMachine.currentstate.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.currentstate.FixedUpdate();
    }

    public void SetbodyPartReferences()
    {
        head.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        torso.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        armLeft.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        armRight.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        legLeft.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        legRight.GetComponent<BodyPart>().SetPlayerReference(gameObject);
    }
}
