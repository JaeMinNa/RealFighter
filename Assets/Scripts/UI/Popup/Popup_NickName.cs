using System;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Popup_NickName : UIElement
{
    #region Cahsed Object
    [SerializeField] TMP_Text m_Text = null;
    [SerializeField] TMP_InputField m_InputField = null;
    [SerializeField] private Button Btn_Ok = null;
    [SerializeField] private Button Btn_Close = null;
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
        Btn_Close.onClick.AddListener(OnClick_Close);

        LoadBannedWords();
    }

    public override void OnClose()
    {
        
    }

    public override void OnOpen(List<object> Args)
    {
        m_Text.text = DataManager.Instance.GetMyUserData().UserCommonData.NickName;
        m_InputField.text = DataManager.Instance.GetMyUserData().UserCommonData.NickName;
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
            Debug.LogError("금칙어 CSV 파일을 찾지 못했습니다.");
            return;
        }

        // 줄 단위로 파싱
        string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            string word = line.Trim();
            if (!string.IsNullOrEmpty(word))
            {
                // 중복 방지
                if (!m_BannedWords.Contains(word))
                    m_BannedWords.Add(word);
            }
        }

        Debug.Log($"금칙어 {m_BannedWords.Count}개 로드 완료");
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
    #endregion

    #region Button

    private void OnClick_Ok()
    {
        // 닉네임 입력 안했을 때
        if (string.IsNullOrEmpty(m_InputField.text))
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
                Message = "부적절한 닉네임입니다.",
            });
            return;
        }

        // 뒤끝 서버 닉네임 수정
        if (!BackendManager.Instance.UpdateNickname(m_InputField.text))
        {
            UIManager.Instance.OpenSystemPopup(new MessageData
            {
                Type = PopupType.OkOnly,
                Message = "이미 존재하거나, 사용할 수 없는 닉네임입니다.",
            });
            return;
        }

        DataManager.Instance.GetMyUserData().UserCommonData.NickName = m_Text.text;
        DataManager.Instance.SaveData();
        UIManager.Instance.Refresh();
        SoundManager.Instance.StartSFX("ButtonClick");

        UIManager.Instance.Close<Popup_NickName>();
    }

    private void OnClick_Close()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Close<Popup_NickName>();
    }
    #endregion
}
