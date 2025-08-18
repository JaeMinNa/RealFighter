using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

public class BattleModule : MonoBehaviour
{
    #region Member Property
    protected GameObject m_CharacterRoot = null;
    protected GameObject m_CameraRoot = null;
    protected GameObject m_EnvironmentRoot = null;

    protected bool m_IsStartGame = false;
    #endregion

    #region Instance
    // 인스턴스
    private static BattleModule m_Instance;

    public static BattleModule Instance
    {
        get
        {
            return m_Instance;
        }
    }

    public static T CreateModule<T>() where T : BattleModule
    {
        GameObject obj = GameObject.Find("BattleModule");

        if (obj == null)
        {
            obj = new GameObject("BattleModule");
            DontDestroyOnLoad(obj);
        }

        var Module = obj.GetComponent<T>();
        if (Module != null)
        {
            DestroyModule();
        }

        Module = obj.AddComponent<T>();
        m_Instance = Module;

        return Module;
    }

    public static void DestroyModule()
    {
        DestroyImmediate(m_Instance);
        m_Instance = null;
    }
    #endregion

    #region Unity Method
    protected virtual void Update()
    {

    }

    #endregion

    // BattleModule을 상속받는 Module에서 대부분 공통으로 사용하는 기능을 구현
    // 자식 Module에서 구현하지 않아도 된다.
    #region Virtual Method
    // 게임 시작
    // 로드 할 내용이 많이 때문에 Delay를 준다.
    public async virtual UniTask StartGame()
    {
        // 1. 모든 UI 닫기
        UIManager.Instance.CloseAll();

        //// 2.

        //// ....
    }

    // 게임 끝
    protected virtual void EndGame()
    {
        //if (MonsterMovementSystem != null)
        //    Destroy(MonsterMovementSystem);

        //if (m_AutoPlayController != null)
        //    Destroy(m_AutoPlayController);

        //SetStartGame(false);
        //m_IsPause = true;
        //m_IsEndGame = true;

        m_IsStartGame = false;

        // Module 제거
        DestroyModule();
    }
    #endregion

    // BattleModule을 상속받는 module에서 별다른 구현 없이 공통적으로 사용하는 기능을 구현
    #region Public Method
    public bool IsModule<T>() where T : BattleModule
    {
        return this is T;
    }

    public void SetRootObject(GameObject cameraRoot, GameObject environmentRoot, GameObject characterRoot)
    {
        m_CameraRoot = cameraRoot;
        m_EnvironmentRoot = environmentRoot;
        m_CharacterRoot = characterRoot;
    }

    public void SetPause(bool isOn, bool isShowPauseUI = false)
    {
        //m_IsPause = isOn;

        //if (m_IsPause)
        //{
        //    Time.timeScale = 0f;

        //    if (isShowPauseUI)
        //    {
        //        var arenaModule = Instance as PvPArenaModule;
        //        if (arenaModule != null)
        //            UIManager.Instance.Open<Popup_Arena_Pause>(UI.Popup, "UI/Popup/Popup_Arena_Pause");
        //        else
        //            UIManager.Instance.Open<PauseWindow>(UI.Main, "UI/Ingame/PauseWindow");

        //        arenaModule = null;
        //    }
        //}
        //else
        //{
        //    Time.timeScale = 1f;
        //}
    }
    #endregion

    // 나중에 구현
    #region ObjectPool
    //// 인게임 내 오브젝트들의 풀을 생성하는 메서드
    //// ex) 상자, 경험치석, 체력회복템 등 
    //private void CreateObjectPool()
    //{
    //    // 전체 오브젝트 리스트
    //    var objInfoList = m_ObjectInfoTable.Values.Where(Data => Data.Key > 0).ToList();
    //    // 오브젝트 타입 리스트
    //    var objTypeList = DataManager.GetTable<ObjectType>(TableType.ObjectType).Values.Where(Data => Data.Key != "empty").Select(Data => Data.Key).ToList();
    //    // 오브젝트 타입별 루트 프리팹 컨테이너
    //    var dicObjRoot = new Dictionary<string, GameObject>();

    //    // 오브젝트 타입별 루트 프리팹을 로드한 뒤 딕셔너리에 넣어준다.
    //    for (int index = 0; index < objTypeList.Count; ++index)
    //    {
    //        // 슬라임 이벤트가 진행중이지 않으면 풀을 만들지 않는다.
    //        if (User.UserEventData.SlimeData == null && objTypeList[index] == $"{eObjectType.SlimeGold}")
    //            continue;

