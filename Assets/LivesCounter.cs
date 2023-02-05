using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesCounter : MonoBehaviour
{

    public Text livesTxt;

    // Start is called before the first frame update
    void OnEnable()
    {
        if(GlobalValue.SaveLives <= 9)
        {
            livesTxt.text = "0" + GlobalValue.SaveLives.ToString();
        }
        else
        {
            livesTxt.text = GlobalValue.SaveLives.ToString();
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
