using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyWindow : UIElement
{
    #region Cashed Object
    [Header("Top")]
    [SerializeField] private TMP_Text Text_NickName = null;
    [SerializeField] private Image Img_Character = null;
    [SerializeField] private TMP_Text Text_Level = null;
    [SerializeField] private TMP_Text Text_Hero = null;
    [SerializeField] private TMP_Text Text_Gold = null;
    [SerializeField] private Button Btn_Setting = null;

    [Header("Contents")]
    [SerializeField] private Button Btn_PVP = null;
    [SerializeField] private Button Btn_Training = null;

    [Header("Bottom")]
    [SerializeField] private Button Btn_Hero = null;
    [SerializeField] private Button Btn_Shop = null;
    [SerializeField] private Button Btn_Ranking = null;
    #endregion

    #region Override Method
    public override void Init()
    {
        Btn_Hero.onClick.AddListener(OnClick_Hero);
        Btn_Shop.onClick.AddListener(OnClick_Shop);
        Btn_Ranking.onClick.AddListener(OnClick_Ranking);
        Btn_Setting.onClick.AddListener(OnClick_Setting);   
        Btn_PVP.onClick.AddListener(OnClick_PVP);
        Btn_Training.onClick.AddListener(OnClick_Training);
    }

    public override void OnClose()
    {
        
    }

    public override void OnOpen(List<object> Args)
    {
        SetTopUI();
    }

    public override void OnRefresh()
    {
        SetTopUI();
    }
    #endregion

    #region Private Method
    private void SetTopUI()
    {
        Text_NickName.text = DataManager.Instance.GetMyUserData().UserCommonData.NickName;
        Img_Character.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Character/Character_{DataManager.Instance.GetMyUserData().UserCommonData.Image}");
        Text_Level.text = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Level.ToString();
        Text_Hero.text = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.HeroName;
        Text_Gold.text = DataManager.Instance.GetMyUserData().UserCommonData.Gold.ToString();
    }
    #endregion

    #region Button
    private async void OnClick_PVP()
    {
        await ScenesManager.Instance.LoadScene("GameScene");
    }

    private void OnClick_Training()
    {

    }

    private void OnClick_Hero()
    {

    }

    private void OnClick_Shop()
    {

    }

    private void OnClick_Ranking()
    {

    }

    private void OnClick_Setting()
    {
        UIManager.Instance.Open<Popup_Setting>(UI.Popup, "Prefabs/UI/Popup/Popup_Setting");
    }
    #endregion
}
