using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(1)]
public class EnemyController : MonoBehaviour
{
    public int npcID;

    [Header("Body Parts")]
    public GameObject head;
    public GameObject torso;
    public GameObject leftUpperArm;
    public GameObject rightUpperArm;
    public GameObject leftForearm;
    public GameObject rightForearm;
    public GameObject legLeftUpper;
    public GameObject legRightUpper;
    public GameObject legLeftLower;
    public GameObject legRightLower;

    [Header("Skinned Mesh Parts")]
    public Transform NakedParts;
    public Transform ArmourSlots;

    #region State Machine
    public EnemyStateMachine stateMachine;
    public EnemyIdleState idleState;
    public EnemyPatrollingState patrollingState;
    public EnemyChasingState chasingState;
    public EnemyClosingInState closingInState;
    public EnemyAttackState attackState;
    public EnemyMoveOutState moveOutState;
    public EnemyFleeState fleeState;
    #endregion

    [SerializeField] private string curState;

    private void Awake()
    {
        stateMachine = new EnemyStateMachine();
        idleState = new EnemyIdleState(this, stateMachine);
        patrollingState = new EnemyPatrollingState(this, stateMachine);
        chasingState = new EnemyChasingState(this, stateMachine);
        closingInState = new EnemyClosingInState(this, stateMachine);
        attackState = new EnemyAttackState(this, stateMachine);
        moveOutState = new EnemyMoveOutState(this, stateMachine);
        fleeState = new EnemyFleeState(this, stateMachine);

        SetbodyPartReferences();
        NPCManager.Instance.enemyNPCs.Add(this);
        //NPCManager.Instance.SetEnemyDestinationOffsets();
    }

    void Start()
    {
        stateMachine.Initialize(idleState, gameObject);
    }

    void Update()
    {
        stateMachine.currentstate.Update();
        curState = stateMachine.currentstate.ToString();
    }

    private void FixedUpdate()
    {
        stateMachine.currentstate.FixedUpdate();
    }

    public void SetbodyPartReferences()
    {
        head.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        torso.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        leftForearm.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        rightForearm.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        leftUpperArm.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        rightUpperArm.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        legRightUpper.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        legLeftUpper.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        legRightLower.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        legLeftLower.GetComponent<BodyPart>().SetPlayerReference(gameObject);
    }
}
