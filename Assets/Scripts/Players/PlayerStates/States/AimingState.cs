using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class AimingState : PlayerStateBase
{
    public AimingState(string name, StateMachineController stateMachineController) : base(name, stateMachineController)
    {
        playerRef = stateMachineController.GetComponent<PlayerController>();
        networkAnimator = stateMachineController.networkAnimator;
    }
    private float aimAnimation;

    public override void StateEnter()
    {
        base.StateEnter();
        playerRef.sprintFactor = 0.7f;
        this.playerRef.shootRefraction=0.01f;
    }

    public override void StateExit()
    {
        networkAnimator.Animator.SetFloat("Aiming", 0);
        this.playerRef.shootRefraction = 0.1f;

    }

    public override void StateInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        this.playerRef.Move(x, y);
    }

    public override void StateUpdate()
    {
        if (!playerRef.isGrounded)
        {
            stateMachineController.SetState("Falling");
            return;

        }
        StateInput();
        this.networkAnimator.Animator.SetFloat("X", this.playerRef.move.x);
        this.networkAnimator.Animator.SetFloat("Y", this.playerRef.move.z);
        playerRef.CreateAimTargetPos();
        //this.playerRef.AimAinimation(ref aimAnimation,networkAnimator);
        networkAnimator.Animator.SetFloat("Aiming", 1);


        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            stateMachineController.SetState("Crouch");
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            stateMachineController.SetState("Movement");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachineController.SetState("Jump");
            return;
        }


    }
    public override void StatePhysicsUpdate()
    {
    }   
    public override void StateLateUpdate()
    {
        playerRef.RotatePlayer();

    }


}
