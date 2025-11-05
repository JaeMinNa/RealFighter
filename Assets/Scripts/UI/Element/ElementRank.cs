using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementRank : MonoBehaviour
{
    #region Cashed Object
    [SerializeField] private Image Img_Character = null;
    [SerializeField] private TMP_Text Text_NickName = null;
    [SerializeField] private TMP_Text Text_Rank = null;
    [SerializeField] private TMP_Text Text_RankPoint = null;
    [SerializeField] private Image Img_Rank = null;
    #endregion

    #region Public Method
    public void SetRank(RankData data)
    {
        Img_Character.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Character/Character_{data.Image}");
        Text_Rank.text = data.Rank.ToString();
        Text_NickName.text = data.NickName;
        Text_RankPoint.text = data.RankPoint.ToString();

        if(data.Rank == 1)
            Img_Rank.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Rank/Icon_Badge03_Gold");
        else if (data.Rank == 2)
            Img_Rank.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Rank/Icon_Badge03_Silver");
        else if (data.Rank == 3)
            Img_Rank.sprite = ResourceLoader.LoadAssetResources<Sprite>($"Textures/Rank/Icon_Badge03_Bronze");
        else
        {
            Color c = Img_Rank.color;
            c.a = 0f;           
            Img_Rank.color = c;     
        }
    }
    #endregion
}
