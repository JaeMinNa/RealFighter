using Cysharp.Threading.Tasks;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [Header("Lobby_0")]
    public Transform Trans_BattleButton = null;

    [Header("Ingame")]
    public Transform Trans_AttackButton = null;
    public Transform Trans_DeffenceButton = null;
    public Transform Trans_Ready = null;

    [Header("Lobby_1")]
    public Transform Trans_ShopButton = null;

    private LobbyWindow m_LobbyWindow = null;
    private IngameWindow m_IngameWindow = null;
    private PVPModule m_PVPModule = null;

    #region Lobby_0
    public async UniTask OnClick_Battle()
    {
        if (m_LobbyWindow == null)
        {
            // IngameWindow 가져올 때 까지 대기
            await UniTask.WaitUntil(() => UIManager.Instance.GetOpened<LobbyWindow>() != null);
            m_LobbyWindow = UIManager.Instance.GetOpened<LobbyWindow>();
        }

        m_LobbyWindow.OnClick_PVP();
    }
    #endregion

    #region Ingame
    public async void OnClick_Attack()
    {
        if (m_IngameWindow == null)
        {
            // IngameWindow 가져올 때 까지 대기
            await UniTask.WaitUntil(() => UIManager.Instance.GetOpened<IngameWindow>() != null);
            m_IngameWindow = UIManager.Instance.GetOpened<IngameWindow>();
        }

        if (m_PVPModule == null)
        {
            // PVPModule 가져올 때 까지 대기
            await UniTask.WaitUntil(() => BattleModule.Instance as PVPModule != null);
            m_PVPModule = BattleModule.Instance as PVPModule;
        }

        m_IngameWindow.OnClick_MyAttacks(2);
    }

    public void OnClick_Ready()
    {
        m_IngameWindow.OnClick_Ready();
    }

    public void OnClick_Deffence()
    {
        m_IngameWindow.OnClick_MyDefences(0);
    }

    public async void OnClick_Lobby()
    {
        // Popup_Result 가져올 때 까지 대기
        await UniTask.WaitUntil(() => UIManager.Instance.GetOpened<Popup_Result>() != null);
        var popup = UIManager.Instance.GetOpened<Popup_Result>();

        popup.OnClick_Home();
    }
    #endregion
}
