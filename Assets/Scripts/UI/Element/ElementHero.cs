using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementHero : MonoBehaviour
{
    #region Cashed Object
    [SerializeField] private Slider Slider_Exp = null;
    [SerializeField] private Image Img_Element = null;
    [SerializeField] private Image Img_BackGlow = null;
    [SerializeField] private TMP_Text Text_Level = null;
    [SerializeField] private TMP_Text Text_HeroName = null;
    [SerializeField] private TMP_Text Text_Atk_High = null;
    [SerializeField] private TMP_Text Text_Atk_Mid = null;
    [SerializeField] private TMP_Text Text_Atk_Low = null;
    [SerializeField] private RawImage RawImg_Hero = null;
    [SerializeField] private Button Btn_Click = null;
    #endregion

    #region Member Property
    private Action m_Action = null;
    #endregion

    #region Public Method
    public void SetHero(HeroData data)
    {
        Initial();

        Slider_Exp.value = (float)data.Exp / (float)(data.Level * 10) * 100f;
        Text_Level.text = $"Lv.{data.Level}";
        Text_HeroName.text = data.HeroName.ToString();
        Text_Atk_High.text = $"HIGH : {DamageUtil.GetSkillDamage(data, 0)}";
        Text_Atk_Mid.text = $"MID : {DamageUtil.GetSkillDamage(data, 1)}";
        Text_Atk_Low.text = $"LOW : {DamageUtil.GetSkillDamage(data, 2)}";

        SetHeroImage(data);
    }

    public void SetSelect(bool isOn)
    {
        if (isOn)
        {
            Img_Element.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Element/Frame_ItemFrame06_s");
            Img_BackGlow.color = new Color(179f / 255f, 179f / 255f, 70f / 255f, 100f / 255f);
        }
        else
        {
            Img_Element.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Element/Frame_ItemFrame06_n");
            Img_BackGlow.color = new Color(0f / 255f, 134f / 255f, 255f / 255f, 100f / 255f);
        }
    }

    public void SetButton(Action action)
    {
        m_Action = action;

        if (m_Action != null)
            Btn_Click.onClick.AddListener(() => m_Action());
    }
    #endregion

    #region Private Method
    private void Initial()
    {
        SetSelect(false);
    }

    private void SetHeroImage(HeroData data)
    {
        if (data.HeroName == "BLAZE")
            RawImg_Hero.uvRect = new Rect(0.03f, 0.5f, 0.07f, 0.06f);
        else if (data.HeroName == "DOMINICK")
            RawImg_Hero.uvRect = new Rect(0.119f, 0.5f, 0.07f, 0.06f);
        else if (data.HeroName == "DRAKE")
            RawImg_Hero.uvRect = new Rect(0.205f, 0.5f, 0.07f, 0.06f);
        else if (data.HeroName == "IRIS")
            RawImg_Hero.uvRect = new Rect(0.292f, 0.5f, 0.07f, 0.06f);
        else if (data.HeroName == "JIN")
            RawImg_Hero.uvRect = new Rect(0.379f, 0.5f, 0.07f, 0.06f);
        else if (data.HeroName == "MAVERICK")
            RawImg_Hero.uvRect = new Rect(0.465f, 0.5f, 0.07f,
                0.06f);
        else if (data.HeroName == "ORIANNA")
            RawImg_Hero.uvRect = new Rect(0.55f, 0.5f, 0.07f, 0.06f);
        else if (data.HeroName == "REX")
            RawImg_Hero.uvRect = new Rect(0.639f, 0.5f, 0.07f, 0.06f);
        else if (data.HeroName == "SERENA")
            RawImg_Hero.uvRect = new Rect(0.725f, 0.5f, 0.07f, 0.06f);
        else if (data.HeroName == "STEELTON")
            RawImg_Hero.uvRect = new Rect(0.811f, 0.5f, 0.07f, 0.06f);
        else
            RawImg_Hero.uvRect = Rect.zero;
    }
    #endregion
}
