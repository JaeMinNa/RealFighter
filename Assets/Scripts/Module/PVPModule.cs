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
    public int[] MyCanUseSkillCounts { get; private set; } = new int[3];
    public bool IsMyReady { get; set; }
    public int MySelectBtnNum { get; set; }
    public HeroAnim MyHeroAnim { get; private set; }
    public int MyCombo { get; private set; }
    public bool IsMyCombo { get; private set; }
    public bool IsMyCritical { get; private set; }

    // Enemy UserData
    public UserData EnemyUserData { get; private set; }
    public int EnemyCurHp { get; private set; }
    public int[] EnemyCanUseSkillCounts { get; private set; } = new int[3];
    public bool IsEnemyReady { get; set; }
    public int EnemySelectBtnNum { get; set; }
    public HeroAnim EnemyHeroAnim { get; private set; }
    public int EnemyCombo { get; private set; }
    public bool IsEnemyCombo { get; private set; }
    public bool IsEnemyCritical { get; private set; }

    // Field
    public MainFeild Feild { get; private set; }

    public bool IsLeftPlayer { get; private set; }
    public bool IsAttackTurn { get; private set; }
    public bool IsBattle { get; private set; }

    private CinemachineCamera m_Cinemachine = null;
    private CinemachineSplineDolly m_SplineDolly = null;
    #endregion

    #region Unity Method
    protected async override void Update()
    {
        if (!m_IsStartGame)
            return;

        base.Update();

        if(CurTime >= 0)
            CurTime -= Time.deltaTime;

        if (CurTime <= 0)
        {
            IsMyReady = true;
            IsEnemyReady = true;

            // Button을 클릭하지 않으면 랜덤 클릭
            if (MySelectBtnNum == -1)
            {
                while (MySelectBtnNum == -1)
                {
                    int value = RandomUtil.GetRandomIndex(0, 2);

                    if (MyCanUseSkillCounts[value] > 0)
                        MySelectBtnNum = value;
                }
            }

            if (EnemySelectBtnNum == -1)
            {
                while (EnemySelectBtnNum == -1)
                {
                    int value = RandomUtil.GetRandomIndex(0, 2);

                    if (EnemyCanUseSkillCounts[value] > 0)
                        EnemySelectBtnNum = value;
                }
            }
        }

        if(IsMyReady && IsEnemyReady && !IsBattle)
        {
            Debug.LogWarning("전투 시작!");

            IsBattle = true;

            await StartBattle();

            NextTurn();

            if (CurRound == ClientDef.MaxRound)
            {
                if (CurHp >= EnemyCurHp)
                    m_Win = true;

                EndGame();
                return;
            }
        }
    }
    #endregion

    #region Overrid Method
    public async override UniTask StartGame()
    {
        await base.StartGame();

        // 게임 시작 시, 최초 한번만 실행되는 것들
        InitialGame();

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

    #region Initial
    // 최초 한번만 실행 되는 것들
    private void InitialGame()
    {
        // 최초 한번만 초기화 하는 것들
        CurTurn = 0;
        CurRound = 1;
        CurHp = 100;
        EnemyCurHp = 100;
        MyCombo = 0;
        EnemyCombo = 0;
        IsMyCombo = false;
        IsEnemyCombo = false;
        IsMyCritical = false;
        IsEnemyCritical = false;

        StartTurn();

        for (int index = 0; index < MyCanUseSkillCounts.Length; ++index)
        {
            MyCanUseSkillCounts[index] = ClientDef.SkillMaxCount;
        }

        for (int index = 0; index < EnemyCanUseSkillCounts.Length; ++index)
        {
            EnemyCanUseSkillCounts[index] = ClientDef.SkillMaxCount;
        }

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

    // 턴 변경 시, 실행되는 것들
    private void StartTurn()
    {
        CurTime = ClientDef.TurnTime;
        IsMyReady = false;
        IsEnemyReady = false;
        IsBattle = false;
        MySelectBtnNum = -1;
        EnemySelectBtnNum = -1;
    }

    // 라운드 변경
    private async void NextRound()
    {
        CurRound++;

        if(CurRound > ClientDef.MaxRound)
        {
            await UniTask.Delay(2000);

            // 나중에 무승부인 경우 추가
            if (CurHp >= EnemyCurHp)
                m_Win = true;
            else
                m_Win = false;

            EndGame();
        }
    }

    // 턴 변경
    private void NextTurn()
    {
        Debug.LogWarning("턴 종료!");

        StartTurn();

        CurTurn++;
        IsAttackTurn = !IsAttackTurn;

        // CurTurn이 2의 배수일 때 다음 라운드로
        if (CurTurn % 2 == 0)
        {
            NextRound();
        }

        var ingameWindow = UIManager.Instance.GetOpened<IngameWindow>();
        ingameWindow.SetUI_Skill();
        ingameWindow.SetUI_Top();
    }
    #endregion

    private void CreateHeroes()
    {
        var myHero = ResourceLoader.LoadAssetResources<GameObject>($"Prefabs/Heroes/{DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.HeroName}");
        var enemyHero = ResourceLoader.LoadAssetResources<GameObject>($"Prefabs/Heroes/{EnemyUserData.UserHeroData.EquipHero.HeroName}");

        GameObject myHeroObj = null;
        GameObject enemyHeroObj = null;

        if (IsLeftPlayer)
        {
            myHeroObj = Instantiate(myHero, Feild.GetTransformPlayer(true).position, Quaternion.Euler(0f, 90f, 0f), m_CharacterRoot.transform);
            enemyHeroObj = Instantiate(enemyHero, Feild.GetTransformPlayer(false).position, Quaternion.Euler(0f, -90f, 0f), m_CharacterRoot.transform);
        }
        else
        {
            myHeroObj = Instantiate(myHero, Feild.GetTransformPlayer(false).position, Quaternion.Euler(0f, -90f, 0f), m_CharacterRoot.transform);
            enemyHeroObj = Instantiate(enemyHero, Feild.GetTransformPlayer(true).position, Quaternion.Euler(0f, 90f, 0f), m_CharacterRoot.transform);
        }

        MyHeroAnim = myHeroObj.GetComponent<HeroAnim>();
        EnemyHeroAnim = enemyHeroObj.GetComponent<HeroAnim>();
    }

    private async UniTask StartBattle()
    {
        var ingameWindow = UIManager.Instance.GetOpened<IngameWindow>();

        if (IsAttackTurn)
        {
            MyCanUseSkillCounts[MySelectBtnNum]--;

            if(IsMyCritical)
            {
                MyHeroAnim.Anim.SetTrigger($"Skill_Cri");

                await UniTask.Delay((int)(MyHeroAnim.CriticalTime * 1000));

                EnemyHeroAnim.Anim.SetTrigger("Hit_Cri");

                int damage = DamageUtil.GetSkillDamage(DataManager.Instance.GetMyUserData().UserHeroData.EquipHero, MySelectBtnNum);
                if (MySelectBtnNum != EnemySelectBtnNum)
                    damage *= 2;
                else
                    damage /= 2;

                EnemyCurHp -= damage;
                ingameWindow.SetUI_Players();

                if (EnemyCurHp <= 0)
                {
                    EnemyHeroAnim.Anim.SetTrigger("Die");

                    await UniTask.Delay(2000);  // Die 시간도 추가

                    m_Win = true;
                    EndGame();
                }

                IsMyCombo = false;
                MyCombo = 0;
                IsMyCritical = false;

                await UniTask.Delay(1000);
            }
            else
            {
                MyHeroAnim.Anim.SetTrigger($"Skill_{MySelectBtnNum}");

                await UniTask.Delay((int)(MyHeroAnim.SkillTimes[MySelectBtnNum] * 1000));

                // 공격 성공 시
                if (MySelectBtnNum != EnemySelectBtnNum)
                {
                    EnemyHeroAnim.Anim.SetTrigger("Hit");
                    EnemyCurHp -= DamageUtil.GetSkillDamage(DataManager.Instance.GetMyUserData().UserHeroData.EquipHero, MySelectBtnNum);
                    ingameWindow.SetUI_Players();

                    if (EnemyCurHp <= 0)
                    {
                        EnemyHeroAnim.Anim.SetTrigger("Die");

                        await UniTask.Delay(2000);  // Die 시간도 추가

                        m_Win = true;
                        EndGame();
                    }

                    // 콤보
                    if (IsMyCombo)
                    {
                        MyCombo++;

                        if (MyCombo == 3)
                            IsMyCritical = true;
                    }
                    else
                    {
                        MyCombo = 1;
                        IsMyCombo = true;
                    }

                    await UniTask.Delay(1000);
                }
                else
                {
                    IsMyCombo = false;
                    MyCombo = 0;

                    EnemyHeroAnim.Anim.SetTrigger("Block");

                    await UniTask.Delay(1000);
                }
            }
        }
        else
        {
            EnemyCanUseSkillCounts[EnemySelectBtnNum]--;

            if(IsEnemyCritical)
            {
                EnemyHeroAnim.Anim.SetTrigger($"Skill_Cri");

                await UniTask.Delay((int)(EnemyHeroAnim.CriticalTime * 1000));

                MyHeroAnim.Anim.SetTrigger("Hit_Cri");

                int damage = DamageUtil.GetSkillDamage(EnemyUserData.UserHeroData.EquipHero, MySelectBtnNum);
                if (MySelectBtnNum != EnemySelectBtnNum)
                    damage *= 2;
                else
                    damage /= 2;

                CurHp -= damage;
                ingameWindow.SetUI_Players();

                if (CurHp <= 0)
                {
                    MyHeroAnim.Anim.SetTrigger("Die");

                    await UniTask.Delay(2000);  // Die 시간도 추가

                    m_Win = false;
                    EndGame();
                }

                IsEnemyCombo = false;
                EnemyCombo = 0;
                IsEnemyCritical = false;

                await UniTask.Delay(1000);
            }
            else
            {
                EnemyHeroAnim.Anim.SetTrigger($"Skill_{EnemySelectBtnNum}");

                await UniTask.Delay((int)(EnemyHeroAnim.SkillTimes[EnemySelectBtnNum] * 1000));

                // 공격 성공 시
                if (MySelectBtnNum != EnemySelectBtnNum)
                {
                    MyHeroAnim.Anim.SetTrigger("Hit");
                    CurHp -= DamageUtil.GetSkillDamage(EnemyUserData.UserHeroData.EquipHero, EnemySelectBtnNum);
                    ingameWindow.SetUI_Players();

                    if (CurHp <= 0)
                    {
                        MyHeroAnim.Anim.SetTrigger("Die");

                        await UniTask.Delay(2000);  // Die 시간도 추가

                        m_Win = false;
                        EndGame();
                    }

                    // 콤보
                    if (IsEnemyCombo)
                    {
                        EnemyCombo++;

                        if (EnemyCombo == 3)
                            IsEnemyCritical = true;
                    }
                    else
                    {
                        EnemyCombo = 1;
                        IsEnemyCombo = true;
                    }

                    await UniTask.Delay(1000);
                }
                else
                {
                    // 콤보
                    IsEnemyCombo = false;
                    EnemyCombo = 0;

                    MyHeroAnim.Anim.SetTrigger("Block");

                    await UniTask.Delay(1000);
                }
            }
        }
    }

    #region Camera
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
            Camera.main.transform.position = new Vector3(-1.3f, 2.8f, -0.7f);
            Camera.main.transform.rotation = Quaternion.Euler(14f, -304f, 1.2f);
        }
        else
        {
            Camera.main.transform.position = new Vector3(1.3f, 2.8f, 0.7f);
            Camera.main.transform.rotation = Quaternion.Euler(14f, -116f, 1.2f);
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

    #endregion
}
