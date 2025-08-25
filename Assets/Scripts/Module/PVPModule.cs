using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class PVPModule : BattleModule
{
    #region Member Property
    // My UserData
    public int CurRound { get; private set; }
    public float CurTime { get; private set; } = 30f;
    public int CurHp { get; private set; }

    // Enemy UserData
    public UserData EnemyUserData { get; private set; }
    public int EnemyCurHp { get; private set; }

    // Field
    public MainFeild Feild { get; private set; }

    public bool IsLeftPlayer { get; private set; }
    #endregion

    #region Member Property
    private CinemachineCamera m_Cinemachine = null;
   private CinemachineSplineDolly m_SplineDolly = null;
    #endregion

    #region Unity Method
    protected override void Update()
    {
        if (!m_IsStartGame)
            return;

        base.Update();

        CurTime -= Time.deltaTime;
        if (CurTime < 0)
        {
            Debug.LogWarning("Round ����!");

            NextRound();
        }
    }
    #endregion

    #region Overrid Method
    public async override UniTask StartGame()
    {
        await base.StartGame();

        // ���� �ʱ�ȭ
        StartRound();

        // AI �� ���� (�ӽ�)
        EnemyUserData = DataManager.Instance.GetAIUserData();

        // �ʵ� ����
        var field = ResourceLoader.LoadAssetResources<GameObject>("Prefabs/Map/MainField");
        Instantiate(field, Vector3.zero, Quaternion.identity, m_EnvironmentRoot.transform);
        Feild = field.GetComponent<MainFeild>();

        // ����� ����
        CreateHeroes();

        // ī�޶� ȿ��
        await SetCameraMove();

        // ��� �غ� �Ϸ� �Ǿ��� ��
        m_IsStartGame = true;
    }

    protected override void EndGame()
    {
        base.EndGame();
    }
    #endregion

    #region Public Method
    #endregion

    #region Private Method
    // Round �ʱ�ȭ, ���� �ѹ��� ����
    private void StartRound()
    {
        CurRound = 1;
        CurTime = 30f;
        CurHp = 100;
        EnemyCurHp = 100;

        // Player�� ��ġ ����
        var randomIndex = RandomUtil.GetRandomIndex(0, 1);
        if (randomIndex == 0)
            IsLeftPlayer = true;
        else
           IsLeftPlayer = false;
    }

    // ���� Round�� �Ѿ ������ ����
    private void NextRound()
    {
        CurRound++;
        CurTime = 30f;

        var ingameWindow = UIManager.Instance.GetOpened<IngameWindow>();
        ingameWindow.SetUI_Top();
    }

    private void CreateHeroes()
    {
        var myHero = ResourceLoader.LoadAssetResources<GameObject>($"Prefabs/Heroes/{DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.HeroName}");
        var enemyHero = ResourceLoader.LoadAssetResources<GameObject>($"Prefabs/Heroes/{EnemyUserData.UserHeroData.EquipHero.HeroName}");

        Instantiate(IsLeftPlayer ? myHero : enemyHero, Feild.GetTransformPlayer(true).position, Quaternion.Euler(0f, 90f, 0f), m_CharacterRoot.transform);
        Instantiate(IsLeftPlayer ? enemyHero : myHero, Feild.GetTransformPlayer(false).position, Quaternion.Euler(0f, -90f, 0f), m_CharacterRoot.transform);
    }

    private async UniTask SetCameraMove()
    {
        m_Cinemachine = GameObject.FindWithTag("Cinemachine").GetComponent<CinemachineCamera>();
        m_SplineDolly = GameObject.FindWithTag("Cinemachine").GetComponent<CinemachineSplineDolly>();
        m_Cinemachine.Follow = Feild.GetTransfromField();

        // ī�޶� ������ ����
        await StartCinemachine();

        // �ó׸ӽ� ��Ȱ��ȭ
        m_Cinemachine.gameObject.SetActive(false);

        // ī�޶� ����
        Camera.main.transform.position = new Vector3(-1.33f, 3.3f, -1.78f);
        Camera.main.transform.rotation = Quaternion.Euler(23.4f, 37.23f, 1.4f);
    }

    private async UniTask StartCinemachine()
    {
        float time = 0f;
        float CameraTime = 2f;

        while (time < CameraTime)
        {
            time += Time.deltaTime;
            float positionValue = Mathf.Clamp01(time / CameraTime);

            m_SplineDolly.CameraPosition = positionValue;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        m_SplineDolly.CameraPosition = 1f; // ������ ��ġ ����
    }
    #endregion
}
