using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using UnityEngine.Advertisements;

public class Menu_Gameover : MonoBehaviour {
    public GameObject btnNext;

    private void Awake()
    {
        btnNext.SetActive(GlobalValue.levelPlaying < GlobalValue.LevelHighest);
    }

    public void TryAgain()
    {
        GameManager.Instance.ResetLevel();
    }
}
