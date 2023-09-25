using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatsController : NetworkBehaviour, IDamageable
{
    public UnityAction OnSpawnPlayer;
    [Header("References")]
    public StatsTemplate[] statsTemplates;
    public NetworkVariable <int> statsTemplateSelected;

    [Header("Stats")]
    [SerializeField] private NetworkVariable<int> haste;
    [SerializeField] private NetworkVariable<int> health;
    [SerializeField] private NetworkVariable<int> stamina;
    [SerializeField] private NetworkVariable<int> damage;
    [SerializeField] private NetworkVariable<int> armor;
    [SerializeField] private NetworkVariable<float> speed;



    [Header("Interfaces")]
    private IDamageable iDamageable;

    [Header("NetCode")]
    public NetworkObject playerObj;
    //public NetworkVariable<float> netHaste = new NetworkVariable<float>();

    private void Awake()
    {
     
    }
    void Start()
    {
        if (IsOwner)
        {
            OnSpawnPlayer += InitializateStats;
            IDamageable iDamageable = GetComponent<IDamageable>();
        }

    }

    void Update()
    {

    }


    void InitializateStats()
    {

        playerObj = GetComponent<NetworkObject>();
        if (statsTemplates[statsTemplateSelected.Value] == null)
        {
            Debug.LogError("StatsTemplate is null");
            return;
        }
        Debug.Log("StatsTemplate chosed by " + playerObj.OwnerClientId.ToString() + " " + statsTemplates[statsTemplateSelected.Value].preset);
        SetHasteServerRpc(statsTemplates[statsTemplateSelected.Value].haste);
        SetHealthServerRpc(statsTemplates[statsTemplateSelected.Value].health);
        SetStaminaServerRpc(statsTemplates[statsTemplateSelected.Value].stamina);
        SetDamageServerRpc(statsTemplates[statsTemplateSelected.Value].damage);
        SetArmorServerRpc(statsTemplates[statsTemplateSelected.Value].armor);
        SetSpeedServerRpc(statsTemplates[statsTemplateSelected.Value].speed);



    }

    public void SetStats()
    {
        if (IsOwner)
        {
            InitializateStats();
        }
    }


    public void SetTemplate(int index)
    {
        if (IsServer)
        {
           statsTemplateSelected.Value = index;
        }
        else
        {
            SetTemplaterServerRpc(index);
        }
        
    }

    public void TakeDamage(int damage)
    {
        SetHealthServerRpc(health.Value - damage);
        if (health.Value <= 0)
        {
            Destroy(gameObject);
        }
    }


    public float GetSpeed()
    {
        return speed.Value;
    }
    #region ServerRpc

    //template selected
    [ServerRpc]
    private void SetTemplaterServerRpc(int index)
    {
        statsTemplateSelected.Value = index;
    }
    //Stats
    [ServerRpc]
    private void SetHealthServerRpc(int healthPoint)
    {
         health.Value = healthPoint;
    }
    [ServerRpc]
    private void SetHasteServerRpc(int hastePoint)
    {
        haste.Value = hastePoint;
    }
    [ServerRpc]
    private void SetArmorServerRpc(int armorPoint)
    {
        armor.Value = armorPoint;
    }
    [ServerRpc]
    private void SetDamageServerRpc(int damagePoint)
    {
        damage.Value = damagePoint;
    }
    [ServerRpc]
    private void SetStaminaServerRpc(int staminaPoint)
    {
        stamina.Value = staminaPoint;
    }
    [ServerRpc]
    private void SetSpeedServerRpc(float speedPoint)
    {
        speed.Value = speedPoint;
    }


    #endregion
}