using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class JumpState : PlayerStateBase
{
    public JumpState(string name, StateMachineController stateMachineController) : base(name, stateMachineController)
    {
        playerRef = stateMachineController.GetComponent<PlayerController>();
        networkAnimator = stateMachineController.GetComponent<NetworkAnimator>();
    }
    Vector3 moveDir;
    public override void StateEnter()
    {
        base.StateEnter();
        moveDir = playerRef.move;
        this.playerRef.Jump();
        this.playerRef.gravityMultiplier = 1;
        networkAnimator.Animator.Play("Jump");

    }

    public override void StateExit()
    {
        //animation


    }

    public override void StateInput()
    {

    }

    public override void StateUpdate()
    {
        //TODO: fix jump animation

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachineController.SetState("Jetpack");
        }

        this.playerRef.Shoot();
        this.playerRef.Reloading();
    }
    public override void StatePhysicsUpdate()
    {
        playerRef.ApplyGravity();
    }
    public override void StateLateUpdate()
    {
        if (playerRef._bodyVelocity.y < 0)
        {
            stateMachineController.SetState("Falling");
        }
        playerRef.ApplyMovement(moveDir);
        playerRef.RotatePlayer();
    }

}
