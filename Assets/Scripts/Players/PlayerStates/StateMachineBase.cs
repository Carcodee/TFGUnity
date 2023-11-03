using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[System.Serializable]
public abstract class StateMachineBase : NetworkBehaviour
{
    public string stateName;
    public StateMachineController stateMachineController;
    public abstract void StateEnter();
    public abstract void StateExit();
    public abstract void StateUpdate();
    public abstract void StatePhysicsUpdate();
    public abstract void StateLateUpdate();
}
