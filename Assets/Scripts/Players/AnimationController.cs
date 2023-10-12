using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class AnimationController : NetworkBehaviour
{
    NetworkAnimator networkAnimator;
    float aimAnimation;
    public NetworkVariable<float> networkAimAnimation = new NetworkVariable<float>();
    public NetworkVariable<float> networkXMovement = new NetworkVariable<float>();
    public NetworkVariable<float> networkYMovement = new NetworkVariable<float>();

    public NetworkVariable<bool> networkIsSprinting = new NetworkVariable<bool>();
    public NetworkVariable<bool> networkIsCrouching = new NetworkVariable<bool>();
    public NetworkVariable<float> networkSlidingTimer = new NetworkVariable<float>();

    public float slidingTimer=1f;
    void Start()
    {

        GetReferences();
    }

    void Update()
    {

        if (IsOwner)
        {
            MovementAnimation();
            AimAnimation();
            CrouchAndSprint();
            CrouchAnim();
            SetSprintAnim();

        }
    }
    private void FixedUpdate()
    {
  
    }

    void GetReferences()
    {
        networkAnimator = GetComponent<NetworkAnimator>();

    }


    void MovementAnimation()
    {

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        SetMoveAnimationState(x, y);

    }

    public void SetSprintAnim()
    {
        if (IsServer)
        {
            networkAnimator.Animator.SetBool("Sprint", networkIsSprinting.Value);

        }
        else
        {
            SetSprintAnimationServerRpc();
        }
    }
    public void CrouchAnim()
    {
        if (IsServer)
        {
           networkAnimator.Animator.SetBool("Crouch", networkIsCrouching.Value);
        }
        else
        {
            SetCrouchAnimatorServerRpc();
        }  

    }
    public void CrouchAndSprint()
    {
        if(networkIsCrouching.Value&&networkIsCrouching.Value)
        {
            SetSlidingTimer(Time.deltaTime);
            SetIsSprinting(true);
            SetIsCrouching(true);

            if (networkSlidingTimer.Value>= slidingTimer)
            {
                SetIsCrouching(false);
                SetSlidingTimer(0);
            }
            return;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SetIsSprinting(true);

            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                //slide
                SetIsCrouching(true);
            }
            return;
        }
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            SetIsCrouching(true);

            return;
        }

        SetIsCrouching(false);
        SetIsSprinting(false);

    }
    void AimAnimation() {

 
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
            SetAimAnimationState(LerpedAnim);

        

    }
    //public void FinishJump()
    //{
    //    jump = false;
    //    networkAnimator.Animator.SetBool("Jump", jump);

    //}

    //aim
    public void SetMoveAnimation(float x, float y)
    {
        networkAnimator.Animator.SetFloat("X", x);
        networkAnimator.Animator.SetFloat("Y", y);
    }
    public void SetAimAnimation(float aimAnimation)
    {
        networkAnimator.Animator.SetFloat("Aiming", aimAnimation);

    }

    //Sliding
    public void SetSlidingTimer(float timeStep)
    {
        if (IsServer)
        {
            if (networkSlidingTimer.Value > slidingTimer)
            {
                networkSlidingTimer.Value = timeStep;
            }
            else
            {
                networkSlidingTimer.Value += timeStep;
            }
        }
        else
        {
            SetSlidingTimerServerRpc(timeStep);
        }

    }
    #region ServerRPCs

    //movement
    public void SetMoveAnimationState(float x, float y)
    {
        if (IsServer)
        {
            SetMoveAnimation(x, y);

        }
        else
        {
            SetXMovementServerRpc(x);
            SetYMovementServerRpc(y);
            SetMoveAnimationClientServerRpc(networkXMovement.Value, networkYMovement.Value);
        }
    }
    //aim
    public void SetAimAnimationState(float t)
    {
        if (IsServer)
        {
            SetAimAnimation(t);

        }
        else
        {
            SetAimLerpTimeServerRpc(t);
            SetAimAnimationClientServerRpc(networkAimAnimation.Value);
        }
    }

    
    public void SetIsCrouching(bool value)
    {
        if (IsServer)
        {
            networkIsCrouching.Value = value;
        }
        else
        {
            SetCrouchingClientServerRpc(value);
        }
    }


    //Sprint
    public void SetIsSprinting(bool value)
    {
        if (IsServer)
        {
            networkIsSprinting.Value = value;
        }
        else
        {
            SetIsSprintingClientServerRpc(value);
        }
    }


    //Crouch
    [ServerRpc]
    public void SetCrouchAnimatorServerRpc()
    {

        networkAnimator.Animator.SetBool("Crouch", networkIsCrouching.Value);
    }





    #endregion

    #region ClientRPCs
    [ServerRpc]
    public void SetAimAnimationClientServerRpc(float aimAnimation)
    {
        networkAnimator.Animator.SetFloat("Aiming", aimAnimation);

    }
    //aim
    [ServerRpc]
    public void SetMoveAnimationClientServerRpc(float x, float y)
    {
        networkAnimator.Animator.SetFloat("X", x);
        networkAnimator.Animator.SetFloat("Y", y);
    }
    [ServerRpc]
    public void SetSprintAnimationServerRpc()
    {
        networkAnimator.Animator.SetBool("Sprint", networkIsSprinting.Value);

    }
    [ServerRpc]
    public void SetAimLerpTimeServerRpc(float value)
    {
        networkAimAnimation.Value = value;
    }


    //Movement

    [ServerRpc]
    public void SetXMovementServerRpc(float valueX)
    {
        networkXMovement.Value = valueX;
    }
    [ServerRpc]
    public void SetYMovementServerRpc(float valueY)
    {
        networkYMovement.Value = valueY;
    }


    [ServerRpc]
    public void SetCrouchingClientServerRpc(bool value)
    {
        networkIsCrouching.Value = value;
    }


    [ServerRpc]
    public void SetIsSprintingClientServerRpc(bool value)
    {
        networkIsSprinting.Value = value;

    }
    //Sliding
    [ServerRpc]
    public void SetSlidingTimerServerRpc(float value)
    {
        if (networkSlidingTimer.Value > slidingTimer)
        {
            networkSlidingTimer.Value = value;
        }
        else
        {
            networkSlidingTimer.Value += value;
        }
    }

    #endregion

}


