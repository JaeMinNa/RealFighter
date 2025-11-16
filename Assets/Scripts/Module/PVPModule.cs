using Cysharp.Threading.Tasks;
using Photon.Pun;
using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class PVPModule : BattleModule
{
    #region Public Property
    // My UserData
    public GameObject Obj_MyHero { get; private set; }
    public int CurTurn { get; private set; }
    public int CurRound { get; private set; }
    public float CurTime { get; set; } = 30f;
    public int CurHp { get; private set; }
    public int[] MyCanUseSkillCounts { get; private set; } = new int[3];
    public bool IsMyReady { get; set; }
    public int MySelectBtnNum { get; set; }
    public HeroAnim MyHeroAnim { get; private set; }
    public int MyCombo { get; private set; }
    public bool IsMyCombo { get; private set; }
    public bool IsMyCritical { get; private set; }

    // Enemy UserData
    public GameObject Obj_EnemyHero { get; private set; }
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

    // Photon
    public PhotonController PhotonController_My { get; private set; } = null;
    public PhotonController PhotonController_Enemy { get; private set; } = null;

    #endregion

    #region Member Property
    private CinemachineCamera m_Cinemachine = null;
    private CinemachineSplineDolly m_SplineDolly = null;
    private int m_EnemySelectTime = 0;
    private bool m_IsTutorial = false;
    private bool m_IsTutorial_OneDeffence = false;
    #endregion

    #region Unity Method
    protected async override void Update()
    {
        if (!IsStartGame)
            return;

        base.Update();

        if (!PhotonNetwork.IsConnected)
        {
            // 턴 제한 시간 감소
            if (CurTime >= 0)
                CurTime -= Time.deltaTime;

            // AI 선택 시간에 도달했을 경우 자동 버튼 선택
            if (ClientDef.TurnTime - CurTime > m_EnemySelectTime)
            {
                // 적이 아직 선택을 안 한 경우
                if (EnemySelectBtnNum == -1)
                {
                    // 방어 턴일 때
                    if (!IsAttackTurn)
                    {
                        while (EnemySelectBtnNum == -1)
                        {
                            int value = RandomUtil.GetRandomIndex(0, 2);

                            // 모든 스킬이 0이고 공격 턴이면 그냥 아무거나 선택
                            if (EnemyCanUseSkillCounts[0] == 0 &&
                                EnemyCanUseSkillCounts[1] == 0 &&
                                EnemyCanUseSkillCounts[2] == 0 &&
                                IsAttackTurn)
                                EnemySelectBtnNum = value;

                            if (EnemyCanUseSkillCounts[value] > 0)
                                EnemySelectBtnNum = value;
                        }
                    }
                    // 공격 턴일 때
                    else
                    {
                        if (!m_IsTutorial)
                        {
                            int value = RandomUtil.GetRandomIndex(0, 2);
                            EnemySelectBtnNum = value;
                        }
                    }

                    IsEnemyReady = true;
                }
            }
        }
        else
        {
            // 마스터 클라이언트만 턴 시간 감소
            if (PhotonNetwork.IsMasterClient)
                if (CurTime >= 0)
                    CurTime -= Time.deltaTime;

            // 상대방이 나간 경우 승리 처리
            if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
            {
                m_Result = "Win";
                EndGame();
            }
        }

        // 시간 초과 시 자동 준비 완료 처리
        if (CurTime <= 0)
        {
            IsMyReady = true;
            IsEnemyReady = true;
        }

        // 양쪽 준비 완료 → 배틀 시작
        if (IsMyReady && IsEnemyReady && !IsBattle)
        {
            Debug.LogWarning("배틀 시작!");

            IsBattle = true;
            await StartBattle();
            NextTurn();
        }
    }

    private void OnApplicationQuit()
    {
        // 종료 시 Photon 연결이 있을 경우 종료 처리
        if (PhotonNetwork.IsConnected)
            PhotonManager.Instance.Disconnect(null);

        // 종료 시, 랭크 점수 1 감소 (패널티)
        if (DataManager.Instance.GetMyUserData().UserCommonData.RankPoint > 0)
            DataManager.Instance.GetMyUserData().UserCommonData.RankPoint--;

        // 데이터 저장
        DataManager.Instance.SaveData();
        BackendManager.Instance.SaveData();
    }
    #endregion

    #region Overrid Method
    public async override UniTask StartGame()
    {
        await base.StartGame();

        // 최초 1회 초기 설정
        InitialGame();

        // PVP 모드
        if (PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("PVP 배틀 시작!");
            Debug.LogWarning("상대방 접속까지 대기...");

            // 상대방 입장 또는 10초 타임아웃
            await UniTask.WhenAny(
                UniTask.WaitUntil(() => PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount >= 2),
                UniTask.Delay(TimeSpan.FromSeconds(10))
            );

            // Room 입장 실패
            if (PhotonNetwork.CurrentRoom == null)
            {
                Debug.LogWarning("Room 입장 실패");

                await ScenesManager.Instance.LoadScene("LobbyScene");
                await UniTask.Delay(100);
                UIManager.Instance.OpenSystemPopup(new MessageData
                {
                    Type = PopupType.OkOnly,
                    Message = "Room 입장에 실패 했습니다.",
                    OkAction = () =>
                    {
                        // UIRoot를 찾을 수 없어서 강제로 Root 설정
                        var lobbyScene = GameObject.Find("SceneLoader").GetComponent<LobbyScene>();
                        lobbyScene.SetUIRoot();
                    }
                });
                PhotonManager.Instance.Disconnect(null);
                return;
            }

            // 상대방을 찾을 수 없음
            if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
            {
                Debug.LogWarning("상대방을 찾을 수 없음");

                await ScenesManager.Instance.LoadScene("LobbyScene");
                await UniTask.Delay(100);
                UIManager.Instance.OpenSystemPopup(new MessageData
                {
                    Type = PopupType.OkOnly,
                    Message = "상대방을 찾을 수 없습니다.",
                    OkAction = () =>
                    {
                        // UIRoot를 찾을 수 없어서 강제로 Root 설정
                        var lobbyScene = GameObject.Find("SceneLoader").GetComponent<LobbyScene>();
                        lobbyScene.SetUIRoot();
                    }
                });
                PhotonManager.Instance.Disconnect(null);
                return;
            }

            Debug.LogWarning("상대방 찾기 완료!");

            // PhotonController 생성
            PhotonNetwork.Instantiate("Prefabs/Photon/PhotonController", Vector3.zero, Quaternion.identity);
            Debug.LogWarning("나의 PhotonController 생성 완료!");

            // 양쪽 플레이어의 PhotonController 준비될 때까지 대기
            await UniTask.WhenAny(
                UniTask.WaitUntil(() =>
                {
                    var list = UnityEngine.Object.FindObjectsByType<PhotonController>(FindObjectsSortMode.None);
                    return list != null
                        && list.Length >= 2
                        && list.All(c => c.PhotonView != null && c.PhotonView.ViewID > 0);
                }),
                UniTask.Delay(TimeSpan.FromSeconds(10))
            );

            var controllers = UnityEngine.Object.FindObjectsByType<PhotonController>(FindObjectsSortMode.None);
            if (controllers == null
                || controllers.Length < 2
                || !controllers.All(c => c.PhotonView != null && c.PhotonView.ViewID > 0))
            {
                Debug.LogWarning("상대방 PhotonController를 찾을 수 없음");

                await ScenesManager.Instance.LoadScene("LobbyScene");
                await UniTask.Delay(100);
                UIManager.Instance.OpenSystemPopup(new MessageData
                {
                    Type = PopupType.OkOnly,
                    Message = "상대방 데이터 생성에 실패 했습니다.",
                    OkAction = () =>
                    {
                        // UIRoot를 찾을 수 없어서 강제로 Root 설정
                        var lobbyScene = GameObject.Find("SceneLoader").GetComponent<LobbyScene>();
                        lobbyScene.SetUIRoot();
                    }
                });
                PhotonManager.Instance.Disconnect(null);
                return;
            }

            Debug.LogWarning("상대방 데이터 생성 완료!");

            // 나/상대 PhotonController 정리
            foreach (var c in controllers)
            {
                if (c.PhotonView == null)
                    continue;

                if (c.PhotonView.IsMine)
                    PhotonController_My = c;
                else
                    PhotonController_Enemy = c;
            }

            // 상대방에게 내 데이터 전달
            UserData_Common myCommonData = DataManager.Instance.GetMyUserData().UserCommonData;
            HeroData myHeroData = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero;

            PhotonController_My.PhotonView.RPC(
                "RPCSetMyData",
                RpcTarget.Others,
                myCommonData.NickName, myCommonData.RankPoint, myCommonData.Image,
                myHeroData.HeroName, myHeroData.Skillproficiencies[0], myHeroData.Skillproficiencies[1],
                myHeroData.Skillproficiencies[2], myHeroData.Level, myHeroData.Exp,
                myHeroData.Grade, myHeroData.GradeExp
            );

            Debug.LogWarning("상대방 데이터 불러오는 중...");

            // 상대방 데이터 도착 대기
            await UniTask.WhenAny(
                UniTask.WaitUntil(() => !string.IsNullOrEmpty(PhotonController_Enemy.MyNickName)),
                UniTask.Delay(TimeSpan.FromSeconds(10))
            );

            if (string.IsNullOrEmpty(PhotonController_Enemy.MyNickName))
            {
                Debug.LogWarning("상대방 데이터 불러오기 실패");

                await ScenesManager.Instance.LoadScene("LobbyScene");
                await UniTask.Delay(100);
                UIManager.Instance.OpenSystemPopup(new MessageData
                {
                    Type = PopupType.OkOnly,
                    Message = "상대방 데이터 불러오기에 실패 했습니다.",
                    OkAction = () =>
                    {
                        // UIRoot를 찾을 수 없어서 강제로 Root 설정
                        var lobbyScene = GameObject.Find("SceneLoader").GetComponent<LobbyScene>();
                        lobbyScene.SetUIRoot();
                    }
                });
                PhotonManager.Instance.Disconnect(null);
                return;
            }

            Debug.LogWarning("상대방 데이터 불러오기 완료!");

            UserData_Common EnemyCommonData = new UserData_Common()
            {
                NickName = PhotonController_Enemy.MyNickName,
                RankPoint = PhotonController_Enemy.MyScore,
                Image = PhotonController_Enemy.MyImage
            };

            HeroData EnemyHero = new HeroData()
            {
                HeroName = PhotonController_Enemy.MyHeroName,
                Skillproficiencies = PhotonController_Enemy.MyHeroSkillproficiencies,
                Level = PhotonController_Enemy.MyHeroLevel,
                Exp = PhotonController_Enemy.MyHeroExp,
                Grade = PhotonController_Enemy.MyHeroGrade,
                GradeExp = PhotonController_Enemy.MyHeroGradeExp
            };

            UserData_Hero EnemyUserHeroData = new UserData_Hero()
            {
                EquipHero = EnemyHero
            };

            UserData EnemyData = new UserData()
            {
                UserCommonData = EnemyCommonData,
                UserHeroData = EnemyUserHeroData
            };

            EnemyUserData = EnemyData;
        }
        // AI 모드
        else
        {
            Debug.LogWarning("AI 배틀 시작!");
            EnemyUserData = m_IsTutorial ? DataManager.Instance.GetTutorialAIUserData() : DataManager.Instance.GetAIUserData();
        }

        // 맵 생성
        var field = ResourceLoader.LoadAssetResources<GameObject>("Prefabs/Map/MainField");
        Instantiate(field, Vector3.zero, Quaternion.identity, m_EnvironmentRoot.transform);
        Feild = field.GetComponent<MainFeild>();

        // 히어로 생성
        CreateHeroes();

        // 카메라 셋팅
        await SetCameraMove();

        IsStartGame = true;
    }

    protected override void EndGame()
    {
        if (!IsStartGame)
            return;

        if (PhotonNetwork.IsConnected)
            PhotonManager.Instance.Disconnect(null);

        base.EndGame();
    }
    #endregion

    #region Private Method

    #region Initial
    // 최초 1회 실행
    private void InitialGame()
    {
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
        m_IsTutorial = false;
        m_IsTutorial_OneDeffence = false;

        if (DataManager.Instance.GetMyUserData().UserContentsData.TutorialIndex == 1)
        {
            m_IsTutorial = true;
            m_IsTutorial_OneDeffence = true;
        }
            
        StartTurn();

        for (int index = 0; index < MyCanUseSkillCounts.Length; ++index)
        {
            MyCanUseSkillCounts[index] = ClientDef.SkillMaxCount;
        }

        for (int index = 0; index < EnemyCanUseSkillCounts.Length; ++index)
        {
            EnemyCanUseSkillCounts[index] = ClientDef.SkillMaxCount;
        }

        // 플레이어 위치 설정
        if (!PhotonNetwork.IsConnected)
        {
            if(m_IsTutorial)
            {
                IsLeftPlayer = true;
                IsAttackTurn = true;
            }
            else
            {
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
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
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
    }

    // 턴마다 실행
    private void StartTurn()
    {
        CurTime = ClientDef.TurnTime;
        IsMyReady = false;
        IsEnemyReady = false;
        IsBattle = false;
        MySelectBtnNum = -1;
        EnemySelectBtnNum = -1;
        m_EnemySelectTime = m_IsTutorial ? 1 : RandomUtil.GetRandomIndex(3, 10);
    }

    // 라운드 증가 처리
    private async void NextRound()
    {
        CurRound++;

        if (CurRound > ClientDef.MaxRound)
        {
            await UniTask.Delay(2000);

            if (CurHp > EnemyCurHp)
                m_Result = "Win";
            else if (CurHp < EnemyCurHp)
                m_Result = "Lose";
            else if (CurHp == EnemyCurHp)
                m_Result = "Draw";

            EndGame();
        }
    }

    private async void NextTurn()
    {
        Debug.LogWarning("턴 시작!");

        StartTurn();

        CurTurn++;
        IsAttackTurn = !IsAttackTurn;

        if (CurTurn % 2 == 0)
        {
            NextRound();
        }

        var ingameWindow = UIManager.Instance.GetOpened<IngameWindow>();
        ingameWindow.SetUI_Skill();
        ingameWindow.SetUI_Top();

        if (m_IsTutorial && IsAttackTurn && IsMyCritical)
            await TutorialManager.Instance.StartTutorial(TutorialStep.IngameChat_10);
    }
    #endregion

    private void CreateHeroes()
    {
        var myHero = ResourceLoader.LoadAssetResources<GameObject>($"Prefabs/Hero/Hero_Ingame/{DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.HeroName}");
        var enemyHero = ResourceLoader.LoadAssetResources<GameObject>($"Prefabs/Hero/Hero_Ingame/{EnemyUserData.UserHeroData.EquipHero.HeroName}");

        if (IsLeftPlayer)
        {
            Obj_MyHero = Instantiate(myHero, Feild.GetTransformPlayer(true).position, Quaternion.Euler(0f, 90f, 0f), m_CharacterRoot.transform);
            Obj_EnemyHero = Instantiate(enemyHero, Feild.GetTransformPlayer(false).position, Quaternion.Euler(0f, -90f, 0f), m_CharacterRoot.transform);
        }
        else
        {
            Obj_MyHero = Instantiate(myHero, Feild.GetTransformPlayer(false).position, Quaternion.Euler(0f, -90f, 0f), m_CharacterRoot.transform);
            Obj_EnemyHero = Instantiate(enemyHero, Feild.GetTransformPlayer(true).position, Quaternion.Euler(0f, 90f, 0f), m_CharacterRoot.transform);
        }

        MyHeroAnim = Obj_MyHero.GetComponent<HeroAnim>();
        EnemyHeroAnim = Obj_EnemyHero.GetComponent<HeroAnim>();
    }

    private async UniTask StartBattle()
    {
        var ingameWindow = UIManager.Instance.GetOpened<IngameWindow>();

        if (IsAttackTurn)
        {
            if (MySelectBtnNum != -1)
                MyCanUseSkillCounts[MySelectBtnNum]--;

            if (IsMyCritical)
            {
                if (MySelectBtnNum != -1)
                {
                    await ingameWindow.ShowSkillImage(DataManager.Instance.GetMyUserData().UserHeroData.EquipHero.HeroName, 2);

                    MyHeroAnim.Anim.SetTrigger($"Skill_Cri");

                    await UniTask.Delay((int)(MyHeroAnim.CriticalTime * 1000));

                    SoundManager.Instance.StartSFX_Punch();
                    SoundManager.Instance.StartSFX("Hit", Obj_EnemyHero.transform.position);

                    EnemyHeroAnim.Anim.SetTrigger("Hit_Cri");
                    EffectUtil.StartShake(0.15f, 0.2f);

                    int damage = DamageUtil.GetSkillDamage(DataManager.Instance.GetMyUserData().UserHeroData.EquipHero, MySelectBtnNum);
                    if (MySelectBtnNum != EnemySelectBtnNum)
                        damage *= 2;
                    else
                        damage /= 2;

                    EnemyCurHp -= damage;

                    ingameWindow.SetUI_Players();

                    if (EnemyCurHp <= 0)
                    {
                        // Die Sound 

                        EnemyHeroAnim.Anim.SetTrigger("Die");

                        await UniTask.Delay(2000);  // 사망 애니메이션 시간

                        m_Result = "Win";
                        EndGame();
                    }
                }
                else
                {
                    if (EnemySelectBtnNum != -1)
                        EnemyHeroAnim.Anim.SetTrigger("Block");
                }

                IsMyCombo = false;
                MyCombo = 0;
                IsMyCritical = false;

                await UniTask.Delay(1000);
            }
            else
            {
                if (MySelectBtnNum != -1)
                {
                    MyHeroAnim.Anim.SetTrigger($"Skill_{MySelectBtnNum}");

                    await UniTask.Delay((int)(MyHeroAnim.SkillTimes[MySelectBtnNum] * 1000));
                    SoundManager.Instance.StartSFX_Punch();

                    // 공격 성공
                    if (MySelectBtnNum != EnemySelectBtnNum)
                    {
                        EffectUtil.StartShake(0.1f, 0.2f);
                        SoundManager.Instance.StartSFX("Hit", Obj_EnemyHero.transform.position);
                        EnemyHeroAnim.Anim.SetTrigger("Hit");
                        EnemyCurHp -= DamageUtil.GetSkillDamage(DataManager.Instance.GetMyUserData().UserHeroData.EquipHero, MySelectBtnNum);
                        ingameWindow.SetUI_Players();

                        if (EnemyCurHp <= 0)
                        {
                            // Die Sound 

                            EnemyHeroAnim.Anim.SetTrigger("Die");

                            await UniTask.Delay(2000);  // 사망 애니메이션 시간

                            m_Result = "Win";
                            EndGame();
                        }

                        if (IsMyCombo)
                        {
                            MyCombo++;

                            if (MyCombo == 3)
                            {
                                IsMyCritical = true;
                            }
                        }
                        else
                        {
                            MyCombo = 1;
                            IsMyCombo = true;
                        }
                    }
                    else
                    {
                        // 상쇄됨
                        IsMyCombo = false;
                        MyCombo = 0;

                        EffectUtil.StartShake(0.02f, 0.15f);
                        EnemyHeroAnim.Anim.SetTrigger("Block");
                    }
                }
                else
                {
                    // 나는 선택 안 했지만 상대는 공격함 → 방어 처리
                    IsMyCombo = false;
                    MyCombo = 0;

                    if (EnemySelectBtnNum != -1)
                        EnemyHeroAnim.Anim.SetTrigger("Block");
                }

                await UniTask.Delay(1000);
            }
        }
        else
        {
            if(m_IsTutorial_OneDeffence)
            {
                m_IsTutorial_OneDeffence = false;
                EnemySelectBtnNum = MySelectBtnNum;
            }

            if (EnemySelectBtnNum != -1)
                EnemyCanUseSkillCounts[EnemySelectBtnNum]--;

            if (IsEnemyCritical)
            {
                if (EnemySelectBtnNum != -1)
                {
                    await ingameWindow.ShowSkillImage(EnemyUserData.UserHeroData.EquipHero.HeroName, 2);

                    EnemyHeroAnim.Anim.SetTrigger($"Skill_Cri");

                    await UniTask.Delay((int)(EnemyHeroAnim.CriticalTime * 1000));

                    SoundManager.Instance.StartSFX_Punch();
                    SoundManager.Instance.StartSFX("Hit", Obj_MyHero.transform.position);
                    MyHeroAnim.Anim.SetTrigger("Hit_Cri");
                    EffectUtil.StartShake(0.15f, 0.2f);

                    int damage = DamageUtil.GetSkillDamage(EnemyUserData.UserHeroData.EquipHero, EnemySelectBtnNum);
                    if (MySelectBtnNum != EnemySelectBtnNum)
                        damage *= 2;
                    else
                        damage /= 2;

                    CurHp -= damage;
                    ingameWindow.SetUI_Players();

                    if (CurHp <= 0)
                    {
                        // Die Sound 

                        MyHeroAnim.Anim.SetTrigger("Die");

                        await UniTask.Delay(2000);  // 사망 애니메이션 시간

                        m_Result = "Lose";
                        EndGame();
                    }
                }
                else
                {
                    // 적은 선택 안 했지만 나는 공격 → 방어 애니
                    if (MySelectBtnNum != -1)
                        MyHeroAnim.Anim.SetTrigger("Block");
                }

                IsEnemyCombo = false;
                EnemyCombo = 0;
                IsEnemyCritical = false;

                await UniTask.Delay(1000);
            }
            else
            {
                if (EnemySelectBtnNum != -1)
                {
                    EnemyHeroAnim.Anim.SetTrigger($"Skill_{EnemySelectBtnNum}");

                    await UniTask.Delay((int)(EnemyHeroAnim.SkillTimes[EnemySelectBtnNum] * 1000));
                    SoundManager.Instance.StartSFX_Punch();

                    // 공격 성공
                    if (MySelectBtnNum != EnemySelectBtnNum)
                    {
                        EffectUtil.StartShake(0.1f, 0.2f);
                        SoundManager.Instance.StartSFX("Hit", Obj_MyHero.transform.position);
                        MyHeroAnim.Anim.SetTrigger("Hit");
                        CurHp -= DamageUtil.GetSkillDamage(EnemyUserData.UserHeroData.EquipHero, EnemySelectBtnNum);
                        ingameWindow.SetUI_Players();

                        if (CurHp <= 0)
                        {
                            // Die Sound 

                            MyHeroAnim.Anim.SetTrigger("Die");

                            await UniTask.Delay(2000);  // 사망 애니메이션 시간

                            m_Result = "Lose";
                            EndGame();
                        }

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
                        // 상쇄됨
                        IsEnemyCombo = false;
                        EnemyCombo = 0;

                        EffectUtil.StartShake(0.02f, 0.15f);
                        MyHeroAnim.Anim.SetTrigger("Block");

                        await UniTask.Delay(1000);
                    }
                }
                else
                {
                    // 적은 선택 안 했지만 나는 공격함 → 방어로 처리
                    IsEnemyCombo = false;
                    EnemyCombo = 0;

                    if (MySelectBtnNum != -1)
                        MyHeroAnim.Anim.SetTrigger("Block");
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

        await StartCinemachine();
        m_Cinemachine.gameObject.SetActive(false);

        if (IsLeftPlayer)
        {
            Camera.main.transform.position = new Vector3(-1.3f, 2.8f, -0.7f);
            Camera.main.transform.rotation = Quaternion.Euler(14f, -304f, 1.2f);
        }
        else
        {
            Camera.main.transform.position = new Vector3(1.3f, 2.8f, -0.7f);
            Camera.main.transform.rotation = Quaternion.Euler(12.5f, -63.5f, -2.55f);
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

        m_SplineDolly.CameraPosition = 1f;
    }
    #endregion

    #endregion
}
