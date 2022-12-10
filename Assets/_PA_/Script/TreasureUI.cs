using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TreasureUI : MonoBehaviour
{
    public static TreasureUI Instance;

    public GameObject container;
    public Text amountTxt, liveTxt;
    public GameObject closeTxt;

    bool isShowing = false;
    float delayShowClose = 1;
    float timeShowUp;
    bool allowClose = false;
    GameplayControl controls;

    private void Awake()
    {
        controls = new GameplayControl();
        controls.PlayerControl.Jump.started += ctx => Close();
    }

    private void Start()
    {
        Instance = this;
        DisableAll();
    }

    private void OnEnable()
    {
        controls.PlayerControl.Enable();
    }

    private void OnDisable()
    {
        controls.PlayerControl.Disable();
    }

    void DisableAll()
    {
        container.SetActive(false);
        closeTxt.SetActive(false);
    }

    public void ShowRewarded(int amount, int live)
    {
        Time.timeScale = 0;
        isShowing = true;
        allowClose = false;
        timeShowUp = Time.realtimeSinceStartup;

        SoundManager.ShowRewardedChest();
        container.SetActive(true);
        amountTxt.text = "+" + amount;
        GlobalValue.SavedCoins += amount;

        liveTxt.text = "+" + live;
        GlobalValue.SaveLives += live;
    }

    private void Update()
    {
        if (isShowing && !allowClose)
        {
            if (Time.realtimeSinceStartup > (timeShowUp + delayShowClose))
            {
                closeTxt.SetActive(true);
                allowClose = true;
            }
        }
    }

    public void Close()
    {
        if (allowClose)
        {
            allowClose = false;
            SoundManager.Click();
            Time.timeScale = 1;
            isShowing = false;
            DisableAll();
        }
    }
}
