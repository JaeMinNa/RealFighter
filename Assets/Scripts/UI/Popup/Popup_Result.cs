using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Result : UIElement
{
    #region Cahsed Object
    [SerializeField] private TMP_Text Text_Title = null;
    [SerializeField] private TMP_Text Text_CurLevel = null;
    [SerializeField] private TMP_Text Text_CurExp = null;
    [SerializeField] private Slider Slider_CurExp = null;
    [SerializeField] private Button Btn_Home = null;
    [SerializeField] private GameObject Obj_RewardSlots = null;
    #endregion

    #region Member Property
    private List<ItemData> RewardItems = new List<ItemData>();
    private GameObject m_slotObj = null;
    #endregion

    #region Override Method
    public override void Init()
    {
        Btn_Home.onClick.AddListener(OnClick_Home);

        m_slotObj = ResourceLoader.LoadAssetResources<GameObject>($"Prefabs/Element/ElementSlot");
    }

    public override void OnClose()
    {
        
    }

    public override void OnOpen(List<object> Args)
    {
        if (Args.Count == 0)
            return;

        string arg = (string)Args[0];

        if (arg == "Win")
            SetWin();
        else if (arg == "Lose")
            SetLose();
        else if (arg == "Draw")
            SetDraw();
        else
            return;

        SetResult();
    }

    public override void OnRefresh()
    {
        
    }
    #endregion

    #region Private Method
    private void SetWin()
    {
        Text_Title.text = "VICTORY!";

        HeroUtil.AddHeroExp(ClientDef.WinExp);
        DataManager.Instance.GetMyUserData().UserCommonData.Gold += ClientDef.WinGold;
        DataManager.Instance.GetMyUserData().UserCommonData.RankPoint++;

        // Win 보상
        RewardItems.Add(new ItemData("Exp", ClientDef.WinExp));
        RewardItems.Add(new ItemData("Gold", ClientDef.WinGold));
        RewardItems.Add(new ItemData("Score", 1));
    }

    private void SetLose()
    {
        Text_Title.text = "LOSE..";

        HeroUtil.AddHeroExp(ClientDef.LoseExp);
        DataManager.Instance.GetMyUserData().UserCommonData.Gold += ClientDef.LoseGold;

        // Lose 보상
        RewardItems.Add(new ItemData("Exp", ClientDef.LoseExp));
        RewardItems.Add(new ItemData("Gold", ClientDef.LoseGold));
    }

    private void SetDraw()
    {
        Text_Title.text = "DRAW!";

        HeroUtil.AddHeroExp(ClientDef.LoseExp);
        DataManager.Instance.GetMyUserData().UserCommonData.Gold += ClientDef.LoseGold;

        // Draw 보상
        RewardItems.Add(new ItemData("Exp", ClientDef.DrawExp));
        RewardItems.Add(new ItemData("Gold", ClientDef.DrawGold));
    }

    private void SetResult()
    {
        // 리워드 슬롯
        for (int Index = 0; Index < RewardItems.Count; ++Index)
        {
            var obj = Instantiate(m_slotObj, Obj_RewardSlots.transform);
            obj.GetComponent<ElementSlot>().SetItem(RewardItems[Index]);
        }

        Text_CurLevel.text = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Level.ToString();
        Text_CurExp.text = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Exp.ToString();
        Slider_CurExp.value = (float)DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Exp / (float)(DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.Level * 10) * 100f;

        // 데이터 저장
        DataManager.Instance.SaveData();

        // 뒤끝 저장
        BackendManager.Instance.SaveData();
    }
    #endregion

    #region Button
    private async void OnClick_Home()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        Time.timeScale = 1f;
        await ScenesManager.Instance.LoadScene("LobbyScene");
    }
    #endregion
}
