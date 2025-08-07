using UnityEngine;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private GameObject Root_UI = null;

    // TitleScene ���� ȣ�� ����
    void Start()
    {
        GameManager.Instance.InitDefaultManager();
        UIManager.Instance.SetUIRoot(Root_UI);
        UIManager.Instance.SetActiveRoot(UI.BackGround, false);

        // Ÿ��Ʋ
        UIManager.Instance.Open<TitleWindow>(UI.Main, "Prefabs/TitleWindow", IsBundle: false);
    }
}