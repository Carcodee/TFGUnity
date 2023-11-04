using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class SprintState : PlayerStateBase
{
    public SprintState(string name, StateMachineController stateMachineController) : base(name, stateMachineController)
    {
        playerRef = stateMachineController.GetComponent<PlayerController>();
        networkAnimator = stateMachineController.GetComponent<NetworkAnimator>();
    }
    public override void StateEnter()
    {
        base.StateEnter();
        playerRef.sprintFactor = 2;
    }

    public override void StateExit()
    {
        this.networkAnimator.Animator.SetBool("Sprint", false);
    }

    public override void StateInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        this.playerRef.Move(x,y);
    }

    public override void StateUpdate()
    {
        StateInput();
        this.networkAnimator.Animator.SetFloat("X", this.playerRef.move.x);
        this.networkAnimator.Animator.SetFloat("Y", this.playerRef.move.z);
        this.networkAnimator.Animator.SetBool("Sprint", true);
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            stateMachineController.SetState("Sliding");
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            stateMachineController.SetState("Movement");
        }

    }
    public override void StatePhysicsUpdate()
    {
    }
    public override void StateLateUpdate()
    {
        this.playerRef.ApplyMovement(this.playerRef.move);
    }
}