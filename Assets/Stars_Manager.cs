using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars_Manager : MonoBehaviour
{

    public GameObject[] stars;

    public int currentStar;

    public static Stars_Manager instance;


    public Animator starsAnim;

    public GameObject LiveAnimObj;

    [ContextMenu("Add Star")]
    public void AddStar()
    {
        currentStar++;
        starsAnim.SetTrigger("Show");
        Invoke(nameof(ShowStar), 1f);
    }

    public void ShowStar()
    {
        stars[currentStar].SetActive(true);
        

        if(currentStar >= 2)
        {
            Debug.Log("Dar Vida!!");
            Invoke(nameof(ShowLiveAnim), 1f);
        }
        else
        {
            Invoke(nameof(Hidestar), 1f);
            
        }

        
    }

    public void ShowLiveAnim()
    {
        currentStar = 0;

        foreach (var item in stars)
        {
            item.SetActive(false);
        }

        LiveAnimObj.SetActive(true);
        Invoke(nameof(AddLive), 1f);
    }


    void AddLive()
    {
        GlobalValue.SaveLives++;
        Invoke(nameof(Hidestar), 1f);
    }


    public void Hidestar()
    {
        starsAnim.SetTrigger("Hide");
    }

    // Start is called before the first frame update
    void Start()
    {
        currentStar = -1;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
