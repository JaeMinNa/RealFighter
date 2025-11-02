using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private static PhotonManager m_Instance = null;

    // 기능별 콜백 분리
    private Action onConnectSuccess;
    private Action onConnectFail;

    private Action onDisconnectSuccess;

    private Action onLobbyJoinSuccess;
    private Action onLobbyJoinFail;

    private Action onRoomCreateSuccess;
    private Action onRoomCreateFail;

    private Action onRoomJoinSuccess;
    private Action onRoomJoinFail;

    private Action onRandomJoinSuccess;
    private Action onRandomJoinFail;

    private Action onLeaveRoomSuccess;

    public static PhotonManager Instance
    {
        get
        {
            if (m_Instance == null && Application.isPlaying)
            {
                GameObject obj = GameObject.Find("[Managers]");
                if (obj == null)
                {
                    obj = new GameObject("[Managers]");
                    DontDestroyOnLoad(obj);
                }

                GameObject managerObj = GameObject.Find("[Managers]/PhotonManager");
                if (managerObj == null)
                {
                    managerObj = new GameObject("PhotonManager");
                    managerObj.transform.SetParent(obj.transform);
                }

                m_Instance = managerObj.GetComponent<PhotonManager>();
                if (m_Instance == null)
                {
                    m_Instance = managerObj.AddComponent<PhotonManager>();
                }

                PhotonNetwork.SerializationRate = 60;
                PhotonNetwork.SendRate = 60;
            }

            return m_Instance;
        }
    }

    #region Connection
    public void Connect(Action onSuccess, Action onFail)
    {
        Debug.LogWarning("서버 접속 시도");
        onConnectSuccess = onSuccess;
        onConnectFail = onFail;

        PhotonNetwork.ConnectUsingSettings();
    }

    public void Disconnect(Action onSuccess)
    {
        Debug.LogWarning("서버 연결 해제 시도");
        onDisconnectSuccess = onSuccess;

        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("이미 서버 연결이 해제된 상태");
            onDisconnectSuccess?.Invoke();
            onDisconnectSuccess = null;
            return;
        }

        // 현재 Room 파괴
        if (PhotonNetwork.InRoom)
        {
            Debug.LogWarning($"현재 방: {PhotonNetwork.CurrentRoom.Name}, 파괴");
            PhotonNetwork.LeaveRoom();
        }

        PhotonNetwork.Disconnect();
    }

    public override void OnConnectedToMaster()
    {
        Debug.LogWarning("서버 접속 완료");
        onConnectSuccess?.Invoke();
        onConnectSuccess = null;
        onConnectFail = null;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.DisconnectByClientLogic)
        {
            Debug.LogWarning("서버 연결 해제 완료");
            onDisconnectSuccess?.Invoke();
            onDisconnectSuccess = null;
        }
        else
        {
            Debug.LogWarning($"서버 연결 실패 또는 끊김: {cause}");
            onConnectFail?.Invoke();
            onConnectFail = null;

            // 로비 접속 중 끊긴 경우만 실패 처리
            if (PhotonNetwork.InLobby)
            {
                onLobbyJoinFail?.Invoke();
                onLobbyJoinFail = null;
            }
        }
    }
    #endregion

    #region Lobby
    public void JoinLobby(Action onSuccess, Action onFail)
    {
        Debug.LogWarning("로비 접속 시도");
        onLobbyJoinSuccess = onSuccess;
        onLobbyJoinFail = onFail;

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.LogWarning("로비 접속 완료");
        onLobbyJoinSuccess?.Invoke();
        onLobbyJoinSuccess = null;
        onLobbyJoinFail = null;
    }
    #endregion

    #region Room
    public void CreateRoom(Action onSuccess, Action onFail)
    {
        Debug.LogWarning("방 만들기 시도");
        onRoomCreateSuccess = onSuccess;
        onRoomCreateFail = onFail;

        string roomName = "Room_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2 });
        Debug.LogWarning($"▶ 생성된 방 이름: {roomName}");
    }

    public override void OnCreatedRoom()
    {
        Debug.LogWarning("방 생성 완료 (OnCreatedRoom)");
        // Photon이 자동으로 OnJoinedRoom() 호출하므로 여기서 성공 콜백은 호출하지 않음
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"방 만들기 실패! 코드: {returnCode}, 이유: {message}");
        onRoomCreateFail?.Invoke();
        onRoomCreateFail = null;
        onRoomCreateSuccess = null;
    }

    public void JoinRoom(string roomName, Action onSuccess, Action onFail)
    {
        Debug.LogWarning($"방 참가 시도: {roomName}");
        onRoomJoinSuccess = onSuccess;
        onRoomJoinFail = onFail;

        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.LogWarning("방 참가 완료");

        // 어떤 이유로든 방에 성공적으로 들어왔을 때 실행됨
        onRoomJoinSuccess?.Invoke();
        onRoomJoinSuccess = null;
        onRoomJoinFail = null;

        // 랜덤 참가로 들어온 경우도 포함
        onRandomJoinSuccess?.Invoke();
        onRandomJoinSuccess = null;
        onRandomJoinFail = null;

        // 생성된 방이라면, CreateRoom 성공 콜백도 여기서 실행
        onRoomCreateSuccess?.Invoke();
        onRoomCreateSuccess = null;
        onRoomCreateFail = null;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"방 참가 실패! 코드: {returnCode}, 이유: {message}");
        onRoomJoinFail?.Invoke();
        onRoomJoinFail = null;
        onRoomJoinSuccess = null;
    }

    public void JoinOrCreateRoom(Action onSuccess, Action onFail)
    {
        Debug.LogWarning("방 참가 또는 생성 시도");
        onRoomJoinSuccess = onSuccess;
        onRoomJoinFail = onFail;

        string roomName = "Room_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 2 }, null);
    }

    public void JoinRandomRoom(Action onSuccess, Action onFail)
    {
        Debug.LogWarning("랜덤 방 참가 시도");
        onRandomJoinSuccess = onSuccess;
        onRandomJoinFail = onFail;

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"랜덤 방 참가 실패! 코드: {returnCode}, 이유: {message}");
        onRandomJoinFail?.Invoke();
        onRandomJoinFail = null;
        onRandomJoinSuccess = null;
    }

    public void JoinRandomRoomOrCreateRoom(Action onSuccess, Action onFail)
    {
        Debug.LogWarning("랜덤 방 참가 또는 생성 시도");
        onRandomJoinSuccess = onSuccess;
        onRandomJoinFail = onFail;

        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedMaxPlayers: 2,
            roomOptions: new RoomOptions() { MaxPlayers = 2 });

        // 성공 시 OnJoinedRoom() 에서 호출됨
    }

    public void LeaveRoom(Action onSuccess)
    {
        Debug.LogWarning("방 나가기 시도");
        onLeaveRoomSuccess = onSuccess;

        if (!PhotonNetwork.InRoom)
        {
            Debug.LogWarning("이미 방에 없음");
            onLeaveRoomSuccess?.Invoke();
            onLeaveRoomSuccess = null;
            return;
        }

        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.LogWarning("방 나가기 완료");
        onLeaveRoomSuccess?.Invoke();
        onLeaveRoomSuccess = null;
    }
    #endregion

    #region Player Events
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogWarning($"{newPlayer.NickName} 님이 입장하셨습니다.");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogWarning($"{otherPlayer.NickName} 님이 퇴장하셨습니다.");
    }
    #endregion

    #region Info
    [ContextMenu("Multi Info")]
    public void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.LogWarning($"현재 방 이름: {PhotonNetwork.CurrentRoom.Name}");
            Debug.LogWarning($"현재 방 인원: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
            Debug.LogWarning($"마스터 클라이언트 여부: {PhotonNetwork.IsMasterClient}");

            string playerList = "플레이어 목록: ";
            foreach (var player in PhotonNetwork.PlayerList)
                playerList += player.NickName + ", ";
            Debug.LogWarning(playerList);
        }
        else
        {
            Debug.LogWarning($"접속한 인원수: {PhotonNetwork.CountOfPlayers}");
            Debug.LogWarning($"방 개수: {PhotonNetwork.CountOfRooms}");
            Debug.LogWarning($"방 안의 총 인원수: {PhotonNetwork.CountOfPlayersInRooms}");
            Debug.LogWarning($"로비에 있는지?: {PhotonNetwork.InLobby}");
            Debug.LogWarning($"연결 상태?: {PhotonNetwork.IsConnected}");
        }
    }
    #endregion
}
