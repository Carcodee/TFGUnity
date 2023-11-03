using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class StateMachineController : NetworkBehaviour
{
    [Tooltip("All my states")]
    public StateMachineBase[] states;
    private StateMachineBase currentState;
    public MovementState movementState= new MovementState();
    public JumpState jumpState= new JumpState();
    public SprintState sprintState= new SprintState();
    public CrouchState crouchState= new CrouchState();
    public SlidingState slidingState= new SlidingState();
    public AimingState AimingState= new AimingState();




    public void Initializate()
    {
        states=new StateMachineBase[6];
        states[0] = movementState;
        movementState.stateName = "Movement";
        states[1]=jumpState;
        jumpState.stateName = "Jump";
        states[2]=sprintState;
        sprintState.stateName = "Sprint";
        states[3]=crouchState;
        crouchState.stateName = "Crouch";
        states[4]=slidingState;
        slidingState.stateName = "Sliding";
        states[5]=AimingState;
        AimingState.stateName = "Aiming";
        for (int i = 0; i < states.Length; i++)
        {
            states[i].stateMachineController = this;
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
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                SetState("Sprint");
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    SetState("Sliding");
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                SetState("Aiming");
            }
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                SetState("Crouch");
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SetState("Jump");
            }
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
