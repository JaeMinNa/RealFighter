using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Gacha_Hero : UIElement
{
    #region Cahsed Object
    [SerializeField] private Transform Trans_Content = null;
    [SerializeField] private Button Btn_Ok = null;
    #endregion

    #region Member Property
    private List<HeroData> m_MyHeroes = new List<HeroData>();
    private GameObject m_ElementHero = null;
    private HeroData m_GachaHeroData = null;
    private int m_GachaGrade = -1;
    #endregion

    #region Override Method
    public override void Init()
    {
        Btn_Ok.onClick.AddListener(OnClick_Ok);

        m_ElementHero = ResourceLoader.LoadAssetResources<GameObject>("Prefabs/Element/ElementHero");
        m_MyHeroes = DataManager.Instance.GetMyUserData().UserHeroData.MyHeroes;
    }

    public override void OnClose()
    {

    }

    public override void OnOpen(List<object> Args)
    {
        if (Args.Count < 1)
            return;

        m_GachaGrade = (int)Args[0];

        GachaHero();

        if (m_GachaHeroData == null)
            return;

        var elementHero = Instantiate(m_ElementHero, Trans_Content);
        elementHero.GetComponent<ElementHero>().SetHero(m_GachaHeroData);
    }

    public override void OnRefresh()
    {

    }
    #endregion

    #region Private Method
    private void GachaHero()
    {
        // 히어로 랜덤 데이터 가져오기
        m_GachaHeroData = HeroUtil.GetRandomHeroDataByGrade(m_GachaGrade);

        // 중복 히어로를 이미 가지고 있다면
        bool isGet = false;
        for (int index = 0; index < m_MyHeroes.Count; ++index)
        {
            if (m_MyHeroes[index].HeroName == m_GachaHeroData.HeroName &&
                m_MyHeroes[index].Grade == m_GachaHeroData.Grade)
            {
                isGet = true;
                HeroUtil.AddHeroGradeExp(m_MyHeroes[index]);
                break;
            }
        }

        // 중복 히어로가 없다면
        if(!isGet)
        {
            m_MyHeroes.Add(m_GachaHeroData);
        }

        // 데이터 저장
        DataManager.Instance.SaveData();
    }
    #endregion

    #region Button
    private void OnClick_Ok()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Close<Popup_Gacha_Hero>();
    }
    #endregion
}
