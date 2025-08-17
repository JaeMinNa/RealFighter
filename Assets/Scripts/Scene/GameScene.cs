using UnityEngine;

public class GameScene : MonoBehaviour
{
    [SerializeField] private GameObject Root_Camera = null;
    [SerializeField] private GameObject Root_Environment = null;
    [SerializeField] private GameObject Root_UI = null;
    [SerializeField] private GameObject Root_Character = null;

    private void Start()
    {
        // Module »ý¼º
        if(BattleModule.Instance == null)
        {
            BattleModule.CreateModule<PVPModule>();
            BattleModule.Instance.SetRootObject(Root_Camera, Root_Environment, Root_Character);
            BattleModule.Instance.Initialize();
        }

        UIManager.Instance.SetUIRoot(Root_UI);
        UIManager.Instance.SetActiveRoot(UI.BackGround, false);
        UIManager.Instance.Open<TitleWindow>(UI.Main, "Prefabs/UI/Window/IngameWindow");
    }
}