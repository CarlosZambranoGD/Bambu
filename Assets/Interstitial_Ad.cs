using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Interstitial_Ad : MonoBehaviour
{
    private RewardedAd rewardedAd;
    bool AdShowed;
    public string ad_ID = "ca-app-pub-3940256099942544/5224354917";

    public bool showAd;

    [Range(1,10)]
    public int cadaXparaAds; // cada cuantas muertes o instancias muestra un Ad

    public void Start()
    {

        if (!showAd)
            return;


        GlobalValue.adsCount++;


        Debug.Log("Ads Count: " + GlobalValue.adsCount);

        if(GlobalValue.adsCount >= cadaXparaAds)
        {
            GlobalValue.adsCount = 0;

            MobileAds.Initialize(initstatus => { });

            RequestRewardedVideo();
        }
        else
        {
            showAd = false;
        }

       


    }



    public void Update()
    {

        if (!showAd)
            return;

        if (rewardedAd.IsLoaded())
        {
            if (!AdShowed)
            {

                AdShowed = true;
                rewardedAd.Show();
                GlobalValue.adsCount++;
                Debug.Log("Ad Shown " + GlobalValue.adsCount);
            }

        }
    }


    public void RequestRewardedVideo()
    {
        string adUnitId;
#if UNITY_ANDROID
        adUnitId = ad_ID;
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif

        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdFailedToLoad event received with message: ");
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
    }

    void HandleUserEarnedReward(object sender, Reward args)
    {



        GlobalValue.adsCountForRewards++;

        Menu_GUI.Instance.AdsCountObj.SetActive(true);


    }

}
