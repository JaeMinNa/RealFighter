using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Android;



#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class LocalPushManager : Singleton<LocalPushManager>
{
    #region Instance
    public static LocalPushManager Instance
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

                GameObject ManagerObj = GameObject.Find("[Managers]/LocalPushManager");
                if (ManagerObj == null)
                {
                    ManagerObj = new GameObject("LocalPushManager");
                    ManagerObj.transform.SetParent(Obj.transform);
                }

                m_Instance = ManagerObj.GetComponent<LocalPushManager>();
                if (m_Instance == null)
                {
                    m_Instance = ManagerObj.AddComponent<LocalPushManager>();
                }

                m_Instance.CreateInstance();
            }

            return m_Instance;
        }
    }
    #endregion

    #region Member Property
    #endregion

    #region Override Method
    protected override void CreateInstance()
    {
        Init();
    }

    public override void DestroyInstance() { }
    #endregion

    #region Public Method
    // 지정한 시간에 푸시 알람을 예약합니다.
    public void SchedulePushNotification(LocalPushType pushType, string title, string message, DateTime scheduleTime)
    {
        // 예약 시간이 현재보다 미래인지 확인
        if (scheduleTime <= Util.DateTimeNow)
        {
            Debug.LogWarning("The time is earlier or equal to the current time. Please enter a valid future time.");
            return;
        }

        try
        {
#if UNITY_ANDROID
           
            // Android: 알림 객체 생성 및 설정
            var notification = new AndroidNotification();
            notification.Title = title;
            notification.Text = message;
            notification.FireTime = scheduleTime;
            notification.LargeIcon = "icon_0";
            notification.SmallIcon = "icon_1";
            notification.ShowInForeground = true;
            string channelId = "my_channel_id";

            int pushCode = AndroidNotificationCenter.SendNotification(notification, channelId);

            switch (pushType)
            {
                case LocalPushType.FreeGold:
                    PlayerPrefs.SetInt(ClientDef.LOCALKEY_Push_FreeGold, pushCode);
                    break;

                default:
                    break;
            }

#elif UNITY_IOS
            // iOS: 예약 시간과 현재 시간 간의 간격(TimeInterval) 계산
            TimeSpan interval = scheduleTime - GameManager.Instance.DateTimeNow;

            if (interval.TotalSeconds < 1)
            interval = TimeSpan.FromSeconds(1);

            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = interval,
                Repeats = false
            };

            string notificationId = Guid.NewGuid().ToString();  // 중복되지 않는 고유 ID 생성

            var notification = new iOSNotification()
            {
                Identifier = notificationId,
                Title = title,
                Body = message,
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound | PresentationOption.Badge),
                CategoryIdentifier = "custom_category",
                ThreadIdentifier = "custom_thread",
                Trigger = timeTrigger,
            };

            iOSNotificationCenter.ScheduleNotification(notification);

            switch (pushType)
            {
                case ClientDef.LocalPushType.TreasureTrove:
                    PlayerPrefs.SetString(ClientDef.LOCALKEY_Push_TreasureTrove, notificationId);
                    break;

                default:
                    break;
            }

#endif
        }
        catch (Exception e)
        {
            Debug.LogWarning("푸시알람 예약 중 오류 발생: " + e.ToString());
        }
    }

    public void CancelPushNotification(LocalPushType pushType)
    {
#if UNITY_ANDROID

        int pushCode = 0;

        switch (pushType)
        {
            case LocalPushType.FreeGold:
                pushCode = PlayerPrefs.GetInt(ClientDef.LOCALKEY_Push_FreeGold, 0);
                break;

            default:
                break;
        }

        if (pushCode == 0)
            return;

        AndroidNotificationCenter.CancelScheduledNotification(pushCode);

#elif UNITY_IOS

        string pushCode = string.Empty;

        switch (pushType)
        {
            case ClientDef.LocalPushType.TreasureTrove:
                pushCode = PlayerPrefs.GetString(ClientDef.LOCALKEY_Push_TreasureTrove, string.Empty);
                break;

            default:
                break;
        }

        if (pushCode == string.Empty)
            return;

        iOSNotificationCenter.RemoveScheduledNotification(pushCode);

#endif

        Debug.LogWarning("Complete Cancel to Push Notification.");
    }

    // 모든 예약된 알림을 취소합니다.
    public void CancelAllPushNotifications()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif

        Debug.LogWarning("Cancel All Push Notifications.");
    }
    #endregion

    #region Private Method
    // 앱 시작 시 권한 요청 및 채널 등록, 그리고 예약 알림 호출
    private void Init()
    {
#if UNITY_ANDROID
        // 디바이스의 안드로이드 api level 얻기
        if (!GameManager.Instance.IsEditor)
        {
            int apiLevel = GetAndroidAPILevel();
            Debug.LogWarning("ApiLevel: " + apiLevel);

            // 디바이스의 api level이 33 이상이라면 퍼미션 요청
            if (apiLevel >= 33 &&
                !Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
        }

        RegisterAndroidChannel();
#elif UNITY_IOS

#endif
    }

#if UNITY_ANDROID
    // Android 알림 채널 등록 (앱 실행 시 한 번만 호출)
    private void RegisterAndroidChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "my_channel_id",
            Name = "Real Fighter",
            Importance = Importance.High,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        Debug.LogWarning("Register Android Channel");
    }

    private int GetAndroidAPILevel()
    {
        using (var versionClass = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return versionClass.GetStatic<int>("SDK_INT");
        }
    }
#endif

#if UNITY_IOS
    public IEnumerator RequestNotificationPermission(Action onGranted, Action onDenied)
    {
        // 어떤 권한들을 요청할지 지정 (Alert, Badge, Sound 등)
        var options = AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound;

        // registerForRemoteNotifications = false 이면 원격 푸시 토큰 요청 안 함
        bool registerForRemote = false;

        using (var req = new AuthorizationRequest(options, registerForRemote))
        {
            // 요청 완료될 때까지 대기
            while (!req.IsFinished)
            {
                yield return null;
            }

            Debug.LogWarning("IOS Notification Permission Request finished");
            Debug.LogWarning("Granted: " + req.Granted);
            Debug.LogWarning("Error: " + req.Error);
            Debug.LogWarning("Device Token: " + req.DeviceToken);
        }
    }
#endif
    #endregion
}
