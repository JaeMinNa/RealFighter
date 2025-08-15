using UnityEngine;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private GameObject Root_UI = null;

    // TitleScene 최초 호출 시점
    private void Start()
    {
        // GameManager 셋팅
        GameManager.Instance.InitDefaultManager();

        // 최초의 데이터 로드
        DataManager.Instance.LoadData();

        // Title UI 셋팅
        UIManager.Instance.SetUIRoot(Root_UI);
        UIManager.Instance.SetActiveRoot(UI.BackGround, false);
        UIManager.Instance.Open<TitleWindow>(UI.Main, "Prefabs/UI/Window/TitleWindow");
    }
}