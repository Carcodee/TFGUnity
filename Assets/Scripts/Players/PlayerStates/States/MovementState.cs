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
    Vector2 animInput;
    public override void StateEnter()
    {
        base.StateEnter();
        this.playerRef.sprintFactor = 1;
        networkAnimator.Animator.Play("Movement");
        networkAnimator.Animator.SetFloat("Aiming", 0);

    }

    public override void StateExit()
    {

    }

    public override void StateInput()
    {
        float x= Input.GetAxisRaw("Horizontal");
        float y= Input.GetAxisRaw("Vertical");
        animInput=new Vector2(Input.GetAxis("Horizontal") * this.playerRef.moveAnimationSpeed, Input.GetAxis("Vertical") * this.playerRef.moveAnimationSpeed);
        this.playerRef.Move(x, y);
    }

    public override void StateUpdate()
    {
        StateInput();
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            stateMachineController.SetState("Sprint");
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            stateMachineController.SetState("Crouch");
            return;

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachineController.SetState("Jump");
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            stateMachineController.SetState("Aiming");
            return;

        }
        if (!playerRef.isGrounded)
        {
            stateMachineController.SetState("Falling");
            return;

        }
        
        this.networkAnimator.Animator.SetFloat("X", animInput.x);
        this.networkAnimator.Animator.SetFloat("Y", animInput.y);
        this.networkAnimator.Animator.SetFloat("Speed",  this.playerRef.sprintFactor);
        this.playerRef.Shoot();
        this.playerRef.Reloading();

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
