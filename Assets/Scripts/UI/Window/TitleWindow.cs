using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TitleWindow : UIElement
{
    #region Cashed Object
    [SerializeField] private Button Btn_Start = null;
    #endregion

    #region Override Method
    public override void Init()
    {
        Btn_Start.onClick.AddListener(OnClick_Start);
    }

    public override void OnOpen(List<object> args)
    {

    }

    public override void OnClose()
    {
    }

    public override void OnRefresh()
    {
    }
    #endregion

    #region Button
    private async void OnClick_Start()
    {
        await ScenesManager.Instance.LoadScene("LobbyScene");
    }
    #endregion
}
