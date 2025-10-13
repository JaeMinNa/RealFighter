using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Shop : UIElement
{
    #region Cahsed Object
    [Header("Top UI")]
    [SerializeField] private TMP_Text Text_Gold = null;
    [SerializeField] private Button Btn_Close = null;

    [Header("Left UI")]
    [SerializeField] private List<Button> LeftBtnList = new List<Button>();

    [Header("Contents")]
    [SerializeField] private List<GameObject> ContentsList = new List<GameObject>();

    //[SerializeField] private Transform Trans_Content = null;
    //[SerializeField] private Button Btn_Close = null;
    #endregion

    #region Member Property
    //private List<HeroData> m_MyHeroes = new List<HeroData>();
    //private GameObject m_ElementHero = null;
    #endregion

    #region Override Method
    public override void Init()
    {
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
        for (int index = 0; index < ContentsList.Count; ++index)
            ContentsList[index].SetActive(false);

        obj.SetActive(true);
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
        SetContent(ContentsList[num]);
    }

    //private void OnClick_Hero(int num)
    //{
    //     모든 버튼 초기화
    //    for (int index = 0; index < Trans_Content.childCount; ++index)
    //        Trans_Content.GetChild(index).GetComponent<ElementHero>().SetSelect(false);

    //     해당 버튼 활성화
    //    Trans_Content.GetChild(num).GetComponent<ElementHero>().SetSelect(true);

    //     장착 히어로 변경
    //    DataManager.Instance.GetMyUserData().UserHeroData.EquipHero = m_MyHeroes[num];

    //     UI Refresh
    //    UIManager.Instance.Refresh();

    //     데이터 저장
    //    DataManager.Instance.SaveData();
    //}

    private void OnClick_Close()
    {
        UIManager.Instance.Close<Popup_Shop>();
    }
    #endregion
}
