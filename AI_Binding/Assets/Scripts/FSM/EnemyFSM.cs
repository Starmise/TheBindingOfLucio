using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : BaseFSM
{
    private MeleeState _meleeState;

    public MeleeState MeleeState
    { get { return _meleeState; } }

    public BaseEnemy Owner; //Contexto


    public override void Start()
    {
        _meleeState = gameObject.AddComponent<MeleeState>();
        _meleeState.FSMRef = this;

        base.Start(); // Esto es mandar a llamar funcion del padre.
    }

    public override BaseState GetInitialState()
    {
        return _meleeState; // ESTO NO DEBE SER NULL.
    }

}