using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TitleWindow : UIElement
{
    #region Cashed Object
    [SerializeField] InputField TF_AccountCode = null;
    [SerializeField] InputField TF_Nickname = null;

    [SerializeField] private Text Text_AccountCode = null;
    [SerializeField] private Text Text_Nickname = null;

    [SerializeField] private UnityEngine.UI.Button Btn_Join = null;
    [SerializeField] private UnityEngine.UI.Button Btn_ChangeNickname = null;
    [SerializeField] private UnityEngine.UI.Button Btn_GetData = null;
    #endregion

    #region Override Method
    public override void Init()
    {
        Btn_Join.onClick.AddListener(OnClick_Join);
        Btn_ChangeNickname.onClick.AddListener(OnClick_ChangeNicknaem);
        Btn_GetData.onClick.AddListener(OnClick_GetData);
    }

    public override void OnOpen(List<object> args)
    {
        SetWindow();
    }

    public override void OnClose()
    {
    }

    public override void OnRefresh()
    {
    }
    #endregion

    #region Member Method
    private void SetWindow()
    {
        
    }
    #endregion

    #region Button
    private void OnClick_Join()
    {
        GameManager.Instance.AccountCode = Text_AccountCode.text;

        NetworkManager.Instance.SendPacket(PacketType.GetUserData);
    }

    private void OnClick_ChangeNicknaem()
    {
        //var Data = Util.MakeData($"{UserContents.Find}", Util.ToJson(result), Util.ToJson(StageData));
        var Head = Util.MakeHeaderData(UserContents.ChangeNickName, Text_Nickname.text);
        NetworkManager.Instance.SendContentsPacket(ContentsType.User, Head);
    }

    private void OnClick_GetData()
    {
        if(string.IsNullOrEmpty(GameManager.Instance.AccountCode))
        {
            Debug.Log("아직 로그인 안함!");
        }
        else
        {
            var Header = Util.MakeHeaderData(UserContents.GetData);
            NetworkManager.Instance.SendContentsPacket(ContentsType.User, Header);
        }
    }
    #endregion

    #region Public Method
    public void AccountCodeInput()
    {
        Text_AccountCode.text = TF_AccountCode.text;
    }

    public void NicknameCodeInput()
    {
        Text_Nickname.text = TF_Nickname.text;
    }
    #endregion
}
