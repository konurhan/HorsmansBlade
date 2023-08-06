using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected EnemyController enemy;
    protected EnemyStateMachine fsm;

    public EnemyState(EnemyController enemy, EnemyStateMachine fsm)
    {
        this.enemy = enemy;
        this.fsm = fsm;
    }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void ExitState() { }

    public virtual void EnterState() { }
}
