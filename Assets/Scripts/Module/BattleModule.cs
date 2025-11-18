using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using System.Collections.Generic;

public class BattleModule : MonoBehaviour
{
    public bool IsStartGame { get; set; } = false;

    #region Member Property
    protected GameObject m_CharacterRoot = null;
    protected GameObject m_CameraRoot = null;
    protected GameObject m_EnvironmentRoot = null;
    protected string m_Result = string.Empty;
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

    // BattleModule을 상속받는 Module들은 이곳에 공통적인 기능을 작성하는 영역  
    // 자식 Module에서 반드시 재정의 하지 않아도 된다.
    #region Virtual Method
    // 게임 시작  
    // 로딩 및 연출이 필요한 경우 해당 메서드에서 Delay를 준다.
    public async virtual UniTask StartGame()
    {
        // 1. 모든 UI 닫기
        UIManager.Instance.CloseAll();

        //// 2.

        //// ....
    }

    // 게임 종료
    protected virtual void EndGame()
    {

        Time.timeScale = 0f;
        IsStartGame = false;

        List<object> args = new List<object> { m_Result };
        UIManager.Instance.Open<Popup_Result>(UI.Popup, "Prefabs/UI/Popup/Popup_Result", args);

        // Module 삭제
        DestroyModule();
    }
    #endregion

    // BattleModule을 상속하는 module들을 외부에서 판단하기 위한 메서드 영역
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

    // 아래부터는 필요 시 사용 예정
    #region ObjectPool
    //// 앞으로 사용할 오브젝트들에 대한 풀링을 생성하는 메서드  
    //// ex) 경험치, 골드, 체력회복 등 
    //private void CreateObjectPool()
    //{
    //    // 전체 오브젝트 리스트
    //    var objInfoList = m_ObjectInfoTable.Values.Where(Data => Data.Key > 0).ToList();
    //    // 오브젝트 타입 리스트
    //    var objTypeList = DataManager.GetTable<ObjectType>(TableType.ObjectType).Values.Where(Data => Data.Key != "empty").Select(Data => Data.Key).ToList();
    //    // 오브젝트 타입별 프리팹 딕셔너리
    //    var dicObjRoot = new Dictionary<string, GameObject>();

    //    // 오브젝트 타입별 프리팹 로딩 후 저장
    //    for (int index = 0; index < objTypeList.Count; ++index)
    //    {
    //        // 유저 이벤트에 해당하는 경우에만 풀 생성
    //        if (User.UserEventData.SlimeData == null && objTypeList[index] == $"{eObjectType.SlimeGold}")
    //            continue;

    //        var type = objTypeList[index];

    //        var root = ResourceLoader.LoadAsset<GameObject>($"Prefab/Object/Obj_{type}", $"Obj_{type}");
    //        if (root == null)
    //        {
    //            Debug.LogError($"Obj_{type} 프리팹을 찾을 수 없습니다.");
    //            continue;
    //        }

    //        dicObjRoot.Add(type, root);

    //        root = null;
    //    }

    //    // 실제 오브젝트별 풀 생성
    //    for (int index = 0; index < objInfoList.Count; ++index)
    //    {
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

    //        // TODO: 오브젝트별 풀 개수 설정 필요
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

    //// 풀에서 오브젝트를 가져오는 메서드  
    //// ex) 경험치, 골드, 체력회복 등 
    //public GameObject GetObjectByPool(int key)
    //{
    //    GameObject obj = null;

    //    var objInfo = m_ObjectInfoTable.Values.SingleOrDefault(Data => Data.Key == key);

    //    obj = PoolManager.Instance.GetPooler($"Obj_{objInfo.Type}_{objInfo.Object_Index}").GetAvailable();

    //    // 오브젝트 타입이 박스인 경우
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

    //    // 기본 오브젝트일 경우
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
