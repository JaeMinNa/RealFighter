using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public static DataManager Instance
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

                GameObject managerObj = GameObject.Find("[Managers]/DataManager");
                if (managerObj == null)
                {
                    managerObj = new GameObject("DataManager");
                    managerObj.transform.SetParent(Obj.transform);
                }

                m_Instance = managerObj.GetComponent<DataManager>();
                if (m_Instance == null)
                {
                    m_Instance = managerObj.AddComponent<DataManager>();
                }

                m_Instance.CreateInstance();
            }

            return m_Instance;
        }
    }

    // ���� ���� DataLoader
    private DataLoader m_DataLoader = null;

    #region Override Method
    public override void DestroyInstance()
    {
        
    }

    protected override void CreateInstance()
    {

    }
    #endregion

    #region public Method
    [ContextMenu("Save Data")]
    public void SaveData()
    {
        if (m_DataLoader == null)
        {
            SetDataLoader();
        }

        ES3.Save("UserData", m_DataLoader);

        Debug.LogWarning("UserData ���� ����");
    }

    [ContextMenu("Load Data")]
    public void LoadData()
    {
        if(m_DataLoader == null)
        {
            SetDataLoader();
        }

        if (ES3.FileExists("SaveFile.txt"))
        {
            ES3.LoadInto("UserData", m_DataLoader);

            Debug.LogWarning("UserData �ε� ����");
        }
        else
        {
            Debug.LogWarning("UserData �ε� ����");

            SetUserData();
            Debug.LogWarning("UserData ���� ����");

            SaveData();
        }
    }

    public void DeleteData()
    {
        // ������ ����
        ES3.DeleteFile("SaveFile.txt"); // ���� ���� �̸�
    }

    // ���� ������ DataLoader�� ���� ������ �ش�.
    public void SetDataLoader()
    {
        m_DataLoader = GameObject.Find("DataLoader").GetComponent<DataLoader>();
    }

    public DataLoader GetDataLoader()
    {
        return m_DataLoader;
    }
    #endregion

    #region Private Method
    // ����� �����Ͱ� ���� ��, ���ʷ� �ҷ����� ���� ������
    private void SetUserData()
    {
        // Common
        UserData_Common userData_Common = new UserData_Common()
        {
            AccountCode = "Admin",
            UID = "Admin",
            NickName = "Jaemin",
            Score = 0,
            Image = "1",
            Gold = 0
        };

        m_DataLoader.UserCommonData = userData_Common;

        // Hero
        HeroData heroData = new HeroData()
        {
            HeroName = "JIN",
            SkillDamage_0 = 10,
            SkillDamage_1 = 10,
            SkillDamage_2 = 10,
            Level = 1,
            Exp = 0
        };

        UserData_Hero userData_Hero = new UserData_Hero();
        userData_Hero.EquipHero = heroData;
        userData_Hero.MyHeroes.Add(heroData);

        m_DataLoader.UserHeroData = userData_Hero;
    }
    #endregion
}
