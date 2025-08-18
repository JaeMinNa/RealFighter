using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IngameWindow : UIElement
{
    #region Cashed Object
    [Header("Top")]
    [SerializeField] private TMP_Text Text_Time = null;
    [SerializeField] private TMP_Text Text_Round = null;

    [Header("Player_Left")]
    [SerializeField] private TMP_Text Text_NickName_Left = null;
    [SerializeField] private TMP_Text Text_Score_Left = null;
    [SerializeField] private TMP_Text Text_Hp_Left = null;
    [SerializeField] private Slider Slider_Hp_Left = null;
    [SerializeField] private Image Img_Hero_Left = null;
    [SerializeField] private TMP_Text Text_Level_Left = null;
    [SerializeField] private TMP_Text Text_Hero_Left = null;

    [Header("Player_Right")]
    [SerializeField] private TMP_Text Text_NickName_Right = null;
    [SerializeField] private TMP_Text Text_Score_Right = null;
    [SerializeField] private TMP_Text Text_Hp_Right = null;
    [SerializeField] private Slider Slider_Hp_Right = null;
    [SerializeField] private Image Img_Hero_Right = null;
    [SerializeField] private TMP_Text Text_Level_Right = null;
    [SerializeField] private TMP_Text Text_Hero_Right = null;
    #endregion

    #region Member Property
    private PVPModule m_PVPModule = null;
    //private bool m_IsLeftPlayer;
    #endregion

    #region Unity Method
    private void Update()
    {
        if (m_PVPModule == null)
            return;

        Text_Time.text = TextUtil.ConvertTime(m_PVPModule.CurTime);
    }
    #endregion

    #region Override Method
    public override void Init()
    {
        if (m_PVPModule == null)
            m_PVPModule = BattleModule.Instance as PVPModule;
    }

    public override void OnClose()
    {
                
    }

    public override void OnOpen(List<object> Args)
    {
        SetUI_Player();
        SetUI_Top();
    }

    public override void OnRefresh()
    {
        //SetUI_Player();
        //SetUI_Top();
    }
    #endregion

    #region Public Method
    public void SetUI_Top()
    {
        Text_Round.text = $"Round {m_PVPModule.CurRound}";
    }
    #endregion

    #region Private Method
    private void SetUI_Player()
    {
        if(m_PVPModule.IsLeftPlayer)
        {
            SetUI_Player_Left(DataManager.Instance.GetMyUserData());
            SetUI_Player_Right(m_PVPModule.EnemyUserData);
        }
        else
        {
            SetUI_Player_Left(m_PVPModule.EnemyUserData);
            SetUI_Player_Right(DataManager.Instance.GetMyUserData());
        }
    }

    private void SetUI_Player_Left(UserData userData)
    {
        Text_NickName_Left.text = userData.UserCommonData.NickName;
        Text_Score_Left.text = userData.UserCommonData.Score.ToString();
        Text_Hp_Left.text = $"{(m_PVPModule.IsLeftPlayer ? m_PVPModule.CurHp : m_PVPModule.EnemyCurHp)} <#afd9e9>/ {100}";
        Slider_Hp_Left.value = m_PVPModule.IsLeftPlayer ? m_PVPModule.CurHp : m_PVPModule.EnemyCurHp;
        Img_Hero_Left.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Character/Character_{userData.UserCommonData.Image}");
        Text_Level_Left.text = userData.UserHeroData.EquipHero.Level.ToString();
        Text_Hero_Left.text = userData.UserHeroData.EquipHero.HeroName;
    }

    private void SetUI_Player_Right(UserData userData)
    {
        Text_NickName_Right.text = userData.UserCommonData.NickName;
        Text_Score_Right.text = userData.UserCommonData.Score.ToString();
        Text_Hp_Right.text = $"{(!m_PVPModule.IsLeftPlayer ? m_PVPModule.CurHp : m_PVPModule.EnemyCurHp)} <#ffc9d6>/ {100}";
        Slider_Hp_Right.value = !m_PVPModule.IsLeftPlayer ? m_PVPModule.CurHp : m_PVPModule.EnemyCurHp;
        Img_Hero_Right.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Character/Character_{userData.UserCommonData.Image}");
        Text_Level_Right.text = userData.UserHeroData.EquipHero.Level.ToString();
        Text_Hero_Right.text = userData.UserHeroData.EquipHero.HeroName;
    }
    #endregion
}
