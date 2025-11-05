using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LobbyWindow : UIElement
{
    #region Cashed Object
    [Header("Top")]
    [SerializeField] private Button Btn_Character = null;
    [SerializeField] private TMP_Text Text_NickName = null;
    [SerializeField] private TMP_Text Text_Score = null;
    [SerializeField] private Image Img_Character = null;
    [SerializeField] private Slider Slider_Exp = null;
    [SerializeField] private TMP_Text Text_Level = null;
    [SerializeField] private Image Img_Grade = null;
    [SerializeField] private TMP_Text Text_Grade = null;
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
        DailyInit();

        Btn_Character.onClick.AddListener(OnClick_Character);
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
        // 첫 로그인이라면, 닉네임 설정
        if (DataManager.Instance.GetMyUserData().UserContentsData.IsFirstLogin)
        {
            UIManager.Instance.Open<Popup_NickName>(UI.Popup, "Prefabs/UI/Popup/Popup_NickName");
        }

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
        Text_Score.text = DataManager.Instance.GetMyUserData().UserCommonData?.RankPoint.ToString();
        Slider_Exp.value = (float)DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Exp / (float)(DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Level * 10) * 100f;
        Img_Character.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Character/Character_{DataManager.Instance.GetMyUserData().UserCommonData.Image}");
        Text_Level.text = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Level.ToString();
        Text_Hero.text = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.HeroName;
        Text_Gold.text = DataManager.Instance.GetMyUserData().UserCommonData.Gold.ToString();

        SetGrade();
    }

    private void SetGrade()
    {
        HeroData data = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero;

        if (data.Grade == 0)
        {
            Text_Grade.text = "NORMAL";
            Text_Grade.color = new Color32(176, 176, 176, 255);
            Img_Grade.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Label/Label_Label01_Gray");
        }
        else if (data.Grade == 1)
        {
            Text_Grade.text = "RARE";
            Text_Grade.color = new Color32(77, 163, 255, 255);
            Img_Grade.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Label/Label_Label01_Blue");
        }
        else if (data.Grade == 2)
        {
            Text_Grade.text = "EPIC";
            Text_Grade.color = new Color32(195, 107, 255, 255);
            Img_Grade.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Label/Label_Label01_Purple");
        }
        else
        {
            Text_Grade.text = "UNIQUE";
            Text_Grade.color = new Color32(229, 245, 84, 89);
            Img_Grade.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Label/Label_Label01_Yellow");
        }
    }

    private void DailyInit()
    {
        var userContentsData = DataManager.Instance.GetMyUserData().UserContentsData;
        if (userContentsData == null)
            return;

        // 오늘 첫 로그인
        if (Util.DateTimeNow.Date != userContentsData.LastLoginTime.Date)
        {
            // 일일 초기화
            userContentsData.IsGotFreeGold = false;
        }
        else
        {

        }

        userContentsData.LastLoginTime = Util.DateTimeNow;
    }
    #endregion

    #region Button
    private void OnClick_PVP()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Open<Popup_BattleLoading>(UI.Popup, "Prefabs/UI/Popup/Popup_BattleLoading");
    }

    private void OnClick_Training()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Open<Popup_Training>(UI.Popup, "Prefabs/UI/Popup/Popup_Training");
    }

    private void OnClick_Hero()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Open<Popup_Hero>(UI.Popup, "Prefabs/UI/Popup/Popup_Hero");
    }

    private void OnClick_Shop()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Open<Popup_Shop>(UI.Popup, "Prefabs/UI/Popup/Popup_Shop");
    }

    private void OnClick_Ranking()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Open<Popup_Rank>(UI.Popup, "Prefabs/UI/Popup/Popup_Rank");
    }

    private void OnClick_Setting()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Open<Popup_Setting>(UI.Popup, "Prefabs/UI/Popup/Popup_Setting");
    }

    private void OnClick_Character()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Open<Popup_SelectCharacter>(UI.Popup, "Prefabs/UI/Popup/Popup_SelectCharacter");
    }
    #endregion
}
