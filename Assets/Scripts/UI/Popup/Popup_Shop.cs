using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Shop : UIElement
{
    private enum ShopType
    {
        None,

        Hero,
        Item,

        Max
    }

    #region Cahsed Object
    [Header("Top UI")]
    [SerializeField] private TMP_Text Text_Gold = null;
    [SerializeField] private Button Btn_Close = null;

    [Header("Left UI")]
    [SerializeField] private List<Button> LeftBtnList = new List<Button>();

    [Header("Contents")]
    [SerializeField] private List<GameObject> ContentList = new List<GameObject>();
    [SerializeField] private Transform Trans_HeroContent = null;
    [SerializeField] private Transform Trans_GoldContent = null;
    #endregion

    #region Member Property
    private List<ShopData> m_ShopList_Hero = new List<ShopData>();
    private List<ShopData> m_ShopList_Gold = new List<ShopData>();
    private GameObject m_ElementShop = null;
    private ShopData m_ShopData = null;
    private int m_GachaGrade = -1;
    #endregion

    #region Override Method
    public override void Init()
    {
        m_ElementShop = ResourceLoader.LoadAssetResources<GameObject>("Prefabs/Element/ElementShop");
        m_ShopList_Hero = ClientDef.ShopList_Hero;
        m_ShopList_Gold = ClientDef.ShopList_Gold;

        for (int index = 0; index < LeftBtnList.Count; ++index)
        {
            int capturedIndex = index;

            LeftBtnList[index].onClick.AddListener(() => OnClick_LeftBtn(capturedIndex));
        }

        Btn_Close.onClick.AddListener(OnClick_Close);
    }

    public override void OnClose()
    {

    }

    public override void OnOpen(List<object> Args)
    {
        Text_Gold.text = DataManager.Instance.GetMyUserData().UserCommonData.Gold.ToString();

        SetShopList();
        OnClick_LeftBtn(0);
    }

    public override void OnRefresh()
    {
        Text_Gold.text = DataManager.Instance.GetMyUserData().UserCommonData.Gold.ToString();
    }
    #endregion

    #region Private Method
    private void SetLeftBtn(Button btn, bool isOn)
    {
        // ��ư ȿ�� ��Ȱ��ȭ
        btn.transform.GetChild(0).gameObject.SetActive(false);
        btn.transform.GetChild(1).gameObject.SetActive(false);

        // ��ư ȿ�� Ȱ��ȭ
        if(isOn)
            btn.transform.GetChild(1).gameObject.SetActive(true);
        else
            btn.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetContent(GameObject obj)
    {
        for (int index = 0; index < ContentList.Count; ++index)
            ContentList[index].SetActive(false);

        obj.SetActive(true);
    }

    private void SetShopList()
    {
        // Hero
        for (int index = 0; index < m_ShopList_Hero.Count; ++index)
        {
            int capturedIndex = index;

            var elementShop = Instantiate(m_ElementShop, Trans_HeroContent);
            elementShop.GetComponent<ElementShop>().SetShop(m_ShopList_Hero[index]);
            elementShop.GetComponent<ElementShop>().SetButton(() => OnClick_Buy_Hero(capturedIndex));
        }

        // Gold
        for (int index = 0; index < m_ShopList_Gold.Count; ++index)
        {
            int capturedIndex = index;
             
            // Gold
            if(capturedIndex == 0)
            {
                var elementShop = Instantiate(m_ElementShop, Trans_GoldContent);
                elementShop.GetComponent<ElementShop>().SetShop(m_ShopList_Gold[index]);
                elementShop.GetComponent<ElementShop>().SetButton(() => OnClick_Buy_Gold_Free(capturedIndex));
            }
            // Ad
            else if (capturedIndex == 1)
            {
                var elementShop = Instantiate(m_ElementShop, Trans_GoldContent);
                elementShop.GetComponent<ElementShop>().SetShop(m_ShopList_Gold[index]);
                elementShop.GetComponent<ElementShop>().SetButton(() => OnClick_Buy_Gold_Ad(capturedIndex));
                elementShop.GetComponent<ElementShop>().SetAd();
            }
        }
    }

    private void Buy_Hero()
    {
        if(m_GachaGrade == -1)
        {
            UIManager.Instance.OpenSystemPopup(new MessageData
            {
                Type = PopupType.OkOnly,
                Title = "알림",
                Message = "히어로 등급 데이터를 불러올 수 없습니다."
            });
            return;
        }

        // Gold 체크
        if (DataManager.Instance.GetMyUserData().UserCommonData.Gold < m_ShopData.Price)
        {
            UIManager.Instance.OpenSystemPopup(new MessageData
            {
                Type = PopupType.OkOnly,
                Title = "알림",
                Message = "GOLD가 부족합니다."
            });
            return;
        }

        DataManager.Instance.GetMyUserData().UserCommonData.Gold -= m_ShopData.Price;

        // 가챠 
        UIManager.Instance.Open<Popup_Gacha_Hero>(UI.Popup, "Prefabs/UI/Popup/Popup_Gacha_Hero", new List<object> { m_GachaGrade } );

        // UI 갱신
        UIManager.Instance.Refresh();
    }

    private void Buy_Gold_Free()
    {
        // 기존에 예약된 로컬 푸시 제거
        LocalPushManager.Instance.CancelPushNotification(LocalPushType.FreeGold);

        // 푸시를 보낼 시간 계산
        DateTime RewardTime = Util.DateTimeNow.Date.AddDays(1);
        //RewardTime = RewardTime.AddSeconds(30);  // Test 용

        LocalPushManager.Instance.SchedulePushNotification(
            LocalPushType.FreeGold,
            "Free Gold!",
            "무료 Gold를 받을 수 있습니다! 받으러 오세요~",
            RewardTime);

        Debug.LogWarning($"Local Push Sucess! Send Push on {RewardTime}");

        // 이미 오늘 받았는지 체크
        if (DataManager.Instance.GetMyUserData().UserContentsData.IsGotFreeGold)
        {
            UIManager.Instance.OpenSystemPopup(new MessageData
            {
                Type = PopupType.OkOnly,
                Title = "알림",
                Message = "오늘 이미 무료 골드를 획득 했습니다.\n<size=40>* 00:00시에 다시 획득할 수 있습니다.</size>"
            });
            return;
        }

        DataManager.Instance.GetMyUserData().UserContentsData.IsGotFreeGold = true;
        DataManager.Instance.GetMyUserData().UserCommonData.Gold += m_ShopData.Count;
        DataManager.Instance.SaveData();

        // UI 갱신
        UIManager.Instance.Refresh();

        // 완료 팝업
        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkOnly,
            Title = "알림",
            Message = "무료 골드를 획득 했습니다."
        });
    }

    private void Buy_Gold_Ad()
    {
        // 광고 횟수 체크
        if (DataManager.Instance.GetMyUserData().UserContentsData.AdGoldBuyCount >= 5)
        {
            UIManager.Instance.OpenSystemPopup(new MessageData
            {
                Type = PopupType.OkOnly,
                Title = "알림",
                Message = "오늘 이미 모든 광고 골드를 받았습니다.\n<size=40>* 00:00시에 다시 획득할 수 있습니다.</size>"
            });
            return;
        }

        // 광고 
        AdManager.Instance.LoadRewardedAd(() =>
        {
            DataManager.Instance.GetMyUserData().UserContentsData.AdGoldBuyCount++;
            DataManager.Instance.GetMyUserData().UserCommonData.Gold += m_ShopData.Count;
            DataManager.Instance.SaveData();
            UIManager.Instance.Refresh();

            // 완료
            UIManager.Instance.OpenSystemPopup(new MessageData
            {
                Type = PopupType.OkOnly,
                Title = "알림",
                Message = "광고 골드를 획득 했습니다."
            });

            return;
        });

        // 광고 불러오기 실패
        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkOnly,
            Title = "알림",
            Message = "광고 불러오기를 실패 했습니다."
        });
    }
    #endregion

    #region Button
    public void OnClick_LeftBtn(int num)
    {
        SoundManager.Instance.StartSFX("ButtonClick");

        for (int index = 0; index < LeftBtnList.Count; ++index)
            SetLeftBtn(LeftBtnList[index], false);

        SetLeftBtn(LeftBtnList[num], true);
        SetContent(ContentList[num]);
    }

    private void OnClick_Buy_Hero(int num)
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        m_ShopData = m_ShopList_Hero[num];

        if (num == 0)    // Normal
            m_GachaGrade = 0;
        else if (num == 1)  // Rare
            m_GachaGrade = 1;
        else
            m_GachaGrade = -1;

        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkCancel,
            Title = "구매",
            Message = "Hero를 구매 하시겠습니까?",
            OkAction = () => { Buy_Hero(); }
        });
    }

    private void OnClick_Buy_Gold_Free(int num)
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        m_ShopData = m_ShopList_Gold[num];

        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkCancel,
            Title = "구매",
            Message = "무료 골드를 획득 하시겠습니까?",
            OkAction = () => { Buy_Gold_Free(); }
        });
    }

    private void OnClick_Buy_Gold_Ad(int num)
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        m_ShopData = m_ShopList_Gold[num];

        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkCancel,
            Title = "광고 보상",
            Message = "광고를 시청하고, 골드를 획득 하시겠습니까?",
            OkAction = () => { Buy_Gold_Ad(); }
        });
    }

    private void OnClick_Close()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Close<Popup_Shop>();
    }
    #endregion
}
