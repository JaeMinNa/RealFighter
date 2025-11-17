using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Assertions.Must;
using Cysharp.Threading.Tasks.Triggers;
using Cysharp.Threading.Tasks;
using Photon.Pun;

public class IngameWindow : UIElement
{
    #region Cashed Object
    [Header("Top")]
    [SerializeField] private TMP_Text Text_Time = null;
    [SerializeField] private TMP_Text Text_Round = null;
    [SerializeField] private GameObject Obj_AtkTurn = null;
    [SerializeField] private GameObject Obj_DefTurn = null;

    [Header("Under")]
    [SerializeField] private Button Btn_Ready = null;
    [SerializeField] private Button Btn_Exit = null;
    [SerializeField] private Button[] Btn_Emoticons = null;

    [Header("Player_Left")]
    [SerializeField] private TMP_Text Text_NickName_Left = null;
    [SerializeField] private TMP_Text Text_Score_Left = null;
    [SerializeField] private TMP_Text Text_Hp_Left = null;
    [SerializeField] private Slider Slider_Hp_Left = null;
    [SerializeField] private Image Img_Hero_Left = null;
    [SerializeField] private TMP_Text Text_Level_Left = null;
    [SerializeField] private TMP_Text Text_Hero_Left = null;
    [SerializeField] private Image Img_Emoticon_Left = null;

    [Header("Player_Right")]
    [SerializeField] private TMP_Text Text_NickName_Right = null;
    [SerializeField] private TMP_Text Text_Score_Right = null;
    [SerializeField] private TMP_Text Text_Hp_Right = null;
    [SerializeField] private Slider Slider_Hp_Right = null;
    [SerializeField] private Image Img_Hero_Right = null;
    [SerializeField] private TMP_Text Text_Level_Right = null;
    [SerializeField] private TMP_Text Text_Hero_Right = null;
    [SerializeField] private Image Img_Emoticon_Right = null;

    [Header("SkillInfo")]
    [SerializeField] private GameObject Obj_MySkillInfoPanel = null;
    [SerializeField] private GameObject Obj_EnemySkillInfoPanel = null;

    [Header("SkillInfo_My_Attack")]
    [SerializeField] private GameObject Obj_AttackPanel = null;
    [SerializeField] private TMP_Text Text_MyATK_0 = null;
    [SerializeField] private TMP_Text Text_MyCount_0 = null;
    [SerializeField] private TMP_Text Text_MyATK_1 = null;
    [SerializeField] private TMP_Text Text_MyCount_1 = null;
    [SerializeField] private TMP_Text Text_MyATK_2 = null;
    [SerializeField] private TMP_Text Text_MyCount_2 = null;
    [SerializeField] private Button[] Btn_MyAttacks = null;
    [SerializeField] private TMP_Text Text_MyCombo = null;
    [SerializeField] private GameObject Obj_MyCritical = null;

    [Header("SkillInfo_My_Defence")]
    [SerializeField] private GameObject Obj_DefencePanel = null;
    [SerializeField] private Button[] Btn_MyDefences = null;

    [Header("SkillInfo_Enemy")]
    [SerializeField] private TMP_Text Text_EnemyATK_0 = null;
    [SerializeField] private TMP_Text Text_EnemyCount_0 = null;
    [SerializeField] private TMP_Text Text_EnemyATK_1 = null;
    [SerializeField] private TMP_Text Text_EnemyCount_1 = null;
    [SerializeField] private TMP_Text Text_EnemyATK_2 = null;
    [SerializeField] private TMP_Text Text_EnemyCount_2 = null;
    [SerializeField] private GameObject[] Obj_EnemyDangerIcons = null;
    [SerializeField] private TMP_Text Text_EnemyCombo = null;
    [SerializeField] private GameObject Obj_EnemyCritical = null;

    [Header("Images")]
    [SerializeField] private Image SkillImage = null;
    #endregion

    #region Member Property
    private PVPModule m_PVPModule = null;
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

        for (int index = 0; index < Btn_MyAttacks.Length; ++index)
        {
            int capturedIndex = index;
            Btn_MyAttacks[index].onClick.AddListener(() => OnClick_MyAttacks(capturedIndex));
        }

        for (int index = 0; index < Btn_MyDefences.Length; ++index)
        {
            int capturedIndex = index;
            Btn_MyDefences[index].onClick.AddListener(() => OnClick_MyDefences(capturedIndex));
        }

        for (int index = 0; index < Btn_Emoticons.Length; ++index)
        {
            int capturedIndex = index;
            Btn_Emoticons[index].onClick.AddListener(() => OnClick_Emoticon(capturedIndex));
        }

