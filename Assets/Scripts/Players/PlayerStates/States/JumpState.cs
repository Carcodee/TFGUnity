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
        if (playerRef.isGrounded)
        {
            playerRef.Jump();
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
        playerRef.ApplyMovement(moveDir);
        playerRef.ApplyJump();
        playerRef.RotatePlayer();
    }

}
