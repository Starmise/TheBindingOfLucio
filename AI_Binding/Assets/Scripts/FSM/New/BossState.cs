using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossState
{
    protected BossStateMachine stateMachine;
    protected GameObject boss;

    public BossState(BossStateMachine stateMachine, GameObject boss)
    {
        this.stateMachine = stateMachine;
        this.boss = boss;
    }

    // Métodos virtuales para sobrescribir en los estados concretos
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void UpdateLogic() { }
    public virtual void UpdatePhysics() { }
}
