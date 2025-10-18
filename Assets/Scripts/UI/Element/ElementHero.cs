using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementHero : MonoBehaviour
{
    #region Cashed Object
    [SerializeField] private Slider Slider_Exp = null;
    [SerializeField] private Slider Slider_GradeExp = null;
    [SerializeField] private Image Img_Element = null;
    [SerializeField] private Image Img_BackGlow = null;
    [SerializeField] private TMP_Text Text_Level = null;
    [SerializeField] private TMP_Text Text_HeroName = null;
    [SerializeField] private Image Img_Grade = null;
    [SerializeField] private TMP_Text Text_Grade = null;
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
        Slider_GradeExp.value = (float)data.GradeExp / (float)ClientDef.MaxGradeExp * 100f;
        Text_Level.text = $"Lv.{data.Level}";
        Text_HeroName.text = data.HeroName.ToString();
        Text_Atk_High.text = $"HIGH : {DamageUtil.GetSkillDamage(data, 0)}";
        Text_Atk_Mid.text = $"MID : {DamageUtil.GetSkillDamage(data, 1)}";
        Text_Atk_Low.text = $"LOW : {DamageUtil.GetSkillDamage(data, 2)}";

        SetHeroImage(data);
        SetGrade(data);
    }

    public void SetSelect(bool isOn)
    {
        if (isOn)
            Img_Element.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Element/Frame_ItemFrame06_s");
        else
            Img_Element.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Element/Frame_ItemFrame06_n");
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

    private void SetGrade(HeroData data)
    {
        if(data.Grade == 0)
        {
            Text_Grade.text = "NORMAL";
            Text_Grade.color = new Color32(176, 176, 176, 255);
            Img_Grade.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Label/Label_Label01_Gray");
            Img_BackGlow.color = new Color32(60, 78, 107, 180);
        }
        else if(data.Grade == 1)
        {
            Text_Grade.text = "RARE";
            Text_Grade.color = new Color32(77, 163, 255, 255);
            Img_Grade.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Label/Label_Label01_Blue");
            Img_BackGlow.color = new Color32(46, 181, 229, 30);
        }
        else if (data.Grade == 2)
        {
            Text_Grade.text = "EPIC";
            Text_Grade.color = new Color32(195, 107, 255, 255);
            Img_Grade.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Label/Label_Label01_Purple");
            Img_BackGlow.color = new Color32(115, 107, 255, 31);
        }
        else
        {
            Text_Grade.text = "UNIQUE";
            Text_Grade.color = new Color32(229, 245, 84, 89);
            Img_Grade.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Label/Label_Label01_Yellow");
            Img_BackGlow.color = new Color32(225, 251, 1, 13);
        }
    }
    #endregion
}
