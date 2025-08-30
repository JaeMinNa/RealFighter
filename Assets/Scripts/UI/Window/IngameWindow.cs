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

    [Header("SkillInfo_My_Attack")]
    [SerializeField] private GameObject Obj_AttackPanel = null;
    [SerializeField] private TMP_Text Text_MyATK_0 = null;
    [SerializeField] private TMP_Text Text_MyCount_0 = null;
    [SerializeField] private TMP_Text Text_MyATK_1 = null;
    [SerializeField] private TMP_Text Text_MyCount_1 = null;
    [SerializeField] private TMP_Text Text_MyATK_2 = null;
    [SerializeField] private TMP_Text Text_MyCount_2 = null;

    [Header("SkillInfo_My_Defence")]
    [SerializeField] private GameObject Obj_DefencePanel = null;

    [Header("SkillInfo_Enemy")]
    [SerializeField] private TMP_Text Text_EnemyATK_0 = null;
    [SerializeField] private TMP_Text Text_EnemyCount_0 = null;
    [SerializeField] private TMP_Text Text_EnemyATK_1 = null;
    [SerializeField] private TMP_Text Text_EnemyCount_1 = null;
    [SerializeField] private TMP_Text Text_EnemyATK_2 = null;
    [SerializeField] private TMP_Text Text_EnemyCount_2 = null;
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
        SetUI_Skill();
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

    public void SetUI_Skill()
    {
        Obj_AttackPanel.SetActive(false);
        Obj_DefencePanel.SetActive(false);

        if(m_PVPModule.IsAttackTurn)
        {
            // My
            Text_MyATK_0.text = $"ATK : {DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.SkillDamage_0}";
            Text_MyCount_0.text = $"{m_PVPModule.MyCanUseSkillCount_0} / {ClientDef.SkillMaxCount}";
            Text_MyATK_1.text = $"ATK : {DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.SkillDamage_1}";
            Text_MyCount_1.text = $"{m_PVPModule.MyCanUseSkillCount_1} / {ClientDef.SkillMaxCount}";
            Text_MyATK_2.text = $"ATK : {DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.SkillDamage_2}";
            Text_MyCount_2.text = $"{m_PVPModule.MyCanUseSkillCount_2} / {ClientDef.SkillMaxCount}";

            Obj_AttackPanel.SetActive(true);
        }
        else
        {
            Obj_DefencePanel.SetActive(true);
        }

        // Enemy
        Text_EnemyATK_0.text = $"ATK : {m_PVPModule.EnemyUserData.UserHeroData.EquipHero.SkillDamage_0}";
        Text_EnemyCount_0.text = $"{m_PVPModule.EnemyCanUseSkillCount_0} / {ClientDef.SkillMaxCount}";
        Text_EnemyATK_1.text = $"ATK : {m_PVPModule.EnemyUserData.UserHeroData.EquipHero.SkillDamage_1}";
        Text_EnemyCount_1.text = $"{m_PVPModule.EnemyCanUseSkillCount_1} / {ClientDef.SkillMaxCount}";
        Text_EnemyATK_2.text = $"ATK : {m_PVPModule.EnemyUserData.UserHeroData.EquipHero.SkillDamage_2}";
        Text_EnemyCount_2.text = $"{m_PVPModule.EnemyCanUseSkillCount_2} / {ClientDef.SkillMaxCount}";
    }
    #endregion
}
