using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatsController : NetworkBehaviour, IDamageable
{
    public UnityAction OnSpawnPlayer;
    [Header("References")]
    public StatsTemplate[] statsTemplates;
    public NetworkVariable <int> statsTemplateSelected;

    [Header("Stats")]
    [SerializeField] private NetworkVariable<int> haste = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> health = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> stamina = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> damage = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> armor = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> speed = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> playerLevel = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> avaliblePoints = new NetworkVariable<int>();
    public string[] statHolder;

    [Header("Current Gamelogic")]
    public NetworkVariable<zoneColors> zoneAsigned=new NetworkVariable<zoneColors>();
    public Transform coinPosition;
    public PlayerZoneController playerZoneController;

    [Header("Interfaces")]
    private IDamageable iDamageable;

    [Header("NetCode")]
    public NetworkObject playerObj;

    private void Awake()
    {
     
    }
    void Start()
    {
        if (IsOwner)
        {
            OnSpawnPlayer += InitializateStats;
            iDamageable = GetComponent<IDamageable>();
        }

    }

    void Update()
    {

    }
    public void FillStatHolder()
    {
        //change this to the heap memory
        statHolder = new string[6];
        statHolder[0] = nameof(haste);
        statHolder[1] = nameof(health);
        statHolder[2] = nameof(stamina);
        statHolder[3] = nameof(damage);
        statHolder[4] = nameof(armor);
        statHolder[5] = nameof(speed);

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
        SetLevelServerRpc(1);
        SetAvaliblePoints(0);

        //Stats on controller player
        Debug.Log("Before setting speed: " + speed.Value);
        transform.GetComponent<PlayerController>().SetSpeedStateServerRpc(statsTemplates[statsTemplateSelected.Value].speed);
        Debug.Log("After setting speed: " + speed.Value);
        FillStatHolder();
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
    public int GetDamageDone()
    {
        return damage.Value;
    }
    public int GetLevel()
    {
        return playerLevel.Value;
    }
    public int GetHealth()
    {
        return health.Value;
    }
    public int GetArmor()
    {
        return armor.Value;
    }
    public int GetHaste()
    {
        return haste.Value;
    }
    public int GetStamina()
    {
        return stamina.Value;
    }
    public int GetAvaliblePoints()
    {
        return avaliblePoints.Value;
    }

    public void LevelUp()
    {
        playerLevel.Value++;
    }
    public void AddAvaliblePoint()
    {
        avaliblePoints.Value++;
    }
    public void RemoveAvaliblePoint()
    {
        avaliblePoints.Value--;
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
    private void SetSpeedServerRpc(int speedPoint)
    {
        speed.Value = speedPoint;
    }
    //Level--------
    [ServerRpc]
    public void SetLevelServerRpc(int val)
    {
        playerLevel.Value= val;
    }
    //AvaliblePoints
    public void SetAvaliblePoints(int val)
    {
        avaliblePoints.Value=val;
    }



    [ServerRpc]
    public void SetZoneAsignedStateServerRpc(zoneColors zone)
    {
        if (IsServer)
        {
            zoneAsigned.Value = zone;
        }
        else
        {
            SetZoneAsignedClientServerRpc(zone);
        }
    }

    [ServerRpc]
    public void SetZoneAsignedClientServerRpc(zoneColors zone)
    {
        zoneAsigned.Value = zone;
    }


    #endregion
}
