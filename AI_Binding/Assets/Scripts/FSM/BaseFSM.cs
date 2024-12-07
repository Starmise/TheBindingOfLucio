using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFSM : MonoBehaviour
{
    // Variable que indica el estado actual de la FSM.
    private BaseState currentState;

    public virtual void Start()
    {
        // 1) al iniciar la máquina de estados, debe entrar al estado Inicial y asignarlo como el estado actual.
        currentState = GetInitialState();

        if (currentState == null)
        {
            Debug.LogWarning("Peligro, el estado inicial no es valido.");
            return;
        }

        currentState.OnEnter();
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }

    public void ChangeState(BaseState newState)
    {
        // 3) Sale del estado actual y asigna uno nuevo al actua
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }

    public virtual BaseState GetInitialState()
    {
        // Regresa null para que cause error porque la funcion de esta clase padre nunca debe de usarse,
        // siempre se le debe de hacer un override
        return null;
    }
}