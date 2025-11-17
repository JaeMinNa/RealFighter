using Photon.Pun;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Popup_BattleLoading : UIElement
{
    #region Cahsed Object
    [SerializeField] private TMP_Text Text_PlayerCount = null;
    [SerializeField] private Button Btn_Close = null;
    #endregion

    #region Member Property
    private bool m_IsRoom = false;
    private bool m_IsGameStart = false;
    private float m_WaitTime = 0;
    private bool m_IsTutorial = false;
    #endregion

    #region Unity Method
    private async void Update()
    {
        if(m_IsRoom)
        {
            m_WaitTime -= Time.deltaTime;

            if(m_WaitTime < 0 && !m_IsGameStart)
            {
                m_IsGameStart = true;

                // AI 모드 시작
                PhotonManager.Instance.Disconnect(async () => { await ScenesManager.Instance.LoadScene("GameScene"); });
            }

            // PVP 모드 시작
            if(PhotonNetwork.InRoom && PhotonNetwork.IsConnected 
                && PhotonNetwork.CurrentRoom.PlayerCount == 2 && !m_IsGameStart && !m_IsTutorial)
            {
                m_IsGameStart = true;
                await ScenesManager.Instance.PhotonLoadScene("GameScene");
            }
        }
    }
    #endregion

    #region Override Method
    public override void Init()
    {
        m_WaitTime = DataManager.Instance.GetMyUserData().UserContentsData.TutorialIndex == 1 ? 1f : ClientDef.RoomWaitTime;
        m_IsTutorial = DataManager.Instance.GetMyUserData().UserContentsData.TutorialIndex == 1 ? true : false;
        Btn_Close.onClick.AddListener(OnClick_Close);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnClose()
    {

    }

    public override void OnOpen(List<object> Args)
    {
        Text_PlayerCount.gameObject.SetActive(false);

        // 서버 접속 시도
        PhotonManager.Instance.Connect(SuccessConnect, FailConnect);
    }

    public override void OnRefresh()
    {

    }
    #endregion

    #region Private Method
    private void SuccessConnect()
    {
        // Room 랜덤 생성
        PhotonManager.Instance.JoinRandomRoom(SuccessJoinRandomRoom, FailJoinRandomRoom);

        // 에디터 전용 
        if(GameManager.Instance.IsEditor)
        {
            Text_PlayerCount.text = $"[에디터 전용]  Players : {PhotonNetwork.CountOfPlayers}";
            Text_PlayerCount.gameObject.SetActive(true);
        }
    }

    private void FailConnect()
    {
        UIManager.Instance.Close<Popup_BattleLoading>();

        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkOnly,
            Title = "알림",
            Message = "서버 접속에 실패하였습니다."
        });
    }

    private void SuccessJoinRandomRoom()
    {
        m_IsRoom = true;
    }

    private void FailJoinRandomRoom()
    {
        PhotonManager.Instance.CreateRoom(SuccessCreateRoom, FailCreateRoom);
    }

    private void SuccessCreateRoom()
    {
        m_IsRoom = true;
    }

    private void FailCreateRoom()
    {
        UIManager.Instance.Close<Popup_BattleLoading>();

        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkOnly,
            Title = "알림",
            Message = "Room 생성에 실패하였습니다."
        });
    }
    #endregion

    #region Button
    private void OnClick_Close()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Close<Popup_BattleLoading>();

        PhotonManager.Instance.LeaveRoom(null);
        PhotonManager.Instance.Disconnect(null);
    }
    #endregion
}
