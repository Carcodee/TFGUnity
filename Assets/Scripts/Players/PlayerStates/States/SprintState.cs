using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintState : PlayerStateBase
{
    
    public override void StateEnter()
    {
        base.StateEnter();
        playerRef.sprintFactor = 2;
    }

    public override void StateExit()
    {

    }

    public override void StateInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        this.playerRef.Move(x,y);
    }

    public override void StateUpdate()
    {
        StateInput();
        this.networkAnimator.Animator.SetFloat("X", this.playerRef.move.x);
        this.networkAnimator.Animator.SetFloat("Y", this.playerRef.move.z);
        this.networkAnimator.Animator.SetBool("Sprint", true);
    }
    public override void StatePhysicsUpdate()
    {
    }
    public override void StateLateUpdate()
    {
        this.playerRef.ApplyMovement(this.playerRef.move);
    }
}
