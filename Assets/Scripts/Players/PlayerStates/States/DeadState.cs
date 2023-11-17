using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class DeadState :PlayerStateBase
{
    public DeadState(string name, StateMachineController stateMachineController) : base(name, stateMachineController)
    {
        playerRef = stateMachineController.GetComponent<PlayerController>();
        networkAnimator = stateMachineController.networkAnimator;

    }

    bool isBattleRoyale;
    float currentRespawnTimer;
    
    public override void StateEnter()
    {
        currentRespawnTimer= 0;
        
    }

    public override void StateExit()
    {
        this.playerRef.ActivatePlayer();
        int health = playerRef.playerStats.statHolder[1];
        this.playerRef.playerStats.SetHealth(health);
        //respawn
    }

    public override void StateLateUpdate()
    {
        
    }

    public override void StateInput()
    {
        
    }

    public override void StatePhysicsUpdate()
    {
    }

    public override void StateUpdate()
    {
        currentRespawnTimer += Time.deltaTime;
        Debug.Log("Time to respawn: "+GameController.instance.respawnTime);
        
        if (currentRespawnTimer>GameController.instance.respawnTime)
        {
            stateMachineController.SetState("Movement");
        }
    }


}
