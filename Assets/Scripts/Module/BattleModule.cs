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
    // �ν��Ͻ�
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

    // BattleModule�� ��ӹ޴� Module���� ��κ� �������� ����ϴ� ����� ����
    // �ڽ� Module���� �������� �ʾƵ� �ȴ�.
    #region Virtual Method
    // ���� ����
    // �ε� �� ������ ���� ������ Delay�� �ش�.
    public async virtual UniTask StartGame()
    {
        // 1. ��� UI �ݱ�
        UIManager.Instance.CloseAll();

        //// 2.

        //// ....
    }

    // ���� ��
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

        // Module ����
        DestroyModule();
    }
    #endregion

    // BattleModule�� ��ӹ޴� module���� ���ٸ� ���� ���� ���������� ����ϴ� ����� ����
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

    // ���߿� ����
    #region ObjectPool
    //// �ΰ��� �� ������Ʈ���� Ǯ�� �����ϴ� �޼���
    //// ex) ����, ����ġ��, ü��ȸ���� �� 
    //private void CreateObjectPool()
    //{
    //    // ��ü ������Ʈ ����Ʈ
    //    var objInfoList = m_ObjectInfoTable.Values.Where(Data => Data.Key > 0).ToList();
    //    // ������Ʈ Ÿ�� ����Ʈ
    //    var objTypeList = DataManager.GetTable<ObjectType>(TableType.ObjectType).Values.Where(Data => Data.Key != "empty").Select(Data => Data.Key).ToList();
    //    // ������Ʈ Ÿ�Ժ� ��Ʈ ������ �����̳�
    //    var dicObjRoot = new Dictionary<string, GameObject>();

    //    // ������Ʈ Ÿ�Ժ� ��Ʈ �������� �ε��� �� ��ųʸ��� �־��ش�.
    //    for (int index = 0; index < objTypeList.Count; ++index)
    //    {
    //        // ������ �̺�Ʈ�� ���������� ������ Ǯ�� ������ �ʴ´�.
    //        if (User.UserEventData.SlimeData == null && objTypeList[index] == $"{eObjectType.SlimeGold}")
    //            continue;

    //        var type = objTypeList[index];

    //        var root = ResourceLoader.LoadAsset<GameObject>($"Prefab/Object/Obj_{type}", $"Obj_{type}");
    //        if (root == null)
    //        {
    //            Debug.LogError($"Obj_{type} �������� �����ϴ�.");
    //            continue;
    //        }

    //        dicObjRoot.Add(type, root);

    //        root = null;
    //    }

    //    // ��� ������Ʈ Ÿ�Ժ��� Ǯ�� ����� �ܰ�
    //    // ��Ʈ �������� ã�� �� �� �ڽ����� �� �������� �ε�����ش�.
    //    for (int index = 0; index < objInfoList.Count; ++index)
    //    {
    //        // ������ �̺�Ʈ�� ���������� ������ Ǯ�� ������ �ʴ´�.
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

    //        // TODO: ������Ʈ���� Ǯ ����� �����ؾ��ҵ� => ���̺�?
    //        var count = 5;

    //        if (info.Type == $"{eObjectType.Exp}")
    //            count = 350;
    //        if (info.Type == $"{eObjectType.SlimeGold}")
    //            count = 50;

    //        // Ǯ ����
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

    //// �ΰ��� �� ������Ʈ���� �������� �޼��� 
    //// ex) ����, ����ġ��, ü��ȸ���� �� 
    //public GameObject GetObjectByPool(int key)
    //{
    //    GameObject obj = null;

    //    var objInfo = m_ObjectInfoTable.Values.SingleOrDefault(Data => Data.Key == key);

    //    obj = PoolManager.Instance.GetPooler($"Obj_{objInfo.Type}_{objInfo.Object_Index}").GetAvailable();

    //    // ������ ���
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

    //    // ���� ������Ʈ��
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