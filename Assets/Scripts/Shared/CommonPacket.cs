using System.Collections.Generic;

public enum PacketType
{
    None,

    GetUserData,        // ���� ������ ��Ŷ (�α��� ��, �ѹ��� ȣ��)
    ContentsPacket,     // ������ ��Ŷ (�Ϲ������� ����ϴ� �� �� ��ſ��� ���)

    Max,
}

#region Contents Packet
// Ŭ���̾�Ʈ�� ���޹޴� ��Ŷ�� Header�� �ش��Ѵ�
// Header�� ������ ������ �̷���� �� ���� �� ���� �Ǵ� ����� �����ִ� �뵵�̴�
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
// Ŭ���̾�Ʈ�� ���޹޴� ��Ŷ�� Body�� �ش��Ѵ�
// Body�� �������� ���� ���� ó��
public enum ReceiveType
{
    None,

    UpdateUserCommonData,       // ���� ��� ��, UserData ��ü�� ����

    ShowMessage,

    Max,
}
#endregion

#region Result Packet
public enum PacketState
{
    None,                   // ���� ��Ŷ

    InvalidUser,            // ������ ����
    RequireUpdate,          // ������Ʈ �ʿ�
    VersionError,           // ���� ����
    AuthKeyError,           // ����Ű ����
    ShutDown,               // ���� ����
    BanUser,                // �� ����
    TransferError,          // �߸��� �ΰ��ڵ�
    TransferTimeOver,       // �ΰ��ڵ� �ð��ʰ�
    UnknownPacket,          // �˼� ���� ��Ŷ �ε���
    ServerError,            //���������͸� ã�� �� ���� �Ǵ� �߸��� ���ڰ����� ���� ���� ���� ����
    ServerException,        // ���� ũ����
    NetworkError,           // ��� ����

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