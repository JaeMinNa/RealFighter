using Cysharp.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    private TutorialController m_TutorialController = null;

    public static TutorialManager Instance
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

                GameObject managerObj = GameObject.Find("[Managers]/TutorialManager");
                if (managerObj == null)
                {
                    managerObj = new GameObject("TutorialManager");
                    managerObj.transform.SetParent(Obj.transform);
                }

                m_Instance = managerObj.GetComponent<TutorialManager>();
                if (m_Instance == null)
                {
                    m_Instance = managerObj.AddComponent<TutorialManager>();
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
    public async UniTask StartTutorial(TutorialStep step)
    {
        switch (step)
        {

            case TutorialStep.LobbyChat_0:

                // TutorialController 가져올 때 까지 대기
                await UniTask.WaitUntil(() => UIManager.Instance.GetOpened<LobbyWindow>().GetComponent<TutorialController>() != null);
                m_TutorialController = UIManager.Instance.GetOpened<LobbyWindow>().GetComponent<TutorialController>();

                TutorialData data_0 = new TutorialData()
                {
                    ChatText = "어서와라. 여긴 네가 실력을 증명해야 하는 결투장이다!",
                    Action_Chat = async () => await StartTutorial(TutorialStep.LobbyChat_1),
                };
                await SetTutorial(data_0);

                break;

            case TutorialStep.LobbyChat_1:

                TutorialData data_1 = new TutorialData()
                {
                    ChatText = "긴 말 필요 없이 바로 실전으로 가보자고! BATTLE을 클릭해봐!",
                    Action_Chat = async () => await StartTutorial(TutorialStep.ClickBattle),
                };
                await SetTutorial(data_1);

                break;

            case TutorialStep.ClickBattle:

                TutorialData data_2 = new TutorialData()
                {
                    MaskSize = new Vector2(450, 180),
                    MaskPos = m_TutorialController.Trans_BattleButton.position,
                    Action_Mask = async () => 
                    {
                        UIManager.Instance.Open<TouchBlockWindow>(UI.Mask, "Prefabs/UI/TouchBlock/TouchBlockWindow");
                        DataManager.Instance.GetMyUserData().UserContentsData.TutorialIndex++;
                        await m_TutorialController.OnClick_Battle();
                    },
                    IsDown = true
                };
                await SetTutorial(data_2);

                break;

            case TutorialStep.IngameChat_0:

                // TutorialController 가져올 때 까지 대기
                await UniTask.WaitUntil(() => UIManager.Instance.GetOpened<IngameWindow>().GetComponent<TutorialController>() != null);
                m_TutorialController = UIManager.Instance.GetOpened<IngameWindow>().GetComponent<TutorialController>();

                TutorialData data_3 = new TutorialData()
                {
                    TimeScale = 0f,
                    ChatText = "초보자인 것 같으니 한번만 설명해주지. 잘 듣도록!",
                    Action_Chat = async () => await StartTutorial(TutorialStep.IngameChat_1),
                };
                await SetTutorial(data_3);

                break;

            case TutorialStep.IngameChat_1:

                TutorialData data_4 = new TutorialData()
                {
                    ChatText = "먼저 공격을 해보자. 해당 버튼을 클릭해봐!",
                    Action_Chat = async () => await StartTutorial(TutorialStep.ClickAttack),
                };
                await SetTutorial(data_4);

                break;

            case TutorialStep.ClickAttack:

                TutorialData data_5 = new TutorialData()
                {
                    MaskSize = new Vector2(500, 150),
                    MaskPos = m_TutorialController.Trans_AttackButton.position,
                    IsUp = true,
                    Action_Mask = async () =>
                    {
                        m_TutorialController.OnClick_Attack();
                        await StartTutorial(TutorialStep.ClickReady_0);
                    },
                };
                await SetTutorial(data_5);

                break;

            case TutorialStep.ClickReady_0:

                TutorialData data_6 = new TutorialData()
                {
                    MaskSize = new Vector2(380, 180),
                    MaskPos = m_TutorialController.Trans_Ready.position,
                    IsDown = true,
                    Action_Mask = async () =>
                    {
                        CloseTutorialMask();
                        m_TutorialController.OnClick_Ready();
                        Time.timeScale = 1f;

                        UIManager.Instance.Open<TouchBlockWindow>(UI.Mask, "Prefabs/UI/TouchBlock/TouchBlockWindow");
                        await UniTask.Delay(3000);
                        UIManager.Instance.Close<TouchBlockWindow>();

                        await StartTutorial(TutorialStep.IngameChat_2);
                    },
                };
                await SetTutorial(data_6);

                break;

            case TutorialStep.IngameChat_2:

                TutorialData data_7 = new TutorialData()
                {
                    TimeScale = 0f,
                    ChatText = "공격에 성공했군. 상대방이 어디를 막을지 잘 예측하여 피해서 공격하는 것이 중요하다.",
                    Action_Chat = async () => await StartTutorial(TutorialStep.IngameChat_3),
                };
                await SetTutorial(data_7);

                break;

            case TutorialStep.IngameChat_3:

                TutorialData data_8 = new TutorialData()
                {
                    ChatText = "어디를 공격하는지 어떻게 아냐고?",
                    Action_Chat = async () => await StartTutorial(TutorialStep.IngameChat_4),
                };
                await SetTutorial(data_8);

                break;

            case TutorialStep.IngameChat_4:

                TutorialData data_9 = new TutorialData()
                {
                    ChatText = "공격에는 각각 카운트 제한이 있기 때문에 이 정보를 통해 상대방의 공격을 예측할 수 있다!",
                    Action_Chat = async () => await StartTutorial(TutorialStep.IngameChat_5),
                };
                await SetTutorial(data_9);

                break;

            case TutorialStep.IngameChat_5:

                TutorialData data_10 = new TutorialData()
                {
                    ChatText = "하나의 공격 카운트를 모두 소모해 버리면, 그만큼 선택지가 줄어들테니, 방어하기가 쉽겠지?",
                    Action_Chat = async () => await StartTutorial(TutorialStep.IngameChat_6),
                };
                await SetTutorial(data_10);

                break;

            case TutorialStep.IngameChat_6:

                TutorialData data_11 = new TutorialData()
                {
                    ChatText = "그래서 후반으로 갈 수록 이 카운트를 잘 관리하는 것이 중요하고 할 수 있다구!",
                    Action_Chat = async () => await StartTutorial(TutorialStep.IngameChat_7),
                };
                await SetTutorial(data_11);

                break;

            case TutorialStep.IngameChat_7:

                TutorialData data_12 = new TutorialData()
                {
                    ChatText = "이번에는 방어를 해보자. 해당 버튼을 클릭해봐!",
                    Action_Chat = async () => await StartTutorial(TutorialStep.ClickDeffence),
                };
                await SetTutorial(data_12);

                break;

            case TutorialStep.ClickDeffence:

                TutorialData data_13 = new TutorialData()
                {
                    MaskSize = new Vector2(500, 150),
                    MaskPos = m_TutorialController.Trans_DeffenceButton.position,
                    IsDown = true,
                    Action_Mask = async () =>
                    {
                        m_TutorialController.OnClick_Deffence();
                        await StartTutorial(TutorialStep.ClickReady_1);
                    },
                };
                await SetTutorial(data_13);

                break;

            case TutorialStep.ClickReady_1:

                TutorialData data_14 = new TutorialData()
                {
                    MaskSize = new Vector2(380, 180),
                    MaskPos = m_TutorialController.Trans_Ready.position,
                    IsDown = true,
                    Action_Mask = async () =>
                    {
                        CloseTutorialMask();
                        m_TutorialController.OnClick_Ready();
                        Time.timeScale = 1f;

                        UIManager.Instance.Open<TouchBlockWindow>(UI.Mask, "Prefabs/UI/TouchBlock/TouchBlockWindow");
                        await UniTask.Delay(3000);
                        UIManager.Instance.Close<TouchBlockWindow>();

                        await StartTutorial(TutorialStep.IngameChat_8);
                    },
                };
                await SetTutorial(data_14);

                break;

            case TutorialStep.IngameChat_8:

                TutorialData data_15 = new TutorialData()
                {
                    TimeScale = 0f,
                    ChatText = "방어에 성공했군. 상대의 공격을 잘 예측했다!",
                    Action_Chat = async () => await StartTutorial(TutorialStep.IngameChat_9),
                };
                await SetTutorial(data_15);

                break;

            case TutorialStep.IngameChat_9:

                TutorialData data_16 = new TutorialData()
                {
                    ChatText = "재능이 있는 것 같군. 상대방을 쓰러트려 보도록!",
                    Action_Chat = () => 
                    {
                        CloseTutorialMask();
                        Time.timeScale = 1f;
                    },
                };
                await SetTutorial(data_16);

                break;

            // 크리티컬 공격
            case TutorialStep.IngameChat_10:

                TutorialData data_17 = new TutorialData()
                {
                    TimeScale = 0f,
                    ChatText = "공격을 3번 연속 성공하면, 아주 강력한 한 방을 날릴 수 있지.",
                    Action_Chat = async () => await StartTutorial(TutorialStep.IngameChat_11),
                };
                await SetTutorial(data_17);

                break;

            case TutorialStep.IngameChat_11:

                TutorialData data_18 = new TutorialData()
                {
                    ChatText = "상대방이 방어를 성공해도 데미지를 줄 수 있기 때문에 역전의 찬스라고 할 수 있다구!",
                    Action_Chat = async () => await StartTutorial(TutorialStep.IngameChat_12),
                };
                await SetTutorial(data_18);

                break;

            case TutorialStep.IngameChat_12:

                TutorialData data_19 = new TutorialData()
                {
                    ChatText = "강력한 한 방을 날려보자!",
                    Action_Chat = () => 
                    {
                        Time.timeScale = 1f;
                        CloseTutorialMask();
                    },
                };
                await SetTutorial(data_19);

                break;

            // 리워드 팝업
            case TutorialStep.IngameChat_13:

                TutorialData data_20 = new TutorialData()
                {
                    ChatText = "자네 같은 천재는 오랜만인데?",
                    Action_Chat = async () => await StartTutorial(TutorialStep.IngameChat_14),
                };
                await SetTutorial(data_20);

                break;

            case TutorialStep.IngameChat_14:

                TutorialData data_21 = new TutorialData()
                {
                    ChatText = "버튼을 클릭하고 로비로 나가보자.",
                    Action_Chat = async () => await StartTutorial(TutorialStep.ClickLobby),
                };
                await SetTutorial(data_21);

                break;

            case TutorialStep.ClickLobby:

                // Popup_Result 가져올 때 까지 대기
                await UniTask.WaitUntil(() => UIManager.Instance.GetOpened<Popup_Result>() != null);
                var popupResult = UIManager.Instance.GetOpened<Popup_Result>();

                TutorialData data_22 = new TutorialData()
                {
                    MaskSize = new Vector2(510, 200),
                    MaskPos = popupResult.Trans_LobbyButton.position,
                    IsDown = true,
                    Action_Mask = () =>
                    {
                        DataManager.Instance.GetMyUserData().UserContentsData.TutorialIndex++;
                        m_TutorialController.OnClick_Lobby();
                    },
                };
                await SetTutorial(data_22);

                break;

            // 다시 Lobby로
            case TutorialStep.LobbyChat_2:

                TutorialData data_23 = new TutorialData()
                {
                    ChatText = "본격적으로 격투에 참가하기 위해서는 히어로가 필요하겠지?",
                    Action_Chat = async () => await StartTutorial(TutorialStep.LobbyChat_3),
                };
                await SetTutorial(data_23);

                break;

            case TutorialStep.LobbyChat_3:

                TutorialData data_24 = new TutorialData()
                {
                    ChatText = "내가 자네에게는 특별히 골드를 선물하도록 하겠네!",
                    Action_Chat = async () =>
                    {
                        DataManager.Instance.GetMyUserData().UserContentsData.TutorialIndex++;
                        DataManager.Instance.GetMyUserData().UserCommonData.Gold += ClientDef.TutorialRewardGold;
                        DataManager.Instance.SaveData();
                        UIManager.Instance.Refresh();
                        await StartTutorial(TutorialStep.LobbyChat_4);
                    }
                };
                await SetTutorial(data_24);

                break;

            // 상점으로 가기
            case TutorialStep.LobbyChat_4:

                TutorialData data_25 = new TutorialData()
                {
                    ChatText = "이제 자네만의 히어로를 뽑으로 가보자구! Shop 버튼을 누르도록!",
                    Action_Chat = async () => await StartTutorial(TutorialStep.ClickShop),
                };
                await SetTutorial(data_25);

                break;

            case TutorialStep.ClickShop:

                // TutorialController 가져올 때 까지 대기
                await UniTask.WaitUntil(() => UIManager.Instance.GetOpened<LobbyWindow>().GetComponent<TutorialController>() != null);
                m_TutorialController = UIManager.Instance.GetOpened<LobbyWindow>().GetComponent<TutorialController>();

                TutorialData data_26 = new TutorialData()
                {
                    MaskSize = new Vector2(280, 230),
                    MaskPos = m_TutorialController.Trans_ShopButton.position,
                    IsDown = true,
                    Action_Mask = async () => 
                    { 
                        m_TutorialController.OnClick_Shop();
                        await StartTutorial(TutorialStep.ShopChat_0);
                    },
                };
                await SetTutorial(data_26);

                break;

            case TutorialStep.ShopChat_0:

                TutorialData data_27 = new TutorialData()
                {
                    ChatText = "히어로 팩을 누르도록!",
                    Action_Chat = async () => await StartTutorial(TutorialStep.ClickBuy),
                };
                await SetTutorial(data_27);

                break;

            case TutorialStep.ClickBuy:

                // Popup_Shop 가져올 때 까지 대기
                await UniTask.WaitUntil(() => UIManager.Instance.GetOpened<Popup_Shop>() != null);
                var popupShop = UIManager.Instance.GetOpened<Popup_Shop>();

                TutorialData data_28 = new TutorialData()
                {
                    MaskSize = new Vector2(300, 250),
                    MaskPos = popupShop.Trans_HeroPack_0.position,
                    IsDown = true,
                    Action_Mask = async () => 
                    {
                        await m_TutorialController.OnClick_Buy();
                        await StartTutorial(TutorialStep.ClickBuyOk);
                    },
                };
                await SetTutorial(data_28);

                break;

            case TutorialStep.ClickBuyOk:

                await UniTask.WaitUntil(() => UIManager.Instance.GetOpened<Popup_System>() != null);
                var popupSystem = UIManager.Instance.GetOpened<Popup_System>();

                TutorialData data_29 = new TutorialData()
                {
                    MaskSize = new Vector2(450, 180),
                    MaskPos = popupSystem.Trans_OkButton.position,
                    IsDown = true,
                    Action_Mask = async () =>
                    {
                        await m_TutorialController.OnClick_BuyOk();

                        // 튜토리얼용 히어로 삭제하고 새로 뽑은 히어로를 장착
                        DataManager.Instance.GetMyUserData().UserHeroData.MyHeroes.RemoveAt(0);
                        DataManager.Instance.GetMyUserData().UserHeroData.EquipHero = DataManager.Instance.GetMyUserData().UserHeroData.MyHeroes[0];

                        DataManager.Instance.GetMyUserData().UserContentsData.TutorialIndex++;
                        DataManager.Instance.SaveData();

                        UIManager.Instance.Refresh();

                        await StartTutorial(TutorialStep.ShopChat_1);
                    },
                };
                await SetTutorial(data_29);

                break;

            case TutorialStep.ShopChat_1:

                TutorialData data_30 = new TutorialData()
                {
                    ChatText = "이거 처음부터 좋은 선수가 나왔잖아?",
                    Action_Chat = async () => await StartTutorial(TutorialStep.ShopChat_2)
                };
                await SetTutorial(data_30);

                break;

            case TutorialStep.ShopChat_2:

                TutorialData data_31 = new TutorialData()
                {
                    ChatText = "자, 이제 격투 챔피언 랭킹 1등을 노려보자구~",
                    Action_Chat = () => 
                    {
                        CloseTutorialMask();
                    },
                };
                await SetTutorial(data_31);

                break;

            default:
                break;
        }
    }
    #endregion

    #region Private Method
    private async UniTask SetTutorial(TutorialData data)
    {
        // TimeScale 설정
        if(data.TimeScale != -1)
            Time.timeScale = data.TimeScale;

        CloseTutorialMask();

        var tutorialMask = UIManager.Instance.Open<TutorialMask>(UI.Mask, "Prefabs/UI/Tutorial/TutorialMask");

        // TutorialMask 가져올 때 까지 대기
        await UniTask.WaitUntil(() => tutorialMask != null);

        // 자식 중, "Mask" Transform 찾기
        Transform maskTrans = tutorialMask.transform.Find("Mask");
        
        if(data.MaskSize == new Vector2(0, 0) || data.MaskPos == new Vector2(0, 0))
        {
            tutorialMask.SetActiveMask(false);
            tutorialMask.SetUpArrow(false);
            tutorialMask.SetDownArrow(false);
        }
        else
        {
            // 사이즈 설정
            maskTrans.GetComponent<RectTransform>().sizeDelta = data.MaskSize;

            // 위치 설정
            Vector2 mousePos = RectTransformUtility.WorldToScreenPoint(null, data.MaskPos);
            maskTrans.transform.position = mousePos;

            // Arrow 설정
            tutorialMask.SetUpArrow(data.IsUp);
            tutorialMask.SetDownArrow(data.IsDown);

            // 버튼 설정
            tutorialMask.SetButtonMask(data.Action_Mask);

            tutorialMask.SetActiveMask(true);
        }

        if(data.ChatText == string.Empty)
        {
            tutorialMask.SetActiveChat(false);
        }
        else
        {
            SoundManager.Instance.StartSFX("Text");

            // Chat 설정
            tutorialMask.SetChatText(data.ChatText);

            // 버튼 설정
            tutorialMask.SetButtonChat(data.Action_Chat);

            tutorialMask.SetActiveChat(true);
        }
    }

    private void CloseTutorialMask()
    {
        // TutorialMask 이미 있다면 닫기
        if (UIManager.Instance.GetOpened<TutorialMask>())
            UIManager.Instance.Close<TutorialMask>();
    }
    #endregion
}
