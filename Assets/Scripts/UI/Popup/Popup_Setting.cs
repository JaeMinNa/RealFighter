using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Popup_Setting : UIElement
{
    [SerializeField] private Button Btn_BGM = null;
    [SerializeField] private GameObject Obj_BGMOn = null;
    [SerializeField] private GameObject Obj_BGMOff = null;
    [SerializeField] private Button Btn_SFX = null;
    [SerializeField] private GameObject Obj_SFXOn = null;
    [SerializeField] private GameObject Obj_SFXOff = null;
    [SerializeField] private Button Btn_Language = null;
    [SerializeField] private Button Btn_LanguageChange = null;
    [SerializeField] private Button Btn_About = null;
    [SerializeField] private Button Btn_Support = null;
    [SerializeField] private Button Btn_Cancel = null;
    [SerializeField] private Button Btn_DeleteData = null;
    [SerializeField] private Button Btn_Exit = null;

    private AudioMixer m_AudioMixer = null;

    #region Overring Method
    public override void Init()
    {
        Btn_BGM.onClick.AddListener(OnClick_BGM);
        Btn_SFX.onClick.AddListener(OnClick_SFX);
        Btn_Language.onClick.AddListener(OnClick_Language);
        Btn_LanguageChange.onClick.AddListener(OnClick_Language);
        Btn_About.onClick.AddListener(OnClick_About);
        Btn_Support.onClick.AddListener(OnClick_Support);
        Btn_Cancel.onClick.AddListener(OnClick_Cancel);
        Btn_DeleteData.onClick.AddListener(OnClick_DeleteData);
        Btn_Exit.onClick.AddListener(OnClick_Exit);

        m_AudioMixer = ResourceLoader.LoadAssetResources<AudioMixer>("AudioMixer/AudioMixer");
    }

    public override void OnClose()
    {
        
    }

    public override void OnOpen(List<object> Args)
    {
        if(PlayerPrefs.GetInt("BGM") == 0)
        {
            Obj_BGMOn.SetActive(true);
            Obj_BGMOff.SetActive(false);
        }
        else
        {
            Obj_BGMOn.SetActive(false);
            Obj_BGMOff.SetActive(true);
        }

        if (PlayerPrefs.GetInt("SFX") == 0)
        {
            Obj_SFXOn.SetActive(true);
            Obj_SFXOff.SetActive(false);
        }
        else
        {
            Obj_SFXOn.SetActive(false);
            Obj_SFXOff.SetActive(true);
        }
    }

    public override void OnRefresh()
    {
        
    }
    #endregion

    #region Private Method
    private void SetBGM(bool isOn)
    {
        if (isOn)
        {
            Obj_BGMOn.SetActive(true);
            Obj_BGMOff.SetActive(false);
            m_AudioMixer.SetFloat("BGM", 0f);
            PlayerPrefs.SetInt("BGM", 0);
        }
        else
        {
            Obj_BGMOn.SetActive(false);
            Obj_BGMOff.SetActive(true);
            m_AudioMixer.SetFloat("BGM", -80f);
            PlayerPrefs.SetInt("BGM", -80);
        }
    }

    private void SetSFX(bool isOn)
    {
        if (isOn)
        {
            Obj_SFXOn.SetActive(true);
            Obj_SFXOff.SetActive(false);
            m_AudioMixer.SetFloat("SFX", 0f);
            PlayerPrefs.SetInt("SFX", 0);
        }
        else
        {
            Obj_SFXOn.SetActive(false);
            Obj_SFXOff.SetActive(true);
            m_AudioMixer.SetFloat("SFX", -80f);
            PlayerPrefs.SetInt("SFX", -80);
        }
    }
    #endregion

    #region Button
    private void OnClick_BGM()
    {
        SoundManager.Instance.StartSFX("ButtonClick");

        if (PlayerPrefs.GetInt("BGM") == 0)
            SetBGM(false);
        else
            SetBGM(true);
    }

    private void OnClick_SFX()
    {
        SoundManager.Instance.StartSFX("ButtonClick");

        if (PlayerPrefs.GetInt("SFX") == 0)
            SetSFX(false);
        else
            SetSFX(true);
    }

    private void OnClick_Language()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkOnly,
            Message = "업데이트 예정입니다.",
        });
    }

    private void OnClick_About()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkOnly,
            Message = "업데이트 예정입니다.",
        });
    }

    private void OnClick_Support()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkOnly,
            Message = "업데이트 예정입니다.",
        });
    }

    private void OnClick_Cancel()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Close<Popup_Setting>();
    }

    private void OnClick_DeleteData()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkCancel,
            Message = "정말 모든 데이터를 삭제 하시겠습니까?",
            OkAction = () => { DataManager.Instance.DeleteData(); }
        });
    }

    private void OnClick_Exit()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.OpenSystemPopup(new MessageData
        { 
            Type = PopupType.OkCancel, 
            Message = "게임을 종료 하시겠습니까?", 
            OkAction = () => { DataManager.Instance.ExitGame(); }  
        });
    }
    #endregion
}
