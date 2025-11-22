using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMask : UIElement
{
    #region Cahsed Object
    [SerializeField] private Button Btn_Mask = null;
    [SerializeField] private GameObject Obj_Mask = null;
    [SerializeField] private GameObject Obj_ArrowUp = null;
    [SerializeField] private GameObject Obj_ArrowDown = null;
    [SerializeField] private GameObject Obj_Chat = null;
    [SerializeField] private Text Text_Chat = null;

    #endregion

    #region Unity Method
    #endregion

    #region Override Method
    public override void Init()
    {
        SetActiveMask(false);
        SetActiveChat(false);
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

    #endregion

    #region Public Method
    public void SetActiveMask(bool isOn)
    {
        Obj_Mask.SetActive(isOn);
    }

    public void SetButtonMask(Action action)
    {
        Btn_Mask.onClick.AddListener(() => action?.Invoke());
    }

    public void SetUpArrow(bool isOn)
    {
        Obj_ArrowUp.SetActive(isOn);
    }

    public void SetDownArrow(bool isOn)
    {
        Obj_ArrowDown.SetActive(isOn);
    }

    public void SetActiveChat(bool isOn)
    {
        Obj_Chat.SetActive(isOn);
    }

    public void SetChatText(string text)
    {
        Text_Chat.text = "";
        Text_Chat.DOText(text, 1f, true).SetUpdate(true);
    }
    #endregion
}