        Btn_Exit.onClick.AddListener(OnClick_Exit);
        Btn_Ready.onClick.AddListener(OnClick_Ready);
    }

    public override void OnClose()
    {

    }

    public override void OnOpen(List<object> Args)
    {
        SetUI_Players();
        SetUI_Skill();
        SetUI_Top();
        SetUI_DangerIcon();

        SkillImage.gameObject.SetActive(false);
        Img_Emoticon_Left.gameObject.SetActive(false);
        Img_Emoticon_Right.gameObject.SetActive(false);
    }

    public override void OnRefresh()
    {
        SetUI_Players();
        SetUI_Top();
        SetUI_Skill();
    }
    #endregion

    #region Public Method

    #region UI
    public void SetUI_Top()
    {
        Obj_AtkTurn.SetActive(false);
        Obj_DefTurn.SetActive(false);

        Text_Round.text = $"Round {m_PVPModule.CurRound}";

        if (m_PVPModule.IsAttackTurn)
            Obj_AtkTurn.SetActive(true);
        else
            Obj_DefTurn.SetActive(true);
    }

    public void SetUI_Players()
    {
        if (m_PVPModule.IsLeftPlayer)
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

    public void SetUI_Skill()
    {
        Obj_AttackPanel.SetActive(false);
        Obj_DefencePanel.SetActive(false);
        Text_MyCombo.transform.gameObject.SetActive(false);
        Text_EnemyCombo.transform.gameObject.SetActive(false);
        Obj_MyCritical.SetActive(false);
        Obj_EnemyCritical.SetActive(false);

        // UI 위치 설정
        if (m_PVPModule.IsLeftPlayer)
        {
            Obj_MySkillInfoPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 0.5f);
            Obj_MySkillInfoPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.5f);
            Obj_MySkillInfoPanel.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
            Obj_MySkillInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(-630f, -180f, 0f);
            Obj_EnemySkillInfoPanel.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 0.5f);
            Obj_EnemySkillInfoPanel.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 0.5f);
            Obj_EnemySkillInfoPanel.GetComponent<RectTransform>().pivot = new Vector2(1f, 0.5f);
            Obj_EnemySkillInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(600f, 170, 0f);
        }
        else
        {
            Obj_MySkillInfoPanel.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 0.5f);
            Obj_MySkillInfoPanel.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 0.5f);
            Obj_MySkillInfoPanel.GetComponent<RectTransform>().pivot = new Vector2(1f, 0.5f);
            Obj_MySkillInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(650f, -20f, 0f);
            Obj_EnemySkillInfoPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 0.5f);
            Obj_EnemySkillInfoPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.5f);
            Obj_EnemySkillInfoPanel.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
            Obj_EnemySkillInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(-600f, 170f, 0f);
        }

        // 사용 가능한 내 공격 스킬 버튼만 활성화
        for (int Index = 0; Index < Btn_MyAttacks.Length; ++Index)
        {
            if (m_PVPModule.MyCanUseSkillCounts[Index] > 0)
                Btn_MyAttacks[Index].interactable = true;
            else
                Btn_MyAttacks[Index].interactable = false;
        }

        for (int Index = 0; Index < Btn_MyDefences.Length; ++Index)
        {
            Btn_MyDefences[Index].interactable = true;
        }

        Btn_Ready.interactable = true;

        // 내 공격/방어 버튼들을 기본 이미지로 초기화
        foreach (var btn in Btn_MyAttacks)
            btn.image.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Button/Btn_MenuButton_Square01_n");

        foreach (var btn in Btn_MyDefences)
            btn.image.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Button/Btn_MenuButton_Square01_n");

        // My
        if (m_PVPModule.IsAttackTurn)
        {
            Text_MyATK_0.text = $"ATK : {DamageUtil.GetSkillDamage(DataManager.Instance.GetMyUserData().UserHeroData.EquipHero, 0)}";
            Text_MyCount_0.text = $"{m_PVPModule.MyCanUseSkillCounts[0]} / {ClientDef.SkillMaxCount}";
            Text_MyATK_1.text = $"ATK : {DamageUtil.GetSkillDamage(DataManager.Instance.GetMyUserData().UserHeroData.EquipHero, 1)}";
            Text_MyCount_1.text = $"{m_PVPModule.MyCanUseSkillCounts[1]} / {ClientDef.SkillMaxCount}";
            Text_MyATK_2.text = $"ATK : {DamageUtil.GetSkillDamage(DataManager.Instance.GetMyUserData().UserHeroData.EquipHero, 2)}";
            Text_MyCount_2.text = $"{m_PVPModule.MyCanUseSkillCounts[2]} / {ClientDef.SkillMaxCount}";

            Obj_AttackPanel.SetActive(true);
        }
        else
        {
            Obj_DefencePanel.SetActive(true);
        }

        SetUI_MyCombo();

        // Enemy
        Text_EnemyATK_0.text = $"ATK : {DamageUtil.GetSkillDamage(m_PVPModule.EnemyUserData.UserHeroData.EquipHero, 0)}";
        Text_EnemyCount_0.text = $"{m_PVPModule.EnemyCanUseSkillCounts[0]} / {ClientDef.SkillMaxCount}";
        Text_EnemyATK_1.text = $"ATK : {DamageUtil.GetSkillDamage(m_PVPModule.EnemyUserData.UserHeroData.EquipHero, 1)}";
        Text_EnemyCount_1.text = $"{m_PVPModule.EnemyCanUseSkillCounts[1]} / {ClientDef.SkillMaxCount}";
        Text_EnemyATK_2.text = $"ATK : {DamageUtil.GetSkillDamage(m_PVPModule.EnemyUserData.UserHeroData.EquipHero, 2)}";
        Text_EnemyCount_2.text = $"{m_PVPModule.EnemyCanUseSkillCounts[2]} / {ClientDef.SkillMaxCount}";

        SetUI_EnemyCombo();
    }

    public async UniTask ShowSkillImage(string heroName, float time)
    {
        SkillImage.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Hero/{heroName}");

        if (SkillImage.sprite == null)
            return;

        SoundManager.Instance.StartSFX("StartSkill");

        SkillImage.gameObject.SetActive(true);

        await UniTask.Delay((int)(time * 1000));

        SkillImage.gameObject.SetActive(false);
    }

    public async void SetEmoticon(bool isLeft, int num)
    {
        SoundManager.Instance.StartSFX("ButtonEmoticon");

        if (isLeft)
        {
            Img_Emoticon_Left.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Emoticon/Emoticon_{num}");
            Img_Emoticon_Left.gameObject.SetActive(true);
            await UniTask.Delay(1500);
            Img_Emoticon_Left.gameObject.SetActive(false);
        }
        else
        {
            Img_Emoticon_Right.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Emoticon/Emoticon_{num}");
            Img_Emoticon_Right.gameObject.SetActive(true);
            await UniTask.Delay(1500);
            Img_Emoticon_Right.gameObject.SetActive(false);
        }
    }
    #endregion

    #endregion

    #region Private Method

    #region UI
    private void SetUI_Player_Left(UserData userData)
    {
        Text_NickName_Left.text = userData.UserCommonData.NickName;
        Text_Score_Left.text = userData.UserCommonData.RankPoint.ToString();
        Text_Hp_Left.text = $"{(m_PVPModule.IsLeftPlayer ? (m_PVPModule.CurHp < 0 ? 0 : m_PVPModule.CurHp) : (m_PVPModule.EnemyCurHp < 0 ? 0 : m_PVPModule.EnemyCurHp))} <#afd9e9>/ {100}";
        Slider_Hp_Left.value = m_PVPModule.IsLeftPlayer ? m_PVPModule.CurHp : m_PVPModule.EnemyCurHp;
        Img_Hero_Left.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Character/Character_{userData.UserCommonData.Image}");
        Text_Level_Left.text = userData.UserHeroData.EquipHero.Level.ToString();
        Text_Hero_Left.text = userData.UserHeroData.EquipHero.HeroName;
    }

    private void SetUI_Player_Right(UserData userData)
    {
        Text_NickName_Right.text = userData.UserCommonData.NickName;
        Text_Score_Right.text = userData.UserCommonData.RankPoint.ToString();
        Text_Hp_Right.text = $"{(!m_PVPModule.IsLeftPlayer ? (m_PVPModule.CurHp < 0 ? 0 : m_PVPModule.CurHp) : (m_PVPModule.EnemyCurHp < 0 ? 0 : m_PVPModule.EnemyCurHp))} <#ffc9d6>/ {100}";
        Slider_Hp_Right.value = !m_PVPModule.IsLeftPlayer ? m_PVPModule.CurHp : m_PVPModule.EnemyCurHp;
        Img_Hero_Right.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Character/Character_{userData.UserCommonData.Image}");
        Text_Level_Right.text = userData.UserHeroData.EquipHero.Level.ToString();
        Text_Hero_Right.text = userData.UserHeroData.EquipHero.HeroName;
    }

    private void SetUI_DangerIcon()
    {
        List<int> damages = new List<int>() {DamageUtil.GetSkillDamage(m_PVPModule.EnemyUserData.UserHeroData.EquipHero, 0),
                                             DamageUtil.GetSkillDamage(m_PVPModule.EnemyUserData.UserHeroData.EquipHero, 1),
                                             DamageUtil.GetSkillDamage(m_PVPModule.EnemyUserData.UserHeroData.EquipHero, 2)};

        int maxValue = damages.Max();

        List<int> maxIndices = damages
            .Select((value, index) => new { value, index })
            .Where(x => x.value == maxValue)
            .Select(x => x.index)
            .ToList();

        int chosenIndex = maxIndices[RandomUtil.GetRandomIndex(0, maxIndices.Count - 1)];

        foreach (var icon in Obj_EnemyDangerIcons)
            icon.SetActive(false);

        Obj_EnemyDangerIcons[chosenIndex].SetActive(true);
    }

    private void SetUI_MyCombo()
    {
        if (m_PVPModule.MyCombo != 0)
        {
            if (m_PVPModule.MyCombo == 3)
                Obj_MyCritical.SetActive(true);
            else
            {
                Text_MyCombo.text = m_PVPModule.MyCombo.ToString();
                Text_MyCombo.transform.gameObject.SetActive(true);
            }
        }
    }

    private void SetUI_EnemyCombo()
    {
        if (m_PVPModule.EnemyCombo != 0)
        {
            if (m_PVPModule.EnemyCombo == 3)
                Obj_EnemyCritical.SetActive(true);
            else
            {
                Text_EnemyCombo.text = m_PVPModule.EnemyCombo.ToString();
                Text_EnemyCombo.transform.gameObject.SetActive(true);
            }
        }
    }
    #endregion

    #endregion

    #region Button
    public void OnClick_MyAttacks(int num)
    {
        if (m_PVPModule.IsMyReady)
            return;

        SoundManager.Instance.StartSFX("ButtonClick");

        // 내 공격 버튼들을 기본 이미지로 초기화
        foreach (var btn in Btn_MyAttacks)
            btn.image.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Button/Btn_MenuButton_Square01_n");

        // Select Image
        Btn_MyAttacks[num].image.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Button/Btn_MenuButton_Square01_s");

        m_PVPModule.MySelectBtnNum = num;
    }

    public void OnClick_MyDefences(int num)
    {
        if (m_PVPModule.IsMyReady)
            return;

        SoundManager.Instance.StartSFX("ButtonClick");

        // 내 방어 버튼들을 기본 이미지로 초기화
        foreach (var btn in Btn_MyDefences)
            btn.image.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Button/Btn_MenuButton_Square01_n");

        // Select Image
        Btn_MyDefences[num].image.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Button/Btn_MenuButton_Square01_s");

        m_PVPModule.MySelectBtnNum = num;
    }

    private void OnClick_Exit()
    {
        m_PVPModule.IsStartGame = false;
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkCancel,
            Message = "패널티를 받고 정말 나가시겠습니까?",
            OkAction = async () =>
            {
                if (PhotonNetwork.IsConnected)
                    PhotonManager.Instance.Disconnect(null);

                if (DataManager.Instance.GetMyUserData().UserCommonData.RankPoint > 0)
                    DataManager.Instance.GetMyUserData().UserCommonData.RankPoint--;

                BackendManager.Instance.SaveData();

                await ScenesManager.Instance.LoadScene("LobbyScene");
            }
        });
    }

    public void OnClick_Ready()
    {
        if (m_PVPModule.IsMyReady)
            return;

        if (m_PVPModule.MySelectBtnNum == -1)
            return;

        SoundManager.Instance.StartSFX("ButtonClick");

        m_PVPModule.IsMyReady = true;
        Btn_Ready.image.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Button/Btn_TextButton_Square01_Gray");
        Btn_Ready.interactable = false;

        foreach (var btn in Btn_MyAttacks)
            btn.interactable = false;

        foreach (var btn in Btn_MyDefences)
            btn.interactable = false;
    }

    private void OnClick_Emoticon(int num)
    {
        if (!PhotonNetwork.IsConnected)
        {
            if (m_PVPModule.IsLeftPlayer)
            {
                if (Img_Emoticon_Left.gameObject.activeSelf)
                    return;

                SetEmoticon(true, num);
            }
            else
            {
                if (Img_Emoticon_Right.gameObject.activeSelf)
                    return;

                SetEmoticon(false, num);
            }
        }
        else
        {
            if (m_PVPModule.IsLeftPlayer)
            {
                if (Img_Emoticon_Left.gameObject.activeSelf)
                    return;

                m_PVPModule.PhotonController_My.PhotonView.RPC("RPCPlayEmoticon", RpcTarget.All, true, num);
            }
            else
            {
                if (Img_Emoticon_Right.gameObject.activeSelf)
                    return;

                m_PVPModule.PhotonController_My.PhotonView.RPC("RPCPlayEmoticon", RpcTarget.All, false, num);
            }
        }
    }
    #endregion
}
