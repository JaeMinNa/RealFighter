using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Setting : UIElement
{
    [SerializeField] private Button Btn_Cancel = null;
    [SerializeField] private Button Btn_DeleteData = null;
    [SerializeField] private Button Btn_Exit = null;

    #region Overring Method
    public override void Init()
    {
        Btn_Cancel.onClick.AddListener(OnClick_Cancel);
        Btn_DeleteData.onClick.AddListener(OnClick_DeleteData);
        Btn_Exit.onClick.AddListener(OnClick_Exit);
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

    #region Button
    private void OnClick_Cancel()
    {
        UIManager.Instance.Close<Popup_Setting>();
    }

    private void OnClick_DeleteData()
    {
        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkCancel,
            Message = "���� �����͸� ������ ���� �Ͻðڽ��ϱ�?",
            OkAction = () => { DataManager.Instance.DeleteData(); }
        });
    }

    private void OnClick_Exit()
    {
        UIManager.Instance.OpenSystemPopup(new MessageData
        { 
            Type = PopupType.OkCancel, 
            Message = "������ ���� �Ͻðڽ��ϱ�?", 
            OkAction = () => { DataManager.Instance.ExitGame(); }  
        });
    }
    #endregion
}
