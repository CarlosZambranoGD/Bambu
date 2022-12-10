using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum TUTORIAL_TYPE { melee, range, jump, parachute}

public class ShowTutorialUI : MonoBehaviour
{
    public static ShowTutorialUI Instance;
    public GameObject containHolder;
    public GameObject meleeGUI, rangeGUI, jumpGUI, parachuteGUI;
    public GameObject closeTxt;

    bool isShowing = false;
    float delayShowClose = 1;
    float timeShowUp;
    bool allowClose = false;
    GameplayControl controls;
    private void Awake()
    {
        Instance = this;
        controls = new GameplayControl();
        controls.PlayerControl.Jump.started += ctx => Close();
    }

    private void Start()
    {
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
        containHolder.SetActive(false);
        meleeGUI.SetActive(false);
        rangeGUI.SetActive(false);
        jumpGUI.SetActive(false);
        parachuteGUI.SetActive(false);
        closeTxt.SetActive(false);
    }

    public void ShowTUT(TUTORIAL_TYPE tYPE)
    {
        if (isShowing)
            return;
        containHolder.SetActive(true);
        isShowing = true;
        allowClose = false;
        timeShowUp = Time.realtimeSinceStartup;
        SoundManager.ShowTutorial();
        Time.timeScale = 0;
        switch (tYPE)
        {
            case TUTORIAL_TYPE.melee:
                meleeGUI.SetActive(true);
                break;
            case TUTORIAL_TYPE.range:
                rangeGUI.SetActive(true);
                break;
            case TUTORIAL_TYPE.jump:
                jumpGUI.SetActive(true);
                break;
            case TUTORIAL_TYPE.parachute:
                parachuteGUI.SetActive(true);
                break;
        }
    }

    private void Update()
    {
        if (isShowing && !allowClose)
        {
            if(Time.realtimeSinceStartup > (timeShowUp + delayShowClose))
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
