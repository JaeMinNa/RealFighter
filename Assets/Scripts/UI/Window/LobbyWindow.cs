using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyWindow : UIElement
{
    #region Cashed Object
    // Top
    [SerializeField] private TMP_Text Text_NickName = null;
    [SerializeField] private Image Img_Character = null;
    [SerializeField] private TMP_Text Text_Level = null;
    [SerializeField] private TMP_Text Text_Hero = null;
    [SerializeField] private TMP_Text Text_Gold = null;
    #endregion

    #region Override Method
    public override void Init()
    {
        
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
        Text_NickName.text = DataManager.Instance.GetDataLoader().UserCommonData.NickName;
        Img_Character.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Character/Character_{DataManager.Instance.GetDataLoader().UserCommonData.Image}");
        Text_Level.text = DataManager.Instance.GetDataLoader().UserHeroData.EquipHero.Level.ToString();
        Text_Hero.text = DataManager.Instance.GetDataLoader().UserHeroData.EquipHero.HeroName;
        Text_Gold.text = DataManager.Instance.GetDataLoader().UserCommonData.Gold.ToString();
    }
    #endregion
}
