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
        // 현재까지 데이터를 저장
        DataManager.Instance.SaveData();

        // 씬을 비동기로 로드 (싱글 모드)
        await SceneManager.LoadSceneAsync(sceneName).ToUniTask();

        // 씬 로드 완료 후, DataLoader를 설정
        DataManager.Instance.SetDataLoader();

        // 씬 로드 완료 후, DataLoader에 데이터를 로드
        DataManager.Instance.LoadData();
    }
    #endregion
}
