//using UnityEngine;
//using System.Collections;
////using UnityEngine.Advertisements;

//public enum WatchAdResult { Finished, Failed, Skipped }
//public class UnityAds : MonoBehaviour/*, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener*/
//{
//    public static UnityAds Instance;
//    //delegate   ()
//    public delegate void RewardedAdResult(WatchAdResult result);

//    //event  
//    public static event RewardedAdResult AdResult;

//    [Header("UNITY AD SETUP")]
//    public string UNITY_ANDROID_ID = "1486550";
//    public string UNITY_IOS_ID = "1486551";
//    public bool isTestMode = true;

//    string _adUnitId = "Rewarded_Android"; // This will remain null for unsupported platforms

//    private void Awake()
//    {
//        //#if UNITY_IOS
//        //        _adUnitId = _iOSAdUnitId;
//        //#elif UNITY_ANDROID
//        //        _adUnitId = _androidAdUnitId;
//        //#endif
//        Debug.Log("Atempting to load UnityAds...." + gameObject.name);

//        //if (UnityAds.Instance != null)
//        //{
//        //    Destroy(gameObject);
//        //    return;
//        //}
//        //else
//        //{
//        //    Instance = this;
//        //    DontDestroyOnLoad(gameObject);
//        //}

//        //_adUnitIdNormal = (Application.platform == RuntimePlatform.IPhonePlayer)
//        //   ? _iOsAdUnitIdNormal
//        //   : _androidAdUnitIdNormal;
//    }

 

////    void Start()
////    {
////#if UNITY_ANDROID || UNITY_IOS
////        string gameId = "";
////#if UNITY_IOS
////		gameId = UNITY_IOS_ID;
////#elif UNITY_ANDROID
////        gameId = UNITY_ANDROID_ID;
////#endif
////        if (Advertisement.isSupported)
////        {
////            Advertisement.Initialize(gameId, isTestMode, this);
////        }
////#endif
////    }

////    public void LoadAd()
////    {
////        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
////        Debug.Log("Loading Ad: " + _adUnitId);
////        Advertisement.Load(_adUnitId, this);

////        Advertisement.Load(_adUnitIdNormal, this);
////    }

//    #region NORMAL AD
//    string _androidAdUnitIdNormal = "Interstitial_Android";
//    string _iOsAdUnitIdNormal = "Interstitial_iOS";
//    string _adUnitIdNormal;

//    public void ShowNormalAd()
//    {
////#if UNITY_ANDROID || UNITY_IOS
////        Advertisement.Show(_adUnitIdNormal, this);
////#endif
//    }

//    public bool ForceShowNormalAd()
//    {
////#if UNITY_ANDROID || UNITY_IOS
////        Advertisement.Show(_adUnitIdNormal, this);
////        return true;
////        //if (Advertisement.IsReady())
////        //{
////        //    Advertisement.Show();
////        //    return true;
////        //}
////        //else
////        //    return false;
////#else
//        return false;
////#endif
//    }

//    #endregion

//    #region REWARD AD
//    bool isRewardedAdReadyTemp = false;
//    public bool isRewardedAdReady()
//    {
//#if UNITY_ANDROID || UNITY_IOS
//        return isRewardedAdReadyTemp;
//        //return Advertisement.IsReady("rewardedVideo");
//#else
//        return false;
//#endif
//    }

//    public void ShowRewardVideo()
//    {
//        ShowRewardedAd();
//    }

//    private void ShowRewardedAd()
//    {
////        if (!allowWatch)
////            return;

////#if UNITY_ANDROID || UNITY_IOS
////        if (isRewardedAdReadyTemp)
////        {
////            //var options = new ShowOptions { resultCallback = HandleShowResult };
////            if (!Advertisement.isShowing)
////            {
////                Advertisement.Show(_adUnitId, this);
////                //Advertisement.Show("rewardedVideo", options);
////            }
////            allowWatch = false;

////        }
////#endif
////    }

////    bool allowWatch = true;
////#if UNITY_ANDROID || UNITY_IOS
////    private void HandleShowResult(ShowResult result)
////    {
////        switch (result)
////        {
////            case ShowResult.Finished:
////                Debug.Log("The ad was successfully shown.");
////                AdResult(WatchAdResult.Finished);
////                ; break;
////            case ShowResult.Skipped:
////                Debug.Log("The ad was skipped before reaching the end.");
////                AdResult(WatchAdResult.Skipped);
////                break;
////            case ShowResult.Failed:
////                Debug.LogError("The ad failed to be shown.");
////                AdResult(WatchAdResult.Failed);
////                break;
////        }

////        allowWatch = true;
////    }

////    public void OnUnityAdsAdLoaded(string placementId)
////    {
////        if (placementId.Equals(_adUnitId))
////        {
////            isRewardedAdReadyTemp = true;
////        }
////    }

////    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
////    {
////        throw new System.NotImplementedException();
////    }

////    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
////    {
////        throw new System.NotImplementedException();
////    }

////    public void OnUnityAdsShowStart(string placementId)
////    {

////    }

