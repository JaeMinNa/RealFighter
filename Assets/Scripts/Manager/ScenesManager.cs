using Cysharp.Threading.Tasks;
using Photon.Pun;
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
        // 현재 데이터를 저장
        DataManager.Instance.SaveData();

        // 씬 비동기 로드 (로딩 화면 띄우는 방식 사용 시 대기)
        await SceneManager.LoadSceneAsync(sceneName).ToUniTask();

        // 씬 로드 완료 후 DataLoader 초기화
        DataManager.Instance.SetDataLoader();

        // DataLoader에서 데이터 불러오기
        DataManager.Instance.LoadData();
    }

    public async UniTask PhotonLoadScene(string sceneName)
    {
        Debug.LogWarning($"=== PhotonLoadScene 시작 ===");
        Debug.LogWarning($"[1] AutoSyncScene = {PhotonNetwork.AutomaticallySyncScene}, IsMessageQueueRunning = {PhotonNetwork.IsMessageQueueRunning}, InRoom = {PhotonNetwork.InRoom}, IsMasterClient = {PhotonNetwork.IsMasterClient}");
        Debug.LogWarning($"[1] 현재 방 이름: {(PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Name : "없음")}");

        // 현재 데이터를 저장
        DataManager.Instance.SaveData();

        // Photon 씬 동기화 준비
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.IsMessageQueueRunning = true;

        Debug.LogWarning($"[2] AutoSyncScene = {PhotonNetwork.AutomaticallySyncScene}, IsMessageQueueRunning = {PhotonNetwork.IsMessageQueueRunning}");

        // 씬 로드 완료 여부 플래그
        bool isLoaded = false;
        SceneManager.sceneLoaded += OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.LogWarning($"[OnSceneLoaded] scene={scene.name}, mode={mode}");
            if (scene.name == sceneName)
            {
                isLoaded = true;
                Debug.LogWarning($"[3] Scene Loaded 완료: {sceneName}");
            }
        }

        // 마스터 클라이언트만 LoadLevel 호출
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning($"[MASTER] PhotonNetwork.LoadLevel 호출: {sceneName}");
            PhotonNetwork.LoadLevel(sceneName);
        }
        else
        {
            Debug.LogWarning($"[CLIENT] LoadLevel 직접 호출 불가 — Photon 자동 로드 대기");
        }

        // 씬 로드 완료될 때까지 대기
        await UniTask.WaitUntil(() => isLoaded);

        Debug.LogWarning($"[4] 씬 로드 완료 확인, 데이터 초기화 시작");

        // 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // DataLoader 초기화
        DataManager.Instance.SetDataLoader();

        // DataLoader에서 데이터 불러오기
        DataManager.Instance.LoadData();

        Debug.LogWarning($"{sceneName} 씬 로드 완료!");
    }
    #endregion
}
