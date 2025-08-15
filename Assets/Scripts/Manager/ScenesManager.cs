using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : Singleton<ScenesManager>
{
    public static ScenesManager Instance
    {
        get
        {
            if (m_Instance == null && Application.isPlaying)
            {
                GameObject Obj = GameObject.Find("[Managers]");
                if (Obj == null)
                {
                    Obj = new GameObject("[Managers]");
                    DontDestroyOnLoad(Obj);
                }

                GameObject managerObj = GameObject.Find("[Managers]/SceneManager");
                if (managerObj == null)
                {
                    managerObj = new GameObject("SceneManager");
                    managerObj.transform.SetParent(Obj.transform);
                }

                m_Instance = managerObj.GetComponent<ScenesManager>();
                if (m_Instance == null)
                {
                    m_Instance = managerObj.AddComponent<ScenesManager>();
                }

                m_Instance.CreateInstance();
            }

            return m_Instance;
        }
    }

    #region Override Method
    public override void DestroyInstance()
    {

    }

    protected override void CreateInstance()
    {

    }
    #endregion

    #region Public Method
    public async UniTask LoadScene(string sceneName)
    {
        // ������� �����͸� ����
        DataManager.Instance.SaveData();

        // ���� �񵿱�� �ε� (�̱� ���)
        await SceneManager.LoadSceneAsync(sceneName).ToUniTask();

        // �� �ε� �Ϸ� ��, DataLoader�� ����
        DataManager.Instance.SetDataLoader();

        // �� �ε� �Ϸ� ��, DataLoader�� �����͸� �ε�
        DataManager.Instance.LoadData();
    }
    #endregion
}
