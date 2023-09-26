using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class AnimationController : NetworkBehaviour
{
    NetworkAnimator networkAnimator;
    float aimAnimation;
    public NetworkVariable<float> networkAimAnimation = new NetworkVariable<float>();
    public NetworkVariable<float> networkXMovement = new NetworkVariable<float>();
    public NetworkVariable<float> networkYMovement = new NetworkVariable<float>();

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
        }

    }

    void GetReferences()
    {
        networkAnimator = GetComponent<NetworkAnimator>();

    }

    void MovementAnimation()
    {

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        SetMoveAnimationStateServerRpc(x, y);



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
            SetAimAnimationStateServerRpc(LerpedAnim);

        

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


    #region RPCs


    [ServerRpc]
    public void SetMoveAnimationStateServerRpc(float x, float y)
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

    [ServerRpc]
    public void SetAimAnimationStateServerRpc(float t)
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
    public void SetAimLerpTimeServerRpc(float value)
    {
        networkAimAnimation.Value = 1;
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

    #endregion

}


