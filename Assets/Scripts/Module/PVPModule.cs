using Cysharp.Threading.Tasks;
using Photon.Pun;
using System;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PVPModule : BattleModule
{
    #region Member Property
    // My UserData
    public GameObject Obj_MyHero { get; private set; }
    public int CurTurn { get; private set; }                        // 턴이 변경될 때 마다 1씩 증가
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

    private CinemachineCamera m_Cinemachine = null;
    private CinemachineSplineDolly m_SplineDolly = null;
    private PhotonController m_PhotonController_My = null;
    private PhotonController m_PhotonController_Enemy = null;
    #endregion

    #region Unity Method
    protected async override void Update()
    {
        if (!IsStartGame)
            return;

        base.Update();

        if (!PhotonNetwork.IsConnected)
        {
            if (CurTime >= 0)
                CurTime -= Time.deltaTime;
        }
        else
        {
            if(PhotonNetwork.IsMasterClient)
                if (CurTime >= 0)
                    CurTime -= Time.deltaTime;
        }

        if (CurTime <= 0)
        {
            IsMyReady = true;
            IsEnemyReady = true;

            // Button을 클릭하지 않으면 랜덤 클릭
            //if (MySelectBtnNum == -1)
            //{
            //    while (MySelectBtnNum == -1)
            //    {
            //        int value = RandomUtil.GetRandomIndex(0, 2);

            //        // 모든 스킬 횟수를 다 사용하고, 방어턴인 경우 바로 선택
            //        if (MyCanUseSkillCounts[0] == 0 &&
            //            MyCanUseSkillCounts[1] == 0 &&
            //            MyCanUseSkillCounts[2] == 0 &&
            //            !IsAttackTurn)
            //        {
            //            MySelectBtnNum = value;
            //            break;
            //        }

            //        if (MyCanUseSkillCounts[value] > 0)
            //            MySelectBtnNum = value;
            //    }
            //}

            // 상대의 랜덤 클릭은 AI 모드에서만
            //if(!PhotonNetwork.IsConnected)
            //{
            //    if (EnemySelectBtnNum == -1)
            //    {
            //        while (EnemySelectBtnNum == -1)
            //        {
            //            int value = RandomUtil.GetRandomIndex(0, 2);

            //            // 모든 스킬 횟수를 다 사용하고, 방어턴인 경우 바로 선택
            //            if(EnemyCanUseSkillCounts[0] == 0 &&
            //                EnemyCanUseSkillCounts[1] == 0 &&
            //                EnemyCanUseSkillCounts[2] == 0 && 
            //                IsAttackTurn)
            //            {
            //                EnemySelectBtnNum = value;
            //                break;
            //            }

            //            if (EnemyCanUseSkillCounts[value] > 0)
            //                EnemySelectBtnNum = value;
            //        }
            //    }
            //}
        }

        if(IsMyReady && IsEnemyReady && !IsBattle)
        {
            Debug.LogWarning("전투 시작!");

            IsBattle = true;

            await StartBattle();

            NextTurn();
        }
    }

    private void OnApplicationQuit()
    {
        // 서버 연결 해제
        if (PhotonNetwork.IsConnected)
            PhotonManager.Instance.Disconnect(null);

        // 강제 종료 시, 패배 Score 1점 감소
        if (DataManager.Instance.GetMyUserData().UserCommonData.Score > 0)
            DataManager.Instance.GetMyUserData().UserCommonData.Score--;

        // 데이터 저장
        DataManager.Instance.SaveData();
    }
    #endregion

    #region Overrid Method
    public async override UniTask StartGame()
    {
        await base.StartGame();

        // 게임 시작 시, 최초 한번만 실행되는 것들
        InitialGame();

        // 상대 적 생성
        if (PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("상대방 접속 대기 중...");

            // 상대방 접속 까지 대기
            await UniTask.WhenAny(UniTask.WaitUntil(() => PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount >= 2),UniTask.Delay(TimeSpan.FromSeconds(10)));

            if (PhotonNetwork.CurrentRoom == null)
            {
                Debug.LogWarning("Room 입장 실패");

                UIManager.Instance.OpenSystemPopup(new MessageData
                {
                    Type = PopupType.OkOnly,
                    Message = "접속에 실패하였습니다.",
                    OkAction = async () => { await ScenesManager.Instance.LoadScene("LobbyScene"); }
                });
                return;      
            }

            // 상대방 접속 실패
            if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
            {
                UIManager.Instance.OpenSystemPopup(new MessageData
                {
                    Type = PopupType.OkOnly,
                    Message = "상대방을 찾을 수 없습니다.",
                    OkAction = async () => { await ScenesManager.Instance.LoadScene("LobbyScene"); }
                });
                return;
            }

            Debug.LogWarning("상대방 접속 완료!");

            // PhotonController 생성
            PhotonNetwork.Instantiate("Prefabs/Photon/PhotonController", Vector3.zero, Quaternion.identity).GetComponent<PhotonController>();
            Debug.LogWarning("PhotonController 생성 완료!");

            await UniTask.WhenAny(UniTask.WaitUntil(() =>
            {
                var list = UnityEngine.Object.FindObjectsByType<PhotonController>(FindObjectsSortMode.None);
                return list != null
                    && list.Length >= 2
                    && list.All(c => c.PhotonView != null && c.PhotonView.ViewID > 0);
            }),
            UniTask.Delay(TimeSpan.FromSeconds(10)));

            var controllers = UnityEngine.Object.FindObjectsByType<PhotonController>(FindObjectsSortMode.None);
            if (controllers == null
                || controllers.Length < 2
                || !controllers.All(c => c.PhotonView != null && c.PhotonView.ViewID > 0))
            {
                UIManager.Instance.OpenSystemPopup(new MessageData
                {
                    Type = PopupType.OkOnly,
                    Message = "상대방과 동기화에 실패했습니다.",
                    OkAction = async () => { await ScenesManager.Instance.LoadScene("LobbyScene"); }
                });
                return;
            }

            Debug.LogWarning("동기화 준비 완료!");

            foreach (var c in controllers)
            {
                if (c.PhotonView == null)
                    continue;

                if (c.PhotonView.IsMine)
                    m_PhotonController_My = c;          // 내가 소유한 컨트롤러
                else
                    m_PhotonController_Enemy = c;     // 상대 컨트롤러
            }

            UserData_Common myCommonData = DataManager.Instance.GetMyUserData().UserCommonData;
            HeroData myHeroData = DataManager.Instance.GetMyUserData().UserHeroData.EquipHero;
            m_PhotonController_My.PhotonView.RPC("RPCSetMyData", RpcTarget.Others, myCommonData.NickName, myCommonData.Score, myCommonData.Image,
                                                myHeroData.HeroName,myHeroData.Skillproficiencies[0], myHeroData.Skillproficiencies[1], 
                                                myHeroData.Skillproficiencies[2], myHeroData.Level, myHeroData.Exp, myHeroData.Grade, myHeroData.GradeExp);

            Debug.LogWarning("상대방 데이터 로드 대기 중...");

            await UniTask.WhenAny(UniTask.WaitUntil(() => !string.IsNullOrEmpty(m_PhotonController_Enemy.MyNickName)),UniTask.Delay(TimeSpan.FromSeconds(10)));

            if (string.IsNullOrEmpty(m_PhotonController_Enemy.MyNickName))
            {
                UIManager.Instance.OpenSystemPopup(new MessageData
                {
                    Type = PopupType.OkOnly,
                    Message = "상대방 데이터 로드를 실패했습니다.",
                    OkAction = async () => { await ScenesManager.Instance.LoadScene("LobbyScene"); }
                });
                return;
            }

            Debug.LogWarning("상대방 데이터 로드 완료!");

            UserData_Common EnemyCommonData = new UserData_Common()
            { 
                NickName = m_PhotonController_Enemy.MyNickName,
                Score = m_PhotonController_Enemy. MyScore,
                Image = m_PhotonController_Enemy.MyImage
            };

            HeroData EnemyHero = new HeroData()
            {
                HeroName = m_PhotonController_Enemy.MyHeroName,
                Skillproficiencies = m_PhotonController_Enemy.MyHeroSkillproficiencies,
                Level = m_PhotonController_Enemy.MyHeroLevel,
                Exp = m_PhotonController_Enemy.MyHeroExp,
                Grade = m_PhotonController_Enemy.MyHeroGrade,
                GradeExp = m_PhotonController_Enemy.MyHeroGradeExp
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
        // AI 적 생성
        else
        {
            EnemyUserData = DataManager.Instance.GetAIUserData();
        }

        // 필드 생성
        var field = ResourceLoader.LoadAssetResources<GameObject>("Prefabs/Map/MainField");
        Instantiate(field, Vector3.zero, Quaternion.identity, m_EnvironmentRoot.transform);
        Feild = field.GetComponent<MainFeild>();

        // 히어로 생성
        CreateHeroes();

        // 카메라 효과
        await SetCameraMove();

        // 모든 준비가 완료 되었을 때
        IsStartGame = true;
    }

    protected override void EndGame()
    {
        if (!IsStartGame)
            return;

        IsStartGame = true;

        // 서버 연결 해제
        if (PhotonNetwork.IsConnected)
            PhotonManager.Instance.Disconnect(null);

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
        if(!PhotonNetwork.IsConnected)
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
        else
        {
            if(PhotonNetwork.IsMasterClient)
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
            if (CurHp > EnemyCurHp)
                m_Result = "Win";
            else if (CurHp < EnemyCurHp)
                m_Result = "Lose";
            else if (CurHp == EnemyCurHp)
                m_Result = "Draw";

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
            if(MySelectBtnNum != -1)
                MyCanUseSkillCounts[MySelectBtnNum]--;

            if(IsMyCritical)
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
                        // Die Sound 추가 해야 됨 



                        EnemyHeroAnim.Anim.SetTrigger("Die");

                        await UniTask.Delay(2000);  // Die 시간도 추가

                        m_Result = "Win";
                        EndGame();
                    }
                }
                else
                {
                    if(EnemySelectBtnNum != -1)
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

                    // 공격 성공 시
                    if (MySelectBtnNum != EnemySelectBtnNum)
                    {
                        EffectUtil.StartShake(0.1f, 0.2f);
                        SoundManager.Instance.StartSFX("Hit", Obj_EnemyHero.transform.position);
                        EnemyHeroAnim.Anim.SetTrigger("Hit");
                        EnemyCurHp -= DamageUtil.GetSkillDamage(DataManager.Instance.GetMyUserData().UserHeroData.EquipHero, MySelectBtnNum);
                        ingameWindow.SetUI_Players();

                        if (EnemyCurHp <= 0)
                        {
                            // Die Sound 추가


                            EnemyHeroAnim.Anim.SetTrigger("Die");

                            await UniTask.Delay(2000);  // Die 시간도 추가

                            m_Result = "Win";
                            EndGame();
                            return;
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
                    }
                    else
                    {
                        IsMyCombo = false;
                        MyCombo = 0;

                        EffectUtil.StartShake(0.02f, 0.15f);
                        EnemyHeroAnim.Anim.SetTrigger("Block");

                    }
                }
                else
                {
                    IsMyCombo = false;
                    MyCombo = 0;

                    if(EnemySelectBtnNum != -1)
                        EnemyHeroAnim.Anim.SetTrigger("Block");
                }

                await UniTask.Delay(1000);
            }
        }
        else
        {
            if (EnemySelectBtnNum != -1)
                EnemyCanUseSkillCounts[EnemySelectBtnNum]--;

            if(IsEnemyCritical)
            {
                if(EnemySelectBtnNum != -1)
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
                        // Die Sound 추가


                        MyHeroAnim.Anim.SetTrigger("Die");

                        await UniTask.Delay(2000);  // Die 시간도 추가

                        m_Result = "Lose";
                        EndGame();
                    }
                }
                else
                {
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

                    // 공격 성공 시
                    if (MySelectBtnNum != EnemySelectBtnNum)
                    {
                        EffectUtil.StartShake(0.1f, 0.2f);
                        SoundManager.Instance.StartSFX("Hit", Obj_MyHero.transform.position);
                        MyHeroAnim.Anim.SetTrigger("Hit");
                        CurHp -= DamageUtil.GetSkillDamage(EnemyUserData.UserHeroData.EquipHero, EnemySelectBtnNum);
                        ingameWindow.SetUI_Players();

                        if (CurHp <= 0)
                        {
                            // Die Sound 추가


                            MyHeroAnim.Anim.SetTrigger("Die");

                            await UniTask.Delay(2000);  // Die 시간도 추가

                            m_Result = "Lose";
                            EndGame();
                            return;
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
                        IsEnemyCombo = false;
                        EnemyCombo = 0;

                        EffectUtil.StartShake(0.02f, 0.15f);
                        MyHeroAnim.Anim.SetTrigger("Block");

                        await UniTask.Delay(1000);
                    }
                }
                else
                {
                    IsEnemyCombo = false;
                    EnemyCombo = 0;
                    
                    if(MySelectBtnNum != -1)
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

        m_SplineDolly.CameraPosition = 1f; // 마지막 위치 보정
    }
    #endregion

    #endregion
}
