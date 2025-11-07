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
    private float m_WaitTime = ClientDef.RoomWaitTime;
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

                // 게임 시작 (AI)
                PhotonManager.Instance.Disconnect(async () => { await ScenesManager.Instance.LoadScene("GameScene"); });
            }

            // 서버에 접속하였고, 방에 2명이 있을 때 게임시작
            if(PhotonNetwork.InRoom && PhotonNetwork.IsConnected 
                && PhotonNetwork.CurrentRoom.PlayerCount == 2 && !m_IsGameStart)
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
        PhotonNetwork.AutomaticallySyncScene = true;
        Btn_Close.onClick.AddListener(OnClick_Close);
    }

    public override void OnClose()
    {

    }

    public override void OnOpen(List<object> Args)
    {
        Text_PlayerCount.gameObject.SetActive(false);

        // 서버 접속 시도 -> 상대 찾기
        PhotonManager.Instance.Connect(SuccessConnect, FailConnect);
    }

    public override void OnRefresh()
    {

    }
    #endregion

    #region Private Method
    private void SuccessConnect()
    {
        // 랜덤 방 입장 시도
        PhotonManager.Instance.JoinRandomRoom(SuccessJoinRandomRoom, FailJoinRandomRoom);

        // 에디터 전용, 전체 서버 접속 인원 표시
        if(GameManager.Instance.IsEditor)
        {
            Text_PlayerCount.text = $"[에디터 전용]  Players : {PhotonNetwork.CountOfPlayers}";
            Text_PlayerCount.gameObject.SetActive(true);
        }
    }

    private void FailConnect()
    {
        // 서버 접속 실패
        UIManager.Instance.Close<Popup_BattleLoading>();

        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkOnly,
            Title = "알림",
            Message = "서버 접속에 실패했습니다. 잠시후, 다시 시도해 주세요."
        });
    }

    private void SuccessJoinRandomRoom()
    {
        // 방 입장 성공
        m_IsRoom = true;

        // 플레이어 2명이 있다면 게임 시작
    }

    private void FailJoinRandomRoom()
    {
        // 방 생성
        PhotonManager.Instance.CreateRoom(SuccessCreateRoom, FailCreateRoom);
    }

    private void SuccessCreateRoom()
    {
        // 방 생성 성공
        m_IsRoom = true;

        // 플레이어 2명이 있다면 게임 시작
    }

    private void FailCreateRoom()
    {
        // 방 생성 실패
        UIManager.Instance.Close<Popup_BattleLoading>();

        UIManager.Instance.OpenSystemPopup(new MessageData
        {
            Type = PopupType.OkOnly,
            Title = "알림",
            Message = "방 생성에 실패했습니다. 잠시후, 다시 시도해 주세요."
        });
    }
    #endregion

    #region Button
    private void OnClick_Close()
    {
        SoundManager.Instance.StartSFX("ButtonClick");
        UIManager.Instance.Close<Popup_BattleLoading>();

        // 방 나가기
        PhotonManager.Instance.LeaveRoom(null);

        // 서버 연결 해제
        PhotonManager.Instance.Disconnect(null);
    }
    #endregion
}
