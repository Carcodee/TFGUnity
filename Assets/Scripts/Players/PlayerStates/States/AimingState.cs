using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class AimingState : PlayerStateBase
{
    public AimingState(string name, StateMachineController stateMachineController) : base(name, stateMachineController)
    {
        playerRef = stateMachineController.GetComponent<PlayerController>();
        networkAnimator = stateMachineController.GetComponent<NetworkAnimator>();
    }
    private float aimAnimation;

    public override void StateEnter()
    {
        base.StateEnter();
        playerRef.sprintFactor = 0.3f;
    }

    public override void StateExit()
    {
        networkAnimator.Animator.SetFloat("Aiming", 0);

    }

    public override void StateInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        this.playerRef.Move(x, y);
    }

    public override void StateUpdate()
    {
        StateInput();
        this.networkAnimator.Animator.SetFloat("X", this.playerRef.move.x);
        this.networkAnimator.Animator.SetFloat("Y", this.playerRef.move.z);
        playerRef.CreateAimTargetPos();
        //this.playerRef.AimAinimation(ref aimAnimation,networkAnimator);
        networkAnimator.Animator.SetFloat("Aiming", 1);

        this.playerRef.Shoot();
        this.playerRef.Reloading();
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
        playerRef.ApplyMovement(playerRef.move);

    }


}
