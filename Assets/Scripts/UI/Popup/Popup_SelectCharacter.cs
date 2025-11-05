using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_SelectCharacter : UIElement
{
    #region Cahsed Object
    [SerializeField] private Button[] Btn_Characters = null;
    [SerializeField] private Button Btn_Close = null;
    #endregion

    #region Member Property
    #endregion

    #region Override Method
    public override void Init()
    {
        for (int index = 0; index < Btn_Characters.Length; ++index)
        {
            int capturedIndex = index;
            Btn_Characters[index].onClick.AddListener(() => OnClick_Character(capturedIndex));
        }

        Btn_Close.onClick.AddListener(OnClick_Close);
    }

    public override void OnClose()
    {

    }

    public override void OnOpen(List<object> Args)
    {
        OnClick_Character(int.Parse(DataManager.Instance.GetMyUserData().UserCommonData.Image));
    }

    public override void OnRefresh()
    {

    }
    #endregion

    #region Private Method
    #endregion

    #region Button
    private void OnClick_Character(int num)
    {
        SoundManager.Instance.StartSFX("ButtonClick");

        // 모든 선택 비활성화
        for (int index = 0; index < Btn_Characters.Length; ++index)
        {
            Btn_Characters[index].transform.GetChild(0).gameObject.SetActive(false);
        }

        DataManager.Instance.GetMyUserData().UserCommonData.Image = num.ToString();

        Btn_Characters[num].transform.GetChild(0).gameObject.SetActive(true);

        UIManager.Instance.Refresh();

        // 데이터 저장
        DataManager.Instance.SaveData();

        // 뒤끝 저장
        BackendManager.Instance.SaveData();
    }

    private void OnClick_Close()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Close<Popup_SelectCharacter>();
    }
    #endregion
}
