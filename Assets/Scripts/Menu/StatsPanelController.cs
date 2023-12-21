using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StatsPanelController : MonoBehaviour
{
    public UnityAction OnPannelOpen;
    public UnityAction OnPannelClosed;

    [Header("References")]
    [SerializeField] private PlayerStatsController playerStatsController;
    public GameObject [] statsObjects;

    [Header("Stats")]
    public TextMeshProUGUI[] statValues;


    [Header("HeadStats")]
    public TextMeshProUGUI level;
    public TextMeshProUGUI avaliblePointsText;

    [Header("Buttons")]
    public Button[] addButtons;
    public Button[] removeButtons;
    public Button openPannel;

    [Header("Sesion Variables")]

    [SerializeField] private int avaliblePoints;
    [SerializeField] private int sesionPoints;
    public bool isPanelOpen { get;private set;}

    [Header("Animation")]
    public float animationTime;
    public float animationSpeed;
    public float animationFunction => 1 - Mathf.Pow(1 - animationTime, 3);
    public Transform targetPos;
    public Vector3 endPos;
    public Vector3 startPos;

    [Header("Selector")]
    public GameObject selector;
    public GameObject buttonSelector;
    public int selectorIndex=0;
    public int buttonSelectorIndex=1;

    private void OnEnable()
    {
        OnPannelOpen += OpenPanel;
        OnPannelClosed += ClosePanel;
    }

    private void OnDisable()
    {
        OnPannelOpen -= OpenPanel;
        OnPannelClosed -= ClosePanel;
    }

    void Start()
    {
        selectorIndex = 0;
        buttonSelectorIndex = 1;
        isPanelOpen =false;
        playerStatsController = GetComponentInParent<PlayerStatsController>();
        // StartCoroutine("AddListenersToButtons", 0.5f);
        endPos= targetPos.localPosition;
        startPos= transform.localPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isPanelOpen = !isPanelOpen;
            HandlePanel();
        }
        AnimatePanel();
        HandleSelector();
    }
    public void HandlePanel()
    {
        if (isPanelOpen)
        {
            OnPannelOpen?.Invoke();
        }
        else
        {
            OnPannelClosed?.Invoke();
        }
    }
    public void HandleSelector()
    {
        if (isPanelOpen)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                buttonSelector.transform.position = removeButtons[selectorIndex].transform.position;
                buttonSelectorIndex = 0;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                buttonSelector.transform.position = addButtons[selectorIndex].transform.position;
                buttonSelectorIndex = 1;

            }

            if (Input.GetKeyDown(KeyCode.DownArrow) && selectorIndex < statsObjects.Length - 1)
            {
                selectorIndex++;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && selectorIndex > 0)
            {
                selectorIndex--;
            }

            selector.transform.position = statsObjects[selectorIndex].transform.position;

            if (Input.GetKeyDown(KeyCode.Return) && buttonSelectorIndex == 0)
            {
                removeButtons[selectorIndex].onClick.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Return) && buttonSelectorIndex == 1)
            {
                addButtons[selectorIndex].onClick.Invoke();
            }

        }

    }

    public void AnimatePanel()
    {
        if (isPanelOpen && animationTime<1)
        {
            animationTime+=Time.deltaTime*animationSpeed;
            Mathf.Clamp(animationTime, 0, 1);
        }
        if (!isPanelOpen&&animationTime>0)
        {
            animationTime-=Time.deltaTime*animationSpeed;
            Mathf.Clamp(animationTime, 0, 1);
        }

        transform.localPosition=Vector3.Lerp(startPos, new Vector3(-endPos.x, transform.localPosition.y, 0), animationFunction);
    }
    IEnumerator AddListenersToButtons()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        for (int i = 0; i < addButtons.Length; i++)
        {
            PointButtonFunction(addButtons[i], "+", playerStatsController.statHolderNames[i]);
            PointButtonFunction(removeButtons[i], "-", playerStatsController.statHolderNames[i]);
        }
        openPannel.onClick.RemoveAllListeners();
        openPannel.onClick.AddListener(OnPannelOpen);
    }

    public void PointButtonFunction(Button button, string operation, string stat)
    {
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        if (operation == "+")
        {
            button.onClick.AddListener(() => AddPoint(stat));
        }
        else if (operation == "-")
        {
            button.onClick.AddListener(() => RemovePoint(stat));
        }
    }

    public void AddPoint(string statName)
    {

        if (avaliblePoints <= 0)
        {
            Debug.Log("Not points");
            return;
        }
        for (int i = 0; i < playerStatsController.statHolderNames.Length; i++)
        {
            if (playerStatsController.statHolderNames[i] == statName)
            {
                playerStatsController.AddValueFromButton(i);
                avaliblePoints--;
                playerStatsController.OnStatsChanged?.Invoke();
                UpdateStats();
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
        for (int i = 0; i < playerStatsController.statHolderNames.Length; i++)
        {
            if (playerStatsController.statHolderNames[i] == statName)
            {
                playerStatsController.SustractValueFromButton(i);
                avaliblePoints++;
                playerStatsController.OnStatsChanged?.Invoke();
                UpdateStats();
                Debug.Log("Add Value From Button");
                return;
                //sustract stat
            }
        }

    }

    public void OpenPanel()
    {

        avaliblePoints = playerStatsController.GetAvaliblePoints();
        avaliblePointsText.text = "Avalible Points: " + avaliblePoints.ToString();
        level.text = "Level: " + playerStatsController.GetLevel().ToString();
        sesionPoints = playerStatsController.GetAvaliblePoints();
        for (int i = 0; i < statValues.Length; i++)
        {
            statValues[i].text = playerStatsController.statHolder[i].ToString();
        }

    }
    public void ClosePanel()
    {
        avaliblePoints = 0;
        sesionPoints = 0;
    }
    public void UpdateStats()
    {
        avaliblePointsText.text = "Avalible Points: " + avaliblePoints.ToString();
        level.text = "Level: " + playerStatsController.GetLevel().ToString();
        for (int i = 0; i < statValues.Length; i++)
        {
            statValues[i].text = playerStatsController.statHolder[i].ToString();
        }
    }
}

