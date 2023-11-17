using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class SprintState : PlayerStateBase
{
    public SprintState(string name, StateMachineController stateMachineController) : base(name, stateMachineController)
    {
        playerRef = stateMachineController.GetComponent<PlayerController>();
        networkAnimator = stateMachineController.networkAnimator;
    }
    public override void StateEnter()
    {
        base.StateEnter();
        playerRef.sprintFactor = 1.5f;
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
        this.networkAnimator.Animator.SetFloat("Speed", this.playerRef.sprintFactor);

        this.networkAnimator.Animator.SetBool("Sprint", true);
        
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            stateMachineController.SetState("Movement");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachineController.SetState("Jump");
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            stateMachineController.SetState("Sliding");
            return;
        }

        if (!playerRef.isGrounded)
        {
            stateMachineController.SetState("Falling");
        }

    }
    public override void StatePhysicsUpdate()
    {

    }
    public override void StateLateUpdate()
    {
        this.playerRef.RotatePlayer();
        playerRef.ApplyMovement(playerRef.move);

    }
}
