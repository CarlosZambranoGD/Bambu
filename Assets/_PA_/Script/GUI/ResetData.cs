using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResetData : MonoBehaviour
{
    SoundManager soundManager;

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    public void Reset()
    {
        Debug.Log("RESET DATA");
        bool _removeAd = GlobalValue.RemoveAds;
        PlayerPrefs.DeleteAll();
        GlobalValue.RemoveAds = _removeAd;
        SceneManager.LoadSceneAsync(0);

        SoundManager.Click();
    }

    public void UnlockAll()
    {
        Debug.Log("UNLOCK ALL LEVELS");
        GlobalValue.LevelHighest = 999;
        SceneManager.LoadSceneAsync(0);
        SoundManager.Click();
    }
}
