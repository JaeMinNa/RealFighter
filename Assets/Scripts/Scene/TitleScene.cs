using UnityEngine;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private GameObject Root_UI = null;

    // TitleScene ���� ȣ�� ����
    private void Start()
    {
        // GameManager ����
        GameManager.Instance.InitDefaultManager();

        // ������ ������ �ε�
        DataManager.Instance.LoadData();

        // Title UI ����
        UIManager.Instance.SetUIRoot(Root_UI);
        UIManager.Instance.SetActiveRoot(UI.BackGround, false);
        UIManager.Instance.Open<TitleWindow>(UI.Main, "Prefabs/UI/Window/TitleWindow");
    }
}