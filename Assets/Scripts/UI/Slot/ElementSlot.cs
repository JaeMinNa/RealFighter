using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementSlot : MonoBehaviour
{
    #region Cashed Object
    [SerializeField] private Image Img_Icon = null;
    [SerializeField] private TMP_Text Text_Name = null;
    [SerializeField] private TMP_Text Text_Count = null;
    #endregion

    #region Public Method
    public void SetItem(ItemData data)
    {
        Img_Icon.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Item/Item_Icon_{data.Name}");
        Text_Name.text = data.Name;
        Text_Count.text = data.Count.ToString();
    }
    #endregion
}
