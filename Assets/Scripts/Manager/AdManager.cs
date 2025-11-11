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
    // 배너 광고 로드 (처음 호출 시)
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

    // 배너 광고 생성
    private void CreateBannerView()
    {
        Debug.LogWarning("Creating banner view");

        if (m_BannerView != null)
        {
            DestroyBannerAd();
        }

        m_BannerView = new BannerView(m_AdBannerUnitId, AdSize.Banner, AdPosition.Bottom);

        // 참고: 아래는 반응형(Adaptive) 배너 생성 예시
        // AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        // _bannerView = new BannerView(_adBannerUnitId, adaptiveSize, AdPosition.Bottom);
    }

    // 배너 광고 표시
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

    // 배너 광고 숨기기
    private void HideBannerAd()
    {
        if (m_BannerView != null)
        {
            Debug.LogWarning("Hide banner ad.");
            m_BannerView.Hide();
        }
    }

    // 배너 광고 제거
    private void DestroyBannerAd()
    {
        if (m_BannerView != null)
        {
            Debug.Log("Destroying banner ad.");
            m_BannerView.Destroy();
            m_BannerView = null;
        }
    }

    // 배너 광고 이벤트 등록
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
        m_BannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.LogWarning("Banner view full screen content opened.");
        };
        m_BannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.LogWarning("Banner view full screen content closed.");
        };
    }
    #endregion

    #region Reward
    // 리워드 광고 로드 및 표시
    public void LoadRewardedAd(Action action)
    {
        if (m_IsLoadingReward) return;
        m_IsLoadingReward = true;

        m_Action = action;

        // 이전 광고 객체가 남아 있다면 정리
        if (m_RewardedAd != null)
        {
            m_RewardedAd.Destroy();
            m_RewardedAd = null;
        }

        // 광고 요청 생성
        var adRequest = new AdRequest();

        // 광고 요청 전송
        RewardedAd.Load(m_AdRewardUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                m_IsLoadingReward = false;

                // 에러 처리
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);

                    // 광고 불러오기 실패
                    UIManager.Instance.OpenSystemPopup(new MessageData
                    {
                        Type = PopupType.OkOnly,
                        Title = "알림",
                        Message = "광고 불러오기를 실패 했습니다."
                    });

                    return;
                }

                Debug.LogWarning("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                m_RewardedAd = ad;
                RegisterEventHandlers(m_RewardedAd);
                ShowRewardedAd();
            });
    }

    // 리워드 광고 표시
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

    // 리워드 광고 이벤트 등록
    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.LogWarning(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.LogWarning("Rewarded ad recorded an impression.");
        };
        ad.OnAdClicked += () =>
        {
            Debug.LogWarning("Rewarded ad was clicked.");
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.LogWarning("Rewarded ad full screen content opened.");
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.LogWarning("Rewarded ad full screen content closed.");
        };
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
            // 테스트용 광고 단위 ID
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
            // 실제 배포용 광고 단위 ID (수정 필요)
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

        // Google Mobile Ads SDK 초기화
        MobileAds.Initialize((InitializationStatus initStatus) => { });
    }
    #endregion
}
