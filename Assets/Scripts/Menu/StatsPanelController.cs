using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StatsPanelController : MonoBehaviour
{

    [Header("References")]
    [SerializeField]private PlayerStatsController playerStatsController;

    [Header("Stats")]
    public TextMeshProUGUI hasteStat;
    public TextMeshProUGUI healthStat;
    public TextMeshProUGUI staminaStat;
    public TextMeshProUGUI damageStat;
    public TextMeshProUGUI armorStat;
    public TextMeshProUGUI speedStat;

    [Header("HeadStats")]
    public TextMeshProUGUI level;
    public TextMeshProUGUI avaliblePointsText;

    [Header("Buttons")]
    public Button[] buttons;

    [Header("Sesion Variables")]
    [SerializeField]private int avaliblePoints;
    [SerializeField]private int sesionPoints;

    private void OnEnable()
    {

    }

    void Start()
    {

        playerStatsController=GetComponentInParent<PlayerStatsController>();
        AddListenersToButtons();
    }
    
    void Update() 
    {
        OpenPanel();
    }


    public void AddListenersToButtons()
    {
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(() => AddPoint("damage"));
        Debug.Log("Add Listeners");
    }

    public void AddPoint(string statName)
    {

        if (avaliblePoints <= 0)
        {
            Debug.Log("Not points");
            return;
        }
        for (int i = 0; i < playerStatsController.statHolder.Length; i++)
        {
            if (playerStatsController.statHolder[i] == statName)
            {
                playerStatsController.AddValueFromButtonServerRpc(i);
                avaliblePoints--;
                Debug.Log("Add Value From Button");
                return;
                //add add stat
            }
        }
        Debug.Log("Not finded");

    }
    public void RemovePoint(string statName)
    {
       
        if (avaliblePoints >= sesionPoints) return;
        avaliblePoints++;
        for (int i = 0; i < playerStatsController.statHolder.Length; i++)
        {
            if (playerStatsController.statHolder[i] == statName)
            {
                //sustract stat
            }
        }

    }

    public void OpenPanel()
    {
        //WROOOOOOOOOOng
        // to do this is being called all time so it will never change.
        avaliblePoints = playerStatsController.GetAvaliblePoints();
        avaliblePointsText.text = "Avalible Points: " + avaliblePoints.ToString();
        level.text ="Level: " + playerStatsController.GetLevel().ToString();
        sesionPoints = playerStatsController.GetAvaliblePoints();
    }
    public void UpdateStats()
    {

    }
}
