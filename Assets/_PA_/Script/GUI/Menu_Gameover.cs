using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using UnityEngine.Advertisements;

public class Menu_Gameover : MonoBehaviour {
    public GameObject btnNext;
    public GameObject btnTry;
    public GameObject btnAds;

    public GameObject RewardedAdObj;

    public static Menu_Gameover instance;


    public GameObject AskSaveMe;

    public void OnEnable()
    {
        Debug.Log("GameOver Activado, Vidas: " + GlobalValue.SaveLives);

        if(GlobalValue.SaveLives  <= 0)
        {
            AskSaveMe.SetActive(true);
        }


    }
    private void Awake()
    {
        btnNext.SetActive(GlobalValue.levelPlaying < GlobalValue.LevelHighest);

        //if(GlobalValue.SaveLives <= 0)
        //{
        //    btnTry.SetActive(false);
        //    btnAds.SetActive(true);
        //}
        //else
        //{
        //    btnTry.SetActive(true);
        //    btnAds.SetActive(false);
        //}

        btnTry.SetActive(true);

        instance = this;

    }

    public void TryAgain()
    {
        Debug.Log("Lives on tryBton: " + GlobalValue.SaveLives);

        if(GlobalValue.SaveLives >= 1)
        {
            Debug.Log("Continuar");
            GameManager.Instance.Continue();
        }
        else
        {
            Debug.Log("Reiniciar");
            GameManager.Instance.ResetLevel();
        }
        gameObject.SetActive(false);

        
    }

    public void Ad_RBton()
    {
        btnAds.SetActive(false);
        RewardedAdObj.SetActive(true);
    }

}
