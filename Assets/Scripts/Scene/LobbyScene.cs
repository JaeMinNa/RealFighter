using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    [SerializeField] private GameObject Root_UI = null;

    void Start()
    {
        GameManager.Instance.InitDefaultManager();
        UIManager.Instance.SetUIRoot(Root_UI);
        UIManager.Instance.SetActiveRoot(UI.BackGround, false);

        // ≈∏¿Ã∆≤
        UIManager.Instance.Open<TitleWindow>(UI.Main, "Prefabs/UI/LobbyWindow");
    }
}
