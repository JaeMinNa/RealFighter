using UnityEngine;
using System;
using GoogleMobileAds.Api;

public class AdManager : Singleton<AdManager>
{
    private bool m_IsTestMode = true;
    private RewardedAd m_RewardedAd = null;
    private BannerView m_BannerView = null;
    private string m_AdRewardUnitId = string.Empty;
    private string m_AdBannerUnitId = string.Empty;
    private Action m_Action = null;
    private bool m_IsLoadingReward = false;

    public static AdManager Instance
    {
        get
        {
            if (m_Instance == null && Application.isPlaying)
            {
                GameObject Obj = GameObject.Find("[Managers]");
                if (Obj == null)
                {
                    Obj = new GameObject("[Managers]");
                    DontDestroyOnLoad(Obj);
                }

                GameObject managerObj = GameObject.Find("[Managers]/AdManager");
                if (managerObj == null)
                {
                    managerObj = new GameObject("AdManager");
                    managerObj.transform.SetParent(Obj.transform);
                }

                m_Instance = managerObj.GetComponent<AdManager>();
                if (m_Instance == null)
                {
                    m_Instance = managerObj.AddComponent<AdManager>();
                }

                m_Instance.CreateInstance();
            }

            return m_Instance;
        }
    }

    #region Override Method
    public override void DestroyInstance()
    {

    }

    protected override void CreateInstance()
    {
        Init();
    }
    #endregion

    #region Banner
    //광고 로드, 사용 시 호출
    public void LoadBannerAd()
    {
        if (m_BannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();

        Debug.LogWarning("Loading banner ad.");
        m_BannerView.LoadAd(adRequest);
    }

    //광고 보여주기
    private void CreateBannerView()
    {
        Debug.LogWarning("Creating banner view");

        if (m_BannerView != null)
        {
            DestroyBannerAd();
        }

        m_BannerView = new BannerView(m_AdBannerUnitId, AdSize.Banner, AdPosition.Bottom);

        //적응형 배너(꽉찬 사이즈)
        //AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        //_bannerView = new BannerView(_adBannerUnitId, adaptiveSize, AdPosition.Bottom);
    }

    //광고 표시
    private void ShowBannerAd()
    {
        if (m_BannerView != null)
        {
            Debug.LogWarning("Show banner ad.");
            m_BannerView.Show();
        }
        else
        {
            LoadBannerAd();
        }
    }

    //광고 숨기기
    private void HideBannerAd()
    {
        if (m_BannerView != null)
        {
            Debug.LogWarning("Hide banner ad.");
            m_BannerView.Hide();
        }
    }

    //광고 제거
    private void DestroyBannerAd()
    {
        if (m_BannerView != null)
        {
            Debug.Log("Destroying banner ad.");
            m_BannerView.Destroy();
            m_BannerView = null;
        }
    }

    private void ListenToBannerAdEvents()
    {
        m_BannerView.OnBannerAdLoaded += () =>
        {
            Debug.LogWarning("Banner view loaded an ad with response : "
                + m_BannerView.GetResponseInfo());
        };
        m_BannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        m_BannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.LogWarning(string.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        m_BannerView.OnAdImpressionRecorded += () =>
        {
            Debug.LogWarning("Banner view recorded an impression.");
        };
        m_BannerView.OnAdClicked += () =>
        {
            Debug.LogWarning("Banner view was clicked.");
        };
        m_BannerView.OnAdFullScreenContentOpened += (null);
        {
            Debug.LogWarning("Banner view full screen content opened.");
        };
        m_BannerView.OnAdFullScreenContentClosed += (null);
        {
            Debug.LogWarning("Banner view full screen content closed.");
        };
    }
    #endregion

    #region Reward
    //사용 시, 호출
    public void LoadRewardedAd(Action action)
    {
        if (m_IsLoadingReward) return;
        m_IsLoadingReward = true;

        m_Action = action;

        // Clean up the old ad before loading a new one.
        if (m_RewardedAd != null)
        {
            m_RewardedAd.Destroy();
            m_RewardedAd = null;
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(m_AdRewardUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                m_IsLoadingReward = false;

                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.LogWarning("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                m_RewardedAd = ad;
                RegisterEventHandlers(m_RewardedAd);
                ShowRewardedAd();
            });
    }

    private void ShowRewardedAd()
    {
        if (m_RewardedAd != null && m_RewardedAd.CanShowAd())
        {
            m_RewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"User earned reward: {reward.Amount} {reward.Type}");
                m_Action?.Invoke();
                m_Action = null;  
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not ready yet.");
            LoadRewardedAd(m_Action);
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.LogWarning(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.LogWarning("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.LogWarning("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.LogWarning("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.LogWarning("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
            LoadRewardedAd(m_Action);
        };
    }
    #endregion

    #region Private Method
    private void Init()
    {
        if (m_IsTestMode)
        {
            // 테스트 ID (그대로 사용)
#if UNITY_ANDROID
            m_AdRewardUnitId = "ca-app-pub-3940256099942544/5224354917";
            m_AdBannerUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            m_AdRewardUnitId = "ca-app-pub-3940256099942544/1712485313";
            m_AdBannerUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            m_AdRewardUnitId = "unused";
            m_AdBannerUnitId = "unused";
#endif
        }
        else
        {
            // 광고 ID (수정해야 함)
#if UNITY_ANDROID
            m_AdRewardUnitId = "ca-app-pub-5906820670754550/8653741011";
            m_AdBannerUnitId = "ca-app-pub-5906820670754550/8624255011";
#elif UNITY_IPHONE
            m_AdRewardUnitId = "ca-app-pub-3940256099942544/1712485313";
            m_AdBannerUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            m_AdRewardUnitId = "unused";
            m_AdBannerUnitId = "unused";
#endif
        }

        MobileAds.Initialize((InitializationStatus initStatus) => { });
    }
#endregion
}