using System;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class ElementShop : MonoBehaviour
{
    #region Cashed Object
    [SerializeField] private Image Img_Icon = null;
    [SerializeField] private TMP_Text Text_Name = null;
    [SerializeField] private TMP_Text Text_Count = null;
    [SerializeField] private TMP_Text Text_Gold = null;
    [SerializeField] private TMP_Text Text_Price = null;
    [SerializeField] private TMP_Text Text_Value = null;
    [SerializeField] private Button Btn_Click = null;
    #endregion

    #region Member Property
    private Action m_Action = null;
    #endregion

    #region Public Method
    public void SetShop(ShopData data)
    {
        Img_Icon.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Shop/{data.Name}");
        Text_Name.text = data.Name;

        if (data.Price == 0)
            Text_Price.text = "FREE";
        else
            Text_Price.text = data.Price.ToString();

        SetCount(data.Count);
        SetValue(data.Value);
    }

    public void SetButton(Action action)
    {
        m_Action = action;

        if (m_Action != null)
            Btn_Click.onClick.AddListener(() => m_Action());
    }

    public void SetAd()
    {
        Text_Gold.text = "AD";
    }
    #endregion

    #region Private Method
    private void SetCount(int count)
    {
        if (count <= 0)
        {
            Text_Count.gameObject.SetActive(false);
            return;
        }

        Text_Count.text = $"X {count}";
        Text_Count.gameObject.SetActive(true);
    }

    private void SetValue(int value)
    {
        if (value <= 0)
        {
            Text_Value.transform.parent.gameObject.SetActive(false);
            return;
        }

        Text_Value.text = $"{value}X VALUE";
        Text_Value.transform.parent.gameObject.SetActive(true);
    }
    #endregion
}
