using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFlag : TriggerEvent, IListener {

    public TUTORIAL_TYPE tutorialType;
    bool isWorkded = false;

    public override void OnContactPlayer()
    {
        if (isWorkded)
            return;

        isWorkded = true;
        ShowTutorialUI.Instance.ShowTUT(tutorialType);
    }

    public void IGameOver()
    {
    }

    public void IOnRespawn()
    {
        isWorkded = false;
    }

    public void IOnStopMovingOff()
    {
    }

    public void IOnStopMovingOn()
    {
    }

    public void IPause()
    {
    }

    public void IPlay()
    {
    }

    public void ISuccess()
    {
    }

    public void IUnPause()
    {
    }
}