////    public void OnUnityAdsShowClick(string placementId)
////    {
////        throw new System.NotImplementedException();
////    }

////    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
////    {
////        if (placementId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
////        {
////            AdResult(WatchAdResult.Finished);
////            isRewardedAdReadyTemp = false;
////            Advertisement.Load(_adUnitId, this);
////        }
////        else if (placementId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.SKIPPED))
////            AdResult(WatchAdResult.Skipped);
////        else if (placementId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.UNKNOWN))
////            AdResult(WatchAdResult.Failed);

////        if (placementId.Equals(_adUnitIdNormal))
////        {
////            Advertisement.Load(_adUnitIdNormal, this);
////        }

////        allowWatch = true;
////    }

////    public void OnInitializationComplete()
////    {
////        LoadAd();
//////    }

//////    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
//////    {
//////        throw new System.NotImplementedException();
//////    }
////#else
////    public void OnUnityAdsAdLoaded(string placementId)
////    {
////        throw new System.NotImplementedException();
////    }

////    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
////    {
////        throw new System.NotImplementedException();
////    }

////    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
////    {
////        throw new System.NotImplementedException();
////    }

////    public void OnUnityAdsShowStart(string placementId)
////    {
////        throw new System.NotImplementedException();
////    }

////    public void OnUnityAdsShowClick(string placementId)
////    {
////        throw new System.NotImplementedException();
////    }

////    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
////    {
////        throw new System.NotImplementedException();
////    }

////    public void OnInitializationComplete()
////    {
////        throw new System.NotImplementedException();
////    }

////    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
////    {
////        throw new System.NotImplementedException();
////    }
////#endif
////    #endregion
//}
////using UnityEngine;
////using System.Collections;

////#if UNITY_ANDROID || UNITY_IOS
////using UnityEngine.Advertisements;
////#endif


////public enum WatchAdResult { Finished, Failed, Skipped}
////public class UnityAds : MonoBehaviour
////{
////    public static UnityAds Instance;
////    //delegate   ()
////    public delegate void RewardedAdResult(WatchAdResult result);

////    //event  
////    public static event RewardedAdResult AdResult;
////#if UNITY_ANDROID || UNITY_IOS
////    [Header("UNITY AD SETUP")]
////    public string UNITY_ANDROID_ID = "1486550";
////    public string UNITY_IOS_ID = "1486551";
////    public bool isTestMode = true;
////#endif
////    private void Awake()
////    {
////        if (UnityAds.Instance != null)
////        {
////            Destroy(gameObject);
////            return; 
////        }
////        else
////        {
////            Instance = this;
////            DontDestroyOnLoad(gameObject);
////        }
////    }

////    void Start()
////    {
////        #if UNITY_ANDROID || UNITY_IOS
////        string gameId = "";
////#if UNITY_IOS
////		gameId = UNITY_IOS_ID;
////#elif UNITY_ANDROID
////        gameId = UNITY_ANDROID_ID;
////#endif
////        if (Advertisement.isSupported)
////        {
////            Advertisement.Initialize(gameId, isTestMode);
////        }
////#endif
////    }



////#region NORMAL AD
////    public void ShowNormalAd()
////    {
////        #if UNITY_ANDROID || UNITY_IOS
////        if (Advertisement.IsReady())
////        {
////            Advertisement.Show();
////        }
////#endif
////    }

////    public bool ForceShowNormalAd()
////    {
////#if UNITY_ANDROID || UNITY_IOS
////        if (Advertisement.IsReady())
////        {
////            Advertisement.Show();
////            return true;
////        }
////        else
////            return false;
////#else
////        return false;
////#endif
////    }

////#endregion

////#region REWARD AD
////    public bool isRewardedAdReady()
////    {
////#if UNITY_ANDROID || UNITY_IOS
////        return Advertisement.IsReady("rewardedVideo");
////#else
////        return false;
////#endif
////    }

////    public void ShowRewardVideo()
////    {
////        ShowRewardedAd();
////    }

////    private void ShowRewardedAd()
////    {
////        if (!allowWatch)
////            return;

////        #if UNITY_ANDROID || UNITY_IOS
////        if (Advertisement.IsReady("rewardedVideo"))
////        {
////                var options = new ShowOptions { resultCallback = HandleShowResult };
////                if (!Advertisement.isShowing)
////                    Advertisement.Show("rewardedVideo", options);

////                allowWatch = false;

////        }
////#endif
////    }

////    bool allowWatch = true;
////    #if UNITY_ANDROID || UNITY_IOS
////    private void HandleShowResult(ShowResult result)
////    {
////        switch (result)
////        {
////            case ShowResult.Finished:
////                Debug.Log("The ad was successfully shown.");
////                AdResult(WatchAdResult.Finished);
////                ; break;
////            case ShowResult.Skipped:
////                Debug.Log("The ad was skipped before reaching the end.");
////                AdResult(WatchAdResult.Skipped);
////                break;
////            case ShowResult.Failed:
////                Debug.LogError("The ad failed to be shown.");
////                AdResult(WatchAdResult.Failed);
////                break;
////        }

////        allowWatch = true;
////    }
////#endif
////#endregion
////}