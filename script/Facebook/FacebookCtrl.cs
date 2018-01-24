using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// facebook 广告管理类
/// 需求1：可现在三种广告 （1，原生广告 、 2， 插页式广告， 3，奖励广告）
/// 需求2：一天中只能观看三次奖励广告获取奖励 （本地保存日期 ， 要是改系统时间我就没办法了）
/// 需求3：每次观看奖励广告之后会有冷却时间
/// 需求4：插页式广告没5分钟显示一次 ， 在用户做某种操作后
/// 需求5：在游戏开始时显示一次原生广告
/// </summary>
public class FacebookCtrl : MonoBehaviour
{

    public static readonly string RewardedVideoAd_PLACEMENT_ID = "PLACEMENT_ID";  // 奖励广告版位编号字符串 

    public static readonly string NativeAd_PLACEMENT_ID = "PLACEMENT_ID";  // 原生广告版位编号字符串 

    public static readonly string InterstitialAd_PLACEMENT_ID = "PLACEMENT_ID";  // 插屏广告版位编号字符串 




    private FacebookRewardedVideoAd m_FacebookRewardedVideoAd;  // 奖励广告管理

    public bool Is_Succeed { get; private set; }  // 是否成功播放奖励广告
    public bool Is_DidLoad { get; private set; }  // 奖励广告是否加载完毕
    public bool Is_DidPlay { get; private set; }  // 用户是否播放奖励广告
    public bool Is_WaitTime { get; private set; }  // 奖励广告是否有冷却时间
    public int PlayRewardedNum { get; private set; } // 当前播放奖励视频的次数

    public const string DataTimeString = "FacebookRewardedVideoAdDataTimeString"; // 用于保存时间的本地字段
    public const string PlayRewardedNumString = "FacebookRewardedVideoAdPlayRewardedNum"; // 用于保存播放几次奖励广告的本地字段



    private FacebookNativeAd m_FacebookNativeAd;  // 原生广告管理
    public bool Is_NativeAdLoaded { get; private set; }  // 原生广告是否加载完成




    private FacebookInterstitialAd m_FacebookInterstitialAd;  // 插页广告管理
    public bool Is_InterstitiaLoaded { get; private set; }  // 插页广告是否加载完成
    public bool Is_Show_Recording { get; private set; }  // 是否在录音界面
    public bool Is_InterstitiaTime { get; private set; }  // 插页广告时间是否到达

