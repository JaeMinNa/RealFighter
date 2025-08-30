using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class PVPModule : BattleModule
{
    #region Member Property
    // My UserData
    public int CurTurn { get; private set; }                        // 턴이 변경될 때 마다 1씩 증가
    public int CurRound { get; private set; }
    public float CurTime { get; private set; } = 30f;
    public int CurHp { get; private set; }
    public int MyCanUseSkillCount_0 { get; private set; }
    public int MyCanUseSkillCount_1 { get; private set; }
    public int MyCanUseSkillCount_2 { get; private set; }

    // Enemy UserData
    public UserData EnemyUserData { get; private set; }
    public int EnemyCurHp { get; private set; }
    public int EnemyCanUseSkillCount_0 { get; private set; }
    public int EnemyCanUseSkillCount_1 { get; private set; }
    public int EnemyCanUseSkillCount_2 { get; private set; }

    // Field
    public MainFeild Feild { get; private set; }

    public bool IsLeftPlayer { get; private set; }
    public bool IsAttackTurn { get; private set; }
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
            Debug.LogWarning("턴 종료!");

            NextTurn();
        }
    }
    #endregion

    #region Overrid Method
    public async override UniTask StartGame()
    {
        await base.StartGame();

        // 라운드 초기화
        StartRound();

        // AI 적 생성 (임시)
        EnemyUserData = DataManager.Instance.GetAIUserData();

        // 필드 생성
        var field = ResourceLoader.LoadAssetResources<GameObject>("Prefabs/Map/MainField");
        Instantiate(field, Vector3.zero, Quaternion.identity, m_EnvironmentRoot.transform);
        Feild = field.GetComponent<MainFeild>();

        // 히어로 생성
        CreateHeroes();

        // 카메라 효과
        await SetCameraMove();

        // 모든 준비가 완료 되었을 때
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
    // Round 초기화, 최초 한번만 실행
    private void StartRound()
    {
        CurTurn = 0;
        CurRound = 1;
        CurTime = 30f;
        CurHp = 100;
        EnemyCurHp = 100;
        MyCanUseSkillCount_0 = MyCanUseSkillCount_1 = MyCanUseSkillCount_2 = ClientDef.SkillMaxCount;
        EnemyCanUseSkillCount_0 = EnemyCanUseSkillCount_1 = EnemyCanUseSkillCount_2 = ClientDef.SkillMaxCount;

        // Player의 위치 결정
        var randomIndex = RandomUtil.GetRandomIndex(0, 1);
        if (randomIndex == 0)
        {
            IsLeftPlayer = true;
            IsAttackTurn = true;
        }
        else
        {
            IsLeftPlayer = false;
            IsAttackTurn = false;
        }
    }

    // 다음 Round로 넘어갈 때마다 실행
    private void NextRound()
    {
        Debug.LogWarning("다음 라운드!");

        CurRound++;

        var ingameWindow = UIManager.Instance.GetOpened<IngameWindow>();
        ingameWindow.SetUI_Top();
    }

    // 턴 변경
    private void NextTurn()
    {
        CurTurn++;
        CurTime = 30f;
        IsAttackTurn = !IsAttackTurn;

        var ingameWindow = UIManager.Instance.GetOpened<IngameWindow>();
        ingameWindow.SetUI_Skill();

        // CurTurn이 2의 배수일 때 다음 라운드로
        if (CurTurn % 2 == 0)
        {
            NextRound();
        }
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

        // 카메라 움직임 시작
        await StartCinemachine();

        // 시네머신 비활성화
        m_Cinemachine.gameObject.SetActive(false);

        // 메인 카메라 설정
        if(IsLeftPlayer)
        {
            Camera.main.transform.position = new Vector3(-1.6f, 3.1f, -0.9f);
            Camera.main.transform.rotation = Quaternion.Euler(14f, 63.9f, -1.8f);
        }
        else
        {
            Camera.main.transform.position = new Vector3(1.6f, 3.1f, 0.9f);
            Camera.main.transform.rotation = Quaternion.Euler(14f, 247f, -1.8f);
        }
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

        m_SplineDolly.CameraPosition = 1f; // 마지막 위치 보정
    }
    #endregion
}
