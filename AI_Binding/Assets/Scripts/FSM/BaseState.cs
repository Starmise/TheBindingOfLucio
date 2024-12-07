using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState : MonoBehaviour
{
    public string Name = "BaseState";

    // Necesita tener una referencia o forma de contactar a la maquina de estados que es su dueña.
    public BaseFSM FSMRef;

    public BaseState()
    {
        Name = "BaseState";
    }

    public BaseState(string inName, BaseFSM inBaseFSM)
    {
        Name = inName;
        FSMRef = inBaseFSM;
    }

    public virtual void InitializeState(BaseFSM inBaseFSM)
    {
        FSMRef = inBaseFSM;
    }

    // Enter se llama cuando la FSM asigna a este estado como el estado actual.
    public virtual void OnEnter()
    {
        Debug.Log("Se ha inicilizado el estado: " + Name);
    }

    public virtual void OnUpdate()
    {
        // Lógica del Update, que es un update y se actualiza en cada frame por ser un update
    }

    public virtual void OnExit()
    {
        Debug.Log("OnExit del estado: " + Name);
    }
}