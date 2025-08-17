using UnityEditor;
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
    #region Data
    [ContextMenu("Save Data")]
    public void SaveData()
    {
        if (m_DataLoader == null)
        {
            Debug.LogWarning("DataLoader is null");
            return;
        }

        ES3.Save("UserData", m_DataLoader);

        Debug.LogWarning("UserData 저장 성공");
    }

    [ContextMenu("Load Data")]
    public void LoadData()
    {
        if (m_DataLoader == null)
        {
            Debug.LogWarning("DataLoader is null");
            return;
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
        }
    }

    public void DeleteData()
    {
        // 데이터 삭제
        ES3.DeleteFile("SaveFile.txt");
        Debug.LogWarning("UserData 삭제 성공");

        // PlayerPrefs 삭제
        PlayerPrefs.DeleteAll();
        Debug.LogWarning("PlayerPrefs 삭제 성공");

        // 게임 종료
        ExitGame();
    }

    // 현재 씬에서 DataLoader를 새로 설정해 준다.
    public void SetDataLoader()
    {
        m_DataLoader = GameObject.Find("DataLoader").GetComponent<DataLoader>();
    }

    public UserData GetMyUserData()
    {
        return m_DataLoader.MyUserData;
    }

    public UserData GetAIUserData()
    {
        return SetAIUserData();
    }
    #endregion

    #region Game
    public void ExitGame()
    {
        if(GameManager.Instance.IsEditor)
        {
            EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
    #endregion
    #endregion

    #region Private Method
    // 저장된 데이터가 없을 때, 최초로 불러오는 유저 데이터
    private void SetUserData()
    {
        m_DataLoader.MyUserData = new UserData();

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

        m_DataLoader.MyUserData.UserCommonData = userData_Common;

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

        m_DataLoader.MyUserData.UserHeroData = userData_Hero;

        Debug.LogWarning("UserData 생성 성공");
    }

    private UserData SetAIUserData()
    {
        UserData AIUserData = new UserData();

        // Common
        UserData_Common aiData_Common = new UserData_Common()
        {
            AccountCode = "AI",
            UID = "AI",
            NickName = TextUtil.GetRandomAINickName(),
            Score = RandomUtil.GetRandomIndex(0, 5),
            Image = RandomUtil.GetRandomIndex(1, 5).ToString(),
            Gold = 0
        };

        AIUserData.UserCommonData = aiData_Common;

        // Hero
        HeroData heroData = HeroUtil.GetRandomAIHeroData();

        UserData_Hero aiData_Hero = new UserData_Hero();
        aiData_Hero.EquipHero = heroData;
        aiData_Hero.MyHeroes.Add(heroData);

        AIUserData.UserHeroData = aiData_Hero;

        Debug.LogWarning("EnemyUserData 생성 성공");

        return AIUserData;
    }
    #endregion
}
