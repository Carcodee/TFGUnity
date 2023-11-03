using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class SlidingState : PlayerStateBase
{
    public float slidingTimer;
    public float slidingTime=1.5f;
    private float speed;
    private Vector3 moveDir;
    public override void StateEnter()
    {
        base.StateEnter();
        speed = playerRef.sprintFactor=2.5f;
        moveDir = playerRef.move;

    }

    public override void StateExit()
    {

    }


    public override void StateInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        this.playerRef.Move(x, y);
    }
    public override void StatePhysicsUpdate()
    {

    }

    public override void StateUpdate()
    {
        StateInput();
        this.networkAnimator.Animator.SetFloat("X", this.playerRef.move.x);
        this.networkAnimator.Animator.SetFloat("Y", this.playerRef.move.z);
        this.networkAnimator.Animator.SetBool("Sliding", true);
        slidingTimer += Time.fixedDeltaTime;
        if (slidingTimer > slidingTime)
        {
            slidingTimer = 0;
            stateMachineController.SetState("Movement");
        }
    }
    public override void StateLateUpdate()
    {


    }
}



public class CrouchState : PlayerStateBase
{
    public override void StateEnter()
    {


    }

    public override void StateExit()
    {

    }


    public override void StateInput()
    {

    }
    public override void StateUpdate()
    {
        networkAnimator.Animator.SetBool("Crouch", true);
    }
    public override void StatePhysicsUpdate()
    {

    }
    public override void StateLateUpdate()
    {

    }
}