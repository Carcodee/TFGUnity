using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Netcode.Components;

public class StateMachineController : NetworkBehaviour
{
    [Tooltip("All my states")]
    public StateMachineBase[] states;
    private StateMachineBase currentState;
    public MovementState movementState;
    public JumpState jumpState;
    public SprintState sprintState;
    public CrouchState crouchState;
    public SlidingState slidingState;
    public AimingState AimingState;
    public FallingState FallingState;
    public JetpackState jetpackState;





    public void Initializate()
    {
        movementState = new MovementState("Movement", this);
        jumpState = new JumpState("Jump", this);
        sprintState = new SprintState("Sprint", this);
        crouchState = new CrouchState("Crouch", this);
        slidingState = new SlidingState("Sliding", this);
        AimingState = new AimingState("Aiming", this);
        FallingState= new FallingState("Falling", this);
        jetpackState = new JetpackState("Jetpack", this);

        states =new StateMachineBase[8];
        states[0] = movementState;
        states[1]=jumpState;
        states[2]=sprintState;
        states[3]=crouchState;
        states[4]=slidingState;
        states[5]=AimingState;
        states[6]=FallingState;
        states[7]=jetpackState;

        for (int i = 0; i < states.Length; i++)
        {
            states[i].stateMachineController = this;
            states[i].StateEnter();
        }


        if (states != null && states.Length > 0)
        {
            string initialStateName = states[0].stateName;
            SetState(initialStateName);
        }
    }


    public void StateUpdate()
    {
        if (currentState != null )
        {
            currentState.StateUpdate();
        }
    }

    public void StatePhysicsUpdate()
    {
        if (currentState != null)
        {
            currentState.StatePhysicsUpdate();
        }
    }

    public void StateLateUpdate()
    {
        if (currentState != null)
        {
            currentState.StateLateUpdate();
        }
    }
    public void SetState(string statename)
    {
        StateMachineBase nextState = GetStateWithName(statename);
        if (nextState == null) return;
        //Exit state execution
        if (currentState != null)
        {
            currentState.StateExit();
        }
        //New state
        currentState = nextState;
        //Entry state execution
        currentState.StateEnter();
    }

    private StateMachineBase GetStateWithName(string stateName)
    {
        foreach (StateMachineBase state in states)
        {
            if (state.stateName == stateName)
            {
                return state;
            }
        }
        return null;
    }
}
