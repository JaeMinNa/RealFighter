using BackEnd;
using System;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Rank : UIElement
{
    #region Cahsed Object
    //[SerializeField] TMP_Text m_Text = null;
    //[SerializeField] TMP_InputField m_InputField = null;
    [SerializeField] private ElementRank MyRank = null;
    [SerializeField] private GameObject Obj_RankContent = null;
    [SerializeField] private Button Btn_Close = null;

    #endregion

    #region Member Property
    private GameObject m_ElementRank = null;
    private RankData m_MyRankData = null;
    private List<RankData> m_RankDataList = new List<RankData>(); 
    #endregion

    #region Unity Method
    #endregion

    #region Override Method
    public override void Init()
    {
        Btn_Close.onClick.AddListener(OnClick_Close);

        m_ElementRank = ResourceLoader.LoadAssetResources<GameObject>("Prefabs/Element/ElementRank");
        m_MyRankData = BackendManager.Instance.GetMyRankData();
        m_RankDataList = BackendManager.Instance.GetRankDataList();

        if(m_MyRankData == null || m_RankDataList == null)
        {
            UIManager.Instance.OpenSystemPopup(new MessageData
            {
                Type = PopupType.OkOnly,
                Message = "서버 연결을 실패 하였습니다.",
                OkAction = () =>
                {
                    UIManager.Instance.Close<Popup_Rank>();
                }
            });
        }
    }

    public override void OnClose()
    {

    }

    public override void OnOpen(List<object> Args)
    {
        // 나의 랭킹
        MyRank.SetRank(m_MyRankData);

        // 유저 랭킹
        for (int index = 0; index < m_RankDataList.Count; ++index)
        {
            var elementRank = Instantiate(m_ElementRank, Obj_RankContent.transform);
            elementRank.GetComponent<ElementRank>().SetRank(m_RankDataList[index]);
        }
    }

    public override void OnRefresh()
    {

    }
    #endregion

    #region Private Method
    
    #endregion

    #region Public Method

    #endregion

    #region Button

    private void OnClick_Close()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Close<Popup_Rank>();
    }
    #endregion
}
