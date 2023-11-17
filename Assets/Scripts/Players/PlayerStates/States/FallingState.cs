using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class FallingState : PlayerStateBase
{
    public FallingState(string name, StateMachineController stateMachineController) : base(name, stateMachineController)
    {
        playerRef = stateMachineController.GetComponent<PlayerController>();
        networkAnimator = stateMachineController.networkAnimator;

    }
    Vector3 moveDir;
    public override void StateEnter()
    {
        networkAnimator.Animator.SetBool("Fall", true);
        playerRef._bodyVelocity.y = 0;
        moveDir = playerRef.move;
        this.playerRef.gravityMultiplier = 1;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachineController.SetState("Jetpack");
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            stateMachineController.SetState("Aiming");
        }

        this.playerRef.Shoot();
        this.playerRef.Reloading();
    }
    public override void StatePhysicsUpdate()
    {

    }
    public override void StateLateUpdate()
    {
        playerRef.ApplyMovement(moveDir);
        playerRef.RotatePlayer();
        playerRef.ApplyGravity();
        if (playerRef.characterController.isGrounded)
        {
            stateMachineController.SetState("Movement");
        }
    }

}
