using System;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_NickName : UIElement
{
    #region Cahsed Object
    [SerializeField] TMP_Text m_Text = null;
    [SerializeField] TMP_InputField m_InputField = null;
    [SerializeField] private Button Btn_Ok = null;
    #endregion

    #region Member Property
    private List<string> m_BannedWords = new List<string>();
    #endregion

    #region Unity Method
    #endregion

    #region Override Method
    public override void Init()
    { 
        Btn_Ok.onClick.AddListener(OnClick_Ok);

        LoadBannedWords();
    }

    public override void OnClose()
    {

    }

    public override void OnOpen(List<object> Args)
    {

    }

    public override void OnRefresh()
    {

    }
    #endregion

    #region Private Method
    private void LoadBannedWords()
    {
        if (m_BannedWords.Count > 0) return;

        TextAsset csvFile = ResourceLoader.LoadAssetResources<TextAsset>("CSV/BannedWord/BannedWord");
        if (csvFile == null)
        {
            Debug.LogError("��Ģ�� CSV ������ ã�� �� �����ϴ�.");
            return;
        }

        // �� ������ �и�
        string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            string word = line.Trim();
            if (!string.IsNullOrEmpty(word))
            {
                // �ߺ� ����
                if (!m_BannedWords.Contains(word))
                    m_BannedWords.Add(word);
            }
        }

        Debug.Log($"��Ģ�� {m_BannedWords.Count}�� �ε� �Ϸ�");
    }

    private bool IsBannedNickName(string nickname)
    {
        foreach (var banned in m_BannedWords)
        {
            if (nickname.Contains(banned, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
    #endregion

    #region Public Method
    public void NickNameInput()
    {
        m_Text.text = m_InputField.text;
    }

    //public void EnterButton()
    //{
    //    gameStartPanel.SetActive(false);
    //}
    #endregion

    #region Button
   
    private void OnClick_Ok()
    {
        // 닉네임 입력 안했을 때
        if(string.IsNullOrEmpty(m_InputField.text))
        {
            UIManager.Instance.OpenSystemPopup(new MessageData
            {
                Type = PopupType.OkOnly,
                Message = "닉네임을 입력하세요.",
            });
            return;
        }

        // 글자수 제한
        if (m_InputField.text.Length > 10 || m_InputField.text.Length < 2)
        {
            UIManager.Instance.OpenSystemPopup(new MessageData
            {
                Type = PopupType.OkOnly,
                Message = "2 ~ 9 글자수 닉네임을 입력하세요.",
            });
            return;
        }

        // 금칙어
        if (IsBannedNickName(m_InputField.text))
        {
            UIManager.Instance.OpenSystemPopup(new MessageData
            {
                Type = PopupType.OkOnly,
                Message = "부적절한 닉네임 입니다.",
            });
            return;
        }

        DataManager.Instance.GetMyUserData().UserCommonData.NickName = m_Text.text;
        DataManager.Instance.GetMyUserData().UserContentsData.IsFirstLogin = false;
        DataManager.Instance.SaveData();
        BackendManager.Instance.SaveData();
        UIManager.Instance.Refresh();
        SoundManager.Instance.StartSFX("ButtonClick");

        UIManager.Instance.Close<Popup_NickName>();
    }
    #endregion
}
