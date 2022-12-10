using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class LevelManager : MonoBehaviour, IListener {
	public static LevelManager Instance{ get; private set;}
   [Header("+++ TIMER +++")]
    public int timer = 120;
    public int currentTimer { get; set; }
    int saveTimerCheckPoint;
    public int alarmTimeLess = 60;
    public AudioClip soundTimeLess;
    [Range(0, 1)]
    public float soundTimeLessVolume = 0.5f;
    public AudioClip soundTimeUp;
    [Range(0, 1)]
    public float soundTimeUpVolume = 0.5f;

    void Awake()
    {
        Instance = this;
        currentTimer = timer;
        saveTimerCheckPoint = timer;
    }

    public void SetCheckpointTimer()
    {
        saveTimerCheckPoint = currentTimer;
    }

    IEnumerator CountDownTimer()
    {
        yield return new WaitForSeconds(1);

        while (GameManager.Instance.State != GameManager.GameState.Playing)
        { yield return null; }

        currentTimer--;
        if (currentTimer == alarmTimeLess)
            SoundManager.PlaySfx(soundTimeLess, soundTimeLessVolume);
        else if (currentTimer <= 0)
        {
            SoundManager.PlaySfx(soundTimeUp, soundTimeUpVolume);
            GameManager.Instance.GameOver();
        }

        if (currentTimer > 0)
            StartCoroutine(CountDownTimer());
    }

    public void IPlay()
    {
        StartCoroutine(CountDownTimer());
    }

    public void ISuccess()
    {
       
    }

    public void IPause()
    {
       
    }

    public void IUnPause()
    {
      
    }

    public void IGameOver()
    {
    }

    public void IOnRespawn()
    {
        StopAllCoroutines();
        currentTimer = saveTimerCheckPoint;
        StartCoroutine(CountDownTimer());
    }

    public void IOnStopMovingOn()
    {
       
    }

    public void IOnStopMovingOff()
    {
       
    }
}
