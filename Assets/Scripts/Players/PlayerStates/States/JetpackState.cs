using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class JetpackState : PlayerStateBase
{
    public JetpackState(string name, StateMachineController stateMachineController) : base(name, stateMachineController)
    {
        playerRef = stateMachineController.GetComponent<PlayerController>();
        networkAnimator = stateMachineController.networkAnimator;

    }
    Vector3 moveDir;
    private float aimAnimation;

    public override void StateEnter()
    {
        networkAnimator.Animator.SetBool("Fall", true);
        playerRef._bodyVelocity.y = 0;
        moveDir = playerRef.move;
        this.playerRef.gravityMultiplier = 0.05f;
        aimAnimation = 0;

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
        this.playerRef.AimAinimation(ref aimAnimation, networkAnimator);
        playerRef.CreateAimTargetPos();
        this.playerRef.Shoot();
        this.playerRef.Reloading();
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            stateMachineController.SetState("Falling");
        }
    }
    public override void StatePhysicsUpdate()
    {
        playerRef.ApplyGravity();
        if (playerRef.characterController.isGrounded)
        {
            stateMachineController.SetState("Movement");
        }
    }
    public override void StateLateUpdate()
    {
    }
    
}
