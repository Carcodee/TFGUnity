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
        playerRef.Jump();
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

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            stateMachineController.SetState("Aiming");
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
