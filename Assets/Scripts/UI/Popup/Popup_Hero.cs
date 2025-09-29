using System;
using System.Collections.Generic;
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

        m_MyHeroes = DataManager.Instance.GetMyUserData().UserHeroData.MyHeroes;
        m_ElementHero = ResourceLoader.LoadAssetResources<GameObject>("Prefabs/Element/ElementHero");
    }

    public override void OnClose()
    {

    }

    public override void OnOpen(List<object> Args)
    {
        for (int Index = 0; Index < m_MyHeroes.Count; ++Index)
        {
            var elementHero = Instantiate(m_ElementHero, Trans_Content);
            elementHero.GetComponent<ElementHero>().SetHero(m_MyHeroes[Index]);
        }
    }

    public override void OnRefresh()
    {

    }
    #endregion

    #region Private Method
    #endregion

    #region Button
    private void OnClick_Character(int num)
    {
        
    }

    private void OnClick_Close()
    {
        UIManager.Instance.Close<Popup_Hero>();
    }
    #endregion
}
