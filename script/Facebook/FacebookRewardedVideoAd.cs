using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using AudienceNetwork;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// Facebook 奖励广告执行类
/// </summary>
public class FacebookRewardedVideoAd : MonoBehaviour
{


    /*
     * 

    rewarded Video Ad Did Load;             奖励视频广告加载;
    rewarded Video Ad Will Log Impression;  奖励视频广告将记录印象;
    rewarded Video Ad Did Fail With Error;  奖励视频广告失败，错误;
    rewarded Video Ad Did Click;            奖励视频广告点击;
    rewarded Video Ad Will Close;           奖励视频广告将关闭;
    rewarded Video Ad Did Close;            奖励视频广告关闭;
    rewarded Video Ad Complete;             奖励视频广告完整;
    rewarded Video Ad Did Succeed;          奖励视频广告成功;
    rewarded Video Ad Did Fail;             奖励视频广告失败;



    */

    [System.Serializable]
    public class RewardedVideoAdDidLoad : UnityEvent { }; //奖励视频广告加载;

    [System.Serializable]
    public class RewardedVideoAdDidFailWithError : UnityEvent<string> { }; //rewarded Video Ad Did Fail With Error;  奖励视频广告失败，错误;

    [System.Serializable]
    public class RewardedVideoAdDidClick : UnityEvent { }; // 奖励视频广告点击;

    [System.Serializable]
    public class RewardedVideoAdDidFail : UnityEvent { };  // 奖励视频广告失败;

    [System.Serializable]
    public class RewardedVideoAdDidSucceed : UnityEvent { };  // 奖励视频广告成功;

    [System.Serializable]
    public class RewardedVideoAdComplete : UnityEvent { };  // 奖励视频广告完整;

    [System.Serializable]
    public class RewardedVideoAdClose : UnityEvent { };  // 奖励视频广告关闭;

    public RewardedVideoAdDidLoad onRewardedVideoAdDidLoad = new RewardedVideoAdDidLoad();
    public RewardedVideoAdDidFailWithError onRewardedVideoAdDidFailWithError = new RewardedVideoAdDidFailWithError();
    public RewardedVideoAdDidClick onRewardedVideoAdDidClick = new RewardedVideoAdDidClick();
    public RewardedVideoAdDidSucceed onRewardedVideoAdDidSucceed = new RewardedVideoAdDidSucceed();
    public RewardedVideoAdDidFail onRewardedVideoAdDidFail = new RewardedVideoAdDidFail();
    public RewardedVideoAdComplete onRewardedVideoAdComplete = new RewardedVideoAdComplete();
    public RewardedVideoAdClose onRewardedVideoAdClose = new RewardedVideoAdClose();

    private string uniqueId;


    public void OnFinalResult(string result)
    {
    }
    void Start()
    {
        Debug.Log("FacebookRewardedVideoAd  is start");

    }


    public void init(string uniqueId)
    {
        this.uniqueId = uniqueId;
        rewardedVideoAd = new RewardedVideoAd(uniqueId);
        this.rewardedVideoAd.Register(this.gameObject);
        LoadRewardedVideo();
        // Initiate the request to load the ad.

    }

    private RewardedVideoAd rewardedVideoAd;
    private bool isLoaded;  // 是否加载结束

    public void LoadAD()
    {
        if (this.isLoaded)
        {
            ShowRewardedVideo();
        }
        else
        {
            this.rewardedVideoAd.LoadAd();

        }

    }

    // Load Rewarded Video
    public void LoadRewardedVideo()
    {


        // 加载奖励视频广告完成
        this.rewardedVideoAd.RewardedVideoAdDidLoad = (delegate ()
        {
            Debug.Log("RewardedVideo ad loaded.");
            this.isLoaded = true;
            onRewardedVideoAdDidLoad.Invoke();
        });
        // 加载过程中出现错误
        rewardedVideoAd.RewardedVideoAdDidFailWithError = (delegate (string error)
        {

            Debug.Log("RewardedVideo ad failed to load with error: " + error);
            onRewardedVideoAdDidFailWithError.Invoke(error);
        });

        // 广告日志记录
        rewardedVideoAd.RewardedVideoAdWillLogImpression = (delegate ()
        {
            Debug.Log("RewardedVideo ad logged impression.");
        });

        // 点击奖励广告
        rewardedVideoAd.RewardedVideoAdDidClick = (delegate ()
        {
            onRewardedVideoAdDidClick.Invoke();
        });
        // 奖励广告成功
        rewardedVideoAd.RewardedVideoAdDidSucceed = (delegate ()
        {
            onRewardedVideoAdDidSucceed.Invoke();
        });
        // 奖励广告完整
        rewardedVideoAd.RewardedVideoAdComplete = (delegate ()
        {
            onRewardedVideoAdComplete.Invoke();
        });
        // 奖励广告失败
        rewardedVideoAd.RewardedVideoAdDidFail = (delegate ()
        {
            onRewardedVideoAdDidFail.Invoke();
        });

        // 奖励广告关闭
        rewardedVideoAd.RewardedVideoAdDidClose = (delegate ()
        {
            onRewardedVideoAdClose.Invoke();
        });

    }




    // if load over , play Rewarded Video
    public void ShowRewardedVideo()
    {
        if (this.isLoaded)
        {
            this.rewardedVideoAd.Show();
            this.isLoaded = false;
        }

    }

    void OnDestroy()
    {
        // Dispose of rewardedVideo ad when the scene is destroyed
        if (this.rewardedVideoAd != null)
        {
            this.rewardedVideoAd.Dispose();
        }
        Debug.Log("RewardedVideoAdTest was destroyed!");
    }

}
