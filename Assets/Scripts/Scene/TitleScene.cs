using UnityEngine;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private GameObject Root_UI = null;

    // TitleScene 최초 호출 시 실행
    private void Start()
    {
        // GameManager 초기 설정
        GameManager.Instance.InitDefaultManager();

        // 첫 Scene에서는 수동으로 DataLoader를 설정
        DataManager.Instance.SetDataLoader();

        // 유저 데이터 로드
        DataManager.Instance.LoadData();

        // 뒤끝 서버 로그인
        BackendManager.Instance.Login();

        // Title UI 설정
        UIManager.Instance.SetUIRoot(Root_UI);
        UIManager.Instance.SetActiveRoot(UI.BackGround, false);
        UIManager.Instance.Open<TitleWindow>(UI.Main, "Prefabs/UI/Window/TitleWindow");

        SoundManager.Instance.StartBGM("BGM_Title");
    }
}
