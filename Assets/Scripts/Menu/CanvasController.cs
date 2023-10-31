using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    Canvas canvas;
    [Header("Game")]
    public TextMeshProUGUI timeLeft;
    public TextMeshProUGUI playersAlive;
    public TextMeshProUGUI playersConnected;

    [Header("Player")]
    public TextMeshProUGUI level;
    public TextMeshProUGUI bullets;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI exp;


    [Header("Shapes")]
    public Shapes.Line hpBar;
    public Shapes.Disc playerExp;
    public Shapes.Disc playerBullets;


    [Header("Buttons")]
    public Button openStatsButton;


    [Header("Ref")]
    public PlayerStatsController playerAssigned;

    void Start()
    {
        GetComponents();
    }

    void Update()
    {
        DisplayLevel();
        DisplayBullets();
        SetTimer();
        DisplayPlayersConnected();
    }

    private void GetComponents()
    {
        canvas = GetComponent<Canvas>();
        playerAssigned = GetComponentInParent<PlayerStatsController>();
    }
    private void SetTimer()
    {
        //waiting time
        if (!GameController.instance.started)
        {
            timeLeft.text = "Time to start: " + (GameController.instance.waitingTime - GameController.instance.netTimeToStart.Value).ToString("0.0");
            return;
        }
        //farm time
        if (GameController.instance.started&& !GameController.instance.mapLogic.Value.isBattleRoyale)
        {
            float temp =  GameController.instance.mapLogic.Value.totalTime - GameController.instance.farmStageTimer;
            timeLeft.text = "Farm time: " + temp.ToString("0.0");
        }
        //battle royale time
        else if(GameController.instance.started && GameController.instance.mapLogic.Value.isBattleRoyale)
        {
            timeLeft.text = "Battle Royale stage";
        }

    }
    private void DisplayPlayersConnected()
    {
        if (!GameController.instance.started)
        {
            playersConnected.text = "Players Connected: " + GameController.instance.numberOfPlayers.Value.ToString();
        }
        else
        {
            playersConnected.text = "Players Alive: " + GameController.instance.numberOfPlayersAlive.Value.ToString();
        }
    }
    private void DisplayBullets()
    {
        bullets.text = playerAssigned.currentBullets + "/"+ playerAssigned.totalAmmo;

    }
    private void DisplayHP()
    {


    }
    private void DisplayLevel()
    {

        level.text ="Current Level: " +playerAssigned.GetLevel().ToString();
    }
}
