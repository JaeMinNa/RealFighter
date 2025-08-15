using NUnit.Framework;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    [SerializeField] private GameObject Root_UI = null;

    private void Start()
    {
        UIManager.Instance.SetUIRoot(Root_UI);
        UIManager.Instance.SetActiveRoot(UI.BackGround, false);

        // �κ�
        UIManager.Instance.Open<LobbyWindow>(UI.Main, "Prefabs/UI/Window/LobbyWindow");
    }
}
