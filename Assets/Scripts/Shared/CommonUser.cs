public enum eUserData_Common
{
    AccountCode,        // ID ����, �� ������ ���� �����͸� ��ȸ �Ѵ�.
    UID,                // �������� ���� ������, �ʿ� �� ���
    NickName,

    Max,
}

public class UserData_Common
{
    public string AccountCode = string.Empty;
    public string UID = string.Empty;
    public string NickName = string.Empty;
}