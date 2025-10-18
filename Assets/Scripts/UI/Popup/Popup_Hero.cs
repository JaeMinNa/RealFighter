using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Hero : UIElement
{
    #region Cahsed Object
    [SerializeField] private Transform Trans_Content = null;
    [SerializeField] private Button Btn_Close = null;
    #endregion

    #region Member Property
    private List<HeroData> m_MyHeroes = new List<HeroData>();
    private GameObject m_ElementHero = null;
    #endregion

    #region Override Method
    public override void Init()
    {
        Btn_Close.onClick.AddListener(OnClick_Close);

        m_ElementHero = ResourceLoader.LoadAssetResources<GameObject>("Prefabs/Element/ElementHero");
        m_MyHeroes = DataManager.Instance.GetMyUserData().UserHeroData.MyHeroes;

        // MyHeroData를 정렬
        m_MyHeroes = m_MyHeroes
        .OrderByDescending(data => data.HeroName == DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.HeroName && data.Grade == DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Grade)
        .ThenByDescending(data => data.Grade)
        .ThenByDescending(data => data.GradeExp)
        .ThenByDescending(data => data.Level)
        .ThenByDescending(data => data.Exp)
        .ToList();
    }

    public override void OnClose()
    {

    }

    public override void OnOpen(List<object> Args)
    {
        for (int index = 0; index < m_MyHeroes.Count; ++index)
        {
            int capturedIndex = index;

            var elementHero = Instantiate(m_ElementHero, Trans_Content);
            elementHero.GetComponent<ElementHero>().SetHero(m_MyHeroes[index]);
            elementHero.GetComponent<ElementHero>().SetButton(() => OnClick_Hero(capturedIndex));

            if (m_MyHeroes[index].HeroName == DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.HeroName &&
                m_MyHeroes[index].Grade == DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Grade)
                elementHero.GetComponent<ElementHero>().SetSelect(true);
        }
    }

    public override void OnRefresh()
    {

    }
    #endregion

    #region Private Method
    #endregion

    #region Button
    private void OnClick_Hero(int num)
    {
        // 모든 버튼 초기화
        for (int index = 0; index < Trans_Content.childCount; ++index)
            Trans_Content.GetChild(index).GetComponent<ElementHero>().SetSelect(false);

        // 해당 버튼 활성화
        Trans_Content.GetChild(num).GetComponent<ElementHero>().SetSelect(true);

        // 장착 히어로 변경
        DataManager.Instance.GetMyUserData().UserHeroData.EquipHero = m_MyHeroes[num];

        // UI Refresh
        UIManager.Instance.Refresh();

        // 데이터 저장
        DataManager.Instance.SaveData();
    }

    private void OnClick_Close()
    {
        UIManager.Instance.Close<Popup_Hero>();
    }
    #endregion
}
