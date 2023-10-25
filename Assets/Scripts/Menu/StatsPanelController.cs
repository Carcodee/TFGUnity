using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelController : MonoBehaviour
{
    [Header("References")]
    PlayerStatsController playerStatsController;

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

    [Header("Sesion Variables")]
    int avaliblePoints;
    int sesionPoints;   

    void Start()
    {
        playerStatsController=GetComponent<PlayerStatsController>();
    }

    void Update() 
    {
        
    }
    public void AddPoint(string statName)
    {
        if (avaliblePoints <= 0) return;
        avaliblePoints--;
        for (int i = 0; i < playerStatsController.statHolder.Length; i++)
        {
            if (playerStatsController.statHolder[i]==statName)
            {
                //add add stat
            }
        }

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
        avaliblePoints = playerStatsController.GetAvaliblePoints();
        avaliblePointsText.text = "Avalible Points: " + avaliblePoints.ToString();
        level.text ="Level: " + playerStatsController.GetLevel().ToString();
        sesionPoints = playerStatsController.GetAvaliblePoints();
    }
    public void UpdateStats()
    {

    }
}
