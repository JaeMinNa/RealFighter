using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Training : UIElement
{
    #region Cahsed Object
    [SerializeField] private Slider[] Slider_Skillproficiencies = null;
    [SerializeField] private TMP_Text[] Text_SkillAtks = null;
    [SerializeField] private Button[] Btn_Attacks = null;
    [SerializeField] private TMP_Text Text_Attack = null;
    [SerializeField] private Button Btn_Close = null;
    #endregion

    #region Member Property
    private HeroData m_EquipHeroData = null;
    private Animator m_EnemeyAnimator = null;
    private float m_AttackUITime = 0.5f;
    private int m_Combo = 0;
    private int m_CurSkillNum = -1;
    private Vector3 m_OriginPos_AttackText = Vector3.zero;
    #endregion

    #region Unity Method
    private void Update()
    {
        if (Text_Attack.gameObject.activeSelf && m_AttackUITime > 0)
        {
            m_AttackUITime -= Time.deltaTime;
        }

        if (Text_Attack.gameObject.activeSelf && m_AttackUITime <= 0)
        {
            m_Combo = 0;
            Text_Attack.gameObject.SetActive(false);
            Text_Attack.GetComponent<RectTransform>().anchoredPosition = m_OriginPos_AttackText;
        }
    }
    #endregion

    #region Override Method
    public override void Init()
    {
        m_EquipHeroData = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero;
        m_EnemeyAnimator = GameObject.Find("BLAZE_Ingame").GetComponent<Animator>();
        m_OriginPos_AttackText = Text_Attack.GetComponent<RectTransform>().anchoredPosition;

        Btn_Close.onClick.AddListener(OnClick_Close);

        for (int index = 0; index < Btn_Attacks.Length; ++index)
        {
            int capturedIndex = index;
            Btn_Attacks[index].onClick.AddListener(() => OnClick_Attack(capturedIndex));
        }
    }

    public override void OnClose()
    {

    }

    public override void OnOpen(List<object> Args)
    {
        SetLeftUI();
        Text_Attack.gameObject.SetActive(false);
    }

    public override void OnRefresh()
    {

    }
    #endregion

    #region Private Method
    private void SetLeftUI()
    {
        // 숙련도 Slider
        for (int index = 0; index < Slider_Skillproficiencies.Length; ++index)
        {
            int value = m_EquipHeroData.Skillproficiencies[index] % 100;
            if (value == 0 && m_EquipHeroData.Skillproficiencies[index] != 0)
                value = 100;

            if (m_EquipHeroData.Skillproficiencies[index] > 1000)
                value = 100;

            Slider_Skillproficiencies[index].value = value;
        }

        // Atk
        Text_SkillAtks[0].text = $"HIGH : {DamageUtil.GetSkillDamage(m_EquipHeroData, 0)}";
        Text_SkillAtks[1].text = $"MID : {DamageUtil.GetSkillDamage(m_EquipHeroData, 1)}";
        Text_SkillAtks[2].text = $"LOW   : {DamageUtil.GetSkillDamage(m_EquipHeroData, 2)}";
    }

    private void SetAttackUI(int num)
    {
        m_AttackUITime = 0.5f;

        if (num == 0)
            Text_Attack.text = $"HIGH\n+{m_Combo}";
        else if (num == 1)
            Text_Attack.text = $"MID\n+{m_Combo}";
        else if (num == 2)
            Text_Attack.text = $"LOW\n+{m_Combo}";

        Text_Attack.gameObject.SetActive(true);
    }
    #endregion

    #region Button
    private void OnClick_Attack(int num)
    {
        SoundManager.Instance.StartSFX_Punch();

        // 애니메이션 실행
        m_EnemeyAnimator.SetTrigger("Hit");

        // 콤보 증가
        if (m_CurSkillNum == num || m_Combo == 0)
            m_Combo++;
        else
        {
            m_Combo = 0;
            m_Combo++;
        }

        if(m_Combo != 0)
        {
            Text_Attack.gameObject.SetActive(false);
            Text_Attack.GetComponent<RectTransform>().anchoredPosition = m_OriginPos_AttackText;
        }

        m_CurSkillNum = num;

        // 숙련도 증가
        m_EquipHeroData.Skillproficiencies[num]++;

        // 데이터 저장
        DataManager.Instance.SaveData();

        // UI 갱신
        SetLeftUI();
        SetAttackUI(num);
    }

    private void OnClick_Close()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Close<Popup_Training>();
    }
    #endregion
}
