using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    public GameObject enemy;
    public EnemyState currentstate;

    public void Initialize(EnemyState startState, GameObject enemy)
    {
        this.enemy = enemy;
        currentstate = startState;
        currentstate.EnterState();
    }

    public void ChangeState(EnemyState newState)
    {
        currentstate.ExitState();
        currentstate = newState;
        currentstate.EnterState();
    }
}
