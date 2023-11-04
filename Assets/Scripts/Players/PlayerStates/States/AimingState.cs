using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class AimingState : PlayerStateBase
{
    public AimingState(string name, StateMachineController stateMachineController) : base(name, stateMachineController)
    {
        playerRef = stateMachineController.GetComponent<PlayerController>();
        networkAnimator = stateMachineController.GetComponent<NetworkAnimator>();
    }
    private float aimAnimation;

    public override void StateEnter()
    {
        base.StateEnter();
        playerRef.sprintFactor = 0.3f;
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

    public override void StateUpdate()
    {
        StateInput();
        this.networkAnimator.Animator.SetFloat("X", this.playerRef.move.x);
        this.networkAnimator.Animator.SetFloat("Y", this.playerRef.move.z);
        AimAinimation();
        this.playerRef.Shoot();
        this.playerRef.Reloading();
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            stateMachineController.SetState("Movement");
        }

    }
    public override void StatePhysicsUpdate()
    {
    }
    public override void StateLateUpdate()
    {
        playerRef.ApplyMovement(playerRef.move);
        playerRef.RotatePlayer();
    }

    public void AimAinimation()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            aimAnimation += Time.deltaTime * 5;
        }
        else
        {
            aimAnimation -= Time.deltaTime * 5;
        }
        aimAnimation = Mathf.Clamp(aimAnimation, 0, 1);
        float LerpedAnim = Mathf.Clamp(Mathf.Lerp(0, 1, aimAnimation), 0, 1);
        networkAnimator.Animator.SetFloat("Aiming", aimAnimation);

    }
}
