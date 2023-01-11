using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueLevel : MonoBehaviour
{
    // Start is called before the first frame update
public void NextLevel() {
        GlobalValue.levelPlaying = GlobalValue.LevelHighest;
        if(GlobalValue.LevelHighest > 30){
            GlobalValue.levelPlaying = 30;
        }

        MainMenuHomeScene.Instance.LoadScene("Level " + GlobalValue.levelPlaying);
    }
}
