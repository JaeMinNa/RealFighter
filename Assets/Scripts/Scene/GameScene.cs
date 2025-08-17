using UnityEngine;

public class GameScene : MonoBehaviour
{
    [SerializeField] private GameObject Root_Camera = null;
    [SerializeField] private GameObject Root_Environment = null;
    [SerializeField] private GameObject Root_UI = null;
    [SerializeField] private GameObject Root_Character = null;

    private async void Start()
    {
        // Module ����
        BattleModule.CreateModule<PVPModule>();
        BattleModule.Instance.SetRootObject(Root_Camera, Root_Environment, Root_Character);
        await BattleModule.Instance.StartGame();

        // IngameWindow ����
        UIManager.Instance.SetUIRoot(Root_UI);
        UIManager.Instance.SetActiveRoot(UI.BackGround, false);
        UIManager.Instance.Open<IngameWindow>(UI.Main, "Prefabs/UI/Window/IngameWindow");
    }
}