using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementState : PlayerStateBase
{

    
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
