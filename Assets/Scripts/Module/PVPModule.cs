using Cysharp.Threading.Tasks;
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
    #endregion

    #region Member Property
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
            Debug.LogWarning("Round 종료!");

            NextRound();
        }
    }
    #endregion

    #region Overrid Method
    public async override UniTask StartGame()
    {
        // 라운드 초기화
        StartRound();

        await base.StartGame();

        // AI 적 생성 (임시)
        EnemyUserData = DataManager.Instance.GetAIUserData();

        // 맵 생성
        var map = ResourceLoader.LoadAssetResources<GameObject>("Prefabs/Map/MainField");
        Instantiate(map, Vector3.zero, Quaternion.identity, m_EnvironmentRoot.transform);

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
        CurRound = 1;
        CurTime = 30f;
        CurHp = 100;
        EnemyCurHp = 100;
    }

    // 다음 Round로 넘어갈 때마다 실행
    private void NextRound()
    {
        CurRound++;
        CurTime = 30f;

        var ingameWindow = UIManager.Instance.GetOpened<IngameWindow>();
        ingameWindow.SetUI_Top();
    }
    #endregion
}
