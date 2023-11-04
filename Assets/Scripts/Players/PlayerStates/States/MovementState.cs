using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class MovementState : PlayerStateBase
{
    public MovementState(string name, StateMachineController stateMachineController) : base(name, stateMachineController)
    {
        playerRef = stateMachineController.GetComponent<PlayerController>();
        networkAnimator = stateMachineController.GetComponent<NetworkAnimator>();
    }

    public override void StateEnter()
    {
        base.StateEnter();
        this.playerRef.sprintFactor = 1;
    }

    public override void StateExit()
    {

    }

    public override void StateInput()
    {
        float x= Input.GetAxis("Horizontal");
        float y= Input.GetAxis("Vertical");
        this.playerRef.Move(x, y);
    }

    public override void StateUpdate()
    {
        StateInput();
        this.networkAnimator.Animator.SetFloat("X", this.playerRef.move.x);
        this.networkAnimator.Animator.SetFloat("Y", this.playerRef.move.z);
        this.playerRef.Shoot();
        this.playerRef.Reloading();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            stateMachineController.SetState("Sprint");
        }
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            stateMachineController.SetState("Crouch");
        }
        if (Input.GetKey(KeyCode.Space))
        {
            stateMachineController.SetState("Jump");
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            stateMachineController.SetState("Aiming");
        }
    }
    public override void StatePhysicsUpdate()
    {

    }
    public override void StateLateUpdate()
    {
        this.playerRef.ApplyMovement(this.playerRef.move);
        this.playerRef.RotatePlayer();
    }
}
