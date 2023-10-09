using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
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


    [Header("Panels")]
    public GameObject statsPanel;


    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void Initializate()
    {

    }
}
