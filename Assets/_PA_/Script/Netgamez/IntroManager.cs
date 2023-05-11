using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public GameObject[] oCode;
    public void screenInfo(int index){
        for(int i = 0; i<oCode.Length; i++){
            oCode[i].SetActive(false);
        }
        oCode[index].SetActive(true);
    }
}
