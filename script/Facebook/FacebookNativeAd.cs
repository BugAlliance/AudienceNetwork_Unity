using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using AudienceNetwork;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(RectTransform))]
public class FacebookNativeAd : MonoBehaviour
{


    /*
     * 

    NativeAd Ad Did Load;                   原生广告加载;
    NativeAd Ad Will Log Impression;        原生广告将记录印象;
    NativeAd Ad Did Fail With Error;        原生广告失败，错误;
    NativeAd Ad Did Click;                  原生广告点击;
    NativeAd Ad Did Finish Handling Click;  原生广告点击处理完成;


    */

    [System.Serializable]
    public class NativeAdAdDidLoad : UnityEvent { }; //原生广告加载;

    [System.Serializable]
    public class NativeAdAdDidFailWithError : UnityEvent<string> { }; //rewarded Video Ad Did Fail With Error;  原生广告失败，错误;

    [System.Serializable]
    public class NativeAdAdDidClick : UnityEvent { }; // 原生广告点击;

    [System.Serializable]
    public class NativeAdAdFinishHandlingClick : UnityEvent { };  // 原生广告完成处理单击;

    public NativeAdAdDidLoad onNativeAdAdDidLoad = new NativeAdAdDidLoad();
    public NativeAdAdDidFailWithError onNativeAdAdDidFailWithError = new NativeAdAdDidFailWithError();
    public NativeAdAdDidClick onNativeAdAdDidClick = new NativeAdAdDidClick();
    public NativeAdAdFinishHandlingClick onNativeAdAdFinishHandlingClick = new NativeAdAdFinishHandlingClick();

    private string uniqueId;



    private NativeAd nativeAd;

    // UI elements in scene
    [Header("Text:")]
    public Text
        title;
    public Text socialContext;
    [Header("Images:")]
    public Image
        coverImage;
    public Image iconImage;
    [Header("Buttons:")]
    public Text
        callToAction;
    public Button callToActionButton;



    void Update()
    {
        // Update GUI from native ad
        if (nativeAd != null && nativeAd.CoverImage != null)
        {
            coverImage.sprite = nativeAd.CoverImage;
        }
        if (nativeAd != null && nativeAd.IconImage != null)
        {
            iconImage.sprite = nativeAd.IconImage;
        }
    }

    void OnDestroy()
    {
        // Dispose of native ad when the scene is destroyed
        if (this.nativeAd)
        {
            this.nativeAd.Dispose();
        }
        Debug.Log("NativeAdTest was destroyed!");
    }


    public void init(string uniqueId)
    {
        Debug.Log("InitNativeAd");

        this.uniqueId = uniqueId;

        StartNativeAd();

    }

    public void LoadNativeAd()
    {
        StartNativeAd();
    }

    private void StartNativeAd()
    {
        Debug.Log("StartNativeAd");

        NativeAd nativeAd = new AudienceNetwork.NativeAd(uniqueId);
        this.nativeAd = nativeAd;
        nativeAd.RegisterGameObjectForImpression(gameObject, new Button[] { callToActionButton });
        coverImage.sprite = null;
        iconImage.sprite = null;
        // 原生广告加载结束
        nativeAd.NativeAdDidLoad = (delegate ()
        {
            this.Log("Native ad loaded.");
            Debug.Log("Loading images...");
            // Use helper methods to load images from native ad URLs
            StartCoroutine(nativeAd.LoadIconImage(nativeAd.IconImageURL));
            StartCoroutine(nativeAd.LoadCoverImage(nativeAd.CoverImageURL));

            Debug.Log("Images loaded.");
            title.text = nativeAd.Title;
            socialContext.text = nativeAd.SocialContext;
            callToAction.text = nativeAd.CallToAction;
            onNativeAdAdDidLoad.Invoke();

        });
        // 加载过程中出现错误
        nativeAd.NativeAdDidFailWithError = (delegate (string error)
        {
            this.Log("Native ad failed to load with error: " + error);
            onNativeAdAdDidFailWithError.Invoke(error);
        });
        // 广告日志记录
        nativeAd.NativeAdWillLogImpression = (delegate ()
        {
            this.Log("Native ad logged impression.");
        });
        // 点击广告
        nativeAd.NativeAdDidClick = (delegate ()
        {
            this.Log("Native ad clicked.");
            onNativeAdAdDidClick.Invoke();
        });
        nativeAd.NativeAdDidFinishHandlingClick = (delegate ()
        {
            this.Log("Native ad Did Finish Handling Click.");
        });
        nativeAd.LoadAd();

    }

    private void Log(string s)
    {
        Debug.Log(s);
    }

    public void CloseNativeAD()
    {

        this.transform.GetChild(0).gameObject.SetActive(false);

        onNativeAdAdFinishHandlingClick.Invoke();

    }

}
