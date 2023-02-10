using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ads_Counter_Manager : MonoBehaviour
{
    public Slider barra;
    public float currentValue;
    public float fillSpeed;

    public Animator anim;

    public GameObject ExtraLifeObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
        
        barra.value = Mathf.MoveTowards(barra.value,currentValue, fillSpeed);

    }



    public void UpdateValue()
    {
        currentValue = GlobalValue.adsCountForRewards;
        Invoke(nameof(HideObj), 2f);
    }

    public void HideObj()
    {
        anim.SetTrigger("Hide");

        if(currentValue >= 3)
        {
            ExtraLifeObj.SetActive(true);
            Invoke(nameof(GiveLive), 4f);
        }
    }

     void GiveLive()
    {
        currentValue = 0;
        GlobalValue.SaveLives++;
        GlobalValue.adsCountForRewards = 0;
        gameObject.SetActive(false);
    }

}
