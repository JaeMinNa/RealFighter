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
        // 버튼 효과 비활성화
        btn.transform.GetChild(0).gameObject.SetActive(false);
        btn.transform.GetChild(1).gameObject.SetActive(false);

        // 버튼 효과 활성화
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
             
            // 무료 골드
            if(capturedIndex == 0)
            {
                var elementShop = Instantiate(m_ElementShop, Trans_GoldContent);
                elementShop.GetComponent<ElementShop>().SetShop(m_ShopList_Gold[index]);
                elementShop.GetComponent<ElementShop>().SetButton(() => OnClick_Buy_Gold_Free(capturedIndex));
            }
            // 광고 골드
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
        // 가챠 등급 설정 에러
        if(m_GachaGrade == -1)
        {
            UIManager.Instance.OpenSystemPopup(new MessageData
            {
                Type = PopupType.OkOnly,
                Title = "알림",
                Message = "Gacha 등급 설정 오류 입니다."
            });
            return;
        }

        // 골드 확인
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

        // 골드 감소
        DataManager.Instance.GetMyUserData().UserCommonData.Gold -= m_ShopData.Price;

        // 가챠
        UIManager.Instance.Open<Popup_Gacha_Hero>(UI.Popup, "Prefabs/UI/Popup/Popup_Gacha_Hero", new List<object> { m_GachaGrade } );

        // UI 갱신
        UIManager.Instance.Refresh();
    }

    private void Buy_Gold_Free()
    {
        // 오늘 이미 구매했는지 확인
        if(DataManager.Instance.GetMyUserData().UserContentsData.IsGotFreeGold)
        {
            UIManager.Instance.OpenSystemPopup(new MessageData
            {
                Type = PopupType.OkOnly,
                Title = "알림",
                Message = "무료 구매는 하루에 한번만 가능합니다.\n<size=40>* 00:00 시에 구매횟수가 초기화 됩니다.</size>"
            });
            return;
        }

        // 구매 완료
        DataManager.Instance.GetMyUserData().UserContentsData.IsGotFreeGold = true;

        // 골드 증가
        DataManager.Instance.GetMyUserData().UserCommonData.Gold += m_ShopData.Count;

        // 데이터 저장
        DataManager.Instance.SaveData();

        // UI 갱신
        UIManager.Instance.Refresh();

        // 완료 팝업
        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkOnly,
            Title = "알림",
            Message = "구매를 완료 하였습니다."
        });
    }

    private void Buy_Gold_Ad()
    {
        // 광고 보기



        // 골드 증가
        DataManager.Instance.GetMyUserData().UserCommonData.Gold += m_ShopData.Count;

        // 데이터 저장
        DataManager.Instance.SaveData();

        // UI 갱신
        UIManager.Instance.Refresh();

        // 완료 팝업
        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkOnly,
            Title = "알림",
            Message = "구매를 완료 하였습니다."
        });
    }
    #endregion

    #region Button
    private void OnClick_LeftBtn(int num)
    {
        // 버튼 설정
        for (int index = 0; index < LeftBtnList.Count; ++index)
            SetLeftBtn(LeftBtnList[index], false);

        SetLeftBtn(LeftBtnList[num], true);

        // 컨텐츠 설정
        SetContent(ContentList[num]);
    }

    private void OnClick_Buy_Hero(int num)
    {
        m_ShopData = m_ShopList_Hero[num];

        // 가챠 등급 설정
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
            Message = "정말 구매 하시겠습니까?",
            OkAction = () => { Buy_Hero(); }
        });
    }

    private void OnClick_Buy_Gold_Free(int num)
    {
        m_ShopData = m_ShopList_Gold[num];

        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkCancel,
            Title = "구매",
            Message = "정말 구매 하시겠습니까?",
            OkAction = () => { Buy_Gold_Free(); }
        });
    }

    private void OnClick_Buy_Gold_Ad(int num)
    {
        m_ShopData = m_ShopList_Gold[num];

        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkCancel,
            Title = "구매",
            Message = "정말 구매 하시겠습니까?",
            OkAction = () => { Buy_Gold_Ad(); }
        });
    }

    private void OnClick_Close()
    {
        UIManager.Instance.Close<Popup_Shop>();
    }
    #endregion
}
