using NUnit.Framework;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    [SerializeField] private GameObject Root_UI = null;

    private void Start()
    {
        UIManager.Instance.SetUIRoot(Root_UI);
        UIManager.Instance.SetActiveRoot(UI.BackGround, false);

        // 로비
        UIManager.Instance.Open<LobbyWindow>(UI.Main, "Prefabs/UI/Window/LobbyWindow");

        SoundManager.Instance.StartBGM("BGM_Lobby");
    }

    // 서버가 연결이 끊겨서 로비로 오는 경우, UIRoot가 꼬이기 때문에 다시 설정
    public void SetUIRoot()
    {
        UIManager.Instance.SetUIRoot(Root_UI);
    }
}
