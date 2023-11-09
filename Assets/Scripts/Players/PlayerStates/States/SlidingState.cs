using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class SlidingState : PlayerStateBase
{
    public SlidingState(string name, StateMachineController stateMachineController) : base(name, stateMachineController)
    {
        playerRef = stateMachineController.GetComponent<PlayerController>();
        networkAnimator = stateMachineController.GetComponent<NetworkAnimator>();
    }
    public float slidingTimer;
    public float slidingTime=2.0f;
    private Vector3 moveDir;
    public override void StateEnter()
    {
        base.StateEnter();
        playerRef.sprintFactor = 2.0f;
        moveDir = playerRef.move;
        this.networkAnimator.Animator.Play("Slide");

    }

    public override void StateExit()
    { 
    }

    public override void StateInput()
    {

    }
 

    public override void StateUpdate()
    {
        StateInput();
        this.networkAnimator.Animator.SetFloat("X", moveDir.x);
        this.networkAnimator.Animator.SetFloat("Y", moveDir.z);
        slidingTimer += Time.deltaTime;
        if (slidingTimer > slidingTime)
        {
            slidingTimer = 0;
            stateMachineController.SetState("Movement");
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            stateMachineController.SetState("Jump");
        }
    }
    public override void StateLateUpdate()
    {
        playerRef.ApplyMovement(moveDir);
        playerRef.ApplyGravity();
        playerRef.RotatePlayer();
    }
    public override void StatePhysicsUpdate()
    {

    }
}



public class CrouchState : PlayerStateBase
{
    public CrouchState(string name, StateMachineController stateMachineController) : base(name, stateMachineController)
    {
        playerRef = stateMachineController.GetComponent<PlayerController>();
        networkAnimator = stateMachineController.GetComponent<NetworkAnimator>();
    }
    public override void StateEnter()
    {
       playerRef.sprintFactor = 0.5f;

    }

    public override void StateExit()
    {
        networkAnimator.Animator.SetBool("Crouch", false);
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
        this.networkAnimator.Animator.SetFloat("X", playerRef.move.x);
        this.networkAnimator.Animator.SetFloat("Y", playerRef.move.z);
        networkAnimator.Animator.SetBool("Crouch", true);
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            stateMachineController.SetState("Aiming");
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            stateMachineController.SetState("Movement");
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