    private void Start()
    {

        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            Is_Succeed = false;
            Is_DidLoad = false;
            Is_DidPlay = false;
            Is_WaitTime = false;

            if (!PlayerPrefs.HasKey(DataTimeString))
            {
                PlayRewardedNum = 0;
                PlayerPrefs.SetString(DataTimeString , GetDataTime());
                PlayerPrefs.SetInt(PlayRewardedNumString, PlayRewardedNum);
            }
            DontDestroyOnLoad(this);

            m_FacebookRewardedVideoAd = gameObject.AddComponent<FacebookRewardedVideoAd>();

            m_FacebookRewardedVideoAd.onRewardedVideoAdDidLoad.AddListener(OnRewardedVideoAdDidLoad); // 加载成功
            m_FacebookRewardedVideoAd.onRewardedVideoAdDidFailWithError.AddListener(OnRewardedVideoAdDidFailWithError); // 加载失败
            m_FacebookRewardedVideoAd.onRewardedVideoAdDidClick.AddListener(OnRewardedVideoAdDidClick);    //  广告结束后点击了广告
            m_FacebookRewardedVideoAd.onRewardedVideoAdDidSucceed.AddListener(OnRewardedVideoAdDidSucceed);  // 广告播放成功 
            m_FacebookRewardedVideoAd.onRewardedVideoAdDidFail.AddListener(OnRewardedVideoAdDidFail);       // 广告播放失败
            m_FacebookRewardedVideoAd.onRewardedVideoAdClose.AddListener(OnRewardedVideoAdClose);           // 播放结束后用户关闭广告
            m_FacebookRewardedVideoAd.onRewardedVideoAdComplete.AddListener(OnRewardedVideoAdComplete);           // 奖励广告完整播放
            m_FacebookRewardedVideoAd.init(RewardedVideoAd_PLACEMENT_ID);

            Is_Show_Recording = false;
            Is_NativeAdLoaded = false;
            m_FacebookNativeAd = GameObject.FindObjectOfType<FacebookNativeAd>();
            m_FacebookNativeAd.onNativeAdAdDidLoad.AddListener(OnNativeAdAdDidLoad); // 加载成功
            m_FacebookNativeAd.onNativeAdAdDidFailWithError.AddListener(OnNativeAdAdDidFailWithError); // 加载失败
            m_FacebookNativeAd.onNativeAdAdDidClick.AddListener(OnNativeAdAdDidClick);    //  广告结束后点击了广告
            m_FacebookNativeAd.onNativeAdAdFinishHandlingClick.AddListener(OnNativeAdAdFinishHandlingClick);  // ！！ 自己改过了 隐藏广告的回调
            m_FacebookNativeAd.init(NativeAd_PLACEMENT_ID);


            m_FacebookInterstitialAd = gameObject.AddComponent<FacebookInterstitialAd>();

            m_FacebookInterstitialAd.onInterstitialAdDidLoad.AddListener(OnInterstitialAdDidLoad); // 加载成功
            m_FacebookInterstitialAd.onInterstitialAdDidFailWithError.AddListener(OnInterstitialAdDidFailWithError); // 加载失败
            m_FacebookInterstitialAd.onInterstitialAdDidClick.AddListener(OnInterstitialAdDidClick);    //  广告结束后点击了广告
            m_FacebookInterstitialAd.onInterstitialAdWillClose.AddListener(OnInterstitialAdWillClose);  // 
            m_FacebookInterstitialAd.onInterstitialAdDidClose.AddListener(OnInterstitialAdDidClose);           // 播放结束后用户关闭广告
            m_FacebookInterstitialAd.init(InterstitialAd_PLACEMENT_ID);

            Invoke("ResetInterstitialAd", 300f);

        }
        else
        {
            Destroy(this);
        }
    }


    private string GetDataTime()
    {
        System.DateTime _now = System.DateTime.Now;
        return string.Format("{0}{1}{2}" , _now.Year , _now.Month , _now.Day);
    }

    /// <summary>
    /// 插页广告回调方法
    /// </summary>
    private void OnInterstitialAdDidClose()
    {
        Debug.Log("插页广告关闭");
        Invoke("ResetInterstitialAd", 300f);

    }

    private void OnInterstitialAdWillClose()
    {

    }

    private void OnInterstitialAdDidClick()
    {
        Debug.Log("用户点击了插页广告");

    }

    private void OnInterstitialAdDidFailWithError(string error)
    {
        Debug.Log("插页广告加载错误");
        Is_InterstitiaLoaded = false;
        Invoke("ResetInterstitialAd", 30f);

    }

    private void OnInterstitialAdDidLoad()
    {
        Debug.Log("插页广告加载结束");
        Is_InterstitiaLoaded = true;

    }

    private void ResetInterstitialAd()
    {
        Debug.Log("重置了插页广告");

        Is_InterstitiaTime = true;
        m_FacebookInterstitialAd.LoadInterstitial();
    }

    /// <summary>
    /// 触发插页式广告 显示
    /// </summary>
    private void TestInterstitialAd()
    {
        // 插页式广告 加载结束 时间到达 
        if (Is_InterstitiaLoaded && Is_InterstitiaTime )  
        {
            m_FacebookInterstitialAd.ShowInterstitial();
            Is_InterstitiaLoaded = false;
            Is_InterstitiaTime = false;
        }
    }


    /// <summary>
    /// 原生广告回调方法
    /// </summary>
    private void OnNativeAdAdDidLoad()
    {
        Debug.Log("加载 NativeAd 成功");

    }
    private void OnNativeAdAdDidFailWithError(string error)
    {
        Debug.Log("加载 NativeAd 失败 ： " + error);
        Is_NativeAdLoaded = false;
        m_FacebookNativeAd.LoadNativeAd();

    }
    private void OnNativeAdAdDidClick()
    {
        Debug.Log("用户点击了 NativeAd 广告");
    }
    private void OnNativeAdAdFinishHandlingClick()
    {
        Debug.Log("点击了 NativeAd 广告 事件处理完成");  // 用户关闭了广告界面

    }

    /// <summary>
    /// 显示原生广告入口
    /// </summary>
    private void TestNativeAd()
    {
        m_FacebookNativeAd.transform.GetChild(0).gameObject.SetActive(true);

    }



    /// <summary>
    /// 奖励广告回调方法
    /// </summary>
    private void OnRewardedVideoAdComplete()
    {
        Debug.Log("奖励广告完整播放");

        Is_Succeed = true;

    }

    private void OnRewardedVideoAdClose()
    {
        Debug.Log("用户关闭了奖励广告");

        if (Is_Succeed)  // 本次成功  
        {
            Is_Succeed = false;

        }
        else // 本次失败
        {

        }


    }

    private void OnRewardedVideoAdDidFail()
    {
        Debug.Log("奖励广告播放失败");
        Is_Succeed = false;
    }

    private void OnRewardedVideoAdDidSucceed()
    {
        Debug.Log("奖励广告播放成功");
        Is_Succeed = true;

    }

    private void OnRewardedVideoAdDidClick()
    {
        Debug.Log("用户点击了奖励广告");
        Is_Succeed = true;

    }

    private void OnRewardedVideoAdDidFailWithError(string error)
    {
        Debug.Log("失败 错误码 " + error);
        Is_Succeed = false;
        OnRewardedVideoAdClose();

    }

    /// <summary>
    /// 显示奖励广告
    /// </summary>
    private void OnRewardedVideoAdDidLoad()
    {
        m_FacebookRewardedVideoAd.ShowRewardedVideo();
        Is_DidLoad = true;
    }

    private void PlayRewardedVideoAd()
    {
        if (PlayerPrefs.HasKey(DataTimeString))
        {
            if (string.Equals(PlayerPrefs.GetString(DataTimeString) , GetDataTime()))
            {
                PlayRewardedNum = PlayerPrefs.GetInt(PlayRewardedNumString);
                if (PlayRewardedNum >= 3) // 今天不能再看了
                {

                }
                else  // 今天还可以再看
                {
                    Debug.Log("今天还可以再看" + PlayRewardedNum);
                    if (Is_WaitTime && PlayRewardedNum == 1)
                    {

                    }
                    else if (Is_WaitTime && PlayRewardedNum == 2)
                    {

                    }
                    else
                    {
                        m_FacebookRewardedVideoAd.LoadAD();
                        Is_DidPlay = true;
                    }

                }
            }
            else
            {
                PlayRewardedNum = 0;
                PlayerPrefs.SetString(DataTimeString, GetDataTime());
                PlayerPrefs.SetInt(PlayRewardedNumString, PlayRewardedNum);
                m_FacebookRewardedVideoAd.LoadAD();
                Is_DidPlay = true;
            }
        }

    }

}
