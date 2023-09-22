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
    public int statsTemplateSelected;

    [Header("Stats")]
    [SerializeField] private float haste;
    [SerializeField] private float health;
    [SerializeField] private float stamina;
    [SerializeField] private float damage;
    [SerializeField] private float armor;

    [Header("Interfaces")]
    private IDamageable iDamageable;

    [Header("NetCode")]
    public NetworkObject playerObj;
    //public NetworkVariable<float> netHaste = new NetworkVariable<float>();

    void Start()
    {
        if (IsOwner)
        {
            OnSpawnPlayer+=InitializateStats;
            IDamageable iDamageable = GetComponent<IDamageable>();
        }
    }

    void Update()
    {

    }


    void InitializateStats()
    {

        playerObj = GetComponent<NetworkObject>();
        if (statsTemplates[statsTemplateSelected] == null)
        {
            Debug.LogError("StatsTemplate is null");
            return;
        }
        Debug.Log("StatsTemplate chosed by " + playerObj.OwnerClientId.ToString() + " " + statsTemplates[statsTemplateSelected].preset);
        haste = statsTemplates[statsTemplateSelected].haste;
        health = statsTemplates[statsTemplateSelected].health;
        stamina = statsTemplates[statsTemplateSelected].stamina;
        damage = statsTemplates[statsTemplateSelected].damage;
        armor = statsTemplates[statsTemplateSelected].armor;

    }

    public void SetStats()
    {
        if (IsOwner)
        {
            InitializateStats();
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
