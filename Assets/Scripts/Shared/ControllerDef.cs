using System.Collections.Generic;
using System.Linq;
using System;

public enum ResponseType
{
    None,                   // ������
    Success,                // ����

    ServerError,            // ���� ����
    DuplicateRequest,       // �ߺ� ��û
    UnknownError,           // �� �� ���� ����
    ConnectionError,        // ���� ����
    InvalidRequest,         // �߸��� ��û (������ ���� �� ���� ����)
    GameDataError,          // ���� ������ ����
    UserDataError,          // ���� ������ ����
    InvalidUser,            // ������ ����
    RequireUpdate,          // ������Ʈ �ʿ�
    VersionError,           // ���� ����
    AuthKeyError,           // ����Ű ����
}

public enum MessageType
{
    None,

    Message,                // Gstr ��� �޼���
    ItemMessage,            // Gstr ��� ������ �޼���
    Toast,                  // Gstr ��� �佺Ʈ
    MissionToast,           // �̼� �佺Ʈ

    MessageWithKey,         // {0}{1}�� ����ϴ� Gstr �޼���
    ToastWithKey,           // {0}{1}�� ����ϴ� Gstr �佺Ʈ

    AccountLevelUp,

    Max
}

public enum MsgType
{
    None,
    Toast,
    Message,
}

public class TestResponse
{
    public ResponseType Type { get; set; } = ResponseType.UnknownError;
    public int Code { get; set; }
    public string Message { get; set; }

    public MsgType MsgType { get; set; }

    public string Data { get; set; }

    public List<PacketBody> Bodies { get; set; }

    private const string Separator = "#";

    public TestResponse SetData(params object[] values)
    {
        if (values == null || values.Length == 0)
            throw new ArgumentNullException(nameof(values));

        if (values.Any(x => x == null))
            throw new ArgumentException("Values cannot contain null", nameof(values));

        Data = string.Join(Separator, values);
        return this;
    }

    public TestResponse SetCode(int code)
    {
        Code = code;
        return this;
    }

    public TestResponse SetMessage(string message)
    {
        Message = message;
        return this;
    }

    public TestResponse SetType(ResponseType type)
    {
        Type = type;
        return this;
    }

    public TestResponse SetMsgType(MsgType msgType)
    {
        MsgType = msgType;
        return this;
    }

    public TestResponse AddBody(PacketBody body)
    {
        Bodies ??= new List<PacketBody>();
        Bodies.Add(body);
        return this;
    }

    public TestResponse AddBodies(List<PacketBody> bodies)
    {
        if (bodies == null || bodies.Count == 0)
            return this;

        if (bodies.Any(x => x == null))
            return this;

        Bodies ??= new List<PacketBody>();
        Bodies.AddRange(bodies);
        return this;
    }
}