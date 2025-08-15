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

    // 현재 씬의 DataLoader
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

        Debug.LogWarning("UserData 저장 성공");
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

            Debug.LogWarning("UserData 로드 성공");
        }
        else
        {
            Debug.LogWarning("UserData 로드 실패");

            SetUserData();
            Debug.LogWarning("UserData 생성 성공");

            SaveData();
        }
    }

    public void DeleteData()
    {
        // 데이터 삭제
        ES3.DeleteFile("SaveFile.txt"); // 저장 파일 이름
    }

    // 현재 씬에서 DataLoader를 새로 설정해 준다.
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
    // 저장된 데이터가 없을 때, 최초로 불러오는 유저 데이터
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
