using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using AudienceNetwork;
using UnityEngine.Events;

/// <summary>
/// 插页式广告执行类
/// </summary>
public class FacebookInterstitialAd : MonoBehaviour {
    /*
     * 

    InterstitialAd Ad Did Load;                   插页广告加载;
    InterstitialAd Ad Will Log Impression;        插页广告将记录印象;
    InterstitialAd Ad Did Fail With Error;        插页广告失败，错误;
    InterstitialAd Ad Did Click;                  插页广告点击;
    InterstitialAd Ad Did Finish Handling Click;  插页广告点击处理完成;
    interstitialAd Ad Did Close                   插页广告关闭

    */

    [System.Serializable]
    public class InterstitialAdDidLoad : UnityEvent { }; //插页广告加载;

    [System.Serializable]
    public class InterstitialAdDidFailWithError : UnityEvent<string> { }; //rewarded Video Ad Did Fail With Error;  插页广告失败，错误;

    [System.Serializable]
    public class InterstitialAdDidClick : UnityEvent { }; // 插页广告点击;

    [System.Serializable]
    public class InterstitialAdWillClose : UnityEvent { };  // 插页广告完将要关闭

    [System.Serializable]

    public class InterstitialAdDidClose : UnityEvent { };  // 插页广告关闭


    public InterstitialAdDidLoad onInterstitialAdDidLoad = new InterstitialAdDidLoad();
    public InterstitialAdDidFailWithError onInterstitialAdDidFailWithError = new InterstitialAdDidFailWithError();
    public InterstitialAdDidClick onInterstitialAdDidClick = new InterstitialAdDidClick();
    public InterstitialAdWillClose onInterstitialAdWillClose = new InterstitialAdWillClose();
    public InterstitialAdDidClose onInterstitialAdDidClose = new InterstitialAdDidClose();
    private string uniqueId;


    private InterstitialAd interstitialAd;
    private bool isLoaded;

    public void init(string uniqueId)
    {
        Debug.Log("InitInterstitial");

        this.uniqueId = uniqueId;
        LoadInterstitial();
    }

    public void LoadInterstitial()
    {

        // Create the interstitial unit with a placement ID (generate your own on the Facebook app settings).
        // Use different ID for each ad placement in your app.
        InterstitialAd interstitialAd = new InterstitialAd(uniqueId);
        this.interstitialAd = interstitialAd;
        this.interstitialAd.Register(this.gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        this.interstitialAd.InterstitialAdDidLoad = (delegate ()
        {
            Debug.Log("Interstitial ad loaded.");
            this.isLoaded = true;
            onInterstitialAdDidLoad.Invoke();
        });
        interstitialAd.InterstitialAdDidFailWithError = (delegate (string error)
        {
            Debug.Log("Interstitial ad failed to load with error: " + error);
            onInterstitialAdDidFailWithError.Invoke(error);
        });
        interstitialAd.InterstitialAdWillLogImpression = (delegate ()
        {
            Debug.Log("Interstitial ad logged impression.");
        });
        interstitialAd.InterstitialAdDidClick = (delegate ()
        {
            Debug.Log("Interstitial ad clicked.");
            onInterstitialAdDidClick.Invoke();
        });
        interstitialAd.InterstitialAdDidClose = (delegate ()
        {
            Debug.Log("Interstitial ad Close.");
            onInterstitialAdDidClose.Invoke();
        });

        // Initiate the request to load the ad.
        this.interstitialAd.LoadAd();
    }

    public void ShowInterstitial()
    {
        if (this.isLoaded)
        {
            this.interstitialAd.Show();
            this.isLoaded = false;
        }
    }

    void OnDestroy()
    {
        if (this.interstitialAd != null)
        {
            this.interstitialAd.Dispose();
        }
        Debug.Log("InterstitialAdTest was destroyed!");
    }
}