    //        var type = objTypeList[index];

    //        var root = ResourceLoader.LoadAsset<GameObject>($"Prefab/Object/Obj_{type}", $"Obj_{type}");
    //        if (root == null)
    //        {
    //            Debug.LogError($"Obj_{type} 프리팹이 없습니다.");
    //            continue;
    //        }

    //        dicObjRoot.Add(type, root);

    //        root = null;
    //    }

    //    // 모든 오브젝트 타입별로 풀을 만드는 단계
    //    // 루트 프리팹을 찾은 뒤 그 자식으로 모델 프리팹을 로드시켜준다.
    //    for (int index = 0; index < objInfoList.Count; ++index)
    //    {
    //        // 슬라임 이벤트가 진행중이지 않으면 풀을 만들지 않는다.
    //        if (User.UserEventData.SlimeData == null && objInfoList[index].Type == $"{eObjectType.SlimeGold}")
    //            continue;

    //        var info = objInfoList[index];
    //        var profile = m_ObjectProfileTable.Values.FirstOrDefault(Data => Data.Object_Key == info.Key);

    //        var root = Instantiate(dicObjRoot[info.Type], new Vector2(10000, 10000), Quaternion.identity, parent: m_SamplePrefabRoot.transform);
    //        root.transform.localScale = new Vector3(info.Object_Size, info.Object_Size, info.Object_Size);
    //        var modelPos = root.transform.Find("model");

    //        if (!string.IsNullOrEmpty(profile.Prefab))
    //        {
    //            var model = ResourceLoader.LoadAsset<GameObject>("Prefab/Object/Model", profile.Prefab);
    //            Instantiate(model, modelPos);

    //            model = null;
    //        }

    //        root.SetActive(false);

    //        // TODO: 오브젝트별로 풀 사이즈를 조정해야할듯 => 테이블?
    //        var count = 5;

    //        if (info.Type == $"{eObjectType.Exp}")
    //            count = 350;
    //        if (info.Type == $"{eObjectType.SlimeGold}")
    //            count = 50;

    //        // 풀 생성
    //        PoolManager.Instance.CreatePooler($"Obj_{info.Type}_{info.Object_Index}", root, count, m_CharacterRoot.transform);

    //        info = null;
    //        profile = null;
    //        root = null;
    //        modelPos = null;
    //    }

    //    objInfoList = null;
    //    objTypeList = null;
    //    dicObjRoot = null;
    //}

    //// 인게임 내 오브젝트들을 가져오는 메서드 
    //// ex) 상자, 경험치석, 체력회복템 등 
    //public GameObject GetObjectByPool(int key)
    //{
    //    GameObject obj = null;

    //    var objInfo = m_ObjectInfoTable.Values.SingleOrDefault(Data => Data.Key == key);

    //    obj = PoolManager.Instance.GetPooler($"Obj_{objInfo.Type}_{objInfo.Object_Index}").GetAvailable();

    //    // 상자일 경우
    //    if (objInfo.Type == $"{eObjectType.Box}")
    //    {
    //        var box = obj.GetComponent<ItemBox>();
    //        if (box == null) return null;

    //        var DropTime = DataManager.GetTableCache<BaseMula>(TableType.BaseMula).Values.SingleOrDefault(Data => Data.Key == "ObjectBoxDropTime");
    //        var DropRange = DataManager.GetTableCache<BaseMula>(TableType.BaseMula).Values.SingleOrDefault(Data => Data.Key == "ObjectBoxDropRange");
    //        var IntervalRange = DataManager.GetTableCache<BaseMula>(TableType.BaseMula).Values.SingleOrDefault(Data => Data.Key == "ObjectBoxDropInterval");
    //        box.Init(DropTime.Value_Int, DropTime.Value_Single, float.Parse(DropTime.Value_String), DropRange.Value_Single, float.Parse(DropRange.Value_String), IntervalRange.Value_Int);

    //        box = null;
    //        DropTime = null;
    //        DropRange = null;
    //        IntervalRange = null;

    //        return obj;
    //    }

    //    // 외의 오브젝트들
    //    var objBase = obj.GetComponent<ObjectBase>();
    //    if (objBase == null)
    //        return null;

    //    objBase.Init(objInfo);

    //    objInfo = null;
    //    objBase = null;

    //    return obj;
    //}
    #endregion
}