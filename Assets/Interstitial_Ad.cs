using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class Interstitial_Ad : MonoBehaviour
{

    private InterstitialAd interstitial;
    public string ad_ID = "ca-app-pub-3940256099942544/1033173712";
    public bool ShowAd;
    bool AdShowed;
    // Start is called before the first frame update
    void Start()
    {


        if(ShowAd)
        {
            MobileAds.Initialize(initstatus => { });

            RequestInterstitial();
        }    
        

       

    }

    // Update is called once per frame
    void Update()
    {


        if (!ShowAd)
        {
            return;
        }


        if (interstitial.IsLoaded())
        {
            if (!AdShowed)
            {
                AdShowed = true;
                interstitial.Show();
            }
            
        }
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

}
