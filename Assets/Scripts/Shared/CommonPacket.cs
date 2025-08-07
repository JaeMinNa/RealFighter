using System.Collections.Generic;

public enum PacketType
{
    None,

    GetUserData,        // 유저 데이터 패킷 (로그인 시, 한번만 호출)
    ContentsPacket,     // 컨텐츠 패킷 (일반적으로 사용하는 그 외 통신에서 사용)

    Max,
}

#region Contents Packet
// 클라이언트가 전달받는 패킷의 Header에 해당한다
// Header는 데이터 갱신이 이루어진 후 게임 내 연출 또는 결과를 보여주는 용도이다
public enum ContentsType
{
    None,

    User,

    Max,
}

public enum UserContents
{
    None,

    ChangeNickName,
    GetData,

    Max
}
#endregion

#region Receive Packet
// 클라이언트가 전달받는 패킷의 Body에 해당한다
// Body는 데이터의 갱신 위주 처리
public enum ReceiveType
{
    None,

    UpdateUserCommonData,       // 서버 통신 후, UserData 전체를 갱신

    ShowMessage,

    Max,
}
#endregion

#region Result Packet
public enum PacketState
{
    None,                   // 정상 패킷

    InvalidUser,            // 비정상 유저
    RequireUpdate,          // 업데이트 필요
    VersionError,           // 버전 에러
    AuthKeyError,           // 인증키 에러
    ShutDown,               // 서버 점검
    BanUser,                // 밴 상태
    TransferError,          // 잘못된 인계코드
    TransferTimeOver,       // 인계코드 시간초과
    UnknownPacket,          // 알수 없는 패킷 인덱스
    ServerError,            //유저데이터를 찾을 수 없음 또는 잘못된 인자값으로 인한 서버 로직 오류
    ServerException,        // 서버 크래시
    NetworkError,           // 통신 실패

    Max
}

public class ServerPacket
{
    public PacketType PacketType;
    public PacketState StateType;
    public string Data;
}

public class GamePacket
{
    public ContentsType ContentsType;
    public PacketHeader HeaderData;
    public List<PacketBody> BodyData;
    public long GameDataVersion;
}

public class PacketHeader
{
    public int ContentsIndex = 0;
    public bool Success = false;
    public string Data = string.Empty;
}

public class PacketBody
{
    public int ReceiveType = 0;
    public int ReceiveIndex = 0;
    public string Data = string.Empty;
}
#endregion

#region Log
public class LogData
{
    public string Type = string.Empty;
    public string AddType = string.Empty;
    public string Before = string.Empty;
    public string After = string.Empty;
}
#endregion