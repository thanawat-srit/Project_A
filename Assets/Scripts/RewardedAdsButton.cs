using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System.Collections;
using System.Collections.Generic;
using TMPro;

 
public class RewardedAdsButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] Button _showAdButton;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    [SerializeField] TMP_Text textEnergy;
    [SerializeField] GameObject plateAds;
    string _adUnitId = null; // This will remain null for unsupported platforms
    bool stopCoroutine = false;
    public GameModeButton _gameModeButton;
    Coroutine runningCoroutine; 
    
    void Awake()
    {   
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif
        _showAdButton.interactable = false;
    }
    void Start(){

        LoadAd();
        runningCoroutine = StartCoroutine(AdsCoroutine());
    }

    void Update(){
        if (stopCoroutine == true)
        {
            StopCoroutine(runningCoroutine);
            stopCoroutine = false;
        }
    }

    IEnumerator AdsCoroutine(){
        yield return new WaitForSeconds(1);
        Debug.Log("loading rewarded ads status = " + Advertisement.isInitialized);
        if(Advertisement.isInitialized){
            stopCoroutine = true;
            OnUnityAdsAdLoaded(_adUnitId);

        }
    }
    public void LoadAd()
    {
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }
 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
 
        if (adUnitId.Equals(_adUnitId))
        {
            
            _showAdButton.onClick.AddListener(ShowAd);
            
            _showAdButton.interactable = true;
            
        }
    }
 
    public void ShowAd()
    {
        _showAdButton.interactable = false;
        Advertisement.Show(_adUnitId, this);
    }
 
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            textEnergy.SetText("1/5");
            _gameModeButton.SetEnergrRank(1);
            plateAds.SetActive(false);

            Advertisement.Load(_adUnitId, this);
        }
    }
 
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }
 
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }
 
    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
 
    void OnDestroy()
    {
        _showAdButton.onClick.RemoveAllListeners();
    }
}