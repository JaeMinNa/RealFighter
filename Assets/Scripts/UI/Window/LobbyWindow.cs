using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;

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
    [SerializeField] private Button Btn_Gold = null;
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
        Btn_Gold.onClick.AddListener(OnClcik_Gold);
    }

    public override void OnClose()
    {

    }

    public async override void OnOpen(List<object> Args)
    {
        SetTopUI();

        // 튜토리얼 시작
        if (DataManager.Instance.GetMyUserData().UserContentsData.TutorialIndex == 0)
        {
            //첫 로그인이라면 닉네임 설정 팝업 오픈
            if (DataManager.Instance.GetMyUserData().UserContentsData.IsFirstLogin)
                UIManager.Instance.Open<Popup_NickName>(UI.Popup, "Prefabs/UI/Popup/Popup_NickName");
            else
                await TutorialManager.Instance.StartTutorial(TutorialStep.LobbyChat_0);
        }
        else if (DataManager.Instance.GetMyUserData().UserContentsData.TutorialIndex == 2)
            await TutorialManager.Instance.StartTutorial(TutorialStep.LobbyChat_2);
        else if (DataManager.Instance.GetMyUserData().UserContentsData.TutorialIndex == 3)
            await TutorialManager.Instance.StartTutorial(TutorialStep.LobbyChat_4);
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

        // 오늘이 첫 로그인인지 확인
        if (Util.DateTimeNow.Date != userContentsData.LastLoginTime.Date)
        {
            // 하루 초기화
            userContentsData.IsGotFreeGold = false;
            userContentsData.AdGoldBuyCount = 0;
            userContentsData.TutorialIndex = 0;
        }

        userContentsData.LastLoginTime = Util.DateTimeNow;
    }
    #endregion

    #region Button
    public void OnClick_PVP()
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

    public void OnClick_Shop()
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

    private async void OnClcik_Gold()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        var popup = UIManager.Instance.Open<Popup_Shop>(UI.Popup, "Prefabs/UI/Popup/Popup_Shop");

        await UniTask.WaitUntil(() => popup != null);
        popup.OnClick_LeftBtn(1);
    }
    #endregion
}
