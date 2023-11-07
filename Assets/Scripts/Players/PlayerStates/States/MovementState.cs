using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
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
        networkAnimator.Animator.Play("Movement");


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
        this.networkAnimator.Animator.SetFloat("Speed",  this.playerRef.sprintFactor);
        this.playerRef.Shoot();
        this.playerRef.Reloading();
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            stateMachineController.SetState("Sprint");
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            stateMachineController.SetState("Crouch");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachineController.SetState("Jump");
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        { 
            stateMachineController.SetState("Aiming");
        }
    }
    public override void StatePhysicsUpdate()
    {

    }
    public override void StateLateUpdate()
    {
        this.playerRef.RotatePlayer();
        this.playerRef.ApplyMovement(this.playerRef.move);
    }
}
